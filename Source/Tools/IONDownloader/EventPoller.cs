//******************************************************************************************************
//  EventPoller.cs - Gbtc
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
//  07/25/2020 - Stephen C. Wills
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
    public class EventPoller
    {
        private PollingOptions PollingOptions { get; }

        private string DeviceAcronym => PollingOptions.DeviceAcronym
            ?? throw new ArgumentNullException(nameof(PollingOptions.DeviceAcronym));

        private string DestinationFolder => PollingOptions.DestinationFolder
            ?? throw new ArgumentNullException(nameof(PollingOptions.DestinationFolder));

        public EventPoller(PollingOptions pollingOptions) =>
            PollingOptions = pollingOptions;

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

            List<IONWaveformReader> readerList = await GetWaveformReadersAsync(masterStation)
                .ToListAsync();

            WaveformReaderCollection waveformReaders = new WaveformReaderCollection(readerList);
            Func<IAsyncEnumerable<IONWaveformReader>> queryReaders;

            static DateTime Max(DateTime time1, DateTime time2) =>
                new DateTime(Math.Max(time1.Ticks, time2.Ticks));
            
            if (PollingOptions.DownloadAfter > DateTime.MinValue && PollingOptions.StartFromCheckpoint)
                queryReaders = () => waveformReaders.SeekAsync(Max(PollingOptions.DownloadAfter, GetCheckpoint()));
            else if (PollingOptions.DownloadAfter > DateTime.MinValue)
                queryReaders = () => waveformReaders.SeekAsync(PollingOptions.DownloadAfter);
            else if (PollingOptions.StartFromCheckpoint)
                queryReaders = () => waveformReaders.SeekAsync(GetCheckpoint());
            else
                queryReaders = waveformReaders.AdvanceAsync;

            Lazy<DataSourceRecord> dataSourceFactory = new Lazy<DataSourceRecord>(() =>
            {
                DataSourceBuilder dataSourceBuilder = new DataSourceBuilder(DeviceAcronym);

                foreach (IONWaveformReader reader in readerList)
                    dataSourceBuilder.InsertWaveformChannel(reader.InputName, reader.InputLabel);

                return dataSourceBuilder.DataSource;
            });

            while (true)
            {
                List<IONWaveformReader> queriedReaders = await queryReaders().ToListAsync();

                if (!queriedReaders.Any())
                    break;

                DataSourceRecord dataSource = dataSourceFactory.Value;
                ObservationBuilder observationBuilder = new ObservationBuilder(dataSource, null);

                foreach (IONWaveformReader reader in queriedReaders)
                {
                    if (reader.CurrentWaveform != null)
                        observationBuilder.InsertWaveform(reader.InputLabel, reader.CurrentWaveform);
                }

                ObservationRecord observation = observationBuilder.Observation;

                if (!observation.ChannelInstances.Any())
                    continue;

                if (observation.StartTime < PollingOptions.DownloadAfter)
                    continue;

                if (observation.StartTime > PollingOptions.DownloadBefore)
                    break;

                string tempPath = Path.GetTempFileName();

                try
                {
                    var fileNamingParameters = new
                    {
                        Timestamp = observation.StartTime
                    };

                    string fileName = PollingOptions.EventFileNamingExpression.Interpolate(fileNamingParameters);

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
                    Console.WriteLine(fileName);
                }
                finally
                {
                    File.Delete(tempPath);
                }

                UpdateCheckpoint(observation.StartTime);
                queryReaders = waveformReaders.AdvanceAsync;
            }
        }

        private async IAsyncEnumerable<IONWaveformReader> GetWaveformReadersAsync(IONMasterStation masterStation)
        {
            if (!masterStation.TryGetManagerClass(IONClassType.WaveformRec, out IONManagerClass? waveformManager))
                yield break;

            IONModuleClass[] waveformModules = await waveformManager.GetModuleClassesAsync();

            for (int i = 0; i < waveformModules.Length; i++)
            {
                IONModuleClass waveformModule = waveformModules[i];

                IONUIntArray inputHandles = await waveformModule.ReadInputHandlesAsync();
                IONNumericArrayClass waveformInput = masterStation.GetRegisterClass<IONNumericArrayClass>(inputHandles.First());
                string inputName = await waveformInput.ReadNameAsync();
                string inputLabel = await waveformInput.ReadLabelAsync();

                IONUIntArray outputHandles = await waveformModule.ReadOutputHandlesAsync();
                IONLogClass waveformLog = masterStation.GetRegisterClass<IONLogClass>(outputHandles.First());
                yield return new IONWaveformReader(inputName, inputLabel, waveformLog);
            }
        }

        private DateTime GetCheckpoint()
        {
            string deviceAcronym = DeviceAcronym;
            string connectionString = PollingOptions.DBConnectionString;
            string dataProviderString = PollingOptions.DBDataProviderString;

            using AdoDataConnection connection = new AdoDataConnection(connectionString, dataProviderString);
            return connection.ExecuteScalar<DateTime>("SELECT TimeRecorded FROM IONWaveformCheckpoint WHERE Device = {0}", deviceAcronym);
        }

        private void UpdateCheckpoint(DateTime timestamp)
        {
            string deviceAcronym = DeviceAcronym;
            string connectionString = PollingOptions.DBConnectionString;
            string dataProviderString = PollingOptions.DBDataProviderString;

            using AdoDataConnection connection = new AdoDataConnection(connectionString, dataProviderString);
            int count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM IONWaveformCheckpoint WHERE Device = {0}", deviceAcronym);

            if (count == 0)
                connection.ExecuteNonQuery("INSERT INTO IONWaveformCheckpoint(Device, TimeRecorded) VALUES({0}, {1})", deviceAcronym, timestamp);
            else
                connection.ExecuteNonQuery("UPDATE IONWaveformCheckpoint SET TimeRecorded = {1} WHERE Device = {0}", deviceAcronym, timestamp);
        }
    }
}
