//******************************************************************************************************
//  TrendSeriesBuilder.cs - Gbtc
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
//  01/19/2021 - C. Lackner
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
    public class TrendSeriesBuilder
    {


        private Dictionary<Tuple<Phase, DataClass, Guid>, TrendSeries> m_series;

        public List<TrendSeries> Series => m_series.Values.ToList();
        
        public TrendSeriesBuilder()
        { 
            m_series = new Dictionary<Tuple<Phase, DataClass, Guid>, TrendSeries>();
        }

        public void Build(List<TrendDataRecord> records)
        {
          
            foreach (TrendDataRecord record in records)
            {
                
                switch ((DataClass)record.Class)
                {
                    
                    case (DataClass.Volts):
                        AddPoint(Phase.AN,QuantityCharacteristic.RMS,0,record, "V RMS A");
                        AddPoint(Phase.BN, QuantityCharacteristic.RMS, 1, record, "V RMS B");
                        AddPoint(Phase.CN, QuantityCharacteristic.RMS, 2, record, "V RMS C");
                        AddPoint(Phase.General1, QuantityCharacteristic.RMS, 3, record, "V RMS D");
                        AddPoint(Phase.AB, QuantityCharacteristic.RMS, 4, record, "V RMS AB");
                        AddPoint(Phase.BC, QuantityCharacteristic.RMS, 5, record, "V RMS BC");
                        AddPoint(Phase.CA, QuantityCharacteristic.RMS, 6, record, "V RMS CA");

                        AddPoint(Phase.None, QuantityCharacteristic.Frequency, 72, record, "V FREQ");
                        AddPoint(Phase.Total, QuantityCharacteristic.S0S1, 71, record, "V S0/S1");
                        AddPoint(Phase.Total, QuantityCharacteristic.S2S1, 70, record, "V S2/S1");

                        AddPoint(Phase.PositiveSequence, QuantityCharacteristic.SPos, 68, record, "V S Pos All");
                        AddPoint(Phase.NegativeSequence, QuantityCharacteristic.SNeg, 69, record, "V S Neg All");
                        AddPoint(Phase.ZeroSequence, QuantityCharacteristic.SZero, 67, record, "V S Zero All");

                        AddPoint(Phase.AN, QuantityCharacteristic.AngleFund, 56, record, "V Phase Angle A");
                        AddPoint(Phase.BN, QuantityCharacteristic.AngleFund, 57, record, "V Phase Angle B");
                        AddPoint(Phase.CN, QuantityCharacteristic.AngleFund, 58, record, "V Phase Angle C");
                        AddPoint(Phase.General1, QuantityCharacteristic.AngleFund, 59, record, "V Phase Angle D");
                        AddPoint(Phase.AB, QuantityCharacteristic.AngleFund, 60, record, "V Phase Angle AB");
                        AddPoint(Phase.BC, QuantityCharacteristic.AngleFund, 61, record, "V Phase Angle BC");
                        AddPoint(Phase.CA, QuantityCharacteristic.AngleFund, 62, record, "V Phase Angle CA");


                        break;
                    case (DataClass.Amps):
                        AddPoint(Phase.AN, QuantityCharacteristic.RMS, 0, record, "I RMS A");
                        AddPoint(Phase.BN, QuantityCharacteristic.RMS, 1, record, "i RMS B");
                        AddPoint(Phase.CN, QuantityCharacteristic.RMS, 2, record, "I RMS C");
                        AddPoint(Phase.General1, QuantityCharacteristic.RMS, 3, record, "I RMS D");

                        AddPoint(Phase.Residual, QuantityCharacteristic.RMS, 45, record, "I RMS R");
                        AddPoint(Phase.Net, QuantityCharacteristic.RMS, 46, record, "I RMS NET");

                        AddPoint(Phase.Total, QuantityCharacteristic.S0S1, 43, record, "I S0/S1");
                        AddPoint(Phase.Total, QuantityCharacteristic.S2S1, 44, record, "I S2/S1");

                        AddPoint(Phase.PositiveSequence, QuantityCharacteristic.SPos, 41, record, "I S Pos All");
                        AddPoint(Phase.NegativeSequence, QuantityCharacteristic.SNeg, 42, record, "I S Neg All");
                        AddPoint(Phase.ZeroSequence, QuantityCharacteristic.SZero, 40, record, "I S Zero All");

                        AddPoint(Phase.AN, QuantityCharacteristic.AngleFund, 32, record, "I Phase Angle A");
                        AddPoint(Phase.BN, QuantityCharacteristic.AngleFund, 34, record, "I Phase Angle B");
                        AddPoint(Phase.CN, QuantityCharacteristic.AngleFund, 35, record, "I Phase Angle C");
                        AddPoint(Phase.General1, QuantityCharacteristic.AngleFund, 36, record, "I Phase Angle D");
                        


                        break;


                }

            }
        }

        private TrendSeries GetTrendSeries(Phase phase, DataClass dataClass, Guid characteristic, string name)
        {
            Tuple<Phase, DataClass, Guid> key = new Tuple<Phase, DataClass, Guid>(phase, dataClass, characteristic);

            TrendSeries series;
            if (m_series.TryGetValue(key, out series))
                return series;

            series = new TrendSeries(phase, dataClass, characteristic, name);
            m_series.Add(key, series);

            return m_series[key];
        }

        private void AddPoint(Phase phase, Guid characteristic, int index, TrendDataRecord record, string Name = "")
        {
            TrendSeries series = GetTrendSeries(phase, (DataClass)record.Class, characteristic, Name);
            series.AddPoint(record.Time, record.Value[index], (AggregateType)record.Aggregate);

        }
    }

}
