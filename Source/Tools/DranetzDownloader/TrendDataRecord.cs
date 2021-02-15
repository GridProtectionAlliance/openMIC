//******************************************************************************************************
//  TrendDataRecord.cs - Gbtc
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
    /// This represents a TrendDataRecord as provided by the Meter
    /// </summary>
    public class TrendDataRecord
    {
        private const int dataIndex = 148;
        private Record m_record;

        private List<double> data;
        public int Class { get; set; }
        public int Aggregate { get; set; }
        public DateTime Time { get { return m_record.Time; } }

        public List<double> Value => data;
        public TrendDataRecord(Record record)
        {
            m_record = record;

            Class = (int)Parse16Int(m_record.Data, 20);
            Aggregate = (int)Parse16Int(m_record.Data, 22);

            ParseData();
        }

        private void ParseData()
        {
            int count = GetCount();
            data = new List<double>();
           
            for (int i = 0; i < count; i++)
            {
                data.Add((double)Parse32Float(m_record.Data, i * 4 + dataIndex));
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

        private int GetCount()
        {
            
            if (Class == (int)DataClass.Volts)
                return 78;
            if (Class == (int)DataClass.Amps)
                return 47;


            return 0;
        }

       
    }

    /// <summary>
    /// This represents the Data Class of a Journal Record. 
    /// </summary>
    public enum DataClass
    {
        FlkrST = 7,
        Harm = 3,
        DmD = 4,
        Eng = 5,
        Amps = 1,
        Power = 2,
        Volts = 0,
    }

    /// <summary>
    /// This represents the Aggregate Type (Min Max Avg PRS?) for Journal record
    /// </summary>
    public enum AggregateType
    {
        PRS = 0,
        Max = 1,
        Avg = 3,
        Min = 2,
    }
}
