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

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GSF.Communication;
using GSF.Data;
using GSF.Data.Model;
using GSF.Diagnostics;
using ModbusAdapters.Model;
using openMIC.Model;

namespace openMIC
{
    /// <summary>
    /// Represents a REST based API for openMIC operations.
    /// </summary>
    public class OperationsController : ApiController
    {
        /// <summary>
        /// Gets address of remote scheduler.
        /// </summary>
        public static string RemoteSchedulerAddress { get; private set; }

        /// <summary>
        /// Validates that openMIC operations are responding as expected.
        /// </summary>
        [HttpGet]
        public HttpResponseMessage Index() => new HttpResponseMessage(HttpStatusCode.OK);

        /// <summary>
        /// Queues group of tasks or individual task, identified by <paramref name="taskID"/>, for execution at specified <paramref name="priority"/>.
        /// </summary>
        /// <param name="taskID">Task identifier, i.e., the group task identifier or specific task name. Value is not case sensitive.</param>
        /// <param name="priority">Priority of tasks to use when queuing.</param>
        /// <param name="targets">List of task target names, i.e., <see cref="Downloader"/> device instance acronym or name, as defined in database configuration.</param>
        /// <remarks>
        /// <para>
        /// When not providing a specific task name to execute in the <paramref name="taskID"/> parameter,
        /// there are three group-based task identifiers available:
        /// <list type="bullet">
        /// <item><description><c><see cref="Downloader.AllTasksGroupID">_AllTasksGroup_</see></c></description></item>
        /// <item><description><c><see cref="Downloader.ScheduledTasksGroupID">_ScheduledTasksGroup_</see></c></description></item>
        /// <item><description><c><see cref="Downloader.OffScheduleTasksGroupID">_OffScheduleTasksGroup_</see></c></description></item>
        /// </list>
        /// The <c>_AllTasksGroup_</c> task identifier will queue all available tasks for execution, whereas, the
        /// <c>_ScheduledTasksGroup_</c> task identifier will only queue the tasks that share a common primary schedule.
        /// The <c>_OffScheduleTasksGroup_</c> task identifier will queue all tasks that have an overridden schedule
        /// defined. Note that when the <paramref name="taskID"/> is one of the specified group task identifiers, the
        /// queued tasks will execute immediately, regardless of any specified schedule, overridden or otherwise.
        /// </para>
        /// <para>
        /// Call format:
        /// <code>
        /// http://localhost:8089/api/Operations/QueueTasks?taskID=_AllTasksGroup_&amp;priority=Expedited&amp;target=Meter1&amp;target=Meter2&amp;target=Meter3
        /// </code>
        /// </para>
        /// </remarks>
        [HttpGet]
        public HttpResponseMessage QueueTasks([FromUri] string taskID, [FromUri] QueuePriority priority, [FromUri(Name = "target")] List<string> targets)
        {
            if (RemoteSchedulerAddress is null)
            {
                try
                {
                    if (Program.Host.Model.Global.UseRemoteScheduler)
                    {
                        // Capture IP of caller as remote scheduler, if address is not local
                        string remoteIP = Request.GetOwinContext().Request.RemoteIpAddress;

                        if (!Transport.IsLocalAddress(remoteIP))
                            RemoteSchedulerAddress = remoteIP;
                    }
                    else
                    {
                        RemoteSchedulerAddress = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    Logger.SwallowException(ex);
                }
            }

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

                    // Each downloader target is queued individually to allow for pooled multi-system distribution
                    if (!string.IsNullOrWhiteSpace(acronym))
                        Program.Host.QueueTasks(acronym, taskID, priority);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        /// <summary>
        /// Handles progress updates from remotely scheduled pooled instances.
        /// </summary>
        /// <param name="deviceName">Source device name.</param>
        /// <param name="progressUpdates">Serialized progress updates.</param>
        [HttpPost]
        public HttpResponseMessage ProgressUpdate([FromUri] string deviceName, [FromBody] List<ProgressUpdate> progressUpdates)
        {
            if (Program.Host.Model.Global.UseRemoteScheduler)
                return new HttpResponseMessage(HttpStatusCode.Forbidden);

            DataHub.ProgressUpdate(deviceName, null, progressUpdates);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}