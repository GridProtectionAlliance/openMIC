//******************************************************************************************************
//  OperationsController.cs - Gbtc
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
//  01/20/2020 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GSF.Data;
using GSF.Data.Model;
using openMIC.Model;

namespace openMIC
{
    /// <summary>
    /// Represents a REST based API for openMIC operations.
    /// </summary>
    public class OperationsController : ApiController
    {
        /// <summary>
        /// Validates that openMIC operations are responding as expected.
        /// </summary>
        [HttpGet]
        public HttpResponseMessage Index()
        {
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        /// <summary>
        /// Queues task for operation at specified <paramref name="priority"/>.
        /// </summary>
        /// <param name="priority">Priority of task to use when queuing.</param>
        /// <param name="targets">List of task target names.</param>
        /// <remarks>
        /// Call format:
        /// <code>
        /// http://localhost:8089/api/Operations/QueueTasksWithPriority?priority=Expedited&target=Meter1&target=Meter2&target=Meter3
        /// </code>
        /// </remarks>
        [HttpGet]
        public HttpResponseMessage QueueTasksWithPriority([FromUri] QueuePriority priority, [FromUri(Name = "target")] List<string> targets)
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                TableOperations<Device> deviceTable = new TableOperations<Device>(connection);

                foreach (string target in targets)
                {
                    string acronym = target;

                    // Check if target is using device "Name" instead of "Acronym"
                    if (deviceTable.QueryRecordCountWhere("Acronym = {0}", acronym) == 0)
                    {
                        Device device = deviceTable.QueryRecordWhere("Name = {0}", acronym);

                        if (device != null)
                            acronym = device.Acronym;
                    }

                    if (!string.IsNullOrWhiteSpace(acronym))
                        Program.Host.QueueTasksWithPriority(acronym, priority);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}