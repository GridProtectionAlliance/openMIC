//******************************************************************************************************
//  WaveformReaderCollection.cs - Gbtc
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
//  07/25/2020 - Stephen C. Wills
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
    public class WaveformReaderCollection
    {
        private IEnumerable<IONWaveformReader> Readers { get; }

        private IEnumerable<IONWaveformReader> ActiveReaders { get; set; } =
            Enumerable.Empty<IONWaveformReader>();

        private IEnumerable<IONWaveformReader> AlignedReaders => ActiveReaders
            .Where(reader => !(reader.CurrentWaveform is null))
            .GroupBy(reader => (DateTime)reader.CurrentWaveform!.TimeOfFirstPoint)
            .OrderBy(grouping => grouping.Key)
            .FirstOrDefault() ?? Enumerable.Empty<IONWaveformReader>();

        public WaveformReaderCollection(IEnumerable<IONWaveformReader> readers) =>
            Readers = readers;

        public async IAsyncEnumerable<IONWaveformReader> AdvanceAsync()
        {
            if (!ActiveReaders.Any())
            {
                ActiveReaders = await Readers
                    .ToAsyncEnumerable()
                    .WhereAwait(async reader => await reader.TryAdvanceAsync())
                    .ToListAsync();
            }
            else
            {
                IEnumerable<IONWaveformReader> aligned = AlignedReaders.ToList();
                IEnumerable<IONWaveformReader> unaligned = ActiveReaders.Except(aligned).ToList();

                ActiveReaders = await aligned
                    .ToAsyncEnumerable()
                    .WhereAwait(async reader => await reader.TryAdvanceAsync())
                    .Concat(unaligned.ToAsyncEnumerable())
                    .ToListAsync();
            }

            foreach (IONWaveformReader reader in AlignedReaders)
                yield return reader;
        }

        public async IAsyncEnumerable<IONWaveformReader> SeekAsync(DateTime timestamp)
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
                Console.WriteLine($"[Event] Seeking to {timestamp:yyyy-MM-dd}...");
                await Task.WhenAny(seekAll, Task.Delay(updateInterval));
            }

            ActiveReaders = Readers;

            foreach (IONWaveformReader reader in AlignedReaders)
                yield return reader;
        }
    }
}
