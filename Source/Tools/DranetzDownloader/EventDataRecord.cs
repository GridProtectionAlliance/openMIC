//******************************************************************************************************
//  EventDataRecord.cs - Gbtc
//
//  Copyright © 2021, Grid Protection Alliance.  All Rights Reserved.
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
//  01/12/2021 - C. Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using Gemstone.PQDIF.Logical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DranetzDowloader
{

    /// <summary>
    /// This represents a Cycle EventDataRecord as provided by the Meter
    /// </summary>
    public class EventDataRecord
    {
        private Record m_record;

        private List<int> data;
        private double dT;
        private double scaling;
        private uint microsec;
        public int Input { get; set; }

        public int Cycle { get; set; }

        public DateTime Time { get { return m_record.Time.AddTicks(microsec *10); } }

        public List<double> Value { get { return data.Select(d => scaling * d).ToList(); } }
        public List<DateTime> TS { get { return Enumerable.Range(0, data.Count).Select(item => Time.AddSeconds(item * dT)).ToList(); } }
        public EventDataRecord(Record record)
        {
            m_record = record;
            ParseData();
        }

        private void ParseData()
        {
            int count = ((m_record.Size - 32)/2);
            data = new List<int>();
            Input = Parse16Int(m_record.Data, 22);
            dT = Parse32Float(m_record.Data, 24);
            scaling = Parse32Float(m_record.Data, 28);
            microsec = Parse32Int(m_record.Data, 12);
            Cycle = (int)Parse32Int(m_record.Data, 16);

            for (int i = 0; i < count; i++)
            {
                data.Add(Parse16Int(m_record.Data, i * 2 + 32));
            }
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

        private static float Parse32Float(byte[] data, int start)
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

            return BitConverter.ToSingle(adjData, 0);

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
}
