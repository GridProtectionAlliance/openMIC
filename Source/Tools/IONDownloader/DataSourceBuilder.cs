//******************************************************************************************************
//  DataSourceBuilder.cs - Gbtc
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

using System.Linq;
using Gemstone.PQDIF.Logical;

namespace IONDownloader
{
    public class DataSourceBuilder
    {
        public DataSourceRecord DataSource { get; }

        public DataSourceBuilder(string dataSourceName) =>
            DataSource = DataSourceRecord.CreateDataSourceRecord(dataSourceName);

        public void InsertTrendChannel(PQDIFTrendQuantity trendQuantity)
        {
            string channelName = trendQuantity.ChannelName;

            ChannelDefinition? channelDefinition = DataSource.ChannelDefinitions
                .FirstOrDefault(definition => definition.ChannelName == channelName);

            if (channelDefinition is null)
            {
                channelDefinition = DataSource.AddNewChannelDefinition();
                channelDefinition.ChannelName = trendQuantity.ChannelName;
                channelDefinition.QuantityTypeID = QuantityType.ValueLog;
                channelDefinition.QuantityMeasured = trendQuantity.QuantityMeasured;
                channelDefinition.Phase = trendQuantity.Phase;

                SeriesDefinition timeDefinition = channelDefinition.AddNewSeriesDefinition();
                timeDefinition.QuantityCharacteristicID = QuantityCharacteristic.Instantaneous;
                timeDefinition.ValueTypeID = SeriesValueType.Time;
                timeDefinition.QuantityUnits = QuantityUnits.Seconds;
                timeDefinition.StorageMethodID = StorageMethods.Values | StorageMethods.Scaled;
            }

            SeriesDefinition valuesDefinition = channelDefinition.AddNewSeriesDefinition();
            valuesDefinition.QuantityCharacteristicID = trendQuantity.QuantityCharacteristicID;
            valuesDefinition.ValueTypeID = trendQuantity.ValueTypeID;
            valuesDefinition.StorageMethodID = StorageMethods.Scaled | StorageMethods.Values;

            valuesDefinition.QuantityUnits = channelDefinition.QuantityMeasured switch
            {
                QuantityMeasured.Voltage => QuantityUnits.Volts,
                QuantityMeasured.Current => QuantityUnits.Amps,
                _ => QuantityUnits.None
            };
        }

        public void InsertWaveformChannel(string inputHandleName, string inputHandleLabel)
        {
            ChannelDefinition channelDefinition = DataSource.AddNewChannelDefinition();
            channelDefinition.ChannelName = inputHandleLabel;
            channelDefinition.QuantityTypeID = QuantityType.WaveForm;
            channelDefinition.QuantityMeasured = GetQuantityMeasured(inputHandleName);
            channelDefinition.Phase = GetPhase(inputHandleName);
        }

        private QuantityMeasured GetQuantityMeasured(string waveformName)
        {
            return waveformName switch
            {
                "V1" => QuantityMeasured.Voltage,
                "V2" => QuantityMeasured.Voltage,
                "V3" => QuantityMeasured.Voltage,
                "I1" => QuantityMeasured.Current,
                "I2" => QuantityMeasured.Current,
                "I3" => QuantityMeasured.Current,
                _ => QuantityMeasured.None
            };
        }

        private Phase GetPhase(string waveformName)
        {
            return waveformName switch
            {
                "V1" => Phase.AN,
                "V2" => Phase.BN,
                "V3" => Phase.CN,
                "I1" => Phase.AN,
                "I2" => Phase.BN,
                "I3" => Phase.CN,
                _ => Phase.None
            };
        }
    }
}
