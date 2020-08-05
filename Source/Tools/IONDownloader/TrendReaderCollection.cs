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

            async IAsyncEnumerable<IONTrendPoint> QueryAsync(IONTrendReader reader)
            {
                while (reader.CurrentDate <= date)
                {
                    if (reader.CurrentDate == date)
                    {
                        foreach (IONTrendPoint point in reader.CurrentPoints)
                            yield return point;
                    }

                    if (!await reader.TryAdvanceAsync())
                        break;
                }
            }

            IEnumerable<Task<List<IONTrendPoint>>> queryTasks = ActiveReaders
                .Select(QueryAsync)
                .Select(points => points.ToListAsync().AsTask());

            foreach (List<IONTrendPoint> points in await Task.WhenAll(queryTasks))
            {
                foreach (IONTrendPoint point in points)
                    yield return point;
            }

            CurrentDate = date;
        }

        public async Task SeekAsync(DateTime timestamp)
        {
            foreach (IONTrendReader reader in Readers)
                await reader.SeekAsync(timestamp);
        }
    }
}
