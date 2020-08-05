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
//  07/24/2020 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gemstone.Configuration;
using Gemstone.Configuration.AppSettings;
using Microsoft.Extensions.Configuration;

namespace IONDownloader
{
    partial class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .ConfigureGemstoneDefaults(ConfigureAppSettings, useINI: true)
                .AddCommandLine(args)
                .Build();

            PollingOptions pollingOptions = new PollingOptions();
            configuration.Bind(pollingOptions);

            List<Task> pollTasks = new List<Task>();

            if (pollingOptions.DownloadTrendingData)
            {
                Dictionary<uint, TrendChannelMapping> channelMap = await TrendChannelMapping
                    .ReadMappingsAsync(pollingOptions.TrendMappingPath)
                    .ToDictionaryAsync(mapping => (uint)mapping.Handle);

                TrendPoller trendPoller = new TrendPoller(channelMap, pollingOptions);
                pollTasks.Add(trendPoller.PollAsync());
            }

            if (pollingOptions.DownloadEventData)
            {
                EventPoller eventPoller = new EventPoller(pollingOptions);
                pollTasks.Add(eventPoller.PollAsync());
            }

            await Task.WhenAll(pollTasks);
        }

        private static void ConfigureAppSettings(IAppSettingsBuilder builder)
        {
            const string DefaultDBConnectionString = "Data Source=localhost; Initial Catalog=openMIC; Integrated Security=SSPI";
            const string DefaultDBDataProviderString = "AssemblyName={Microsoft.Data.SqlClient, Culture=neutral, PublicKeyToken=23ec7fc2d6eaa4a5}; ConnectionType=Microsoft.Data.SqlClient.SqlConnection; AdapterType=Microsoft.Data.SqlClient.SqlDataAdapter";
            const string DefaultEventFileNamingExpression = "event-{Timestamp:yyyyMMddTHHmmssfffffff}.pqd";
            const string DefaultTrendFileNamingExpression = "trend-{Timestamp:yyyyMMdd}.pqd";
            builder.Add("DBConnectionString", DefaultDBConnectionString, "Defines the connection to the openMIC database for checkpointing");
            builder.Add("DBDataProviderString", DefaultDBDataProviderString, "Defines the ADO.NET provider for openMIC database connectivity");
            builder.Add("EventFileNamingExpression", DefaultEventFileNamingExpression, "Determines the format of files produced by the event poller");
            builder.Add("TrendFileNamingExpression", DefaultTrendFileNamingExpression, "Determines the format of files produced by the trend poller");
            builder.Add("TrendMappingPath", "trend-mappings.json", "Path to the file that defines mappings for trend channels");
        }
    }
}
