//******************************************************************************************************
//  ObservationBuilder.cs - Gbtc
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
using System.Collections.Generic;
using System.Linq;
using Gemstone.IONProtocol;
using Gemstone.IONProtocol.IONStructureTypes;
using Gemstone.PQDIF.Logical;
using Gemstone.PQDIF.Physical;

namespace IONDownloader
{
    public class ObservationBuilder
    {
        public ObservationRecord Observation { get; }

        public ObservationBuilder(DataSourceRecord dataSource, MonitorSettingsRecord? settings) =>
            Observation = ObservationRecord.CreateObservationRecord(dataSource, settings);

        public void InsertTrendingTimestamps(DateTime date, string channelName, IList<DateTime> timestamps)
        {
            if (!Observation.ChannelInstances.Any())
                Observation.StartTime = date;

            ChannelInstance channelInstance = GetChannelInstance(channelName);
            SeriesInstance timeSeries = channelInstance.AddNewSeriesInstance();
            timeSeries.SeriesScale = new ScalarElement();
            timeSeries.SeriesScale.TypeOfValue = PhysicalType.Real4;
            timeSeries.SeriesScale.SetReal4(60);

            timeSeries.SeriesValues.TypeOfValue = PhysicalType.Integer2;
            timeSeries.SeriesValues.Size = timestamps.Count;

            for (int i = 0; i < timestamps.Count; i++)
            {
                TimeSpan diff = timestamps[i] - date;
                short minutes = (short)Math.Round(diff.TotalMinutes);
                timeSeries.SeriesValues.SetInt2(i, minutes);
            }
        }

        public void InsertTrendingData(PQDIFTrendQuantity trendQuantity, IList<IONTrendPoint> points, double scale)
        {
            ChannelInstance channelInstance = GetChannelInstance(trendQuantity.ChannelName);

            if (!(trendQuantity.HarmonicGroup is null))
                channelInstance.ChannelGroupID = trendQuantity.HarmonicGroup.GetValueOrDefault();

            SeriesInstance valueSeries = channelInstance.AddNewSeriesInstance();
            valueSeries.SeriesScale = new ScalarElement();
            valueSeries.SeriesScale.TypeOfValue = PhysicalType.Real8;
            valueSeries.SeriesScale.Set(scale);

            valueSeries.SeriesValues.TypeOfValue = GetPhysicalType(points.First().Numeric);
            valueSeries.SeriesValues.Size = points.Count;

            for (int i = 0; i < points.Count; i++)
                valueSeries.SeriesValues.Set(i, points[i].Numeric.Value);
        }

        public void InsertWaveform(string label, IONWaveform waveform)
        {
            IONNumericArray? samples = waveform.SamplePoints;

            if (samples is null)
                return;

            double timeOffset = 0.0D;

            if (!Observation.ChannelInstances.Any())
            {
                Observation.StartTime = waveform.TimeOfFirstPoint;
            }
            else
            {
                TimeSpan diff = waveform.TimeOfFirstPoint - Observation.StartTime;
                timeOffset = diff.TotalSeconds;
            }

            ChannelDefinition channelDefinition = GetChannelDefinition(label);
            ChannelInstance channelInstance = Observation.AddNewChannelInstance(channelDefinition);
            SeriesDefinition timeDefinition = channelDefinition.AddNewSeriesDefinition();
            SeriesInstance timeSeries = channelInstance.AddNewSeriesInstance();
            timeDefinition.QuantityCharacteristicID = QuantityCharacteristic.Instantaneous;
            timeDefinition.ValueTypeID = SeriesValueType.Time;
            timeDefinition.QuantityUnits = QuantityUnits.Seconds;
            timeDefinition.StorageMethodID = StorageMethods.Increment | StorageMethods.Scaled;

            timeSeries.SeriesScale = new ScalarElement();
            timeSeries.SeriesScale.TypeOfValue = PhysicalType.Real8;
            timeSeries.SeriesScale.Set(1.0D / waveform.SamplingFrequency.Value);

            timeSeries.SeriesOffset = new ScalarElement();
            timeSeries.SeriesOffset.TypeOfValue = PhysicalType.Real8;
            timeSeries.SeriesOffset.Set(timeOffset);

            timeSeries.SeriesValues.TypeOfValue = PhysicalType.Integer4;
            timeSeries.SeriesValues.Size = 3;
            timeSeries.SeriesValues.SetInt4(0, 1);
            timeSeries.SeriesValues.SetInt4(1, samples.Length);
            timeSeries.SeriesValues.SetInt4(2, 1);

            SeriesDefinition valuesDefinition = channelDefinition.AddNewSeriesDefinition();
            SeriesInstance valueSeries = channelInstance.AddNewSeriesInstance();
            valuesDefinition.QuantityCharacteristicID = QuantityCharacteristic.Instantaneous;
            valuesDefinition.ValueTypeID = SeriesValueType.Val;
            valuesDefinition.StorageMethodID = StorageMethods.Scaled | StorageMethods.Values;

            valuesDefinition.QuantityUnits = channelDefinition.QuantityMeasured switch
            {
                QuantityMeasured.Voltage => QuantityUnits.Volts,
                QuantityMeasured.Current => QuantityUnits.Amps,
                _ => QuantityUnits.None
            };

            IONNumeric scale = waveform.Scale;
            valueSeries.SeriesScale = new ScalarElement();
            valueSeries.SeriesScale.TypeOfValue = GetPhysicalType(scale);
            valueSeries.SeriesScale.Set(scale.Value);

            IONNumeric offset = waveform.Offset;
            valueSeries.SeriesOffset = new ScalarElement();
            valueSeries.SeriesOffset.TypeOfValue = GetPhysicalType(offset);
            valueSeries.SeriesOffset.Set(scale.Value * offset.Value);

            valueSeries.SeriesValues.TypeOfValue = GetPhysicalType(samples);
            valueSeries.SeriesValues.Size = samples.Length;

            for (int i = 0; i < samples.Length; i++)
                valueSeries.SeriesValues.Set(i, samples[i].Value);
        }

        private ChannelInstance GetChannelInstance(string label)
        {
            ChannelInstance? channelInstance = Observation.ChannelInstances
                .FirstOrDefault(instance => instance.Definition.ChannelName == label);

            if (channelInstance == null)
            {
                ChannelDefinition channelDefinition = GetChannelDefinition(label);
                channelInstance = Observation.AddNewChannelInstance(channelDefinition);
            }

            return channelInstance;
        }

        private ChannelDefinition GetChannelDefinition(string label) => Observation.DataSource.ChannelDefinitions
            .First(definition => definition.ChannelName == label);

        private static PhysicalType GetPhysicalType(IONNumeric numeric)
        {
            return (numeric.BaseType, numeric.BitSize) switch
            {
                (IONType.Bool, _) => PhysicalType.Boolean1,
                (IONType.Int, 8) => PhysicalType.Integer1,
                (IONType.Int, 16) => PhysicalType.Integer2,
                (IONType.Int, 32) => PhysicalType.Integer4,
                (IONType.UInt, 8) => PhysicalType.UnsignedInteger1,
                (IONType.UInt, 16) => PhysicalType.UnsignedInteger2,
                (IONType.UInt, 32) => PhysicalType.UnsignedInteger4,
                (IONType.Float, _) => PhysicalType.Real4,
                _ => throw new ArgumentOutOfRangeException(nameof(numeric))
            };
        }

        private static PhysicalType GetPhysicalType(IONNumericArray array)
        {
            return (array.BaseType, array.ElementSize) switch
            {
                (IONType.Bool, _) => PhysicalType.Boolean1,
                (IONType.Int, 1) => PhysicalType.Integer1,
                (IONType.Int, 2) => PhysicalType.Integer2,
                (IONType.Int, 4) => PhysicalType.Integer4,
                (IONType.UInt, 1) => PhysicalType.UnsignedInteger1,
                (IONType.UInt, 2) => PhysicalType.UnsignedInteger2,
                (IONType.UInt, 4) => PhysicalType.UnsignedInteger4,
                (IONType.Float, _) => PhysicalType.Real4,
                _ => throw new ArgumentOutOfRangeException(nameof(array))
            };
        }
    }
}
