//******************************************************************************************************
//  HealthController.cs - Gbtc
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
//  02/11/2025 - C. Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GSF.Data;
using GSF.Data.Model;
using openMIC.Model;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
namespace openMIC;

/// <summary>
/// Represents a REST based API for openMIC health related operations.
/// </summary>
public class HealthController : ApiController
{
    /// <summary>
    /// Represents a status item for openMIC health reporting.
    /// </summary>
    public class StatusItem
    {
        public string Status { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Validates that openMIC operations are responding as expected.
    /// </summary>
    [HttpGet]
    public HttpResponseMessage Index() => new(HttpStatusCode.OK);

    [HttpGet]
    public IHttpActionResult GetSystemStatus()
    {
        List<StatusItem> statusMessages = [];

        statusMessages.Add(new()
        {
            Status = (Program.Host.Model.Global.PoolMachines?.Length ?? 0) > 0 ? "Success" : "Error",
            Description = $"{(Program.Host.Model.Global.PoolMachines?.Length ?? 0)} Nodes are configured for polling"
        });

        using AdoDataConnection connection = new("systemSettings");
        TableOperations<NodeCheckin> tbl = new(connection);
        List<NodeCheckin> nodeStatus = [.. tbl.QueryRecordsWhere("LastCheckin >= {0}", DateTime.UtcNow.AddMinutes(-60))];

        if (nodeStatus.Count < 1)
            statusMessages.Add(new()
            {
                Status = "Error",
                Description = "System has not queued anything on the configured nodes in the last hour"
            });

        int nSuccess = nodeStatus.Count(n => string.IsNullOrEmpty(n.FailureReason));
        int nFailure = nodeStatus.Count - nSuccess;

        if (nSuccess > 0)
            statusMessages.Add(new()
            {
                Status = "Success",
                Description = $"System has successfully queued tasks on {nSuccess} nodes in the last hour"
            });


        if (nFailure > 0)
            statusMessages.Add(new()
            {
                Status = "Error",
                Description = $"System was not successful queuing tasks on {nFailure} nodes in the last hour"
            });

        // Check for version mismatch between host and nodes
        int nVersionMismatch = nodeStatus.Count(n => string.Equals(n.FailureReason, "Found different versions of openMIC"));

        if (nVersionMismatch > 0)
            statusMessages.Add(new()
            {
                Status = "Error",
                Description = $"{nVersionMismatch} nodes have a different version of openMIC than the head node"
            });
        else
            statusMessages.Add(new()
            {
                Status = "Success",
                Description = $"All nodes have the same version of openMIC"
            });

        int nTasksQueued = nodeStatus.Sum(n => n.TasksQueued);

        if (nTasksQueued > 0)
            statusMessages.Add(new()
            {
                Status = "Success",
                Description = $"{nTasksQueued} Tasks have been queued during the last distribution"
            });
        else
            statusMessages.Add(new()
            {
                Status = "Error",
                Description = $"No Tasks were queued during the last distribution"
            });

        return Ok(statusMessages);
    }
}