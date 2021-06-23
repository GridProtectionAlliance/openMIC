//******************************************************************************************************
//  DailyStatistics.cs - Gbtc
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
//  06/22/2021 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;

namespace openMIC.Model
{
    public class DailyStatistics
    {
        public DateTime StartTime;
        public DateTime EndTime;

        public string Meter { get; set; }
        public DateTime? LastSuccessfulConnection { get; set; }
        public DateTime? LastUnsuccessfulConnection { get; set; }
        public string LastUnsuccessfulConnectionExplanation { get; set; }

        public int TotalConnections => TotalSuccessfulConnections + TotalUnsuccessfulConnections;
        public int TotalUnsuccessfulConnections { get; set; }
        public int TotalSuccessfulConnections { get; set; }

        public static DailyStatistics Default = new DailyStatistics();
    }
}