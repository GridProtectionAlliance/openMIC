//******************************************************************************************************
//  TrendSeries.cs - Gbtc
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
    /// This represents a EventData Series for PQDIF
    /// </summary>
    public class EventSeries
    {


        private List<double> data;
        private List<DateTime> time;

        private QuantityMeasured quantityMeasured;

        public DataClass dataClass;
        public Phase phase;

        private Guid quantityCharacteristic;
        private string m_Name = string.Empty;
        private string Name => (string.IsNullOrEmpty(m_Name)? quantityMeasured.ToString() + " " + QuantityCharacteristic.ToString(quantityCharacteristic) + " " + phase.ToString(): m_Name );

        
        public EventSeries(Phase Phase, DataClass DataClass, Guid QCharacteristic, string name): this(Phase, DataClass, QCharacteristic)
        { 
            m_Name = name;
        }
        public EventSeries(Phase Phase, DataClass DataClass, Guid QCharacteristic)
        {

            phase = Phase;

            dataClass = DataClass;
            quantityMeasured = QuantityMeasured.None;
            switch (DataClass)
            {
                case (DataClass.Volts):
                    quantityMeasured = QuantityMeasured.Voltage;
                    break;
                case (DataClass.Amps):
                    quantityMeasured = QuantityMeasured.Current;
                    break;

            }
            quantityCharacteristic = QCharacteristic;
            data = new List<double>();
            time = new List<DateTime>();
        }

        public void AddPoint(EventDataRecord record)
        {
            data.AddRange(record.Value);
            time.AddRange(record.TS);
        }


        /// <summary>
        /// Creates the PQDIF Channeldefinitions
        /// </summary>
        /// <param name="dataSource"></param>
        public void AddChannelDefinitions(DataSourceRecord dataSource)
        {

            if (data.Count == 0)
                return;

            ChannelDefinition channelDefinition = dataSource.AddNewChannelDefinition();
            channelDefinition.QuantityMeasured = quantityMeasured;
            channelDefinition.Phase = phase;
            channelDefinition.ChannelName = Name;
            channelDefinition.QuantityTypeID = QuantityType.ValueLog;

            SeriesDefinition timeDefinition = channelDefinition.AddNewSeriesDefinition();
            timeDefinition.QuantityCharacteristicID = QuantityCharacteristic.Instantaneous;
            timeDefinition.ValueTypeID = SeriesValueType.Time;
            timeDefinition.QuantityUnits = QuantityUnits.Seconds;
            timeDefinition.StorageMethodID = StorageMethods.Values | StorageMethods.Scaled;

            CreateSeriesDef(channelDefinition);
                      
        }

        private void CreateSeriesDef(ChannelDefinition channelDefinition)
        {
            SeriesDefinition valuesDefinition = channelDefinition.AddNewSeriesDefinition();
            valuesDefinition.QuantityCharacteristicID = quantityCharacteristic;
            valuesDefinition.ValueTypeID = SeriesValueType.Val;
            valuesDefinition.StorageMethodID = StorageMethods.Scaled | StorageMethods.Values;

            valuesDefinition.QuantityUnits = channelDefinition.QuantityMeasured switch
            {
                QuantityMeasured.Voltage => QuantityUnits.Volts,
                QuantityMeasured.Current => QuantityUnits.Amps,
                _ => QuantityUnits.None
            };
        }

        public void AddData(ObservationRecord observation)
        {
            if (data.Count == 0)
                return;

            AddTimeSeries(observation);
            
            AddValueSeries(observation);
            
        }

        private void AddTimeSeries(ObservationRecord observation)
        {
            DateTime date;
            if (observation.ChannelInstances.Any())
                date = observation.StartTime;
            else
            {
                date = time.Min();
                observation.StartTime = date;
            }

            ChannelInstance channelInstance = GetChannelInstance(observation);
            SeriesInstance? timeSeries = channelInstance.SeriesInstances.FirstOrDefault(item => item.Definition.ValueTypeID == SeriesValueType.Time);
            if (timeSeries == null)
            {
                timeSeries = channelInstance.AddNewSeriesInstance();
                timeSeries.SeriesScale = new ScalarElement();
                timeSeries.SeriesScale.TypeOfValue = PhysicalType.Real4;
                timeSeries.SeriesScale.SetReal4(60);

                timeSeries.SeriesValues.TypeOfValue = PhysicalType.Integer2;
                timeSeries.SeriesValues.Size = data.Count;

                for (int j = 0; j < data.Count; j++)
                {
                    TimeSpan diff = time[j] - date;
                    short minutes = (short)Math.Round(diff.TotalMinutes);
                    timeSeries.SeriesValues.SetInt2(j, minutes);
                }
            }
            
        }

        private void AddValueSeries(ObservationRecord observation)
        {
           
                ChannelInstance channelInstance = GetChannelInstance(observation);

                SeriesInstance valueSeries = channelInstance.AddNewSeriesInstance();
                valueSeries.SeriesScale = new ScalarElement();
                valueSeries.SeriesScale.TypeOfValue = PhysicalType.Real8;
                valueSeries.SeriesScale.Set(1.0);

                valueSeries.SeriesValues.TypeOfValue = PhysicalType.Real4;
                valueSeries.SeriesValues.Size = data.Count;

                for (int j = 0; j < data.Count; j++)
                    valueSeries.SeriesValues.Set(j, data[j]);
            
        }
        private ChannelInstance GetChannelInstance(ObservationRecord observation)
        {
            ChannelInstance? channelInstance = observation.ChannelInstances
                .FirstOrDefault(instance => instance.Definition.Phase == phase && instance.Definition.QuantityMeasured == quantityMeasured && instance.Definition.ChannelName == Name);

            if (channelInstance == null)
            {
                ChannelDefinition channelDefinition = GetChannelDefinition(observation);
                channelInstance = observation.AddNewChannelInstance(channelDefinition);
            }

            return channelInstance;
        }

        private ChannelDefinition GetChannelDefinition(DataSourceRecord dataSource) => dataSource.ChannelDefinitions
                .FirstOrDefault(definition => definition.Phase == phase && definition.QuantityMeasured == quantityMeasured & definition.ChannelName == Name);

        private ChannelDefinition GetChannelDefinition(ObservationRecord observation) => GetChannelDefinition(observation.DataSource);

        
    }

}
