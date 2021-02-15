//******************************************************************************************************
//  Record.cs - Gbtc
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
//  08/21/2020 - C. Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DranetzDowloader
{

    /// <summary>
    /// This represents a Record of data with 1 timestamp and multiple values
    /// </summary>
    public class Record
    {
        public DateTime Time { get; set; }
        public int Type { get; set; }
        public byte[] Data { get; set; }

        /// <summary>
        /// The Length of the Record (bytes).
        /// </summary>
        public int Size {get; set; }

        /// <summary>
        /// The recordID from the meter.
        /// </summary>
        public int Recordnumber { get; set; }

        /// <summary>
        /// The cycle this data is taken from (only valid for EventRecords).
        /// </summary>
        public int Cycle { get; set; }

        public static List<Record> Parse(byte[] data)
        {
            List<Record> records = new List<Record>();
            int index = 0;
            DateTime epoch = new DateTime(2000, 1, 1);
            while (index < data.Length)
            {
                int length = (int)Parse16Int(data,index);
                byte[] record = new byte[length];
                index = index + 2;

                Buffer.BlockCopy(data, index, record, 0, length);

                int type = (int)Parse32Int(record, 4);
                records.Add(new Record()
                {
                    Size = length,
                    Data = record,
                    Recordnumber = (int)Parse32Int(record, 0),
                    Type = type,
                    Time = epoch.Add(new TimeSpan(0, 0, (int)Parse32Int(record, 8))),
                    Cycle = (type == (int)RecordType.HC_Wave? (int)Parse32Int(record, 16) : 0)
                });
                index = index + length;
            }

            return records;
        }

        private static uint Parse32Int(byte[] data, int start)
        {
            byte[] adjData = new byte[4];
            if (BitConverter.IsLittleEndian)
            {
                adjData[0] = data[start];
                adjData[1] = data[start + 1];
                adjData[2] = data[start + 2];
                adjData[3] = data[start + 3];

            }
            else
            {
                adjData[0] = data[start + 3];
                adjData[1] = data[start + 2];
                adjData[2] = data[start + 1];
                adjData[3] = data[start];
            }

            return BitConverter.ToUInt32(adjData, 0);

        }

        private static ushort Parse16Int(byte[] data, int start)
        {
            byte[] adjData = new byte[2];
            if (BitConverter.IsLittleEndian)
            {
                adjData[1] = data[start + 1];
                adjData[0] = data[start];
            }
            else
            {
                adjData[0] = data[start + 1];
                adjData[1] = data[start];
            }

            return BitConverter.ToUInt16(adjData, 0);

        }
    }

    /// <summary>
    /// This represents the Type of Record. Generally HC_sum and HC_Wave are event data and JOURNAL is trending data
    /// </summary>
    public enum RecordType
    {
        // Confirmed
        Journal = 4,
        Harm_200 = 5,

        // N/A
        SYS_Event = 16,

        // Unconfirmed
        HC_Sum = 0,
        HC_Wave = 1,
        //SYS_Event = 5
    }
}
