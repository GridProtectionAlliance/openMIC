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
using GSF.Data.Model;
using System.Data;
using GSF.Data;

namespace openMIC.Model;

/// <summary>
/// Stores a <see cref="DailyStatistics"/> record for a specific Day and Meter in the database.
/// </summary>
[CustomView(@"
    SELECT 
    MAX(r1.Timestamp) AS Timestamp,
    r1.Meter,
	MAX(r1.LastSuccessfulConnection) AS LastSuccessfulConnection,
    MAX(r1.LastUnsuccessfulConnection) AS LastUnsuccessfulConnection,
	(SELECT r2.TotalSuccessfulConnections FROM DailyStatisticsRecord r2 WHERE r2.Meter = r1.Meter AND MAX(r1.Timestamp) = r2.Timestamp) AS TotalSuccessfulConnections,
	(SELECT r2.TotalUnsuccessfulConnections FROM DailyStatisticsRecord r2 WHERE r2.Meter = r1.Meter AND MAX(r1.Timestamp) = r2.Timestamp) AS TotalUnsuccessfulConnections,
	(SELECT r2.BadDays FROM DailyStatisticsRecord r2 WHERE r2.Meter = r1.Meter AND MAX(r1.Timestamp) = r2.Timestamp) AS BadDays
    FROM DailyStatisticsRecord r1 Group BY (r1.Meter)
    "), AllowSearch, ViewOnly]
public class DailyStatisticsRecord
{
    [PrimaryKey(true)]
    public int ID { get; set; }

    public DateTime Timestamp { get; set; }

    public int BadDays { get; set; }

    public string Meter { get; set; }

    [FieldDataType(DbType.DateTime2, DatabaseType.SQLServer)]
    public DateTime? LastSuccessfulConnection { get; set; }

    [FieldDataType(DbType.DateTime2, DatabaseType.SQLServer)]
    public DateTime? LastUnsuccessfulConnection { get; set; }

    public string LastUnsuccessfulConnectionExplanation { get; set; }

    [NonRecordField]
    public int TotalConnections => TotalSuccessfulConnections + TotalUnsuccessfulConnections;

    public int TotalUnsuccessfulConnections { get; set; }

    public int TotalSuccessfulConnections { get; set; }

}