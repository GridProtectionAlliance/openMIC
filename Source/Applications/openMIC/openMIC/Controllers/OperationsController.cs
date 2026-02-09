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

using GSF.Communication;
using GSF.ComponentModel.DataAnnotations;
using GSF.Configuration;
using GSF.Data;
using GSF.Data.Model;
using GSF.Threading;
using ModbusAdapters.Model;
using Newtonsoft.Json;
using openMIC.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Http;
using static System.Net.WebUtility;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
namespace openMIC;

/// <summary>
/// Represents a REST based API for openMIC operations.
/// </summary>
public class OperationsController : ApiController
{
    private static readonly HttpClient s_http;
    private static readonly ConcurrentDictionary<string, DailyStatistics> s_dailyStatistics;
    private static readonly ShortSynchronizedOperation s_collectDailyStatistics;
    private static readonly Timer s_statisticsTimer;
    private static int s_lastDayOfYear;

    static OperationsController()
    {
        const double DefaultDailyStatsInterval = 60.0D;

        CategorizedSettingsElementCollection systemSettings = ConfigurationFile.Current.Settings["systemSettings"];
        systemSettings.Add("DailyStatsInterval", DefaultDailyStatsInterval, "The interval, in seconds, for collecting the next set of daily device statistics.");
        double dailyStatsInterval = TimeSpan.FromSeconds(systemSettings["DailyStatsInterval"].ValueAs(DefaultDailyStatsInterval)).TotalMilliseconds;

        s_http = new HttpClient(new HttpClientHandler { UseCookies = false });
        s_dailyStatistics = new ConcurrentDictionary<string, DailyStatistics>(StringComparer.OrdinalIgnoreCase);
        s_collectDailyStatistics = new ShortSynchronizedOperation(CollectDailyStatistics, Program.Host.LogException);
        s_statisticsTimer = new Timer(dailyStatsInterval);
        s_lastDayOfYear = DateTime.UtcNow.DayOfYear;

        s_statisticsTimer.Elapsed += (_, __) => s_collectDailyStatistics.RunOnce();
        s_statisticsTimer.AutoReset = true;
        s_statisticsTimer.Start();
    }

    /// <summary>
    /// Gets address of remote scheduler.
    /// </summary>
    public static string RemoteSchedulerAddress { get; private set; }

    /// <summary>
    /// Validates that openMIC operations are responding as expected.
    /// </summary>
    [HttpGet]
    public HttpResponseMessage Index() => new(HttpStatusCode.OK);

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
        try
        {
            if (Program.Host.Model.Global.UseRemoteScheduler)
            {
                // Capture IP of caller as remote scheduler, if address is not local
                string remoteIP = Request.GetOwinContext().Request.RemoteIpAddress;

                if (RemoteSchedulerAddress != remoteIP && !Transport.IsLocalAddress(remoteIP))
                {
                    RemoteSchedulerAddress = remoteIP;
                    Program.Host.LogStatusMessage($"[{nameof(OperationsController)}] Assigned remote scheduler address: {RemoteSchedulerAddress}");
                }
            }
        }
        catch (Exception ex)
        {
            Program.Host.LogException(new InvalidOperationException($"[{nameof(OperationsController)}] Failed to assign remote scheduler address: {ex.Message}", ex));
        }

        string[] resolvedTargets = new string[targets.Count];
        using (AdoDataConnection connection = new("systemSettings"))
        {
            TableOperations<Device> deviceTable = new(connection);

            resolvedTargets = targets.Select(target => { 

                // Check if target is using device "Name" instead of "Acronym"
                if (deviceTable.QueryRecordCountWhere("Acronym = {0}", target) == 0)
                {
                    Device device = deviceTable.QueryRecordWhere("Name = {0}", target);

                    if (device != null)
                        return device.Acronym;
                }
                return target;
            }).ToArray();

        }

        // all downloader target are queued as a set to allow for improoved pooled multi-system distribution
        Program.Host.QueueTasks(resolvedTargets, taskID, priority);

        return new HttpResponseMessage(HttpStatusCode.OK);
    }

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
    /// http://localhost:8089/api/Operations/QueueTasks?taskID=_AllTasksGroup_&amp;priority=Expedited
    /// </code>
    /// </para>
    /// </remarks>
    [HttpPost, Route("QueueTasks")]
    public HttpResponseMessage QueueTasksPost([FromUri] string taskID, [FromUri] QueuePriority priority, [FromBody] List<string> targets)
    {
        return QueueTasks(taskID, priority, targets);
    }


    /// <summary>
    /// Gets version of thisinstance.
    /// </summary>
    [HttpGet]
    public HttpResponseMessage Version()
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(Assembly.GetExecutingAssembly().GetName().Version.ToString())
        };
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

    /// <summary>
    /// Relays allowed service commands for pooled machine synchronizations.
    /// </summary>
    /// <param name="command"></param>
    [HttpGet]
    public HttpResponseMessage RelayCommand([FromUri] string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            return new HttpResponseMessage(HttpStatusCode.BadRequest);

        command = command.Trim();

        if (!ServiceHost.CommandAllowedForRelay(command))
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);

        Program.Host.SendRequest(command);
        return new HttpResponseMessage(HttpStatusCode.OK);
    }

    /// <summary>
    /// Gets list of configured meter names.
    /// </summary>
    /// <returns>List of configured meter names.</returns>
    [HttpGet, ActionName("Meters")]
    public IHttpActionResult GetMeterList()
    {
        string[] meters = Program.Host.Downloaders.Select(adapter => adapter.Name).ToArray();
        return Ok(meters);
    }

    /// <summary>
    /// Tests meter connectivity.
    /// </summary>
    /// <param name="meter">Meter name to test/</param>
    /// <returns><c>true</c> if meter connection succeeds; otherwise, <c>false</c>.</returns>
    [HttpGet, ActionName("Test")]
    public IHttpActionResult TestConnection([FromUri(Name = "id")] string meter)
    {
        Downloader downloader = GetDownloader(meter);
        return Ok(downloader is not null && downloader.TestConnection());
    }

    /// <summary>
    /// Gets current daily stats for specified <paramref name="meter"/>.
    /// </summary>
    /// <param name="meter">Meter name to lookup for stats.</param>
    /// <returns>Current daily stats for specified <paramref name="meter"/>.</returns>
    [HttpGet, ActionName("Statistics")]
    public async Task<DailyStatistics> GetDailyStatistics([FromUri(Name = "id")] string meter)
    {
        Downloader downloader = GetDownloader(meter);

        if (downloader is null)
            return DailyStatistics.Default;

        GlobalSettings settings = Program.Host.Model.Global;
        DailyStatistics dailyStats = GetDailyStatistics(downloader);

        // Return local stats for remote or standalone instance
        if (settings.UseRemoteScheduler || settings.PoolMachines is null || settings.PoolMachines.Length == 0)
            return dailyStats;

        // Return cumulated stats for all pool machines from primary scheduler
        foreach (string targetMachine in settings.PoolMachines)
        {
            if (targetMachine.Equals("localhost"))
                continue;

            string targetUri;

            if (targetMachine.Contains("://"))
            {
                // Pooled target machine specification includes scheme and possible port number
                targetUri = targetMachine;
            }
            else
            {
                // Pooled target machine specification is only target machine name, assume same
                // scheme and port number as local instance
                Uri webHostUri = settings.WebHostUri;
                targetUri = $"{webHostUri.Scheme}://{targetMachine}:{webHostUri.Port}";
            }

            try
            {
                string actionURI = $"{targetUri}/api/Operations/GetDailyStatistics?meter={UrlEncode(meter)}";
                HttpRequestMessage request = new(HttpMethod.Get, actionURI);
                HttpResponseMessage response = await s_http.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    DailyStatistics remoteStats = JsonConvert.DeserializeObject<DailyStatistics>(content);

                    // Cumulate remote stats
                    if (remoteStats.LastSuccessfulConnection > dailyStats.LastSuccessfulConnection)
                        dailyStats.LastSuccessfulConnection = remoteStats.LastSuccessfulConnection;

                    if (remoteStats.LastUnsuccessfulConnection > dailyStats.LastUnsuccessfulConnection)
                    {
                        dailyStats.LastUnsuccessfulConnection = remoteStats.LastUnsuccessfulConnection;
                        dailyStats.LastUnsuccessfulConnectionExplanation = remoteStats.LastUnsuccessfulConnectionExplanation;
                    }

                    dailyStats.TotalSuccessfulConnections += remoteStats.TotalSuccessfulConnections;
                    dailyStats.TotalUnsuccessfulConnections += remoteStats.TotalUnsuccessfulConnections;
                }

            }
            catch (Exception ex)
            {
                Program.Host.LogException(new InvalidOperationException($"Failed to query daily statistics from \"{targetUri}\": {ex.Message}", ex));
            }
        }

        dailyStats.EndTime = DateTime.UtcNow;

        return dailyStats;
    }

    private static Downloader GetDownloader(string name) =>
        Program.Host.Downloaders.FirstOrDefault(adapter => adapter.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    private static DailyStatistics GetDailyStatistics(Downloader downloader)
    {
        string name = downloader.Name;
        DateTime currentTime = DateTime.UtcNow;

        return s_dailyStatistics.GetOrAdd(name, _ => new DailyStatistics
        {
            Meter = name,
            StartTime = currentTime,
            EndTime = currentTime,
            LastSuccessfulConnection = downloader.LastSuccessfulConnectionTime,
            LastUnsuccessfulConnection = downloader.LastFailedConnectionTime,
            LastUnsuccessfulConnectionExplanation = downloader.LastFailedConnectionReason,
            TotalSuccessfulConnections = (int)downloader.SuccessfulConnections,
            TotalUnsuccessfulConnections = (int)downloader.FailedConnections
        });
    }

    private static void CollectDailyStatistics()
    {
        DateTime currentTime = DateTime.UtcNow;
        int currentDayOfYear = currentTime.DayOfYear;
        bool resetStats = false;

        // Reset statistics at the start of each new day (UTC)
        if (currentDayOfYear != s_lastDayOfYear)
        {
            resetStats = s_lastDayOfYear > 0;
            s_lastDayOfYear = currentDayOfYear;
        }

        foreach (Downloader downloader in Program.Host.Downloaders)
        {
            DailyStatistics dailyStats = GetDailyStatistics(downloader);

            if (resetStats)
            {
                downloader.ResetStatistics();
                dailyStats.StartTime = currentTime;
            }

            dailyStats.LastSuccessfulConnection = downloader.LastSuccessfulConnectionTime;
            dailyStats.LastUnsuccessfulConnection = downloader.LastFailedConnectionTime;
            dailyStats.LastUnsuccessfulConnectionExplanation = downloader.LastFailedConnectionReason;
            dailyStats.TotalSuccessfulConnections = (int)downloader.SuccessfulConnections;
            dailyStats.TotalUnsuccessfulConnections = (int)downloader.FailedConnections;
            dailyStats.EndTime = currentTime;
        }
    }
}