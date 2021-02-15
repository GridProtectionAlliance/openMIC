//******************************************************************************************************
//  TrendReaderCollection.cs - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  07/27/2020 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gemstone.IONProtocol;

namespace IONDownloader
{
    public class TrendReaderCollection
    {
        public IONTime CurrentDate { get; private set; }
        private IEnumerable<IONTrendReader> Readers { get; }
        private bool Initialized { get; set; }

        private IEnumerable<IONTrendReader> ActiveReaders => Readers
            .Where(reader => reader.CurrentDate > CurrentDate);

        public TrendReaderCollection(IEnumerable<IONTrendReader> readers) =>
            Readers = readers;

        public async IAsyncEnumerable<IONTrendPoint> AdvanceAsync()
        {
            if (!Initialized)
            {
                IEnumerable<Task> advanceTasks = Readers
                    .Select(reader => reader.TryAdvanceAsync());

                await Task.WhenAll(advanceTasks);
                Initialized = true;
            }

            IONTime date = ActiveReaders
                .Select(reader => reader.CurrentDate.ToDateTime())
                .DefaultIfEmpty(DateTime.MinValue)
                .Min();

            if (date <= CurrentDate)
                yield break;

            int pointCount = 0;
            long lastUpdate = DateTime.UtcNow.Ticks;
            TimeSpan updateInterval = TimeSpan.FromSeconds(2.5D);
            Console.WriteLine($"[Trend {date.ToDateTime():yyyy-MM-dd}] Querying data points...");

            async IAsyncEnumerable<IONTrendPoint> QueryAsync(IONTrendReader reader)
            {
                while (reader.CurrentDate <= date)
                {
                    if (reader.CurrentDate == date)
                    {
                        foreach (IONTrendPoint point in reader.CurrentPoints)
                        {
                            int count = Interlocked.Increment(ref pointCount);
                            long ticks = Interlocked.Read(ref lastUpdate);
                            DateTime now = DateTime.UtcNow;

                            if (now - new DateTime(ticks) > updateInterval && Interlocked.CompareExchange(ref lastUpdate, now.Ticks, ticks) == ticks)
                                Console.WriteLine($"[Trend {date.ToDateTime():yyyy-MM-dd}] {count} points received so far...");

                            yield return point;
                        }
                    }

                    if (!await reader.TryAdvanceAsync())
                        break;
                }
            }

            IEnumerable<Task<List<IONTrendPoint>>> queryTasks = ActiveReaders
                .Select(QueryAsync)
                .Select(points => points.ToListAsync().AsTask());

            List<IONTrendPoint>[] allPoints = await Task.WhenAll(queryTasks);

            foreach (List<IONTrendPoint> points in allPoints)
            {
                foreach (IONTrendPoint point in points)
                    yield return point;
            }

            CurrentDate = date;

            Console.WriteLine($"[Trend {date.ToDateTime():yyyy-MM-dd}] {pointCount} total points received.");
        }

        public async Task SeekAsync(DateTime timestamp)
        {
            long min = IONTime.MinValue.ToDateTime().Ticks;
            long max = IONTime.MaxValue.ToDateTime().Ticks;
            long clampedTicks = Math.Clamp(timestamp.Ticks, min, max);
            DateTime clampedTimestamp = new DateTime(clampedTicks);
            IEnumerable<Task> seekTasks = Readers.Select(reader => reader.SeekAsync(clampedTimestamp));
            Task seekAll = Task.WhenAll(seekTasks);
            TimeSpan updateInterval = TimeSpan.FromSeconds(2.5D);

            while (!seekAll.IsCompleted)
            {
                Console.WriteLine($"[Trend] Seeking to {timestamp:yyyy-MM-dd}...");
                await Task.WhenAny(seekAll, Task.Delay(updateInterval));
            }

            Initialized = true;
        }
    }
}
