//******************************************************************************************************
//  TrendPoller.cs - Gbtc
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gemstone.Communication;
using Gemstone.Data;
using Gemstone.IONProtocol;
using Gemstone.IONProtocol.IONClasses;
using Gemstone.PQDIF.Logical;
using Gemstone.PQDIF.Physical;
using Gemstone.StringExtensions;

namespace IONDownloader
{
    public class TrendPoller
    {
        private Dictionary<uint, TrendChannelMapping> ChannelMap { get; }
        private PollingOptions PollingOptions { get; }

        private string DeviceAcronym => PollingOptions.DeviceAcronym
            ?? throw new ArgumentNullException(nameof(PollingOptions.DeviceAcronym));

        private string DestinationFolder => PollingOptions.DestinationFolder
            ?? throw new ArgumentNullException(nameof(PollingOptions.DestinationFolder));

        public TrendPoller(Dictionary<uint, TrendChannelMapping> channelMap, PollingOptions pollingOptions)
        {
            ChannelMap = channelMap;
            PollingOptions = pollingOptions;
        }

        public async Task PollAsync()
        {
            using IONMasterStation masterStation = new IONMasterStation
            {
                ConnectionString = PollingOptions.IONConnectionString,
                Protocol = TransportProtocol.Tcp
            };

            TaskCompletionSource<object?> connected = new TaskCompletionSource<object?>();
            masterStation.ConnectionEstablished += (sender, args) => connected.SetResult(null);

            masterStation.ProcessException += (sender, args) =>
            {
                if (!connected.Task.IsCompleted)
                    connected.SetException(args.Argument);
            };

            masterStation.Connect();
            await connected.Task;

            List<IONTrendReader> readerList = await GetTrendReadersAsync(masterStation)
                .ToListAsync();

            TrendReaderCollection trendReaders = new TrendReaderCollection(readerList);

            static DateTime Max(DateTime time1, DateTime time2) =>
                new DateTime(Math.Max(time1.Ticks, time2.Ticks));

            if (PollingOptions.DownloadAfter > DateTime.MinValue && PollingOptions.StartFromCheckpoint)
                await trendReaders.SeekAsync(Max(PollingOptions.DownloadAfter, GetCheckpoint()));
            else if (PollingOptions.DownloadAfter > DateTime.MinValue)
                await trendReaders.SeekAsync(PollingOptions.DownloadAfter);
            else if (PollingOptions.StartFromCheckpoint)
                await trendReaders.SeekAsync(GetCheckpoint());

            Lazy<DataSourceRecord> dataSourceFactory = new Lazy<DataSourceRecord>(() =>
            {
                DataSourceBuilder dataSourceBuilder = new DataSourceBuilder(DeviceAcronym);

                IEnumerable<TrendChannelMapping> trendChannelMappings = ChannelMap.Values
                    .OrderBy(mapping => mapping.QuantityDefinition.ChannelName)
                    .ThenBy(mapping => mapping.QuantityDefinition.ValueTypeID);

                foreach (TrendChannelMapping mapping in trendChannelMappings)
                    dataSourceBuilder.InsertTrendChannel(mapping.QuantityDefinition);

                return dataSourceBuilder.DataSource;
            });

            while (true)
            {
                List<IONTrendPoint> queriedPoints = await trendReaders
                    .AdvanceAsync()
                    .ToListAsync();

                if (!queriedPoints.Any())
                    break;

                DataSourceRecord dataSource = dataSourceFactory.Value;
                ObservationBuilder observationBuilder = new ObservationBuilder(dataSource, null);
                IEnumerable<IGrouping<uint, IONTrendPoint>> quantityGroupings = queriedPoints.GroupBy(point => (uint)point.InputHandle);

                var mappedPoints = quantityGroupings.Select(grouping =>
                {
                    if (!ChannelMap.TryGetValue(grouping.Key, out TrendChannelMapping? mapping))
                        return null;

                    List<IONTrendPoint> points = grouping
                        .Where(point => point.Timestamp >= PollingOptions.DownloadAfter)
                        .Where(point => point.Timestamp <= PollingOptions.DownloadBefore)
                        .ToList();

                    if (!points.Any())
                        return null;

                    return new { mapping, points };
                });

                var channelGroupings = mappedPoints
                    .Where(obj => !(obj is null))
                    .Select(obj => obj!)
                    .GroupBy(obj => obj.mapping.QuantityDefinition.ChannelName);

                foreach (var channelGrouping in channelGroupings)
                {
                    List<DateTime> timestamps = channelGrouping
                        .SelectMany(obj => obj.points)
                        .Select(point => point.Timestamp.ToDateTime())
                        .Distinct()
                        .OrderBy(timestamp => timestamp)
                        .ToList();

                    var orderedGrouping = channelGrouping
                        .OrderBy(obj => obj.mapping.QuantityDefinition.ValueTypeID);

                    DateTime date = trendReaders.CurrentDate;
                    string channelName = channelGrouping.Key;
                    observationBuilder.InsertTrendingTimestamps(date, channelName, timestamps);

                    foreach (var obj in orderedGrouping)
                    {
                        PQDIFTrendQuantity trendQuantity = obj.mapping.QuantityDefinition;
                        List<IONTrendPoint> points = obj.points;
                        double scale = obj.mapping.ConversionFactor;
                        observationBuilder.InsertTrendingData(trendQuantity, points, scale);
                    }
                }

                ObservationRecord observation = observationBuilder.Observation;

                if (!observation.ChannelInstances.Any())
                    continue;

                string tempPath = Path.GetTempFileName();

                try
                {
                    var fileNamingParameters = new
                    {
                        Timestamp = observation.StartTime
                    };

                    string fileName = PollingOptions.TrendFileNamingExpression.Interpolate(fileNamingParameters);

                    await using LogicalWriter writer = new LogicalWriter(tempPath);
                    ContainerRecord container = ContainerRecord.CreateContainerRecord();
                    container.FileName = fileName;
                    container.CompressionStyle = CompressionStyle.RecordLevel;
                    container.CompressionAlgorithm = CompressionAlgorithm.Zlib;
                    await writer.WriteAsync(container);
                    await writer.WriteAsync(observation, true);
                    await writer.DisposeAsync();

                    string destinationFolder = DestinationFolder;
                    Directory.CreateDirectory(destinationFolder);

                    string destination = Path.Combine(destinationFolder, fileName);
                    File.Move(tempPath, destination, true);
                }
                finally
                {
                    File.Delete(tempPath);
                }

                UpdateCheckpoint(observation.StartTime);
            }
        }

        private async IAsyncEnumerable<IONTrendReader> GetTrendReadersAsync(IONMasterStation masterStation)
        {
            if (!masterStation.TryGetManagerClass(IONClassType.DataRec, out IONManagerClass? dataRecorderManager))
                yield break;

            IONModuleClass[] dataRecorderModules = await dataRecorderManager.GetModuleClassesAsync();

            for (int i = 0; i < dataRecorderModules.Length; i++)
            {
                IONModuleClass dataRecorderModule = dataRecorderModules[i];
                IONUIntArray inputHandles = await dataRecorderModule.ReadInputHandlesAsync();

                if (inputHandles.Length > 16 && inputHandles[16] != 0)
                {
                    IONBooleanClass enabled = masterStation.GetRegisterClass<IONBooleanClass>(inputHandles[16]);
                    enabled.Timeout = -1;

                    if (!await enabled.ReadValueAsync())
                        continue;
                }

                if (!inputHandles.Any(handle => ChannelMap.ContainsKey(handle)))
                    continue;

                IONUIntArray outputHandles = await dataRecorderModule.ReadOutputHandlesAsync();
                IONLogClass dataRecorderLog = masterStation.GetRegisterClass<IONLogClass>(outputHandles.First());
                dataRecorderLog.Timeout = -1;
                yield return new IONTrendReader(inputHandles, dataRecorderLog);
            }
        }

        private DateTime GetCheckpoint()
        {
            string deviceAcronym = DeviceAcronym;
            string connectionString = PollingOptions.DBConnectionString;
            string dataProviderString = PollingOptions.DBDataProviderString;

            using AdoDataConnection connection = new AdoDataConnection(connectionString, dataProviderString);
            return connection.ExecuteScalar<DateTime>("SELECT TimeRecorded FROM IONTrendingCheckpoint WHERE Device = {0}", deviceAcronym);
        }

        private void UpdateCheckpoint(DateTime timestamp)
        {
            string deviceAcronym = DeviceAcronym;
            string connectionString = PollingOptions.DBConnectionString;
            string dataProviderString = PollingOptions.DBDataProviderString;

            using AdoDataConnection connection = new AdoDataConnection(connectionString, dataProviderString);
            int count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM IONTrendingCheckpoint WHERE Device = {0}", deviceAcronym);

            if (count == 0)
                connection.ExecuteNonQuery("INSERT INTO IONTrendingCheckpoint(Device, TimeRecorded) VALUES({0}, {1})", deviceAcronym, timestamp);
            else
                connection.ExecuteNonQuery("UPDATE IONTrendingCheckpoint SET TimeRecorded = {1} WHERE Device = {0}", deviceAcronym, timestamp);
        }
    }
}
