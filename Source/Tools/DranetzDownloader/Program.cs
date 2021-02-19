//******************************************************************************************************
//  Program.cs - Gbtc
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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Gemstone.Configuration;
using Gemstone.Configuration.AppSettings;
using Gemstone.PQDIF.Logical;
using Gemstone.PQDIF.Physical;
using Gemstone.StringExtensions;
using GSF.Data;
using Microsoft.Extensions.Configuration;

namespace DranetzDowloader
{
    partial class Program
    {
        private static PollingOptions m_pollingOptions;
        static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .ConfigureGemstoneDefaults(ConfigureAppSettings, useINI: true)
                .AddCommandLine(args)
                .Build();

            m_pollingOptions = new PollingOptions();

            configuration.Bind(m_pollingOptions);

            // This is to adjust Time; for Testing.....
            m_pollingOptions.DownloadAfter = new DateTime(2021,2,08,15,00,00);
            m_pollingOptions.DownloadBefore = new DateTime(2021, 2, 08, 21, 00, 00);

             if (m_pollingOptions.DeviceAcronym is null)
                throw new ArgumentNullException(nameof(PollingOptions.DeviceAcronym));


            using (DranetzMeter meter = new DranetzMeter(m_pollingOptions.User, m_pollingOptions.Password, m_pollingOptions.MeterIP, m_pollingOptions.Delay, m_pollingOptions.MaxRequest))
            {
                Console.WriteLine($"Querying Sessions and Analyzers...");

                await meter.LoadAnalyzers();
                await meter.LoadSessions();

                static DateTime Max(DateTime time1, DateTime time2) =>
                new DateTime(Math.Max(time1.Ticks, time2.Ticks));

                static DateTime Min(DateTime time1, DateTime time2) =>
                new DateTime(Math.Min(time1.Ticks, time2.Ticks));

                DateTime startingPoint = m_pollingOptions.DownloadAfter;

                DateTime checkPoint;

                if (m_pollingOptions.DownloadTrendingData && m_pollingOptions.DownloadEventData)
                    checkPoint = Min(GetTrendingCheckpoint(), GetEventCheckpoint());
                else if (m_pollingOptions.DownloadTrendingData)
                    checkPoint = GetTrendingCheckpoint();
                else
                    checkPoint = GetEventCheckpoint();


                if (m_pollingOptions.DownloadAfter > DateTime.MinValue && m_pollingOptions.StartFromCheckpoint)
                    startingPoint = Max(m_pollingOptions.DownloadAfter, checkPoint);
                else if (m_pollingOptions.DownloadAfter > DateTime.MinValue)
                    startingPoint = m_pollingOptions.DownloadAfter;
                else if (m_pollingOptions.StartFromCheckpoint)
                    startingPoint = checkPoint;


                if (meter.Analyzers.Find(item => item.Id == m_pollingOptions.AnalyzerID) == null)
                {
                    Console.WriteLine($"Analyzer with ID {m_pollingOptions.AnalyzerID} not found...");
                    return;
                }

                List<Session> sessions = meter.Sessions;

                List<Record> records = new List<Record>();
                
                for (int i = 0; i < sessions.Count; i++)
                {
                    DateTime start = sessions[i].Time;
                    DateTime? end = null;
                    if (i < (sessions.Count - 1))
                        end = sessions[i + 1].Time;

                    // If Data is outside start range
                    if (end != null && end < startingPoint)
                        continue;

                    // If Data is outside end range
                    if (start > m_pollingOptions.DownloadBefore)
                        continue;

                    // If data is inside range
                    if (end != null && start > startingPoint && end < m_pollingOptions.DownloadBefore)
                    {
                        Console.WriteLine($"Polling entire Session {sessions[i].Id}...");
                        records.AddRange(await sessions[i].LimitRecords(null, null, m_pollingOptions.DownloadEventData));
                        continue;
                    }

                    // If data is across end of range only
                    if (start > startingPoint)
                    {
                        Console.WriteLine($"Polling partial Session {sessions[i].Id}...");
                        records.AddRange(await sessions[i].LimitRecords(null, m_pollingOptions.DownloadBefore, m_pollingOptions.DownloadEventData));
                        continue;
                    }
                    // If Data is across start of range only
                    if (end != null && end < m_pollingOptions.DownloadBefore)
                    {
                        Console.WriteLine($"Polling partial Session {sessions[i].Id}...");
                        records.AddRange(await sessions[i].LimitRecords(startingPoint, null, m_pollingOptions.DownloadEventData));
                        continue;
                    }

                    // if data is across both end and start Range
                    Console.WriteLine($"Polling partial Session {sessions[i].Id}...");
                    records.AddRange(await sessions[i].LimitRecords(startingPoint, m_pollingOptions.DownloadBefore, m_pollingOptions.DownloadEventData));
                }


                // Deal with Trending Data only
                if (m_pollingOptions.DownloadTrendingData)
                {
                    Console.WriteLine($"Processing Trending Data Records...");
                    List<TrendDataRecord> points = records.Where(r => r.Type == (int)RecordType.Journal).Select(item => new TrendDataRecord(item)).ToList();
                    Console.WriteLine($"Processed {points.Count} Trending Data Records...");

                    TrendSeriesBuilder trendBuilder = new TrendSeriesBuilder();
                    trendBuilder.Build(points);
                    List<TrendSeries> trendSeries = trendBuilder.Series;

                    // Create DataSource Record based on Analyzer
                    DataSourceRecord dataSource = DataSourceRecord.CreateDataSourceRecord(meter.Analyzers.First().Name);

                    trendSeries.ForEach(item => item.AddChannelDefinitions(dataSource));

                    ObservationRecord observationRecord = ObservationRecord.CreateObservationRecord(dataSource, null);

                    trendSeries.ForEach(item => item.AddData(observationRecord));

                    string tempPath = Path.GetTempFileName();


                    try
                    {
                        var fileNamingParameters = new
                        {
                            Timestamp = observationRecord.StartTime
                        };

                        string fileName = m_pollingOptions.TrendFileNamingExpression.Interpolate(fileNamingParameters);

                        await using LogicalWriter writer = new LogicalWriter(tempPath);
                        ContainerRecord container = ContainerRecord.CreateContainerRecord();
                        container.FileName = fileName;
                        container.CompressionStyle = CompressionStyle.RecordLevel;
                        container.CompressionAlgorithm = CompressionAlgorithm.Zlib;
                        await writer.WriteAsync(container);
                        await writer.WriteAsync(observationRecord, true);
                        await writer.DisposeAsync();

                        Directory.CreateDirectory(m_pollingOptions.DestinationFolder ?? "./");

                        string destination = Path.Combine(m_pollingOptions.DestinationFolder ?? "./", fileName);
                        File.Move(tempPath, destination, true);

                        if (m_pollingOptions.UpdateCheckpoint && trendBuilder.Checkpoint > DateTime.MinValue)
                            UpdateTrendingCheckpoint(trendBuilder.Checkpoint);
                    }
                    finally
                    {
                        File.Delete(tempPath);
                    }

                   

                }

                //Deal with EventData Only
                if (m_pollingOptions.DownloadEventData)
                {
                    if (m_pollingOptions.DownloadTrendingData && m_pollingOptions.StartFromCheckpoint)
                        startingPoint = GetEventCheckpoint();

                    Console.WriteLine($"Processing Event Data Records...");
                    List<EventDataRecord> points = records.Where(r => r.Type == (int)RecordType.HC_Wave).
                        Select(item => new EventDataRecord(item)).OrderBy(item => item.Cycle).ToList();
                    Console.WriteLine($"Found {points.Count} Event Data Records...");

                    if (points.Count == 0)
                        return;

                    // Seperate into multiple events as applicable
                    int index = 1;
                    int cycle = points[0].Cycle;
                    int grp = cycle;

                    Dictionary<int, List<EventDataRecord>> eventGroup = new Dictionary<int, List<EventDataRecord>>();
                    eventGroup.Add(cycle, new List<EventDataRecord>() { points[0] });

                    while (index < points.Count())
                    {
                        if ((points[index].Cycle - cycle) > 1)
                        {
                            eventGroup.Add(points[index].Cycle, new List<EventDataRecord>() { points[index] });
                            grp = points[index].Cycle;
                        }
                        else
                            eventGroup[grp].Add(points[index]);
                        cycle = points[index].Cycle;
                        index++;
                    }

                    Console.WriteLine($"Found {eventGroup.Count} separate Events...");

                    int i = 0;
                    foreach (List<EventDataRecord> data in eventGroup.Values)
                    {
                       
                        EventSeriesBuilder eventBuilder = new EventSeriesBuilder();
                        eventBuilder.Build(data);
                        List<EventSeries> eventSeries = eventBuilder.Series;
                        i++;


                        // Create DataSource Record based on Analyzer
                        DataSourceRecord dataSource = DataSourceRecord.CreateDataSourceRecord(meter.Analyzers.First().Name);

                        eventSeries.ForEach(item => item.AddChannelDefinitions(dataSource));

                        ObservationRecord observationRecord = ObservationRecord.CreateObservationRecord(dataSource, null);

                        eventSeries.ForEach(item => item.AddData(observationRecord));

                        string tempPath = Path.GetTempFileName();


                        try
                        {
                            var fileNamingParameters = new
                            {
                                Timestamp = observationRecord.StartTime
                            };

                            string fileName = m_pollingOptions.EventFileNamingExpression.Interpolate(fileNamingParameters);

                            await using LogicalWriter writer = new LogicalWriter(tempPath);
                            ContainerRecord container = ContainerRecord.CreateContainerRecord();
                            container.FileName = fileName;
                            container.CompressionStyle = CompressionStyle.RecordLevel;
                            container.CompressionAlgorithm = CompressionAlgorithm.Zlib;
                            await writer.WriteAsync(container);
                            await writer.WriteAsync(observationRecord, true);
                            await writer.DisposeAsync();

                            Directory.CreateDirectory(m_pollingOptions.DestinationFolder ?? "./");

                            string destination = Path.Combine(m_pollingOptions.DestinationFolder ?? "./", fileName);
                            File.Move(tempPath, destination, true);
                            Console.WriteLine($"Processed event {i} out of {eventGroup.Count}...");

                            if (m_pollingOptions.UpdateCheckpoint && eventBuilder.Checkpoint > DateTime.MinValue)
                                UpdateEventCheckpoint(eventBuilder.Checkpoint);
                        }
                        finally
                        {
                            File.Delete(tempPath);
                        }
                    }

                }
            }

        }


        private static void UpdateTrendingCheckpoint(DateTime timestamp)
        {
            string deviceAcronym = m_pollingOptions.DeviceAcronym ?? "";
            string connectionString = m_pollingOptions.DBConnectionString;
            string dataProviderString = m_pollingOptions.DBDataProviderString;

            using AdoDataConnection connection = new AdoDataConnection(connectionString, dataProviderString);
            int count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM DranetzTrendingCheckpoint WHERE Device = {0}", deviceAcronym);

            if (count == 0)
                connection.ExecuteNonQuery("INSERT INTO DranetzTrendingCheckpoint(Device, TimeRecorded) VALUES({0}, {1})", deviceAcronym, timestamp);
            else
                connection.ExecuteNonQuery("UPDATE DranetzTrendingCheckpoint SET TimeRecorded = {1} WHERE Device = {0}", deviceAcronym, timestamp);
        }

        private static void UpdateEventCheckpoint(DateTime timestamp)
        {
            string deviceAcronym = m_pollingOptions.DeviceAcronym ?? ""; 
            string connectionString = m_pollingOptions.DBConnectionString;
            string dataProviderString = m_pollingOptions.DBDataProviderString;

            using AdoDataConnection connection = new AdoDataConnection(connectionString, dataProviderString);
            int count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM DranetzWaveformCheckpoint WHERE Device = {0}", deviceAcronym);

            if (count == 0)
                connection.ExecuteNonQuery("INSERT INTO DranetzWaveformCheckpoint(Device, TimeRecorded) VALUES({0}, {1})", deviceAcronym, timestamp);
            else
                connection.ExecuteNonQuery("UPDATE DranetzWaveformCheckpoint SET TimeRecorded = {1} WHERE Device = {0}", deviceAcronym, timestamp);
        }

        private static DateTime GetEventCheckpoint()
        {
            string deviceAcronym = m_pollingOptions.DeviceAcronym ?? "";
            string connectionString = m_pollingOptions.DBConnectionString;
            string dataProviderString = m_pollingOptions.DBDataProviderString;

            using AdoDataConnection connection = new AdoDataConnection(connectionString, dataProviderString);
            return connection.ExecuteScalar<DateTime>("SELECT TimeRecorded FROM DranetzWaveformCheckpoint WHERE Device = {0}", deviceAcronym);
        }

        private static DateTime GetTrendingCheckpoint()
        {
            string deviceAcronym = m_pollingOptions.DeviceAcronym ?? ""; 
            string connectionString = m_pollingOptions.DBConnectionString;
            string dataProviderString = m_pollingOptions.DBDataProviderString;

            using AdoDataConnection connection = new AdoDataConnection(connectionString, dataProviderString);
            return connection.ExecuteScalar<DateTime>("SELECT TimeRecorded FROM DranetzTrendingCheckpoint WHERE Device = {0}", deviceAcronym);
        }

        private static void ConfigureAppSettings(IAppSettingsBuilder builder)
        {
            const string DefaultIP = "localhost";
            const string DefaultDBDataProviderString = "AssemblyName={Microsoft.Data.SqlClient, Culture=neutral, PublicKeyToken=23ec7fc2d6eaa4a5}; ConnectionType=Microsoft.Data.SqlClient.SqlConnection; AdapterType=Microsoft.Data.SqlClient.SqlDataAdapter";
            const string DefaultEventFileNamingExpression = "event-{Timestamp:yyyyMMddTHHmmssfffffff}.pqd";
            const string DefaultTrendFileNamingExpression = "trend-{Timestamp:yyyyMMdd}.pqd";


            builder.Add("MeterIP", DefaultIP, "Defines the IP of the Dranetz Meter");
            builder.Add("User", "", "Defines the User of the Dranetz Meter");
            builder.Add("Password", "", "Defines the Password of the Dranetz Meter");

            builder.Add("DBDataProviderString", DefaultDBDataProviderString, "Defines the ADO.NET provider for openMIC database connectivity");
            builder.Add("EventFileNamingExpression", DefaultEventFileNamingExpression, "Determines the format of files produced by the event poller");
            builder.Add("TrendFileNamingExpression", DefaultTrendFileNamingExpression, "Determines the format of files produced by the trend poller");
        }

        
    }
}
