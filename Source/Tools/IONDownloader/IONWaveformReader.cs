//******************************************************************************************************
//  IONWaveformReader.cs - Gbtc
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
//  07/24/2020 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Linq;
using System.Threading.Tasks;
using Gemstone.IONProtocol;
using Gemstone.IONProtocol.IONClasses;
using Gemstone.IONProtocol.IONStructureTypes;

namespace IONDownloader
{
    public class IONWaveformReader
    {
        public string InputName { get; }
        public string InputLabel { get; }
        public IONLogClass WaveformLog { get; }
        public IONWaveform? CurrentWaveform { get; private set; }

        private IONUInt CurrentPosition { get; set; }
        private IONUInt LogPosition { get; set; }
        private IONUInt LogRollover { get; set; }
        private bool Initialized { get; set; }

        public IONWaveformReader(string inputName, string inputLabel, IONLogClass waveformLog)
        {
            InputName = inputName;
            InputLabel = inputLabel;
            WaveformLog = waveformLog;
        }

        public async Task<bool> TryAdvanceAsync()
        {
            if (!Initialized)
            {
                LogPosition = await WaveformLog.ReadPositionAsync();
                LogRollover = await WaveformLog.ReadRolloverAsync();
            }

            IONUInt endPosition = (LogPosition != 0)
                ? (IONUInt)(LogPosition - 1)
                : LogRollover;

            if (!Initialized)
            {
                // Seek to the start of the log
                IONUInt startPosition = (LogRollover - LogPosition) < 100
                    ? 100 - (LogRollover - LogPosition + 1)
                    : LogPosition + 100;

                IONLogRecord startRecord = await GetRecordAsync(startPosition, endPosition);
                IONWaveform startWaveform = ToWaveform(startRecord);
                Initialize(startRecord.LogPosition, startWaveform);
                return true;
            }

            // The log position might have moved
            // while we were scanning the log
            if (CurrentPosition >= endPosition)
            {
                LogPosition = await WaveformLog.ReadPositionAsync();

                endPosition = (LogPosition != 0)
                    ? (IONUInt)(LogPosition - 1)
                    : LogRollover;
            }

            if (CurrentPosition < endPosition)
            {
                // Seek to the next record in the log
                IONUInt startPosition = (CurrentPosition < LogRollover)
                    ? CurrentPosition + 1
                    : 0;

                IONLogRecord startRecord = await GetRecordAsync(startPosition, endPosition);
                IONWaveform startWaveform = ToWaveform(startRecord);
                Initialize(startRecord.LogPosition, startWaveform);
                return true;
            }

            return false;
        }

        public async Task SeekAsync(IONTime timestamp)
        {
            LogPosition = await WaveformLog.ReadPositionAsync();
            LogRollover = await WaveformLog.ReadRolloverAsync();

            IONUInt startPosition = (LogRollover - LogPosition) < 100
                ? 100 - (LogRollover - LogPosition + 1)
                : LogPosition + 100;

            IONUInt endPosition = (LogPosition != 0)
                ? (IONUInt)(LogPosition - 1)
                : LogRollover;

            IONLogRecord startRecord = await GetRecordAsync(startPosition, endPosition);
            IONWaveform startWaveform = ToWaveform(startRecord);
            IONTime startTime = startWaveform.TimeOfFirstPoint;

            // The end position can be determined by reading the current
            // position of the waveform log itself, but the start position
            // must be determined by querying from a position before the start of
            // the log and checking the position of the log record that is returned
            startPosition = startRecord.LogPosition;

            if (timestamp <= startTime)
            {
                Initialize(startPosition, startWaveform);
                return;
            }

            IONLogRecord endRecord = await GetRecordAsync(endPosition, endPosition);
            IONWaveform endWaveform = ToWaveform(endRecord);
            IONTime endTime = endWaveform.TimeOfFirstPoint;

            if (timestamp >= endTime)
            {
                Initialize(endPosition, endWaveform);
                return;
            }

            while (true)
            {
                IONUInt midRange = (startPosition <= endPosition)
                    ? (endPosition - startPosition) / 2
                    : (LogRollover - startPosition + 1 + endPosition) / 2;

                IONUInt midPosition = (LogRollover - startPosition) < midRange
                    ? midRange - (LogRollover - startPosition + 1)
                    : startPosition + midRange;

                if (midPosition == startPosition)
                {
                    Initialize(startPosition, startWaveform);
                    return;
                }

                IONLogRecord midRecord = await GetRecordAsync(midPosition, endPosition);
                IONWaveform midWaveform = ToWaveform(midRecord);
                IONTime midTime = midWaveform.TimeOfFirstPoint;

                if (timestamp == midTime)
                {
                    Initialize(midPosition, midWaveform);
                    return;
                }

                if (timestamp < midTime)
                {
                    endPosition = midPosition;
                }
                else
                {
                    startPosition = midPosition;
                    startWaveform = midWaveform;
                }
            }
        }

        public void Reset()
        {
            CurrentPosition = default;
            CurrentWaveform = default;
            Initialized = default;
        }

        private void Initialize(IONUInt position, IONWaveform waveform)
        {
            CurrentPosition = position;
            CurrentWaveform = waveform;
            Initialized = true;
        }

        private async Task<IONLogRecord> GetRecordAsync(IONUInt start, IONUInt end)
        {
            IONRange range = new IONRange(start, end);
            IONLogArray logArray = await WaveformLog.ReadValueAsync(range);
            return logArray.First();
        }

        private IONWaveform ToWaveform(IONLogRecord record)
        {
            if (!(record.LogValue is IONWaveform waveform))
                throw new InvalidOperationException("Log record from waveform log does not contain a waveform");

            return waveform;
        }
    }
}
