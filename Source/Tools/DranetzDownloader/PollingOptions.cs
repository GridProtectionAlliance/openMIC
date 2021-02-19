﻿//******************************************************************************************************
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
//  08/24/2020 - C. Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace DranetzDowloader
{
    public class PollingOptions
    {
        public string? DeviceAcronym { get; set; }
        public string DestinationFolder { get; set; } = string.Empty;
        public string EventFileNamingExpression { get; set; } = string.Empty;
        public string TrendFileNamingExpression { get; set; } = string.Empty;
       
        public string MeterIP { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public int AnalyzerID { get; set; } = 0;
        public string DBDataProviderString { get; set; } = string.Empty;
        public string DBConnectionString { get; set; } = string.Empty;

        public DateTime DownloadAfter { get; set; } = DateTime.MinValue;
        public DateTime DownloadBefore { get; set; } = DateTime.MaxValue;

        public bool DownloadEventData { get; set; } = true;
        public bool DownloadTrendingData { get; set; } = true;

        public int Delay { get; set; } = 10000;
        public int MaxRequest { get; set; } = 150;
        public bool StartFromCheckpoint { get; set; } = true;
        public bool UpdateCheckpoint { get; set; } = true;
    }
}
