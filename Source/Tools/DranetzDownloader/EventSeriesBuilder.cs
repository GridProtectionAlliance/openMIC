//******************************************************************************************************
//  EventSeriesBuilder.cs - Gbtc
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
//  01/29/2021 - C. Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using Gemstone.PQDIF.Logical;
using Gemstone.PQDIF.Physical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DranetzDowloader
{

    /// <summary>
    /// This builds a set of TrendSeries based on TrendDataRecords
    /// </summary>
    public class EventSeriesBuilder
    {


        private Dictionary<Tuple<Phase, DataClass, Guid>, EventSeries> m_series;

        public List<EventSeries> Series => m_series.Values.ToList();

        public DateTime Checkpoint;
        
        public EventSeriesBuilder()
        { 
            m_series = new Dictionary<Tuple<Phase, DataClass, Guid>, EventSeries>();
        }

        public void Build(List<EventDataRecord> records)
        {
            // Group into seperate Inputs
            Checkpoint = DateTime.MinValue;
            foreach (EventDataRecord record in records)
            {
                if (record.Time > Checkpoint)
                    Checkpoint = record.Time;

                switch (record.Input)
                {
                    
                    case (0):
                        AddPoint(Phase.AN,QuantityCharacteristic.Instantaneous,record, DataClass.Volts, "V waveform A");
                        break;
                    case (1):
                        AddPoint(Phase.BN, QuantityCharacteristic.Instantaneous, record, DataClass.Volts, "V waveform B");
                        break;
                    case (2):
                        AddPoint(Phase.CN, QuantityCharacteristic.Instantaneous, record, DataClass.Volts, "V waveform C");
                        break;
                    case (3):
                        AddPoint(Phase.AN, QuantityCharacteristic.Instantaneous, record, DataClass.Amps, "I waveform A");
                        break;
                    case (4):
                        AddPoint(Phase.BN, QuantityCharacteristic.Instantaneous, record, DataClass.Amps, "I waveform B");
                        break;
                    case (5):
                        AddPoint(Phase.CN, QuantityCharacteristic.Instantaneous, record, DataClass.Amps, "i waveform C");
                        break;
                    case (6):
                        AddPoint(Phase.NG, QuantityCharacteristic.Instantaneous, record, DataClass.Amps, "I waveform N");
                        break;
                }

            }
        }

        private EventSeries GetEventSeries(Phase phase, DataClass dataClass, Guid characteristic, string name)
        {
            Tuple<Phase, DataClass, Guid> key = new Tuple<Phase, DataClass, Guid>(phase, dataClass, characteristic);

            EventSeries series;
            if (m_series.TryGetValue(key, out series))
                return series;

            series = new EventSeries(phase, dataClass, characteristic, name);
            m_series.Add(key, series);

            return m_series[key];
        }

        private void AddPoint(Phase phase, Guid characteristic, EventDataRecord record, DataClass dataClass, string Name = "")
        {
            EventSeries series = GetEventSeries(phase, dataClass, characteristic, Name);
            series.AddPoint(record );

        }
    }

}
