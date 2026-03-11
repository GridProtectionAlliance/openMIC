//******************************************************************************************************
//  DailyStatisticsController.cs - Gbtc
//
//  Copyright © 2025, Grid Protection Alliance.  All Rights Reserved.
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
//  03/09/2026 - Natalie Beatty
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using GSF.Data;
using GSF.Data.Model;
using GSF.Web.Model;
using Newtonsoft.Json;
using openMIC.Model;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.UI.WebControls;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
namespace openMIC;

/// <summary>
/// Controller that provides access to <see cref="DailyStatisticsRecord />
/// </summary>
public class DailyStatisticsController : ModelController<DailyStatisticsRecord>
{
    /// <inheritdoc/>
    [HttpPost, ActionName("SearchableList")]
    public override IHttpActionResult GetSearchableList([FromBody] PostData postData)
    {
        return base.GetSearchableList(postData);
    }

    [HttpGet]
    public override IHttpActionResult Get([FromUri] string meter = null) // will be meter name
    {
        if (!GetAuthCheck())
        {
            return Unauthorized();
        }

        if (meter is null)
        {
            return Ok();
        }
        using (AdoDataConnection connection = ConnectionFactory())
        {
            string sql = $"SELECT * FROM DailyStatisticsRecord WHERE meter = '{meter}';";
            object[] param = [];

            if (RootQueryRestriction != null)
            {
                param = RootQueryRestriction.Parameters.ToArray();
            }
            DataTable dataTbl = connection.RetrieveData(sql, param);
            return Ok(JsonConvert.SerializeObject(dataTbl));
        }
    }
}