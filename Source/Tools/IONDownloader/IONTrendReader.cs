//******************************************************************************************************
//  IONTrendReader.cs - Gbtc
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
using Gemstone.IONProtocol.IONClasses;
using Gemstone.IONProtocol.IONStructureTypes;

namespace IONDownloader
{
    public class IONTrendReader
    {
        public IONUIntArray InputHandles { get; }
        public IONLogClass DataRecorderLog { get; }

        public IONTime CurrentDate => CurrentTimestamp.ToDateTime().Date;
        public IONTime CurrentTimestamp => LogArray?[CurrentIndex].Timestamp ?? DateTime.MinValue;

        public IEnumerable<IONTrendPoint> CurrentPoints => (LogArray is null)
            ? Enumerable.Empty<IONTrendPoint>()
            : ToTrendingData(LogArray[CurrentIndex]);

        private int CurrentIndex { get; set; }
        private IONLogArray? LogArray { get; set; }
        private IONUInt CurrentPosition => LogArray?[CurrentIndex].LogPosition ?? 0;
        private IONUInt LogPosition { get; set; }
        private IONUInt LogRollover { get; set; }

        public IONTrendReader(IONUIntArray inputHandles, IONLogClass dataRecorderLog)
        {
            InputHandles = inputHandles;
            DataRecorderLog = dataRecorderLog;
        }

        public async Task<bool> TryAdvanceAsync()
        {
            if (LogArray is null)
            {
                LogPosition = await DataRecorderLog.ReadPositionAsync();
                LogRollover = await DataRecorderLog.ReadRolloverAsync();
            }

            IONUInt endPosition = (LogPosition != 0)
                ? (IONUInt)(LogPosition - 1)
                : LogRollover;

            if (LogArray is null)
            {
                // Seek to the start of the log
                IONUInt startPosition = (LogRollover - LogPosition) < 100
                    ? 100 - (LogRollover - LogPosition + 1)
                    : LogPosition + 100;

                IONLogArray startArray = await GetRecordsAsync(startPosition, endPosition);
                Initialize(startArray);
                return true;
            }

            if (CurrentIndex + 1 < LogArray.Length)
            {
                // Simply advance to the
                // next log in the array
                CurrentIndex++;
                return true;
            }

            IONUInt currentPosition = CurrentPosition;

            // The log position might have moved
            // while we were scanning the log
            if (currentPosition >= endPosition)
            {
                LogPosition = await DataRecorderLog.ReadPositionAsync();

                endPosition = (LogPosition != 0)
                    ? (IONUInt)(LogPosition - 1)
                    : LogRollover;
            }

            if (currentPosition < endPosition)
            {
                // Seek to the next record in the log
                IONUInt startPosition = (currentPosition < LogRollover)
                    ? currentPosition + 1
                    : 0;

                IONLogArray startArray = await GetRecordsAsync(startPosition, endPosition);
                Initialize(startArray);
                return true;
            }

            return false;
        }

        public async Task SeekAsync(IONTime timestamp)
        {
            LogPosition = await DataRecorderLog.ReadPositionAsync();
            LogRollover = await DataRecorderLog.ReadRolloverAsync();

            IONUInt startPosition = (LogRollover - LogPosition) < 100
                ? 100 - (LogRollover - LogPosition + 1)
                : LogPosition + 100;

            IONUInt endPosition = (LogPosition != 0)
                ? (IONUInt)(LogPosition - 1)
                : LogRollover;

            IONLogArray startArray = await GetRecordsAsync(startPosition, endPosition);
            IONLogRecord startRecord = startArray.First();
            IONTime startTime = startRecord.Timestamp;

            // The end position can be determined by reading the current
            // position of the data recorder log itself, but the start position
            // must be determined by querying from a position before the start of
            // the log and checking the position of the log record that is returned
            startPosition = startRecord.LogPosition;

            if (timestamp <= startTime)
            {
                Initialize(startArray);
                return;
            }

            IONLogArray endArray = await GetRecordsAsync(endPosition, endPosition);
            IONLogRecord endRecord = endArray.First();
            IONTime endTime = endRecord.Timestamp;

            if (timestamp >= endTime)
            {
                Initialize(endArray);
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
                    Initialize(startArray);
                    return;
                }

                IONLogArray midArray = await GetRecordsAsync(midPosition, midPosition);
                IONLogRecord midRecord = midArray.First();
                IONTime midTime = midRecord.Timestamp;

                if (timestamp == midTime)
                {
                    Initialize(midArray);
                    return;
                }

                if (timestamp < midTime)
                {
                    endPosition = midPosition;
                }
                else
                {
                    startArray = midArray;
                    startPosition = midPosition;
                }
            }
        }

        public void Reset()
        {
            CurrentIndex = default;
            LogArray = default;
        }

        private void Initialize(IONLogArray logArray)
        {
            CurrentIndex = 0;
            LogArray = logArray;
        }

        private async Task<IONLogArray> GetRecordsAsync(IONUInt start, IONUInt end)
        {
            IONRange range = new IONRange(start, end);
            return await DataRecorderLog.ReadValueAsync(range);
        }

        private IEnumerable<IONTrendPoint> ToTrendingData(IONLogRecord record)
        {
            static bool IsNumeric(IIONValue value) => value switch
            {
                IONBool _ => true,
                IONInt _ => true,
                IONUInt _ => true,
                IONFloat _ => true,
                IONNumeric _ => true,
                _ => false
            };

            static IONNumeric ToNumeric(IIONValue value) => value switch
            {
                IONBool ionBool => ionBool,
                IONInt ionInt => ionInt,
                IONUInt ionUInt => ionUInt,
                IONFloat ionFloat => ionFloat,
                IONNumeric ionNumeric => ionNumeric,
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };

            if (!(record.LogValue is IONList list))
                throw new InvalidOperationException("Log record from data recorder does not contain a list of values");

            IONTime timestamp = record.Timestamp;

            return InputHandles
                .Zip(list, (input, value) => new { input, timestamp, value })
                .Where(obj => IsNumeric(obj.value))
                .Select(obj => new IONTrendPoint(obj.input, obj.timestamp, ToNumeric(obj.value)));
        }
    }
}
