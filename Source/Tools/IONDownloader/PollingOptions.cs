//******************************************************************************************************
//  PollingOptions.cs - Gbtc
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

namespace IONDownloader
{
    public class PollingOptions
    {
        public string? DeviceAcronym { get; set; }
        public string? DestinationFolder { get; set; }
        public string EventFileNamingExpression { get; set; } = string.Empty;
        public string TrendFileNamingExpression { get; set; } = string.Empty;
        public string TrendMappingPath { get; set; } = string.Empty;

        public string DBConnectionString { get; set; } = string.Empty;
        public string DBDataProviderString { get; set; } = string.Empty;
        public string IONConnectionString { get; set; } = string.Empty;

        public DateTime DownloadAfter { get; set; } = DateTime.MinValue;
        public DateTime DownloadBefore { get; set; } = DateTime.MaxValue;

        public bool DownloadEventData { get; set; } = true;
        public bool DownloadTrendingData { get; set; } = true;
        public bool StartFromCheckpoint { get; set; } = true;
        public bool UpdateCheckpoint { get; set; } = true;
    }
}
