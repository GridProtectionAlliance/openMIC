//******************************************************************************************************
//  Downloader.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  02/08/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using GSF;
using GSF.Configuration;
using GSF.Console;
using GSF.Data;
using GSF.Data.Model;
using GSF.Diagnostics;
using GSF.IO;
using GSF.Net.VirtualFtpClient;
using GSF.Net.Smtp;
using GSF.Scheduling;
using GSF.Threading;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF.TimeSeries.Statistics;
using GSF.Units;
using DotRas;
using ModbusAdapters.Model;
using openMIC.Model;
using openMIC.SharedAssets;
using static openMIC.SharedAssets.LogFunctions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable IdentifierTypo
namespace openMIC;

/// <summary>
/// Adapter that implements remote file download capabilities.
/// </summary>
[Description("Downloader: Implements remote file download capabilities")]
[EditorBrowsable(EditorBrowsableState.Advanced)] // Normally defined as an input device protocol
public class Downloader : InputAdapterBase
{
    #region [ Members ]

    // Nested Types

    // Define IDevice implementation for daily reports
    private class DeviceProxy : IDevice
    {
        private readonly Downloader m_parent;

        public DeviceProxy(Downloader parent) => m_parent = parent;

        // Gets or sets total data quality errors of this <see cref="IDevice"/>.
        public long DataQualityErrors { get; set; }

        // Gets or sets total time quality errors of this <see cref="IDevice"/>.
        public long TimeQualityErrors { get; set; }

        // Gets or sets total device errors of this <see cref="IDevice"/>.
        public long DeviceErrors { get; set; }

        // Gets or sets total measurements received for this <see cref="IDevice"/> - in local context "successful connections" per day.
        public long MeasurementsReceived
        {
            get => m_parent.SuccessfulConnections;
            set { } // Ignoring updates
        }

        // Gets or sets total measurements expected to have been received for this <see cref="IDevice"/> - in local context "attempted connections" per day.
        public long MeasurementsExpected
        {
            get => m_parent.AttemptedConnections;
            set { } // Ignoring updates
        }

        // Gets or sets the number of measurements received while this <see cref="IDevice"/> was reporting errors.
        public long MeasurementsWithError { get; set; }

        // Gets or sets the number of measurements (per frame) defined for this <see cref="IDevice"/>.
        public long MeasurementsDefined { get; set; }
    }

    // Define wrapper to store information about remote file and the local path destination
    private class FtpFileWrapper
    {
        public readonly string LocalPath;
        public readonly FtpFile RemoteFile;

        public FtpFileWrapper(string localPath, FtpFile remoteFile)
        {
            LocalPath = localPath;
            RemoteFile = remoteFile;
        }

        public void Get() => RemoteFile.Get(LocalPath);
    }

    private class ProgressUpdateWrapper
    {
        public Downloader Instance { get; }
        public ProgressUpdate Update { get; }

        public ProgressUpdateWrapper(Downloader instance, ProgressUpdate update)
        {
            Instance = instance;
            Update = update;
        }
    }

    // Constants

    /// <summary>
    /// Defines the task identifier that represents all tasks group.
    /// </summary>
    public const string AllTasksGroupID = "_ALLTASKSGROUP_";

    /// <summary>
    /// Defines the task identifier that represents the scheduled tasks group.
    /// </summary>
    public const string ScheduledTasksGroupID = "_SCHEDULEDTASKSGROUP_";

    /// <summary>
    /// Defines the task identifier that represents the off schedule tasks group.
    /// </summary>
    public const string OffScheduleTasksGroupID = "_OFFSCHEDULETASKSGROUP_";

    // Fields
    private readonly RasDialer m_rasDialer;
    private readonly DeviceProxy m_deviceProxy;
    private readonly List<ProgressUpdate> m_trackedProgressUpdates;
    private readonly ICancellationToken m_cancellationToken;
    private Device m_deviceRecord;
    private ConnectionProfile m_connectionProfile;
    private ConnectionProfileTaskQueue m_taskQueue;
    private ConnectionProfileTask[] m_allTasks;
    private ConnectionProfileTask[] m_scheduledTasks;
    private ConnectionProfileTask[] m_offScheduleTasks;
    private readonly object m_taskArrayLock;
    private int m_overallTasksCompleted;
    private int m_overallTasksCount;
    private int m_lastDownloadedFileID;
    private long m_startDialUpTime;
    private bool m_disposed;

    #endregion

    #region [ Constructors ]

    public Downloader()
    {
        m_rasDialer = new();
        m_rasDialer.Error += RasDialer_Error;
        m_deviceProxy = new(this);
        m_trackedProgressUpdates = new();
        m_cancellationToken = new GSF.Threading.CancellationToken();
        m_taskArrayLock = new();
    }

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets or sets the FTP type to use, e.g., FTP or TFTP.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines the FTP type to use, e.g., FTP or TFTP.")]
    [DefaultValue(typeof(FtpType), nameof(FtpType.Ftp))]
    public FtpType FTPType { get; set; }

    /// <summary>
    /// Gets or sets connection host name or IP for transport.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines connection host name or IP for transport.")]
    public string ConnectionHostName { get; set; }

    /// <summary>
    /// Gets or sets connection host user name for transport.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines connection host user name for transport.")]
    [DefaultValue("anonymous")]
    public string ConnectionUserName { get; set; }

    /// <summary>
    /// Gets or sets connection password for transport.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines connection password for transport.")]
    [DefaultValue("anonymous")]
    public string ConnectionPassword { get; set; }

    /// <summary>
    /// Gets or sets connection timeout for transport.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines connection timeout for transport.")]
    [DefaultValue(30000)]
    public int ConnectionTimeout { get; set; }

    /// <summary>
    /// Gets or sets mode of FTP connection.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines mode of FTP connection.")]
    [DefaultValue(true)]
    public bool PassiveFTP { get; set; }

    /// <summary>
    /// Gets or sets mode of FTP connection.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines IP address to send in FTP PORT command.")]
    [DefaultValue("")]
    public string ActiveFTPAddress { get; set; }

    /// <summary>
    /// Gets or sets mode of FTP connection.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines minimum port in active FTP port range.")]
    [DefaultValue(0)]
    public int MinActiveFTPPort { get; set; }

    /// <summary>
    /// Gets or sets mode of FTP connection.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines maximum port in active FTP port range.")]
    [DefaultValue(0)]
    public int MaxActiveFTPPort { get; set; }

    /// <summary>
    /// Gets or sets flag that determines if connection messages should be logged.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines flag that determines if connection messages should be logged.")]
    [DefaultValue(false)]
    public bool LogConnectionMessages { get; set; }

    /// <summary>
    /// Gets or sets connection profile record ID.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines connection profile record ID.")]
    [DefaultValue(0)]
    public int ConnectionProfileID { get; set; }

    /// <summary>
    /// Gets or sets download schedule.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines download schedule.")]
    [DefaultValue("* * * * *")]
    public string Schedule { get; set; }

    /// <summary>
    /// Gets or sets flag that determines if this connection will use dial-up.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Determines if this connection will use dial-up.")]
    [DefaultValue(false)]
    public bool UseDialUp { get; set; }

    /// <summary>
    /// Gets or sets dial-up entry name.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines dial-up entry name.")]
    [DefaultValue("")]
    public string DialUpEntryName { get; set; }

    /// <summary>
    /// Gets or sets dial-up phone number.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines dial-up phone number.")]
    [DefaultValue("")]
    public string DialUpNumber { get; set; }

    /// <summary>
    /// Gets or sets dial-up user name.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines dial-up user name.")]
    [DefaultValue("")]
    public string DialUpUserName { get; set; }

    /// <summary>
    /// Gets or sets dial-up password.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines dial-up password.")]
    [DefaultValue("")]
    public string DialUpPassword { get; set; }

    /// <summary>
    /// Gets or sets maximum retries for a dial-up connection.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines maximum retries for a dial-up connection.")]
    [DefaultValue(3)]
    public int DialUpRetries { get; set; }

    /// <summary>
    /// Gets or sets timeout for a dial-up connection.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines timeout for a dial-up connection.")]
    [DefaultValue(90)]
    public int DialUpTimeout { get; set; }

    /// <summary>
    /// Gets or sets total number of attempted connections.
    /// </summary>
    public long AttemptedConnections { get; set; }

    /// <summary>
    /// Gets or sets total number of successful connections.
    /// </summary>
    public long SuccessfulConnections { get; set; }

    /// <summary>
    /// Gets or sets last successful connection time.
    /// </summary>
    public DateTime? LastSuccessfulConnectionTime { get; set; }

    /// <summary>
    /// Gets or sets total number of failed connections.
    /// </summary>
    public long FailedConnections { get; set; }

    /// <summary>
    /// Gets or sets last failed connection time.
    /// </summary>
    public DateTime? LastFailedConnectionTime { get; set; }

    /// <summary>
    /// Gets or sets last failed connection reason.
    /// </summary>
    public string LastFailedConnectionReason { get; set; }

    /// <summary>
    /// Gets or sets total number of processed files.
    /// </summary>
    public long TotalProcessedFiles { get; set; }

    /// <summary>
    /// Gets or sets total number of attempted dial-ups.
    /// </summary>
    public long AttemptedDialUps { get; set; }

    /// <summary>
    /// Gets or sets total number of successful dial-ups.
    /// </summary>
    public long SuccessfulDialUps { get; set; }

    /// <summary>
    /// Gets or sets total number of failed dial-ups.
    /// </summary>
    public long FailedDialUps { get; set; }

    /// <summary>
    /// Gets or sets number of files downloaded during last execution.
    /// </summary>
    public long FilesDownloaded { get; set; }

    /// <summary>
    /// Gets or sets total number of files downloaded.
    /// </summary>
    public long TotalFilesDownloaded { get; set; }

    /// <summary>
    /// Gets or sets total number of bytes downloaded.
    /// </summary>
    public long BytesDownloaded { get; set; }

    /// <summary>
    /// Gets or sets total connected time, in ticks.
    /// </summary>
    public long TotalConnectedTime { get; set; }

    /// <summary>
    /// Gets or sets total dial-up time, in ticks.
    /// </summary>
    public long TotalDialUpTime { get; set; }

    /// <summary>
    /// Gets or sets <see cref="DataSet" /> based data source available to this <see cref="AdapterBase" />.
    /// </summary>
    public override DataSet DataSource
    {
        get => base.DataSource;
        set
        {
            base.DataSource = value;

            if (Initialized)
            {
                // ReloadConfig was requested, take this opportunity to reload connection profile tasks...
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        LoadTasks();
                    }
                    catch (Exception ex)
                    {
                        OnProcessException(MessageLevel.Warning, new InvalidOperationException($"Failed to reload connection profile tasks: {ex.Message}", ex));
                    }
                });
            }
        }
    }

    /// <summary>
    /// Gets flag that determines if the data input connects asynchronously.
    /// </summary>
    /// <remarks>
    /// Derived classes should return true when data input source is connects asynchronously, otherwise return false.
    /// </remarks>
    protected override bool UseAsyncConnect => false;

    /// <summary>
    /// Gets the flag indicating if this adapter supports temporal processing.
    /// </summary>
    public override bool SupportsTemporalProcessing => false;

    /// <summary>
    /// Returns the detailed status of the data input source.
    /// </summary>
    public override string Status
    {
        get
        {
            StringBuilder status = new();

            string[] poolMachines = Program.Host.Model.Global.PoolMachines;
            bool useRemoteScheduler = Program.Host.Model.Global.UseRemoteScheduler;
            bool primaryScheduler = poolMachines?.Length > 0;

            status.Append(base.Status);
            status.AppendLine($"           Target FTP type: {FTPType.GetDescription()}");
            status.AppendLine($"      Connection host name: {ConnectionHostName.ToNonNullNorWhiteSpace("undefined")}");
            status.AppendLine($"      Connection user name: {ConnectionUserName.ToNonNullNorWhiteSpace("undefined")} - with {(string.IsNullOrWhiteSpace(ConnectionPassword) ? "no" : "a")} password");
            status.AppendLine($"     Connection profile ID: {ConnectionProfileID} - {m_connectionProfile?.Name ?? "undefined"}");
            status.AppendLine($"         Download schedule: {Schedule} - managed {(useRemoteScheduler ? "remotely" : "locally")}");
            status.AppendLine($"       Functional identity: {(useRemoteScheduler ? "Subordinate" : primaryScheduler ? "Primary Scheduler" : "Independent")}");

            if (primaryScheduler)
                status.AppendLine($"    Download pool machines: {string.Join(", ", poolMachines)}");

            status.AppendLine($"   Log connection messages: {LogConnectionMessages}");
            status.AppendLine($"     Attempted connections: {AttemptedConnections}");
            status.AppendLine($"    Successful connections: {SuccessfulConnections}");
            status.AppendLine($"        Failed connections: {FailedConnections}");
            status.AppendLine($"     Total processed files: {TotalProcessedFiles}");
            status.AppendLine($"      Total connected time: {new Ticks(TotalConnectedTime).ToElapsedTimeString(3)}");
            status.AppendLine($"               Use dial-up: {UseDialUp}");

            if (UseDialUp)
            {
                status.AppendLine($"        Dial-up entry name: {DialUpEntryName}");
                status.AppendLine($"            Dial-up number: {DialUpNumber}");
                status.AppendLine($"         Dial-up user name: {DialUpUserName.ToNonNullNorWhiteSpace("undefined")} - with {(string.IsNullOrWhiteSpace(DialUpPassword) ? "no" : "a")} password");
                status.AppendLine($"           Dial-up retries: {DialUpRetries}");
                status.AppendLine($"          Dial-up time-out: {DialUpTimeout}");
                status.AppendLine($"        Attempted dial-ups: {AttemptedDialUps}");
                status.AppendLine($"       Successful dial-ups: {SuccessfulDialUps}");
                status.AppendLine($"           Failed dial-ups: {FailedDialUps}");
                status.AppendLine($"        Total dial-up time: {new Ticks(TotalDialUpTime).ToElapsedTimeString(3)}");
            }

            status.AppendLine($" Connection profiles tasks: {AllTasks.Length}");
            status.AppendLine($"          Files downloaded: {FilesDownloaded}");
            status.AppendLine($"          Bytes downloaded: {BytesDownloaded / (double)SI2.Mega:N3} MB");

            return status.ToString();
        }
    }

    // Gets RAS connection state
    private RasConnectionState RasState => RasConnection.GetActiveConnections().FirstOrDefault(ras => ras.EntryName == DialUpEntryName)?.GetConnectionStatus()?.ConnectionState ?? RasConnectionState.Disconnected;

    // Gets or sets m_allTasks connection profile task array field.
    // All access to m_allTasks field should only occur through this property.
    private ConnectionProfileTask[] AllTasks
    {
        get
        {
            ConnectionProfileTask[] allTasks;

            lock (m_taskArrayLock)
                allTasks = m_allTasks;

            return allTasks;
        }
        set
        {
            lock (m_taskArrayLock)
                m_allTasks = value;
        }
    }

    // Gets or sets m_scheduledTasks connection profile task array field.
    // All access to m_scheduledTasks field should only occur through this property.
    private ConnectionProfileTask[] ScheduledTasks
    {
        get
        {
            ConnectionProfileTask[] scheduledTasks;

            lock (m_taskArrayLock)
                scheduledTasks = m_scheduledTasks;

            return scheduledTasks;
        }
        set
        {
            lock (m_taskArrayLock)
                m_scheduledTasks = value;
        }
    }

    // Gets or sets m_offScheduleTasks connection profile task array field.
    // All access to m_offScheduleTasks field should only occur through this property.
    private ConnectionProfileTask[] OffScheduleTasks
    {
        get
        {
            ConnectionProfileTask[] offScheduleTasks;

            lock (m_taskArrayLock)
                offScheduleTasks = m_offScheduleTasks;

            return offScheduleTasks;
        }
        set
        {
            lock (m_taskArrayLock)
            {
                if (m_offScheduleTasks != null)
                {
                    foreach (ConnectionProfileTask task in m_offScheduleTasks)
                    {
                        if (task != null)
                            UnregisterSchedule(this, task);
                    }
                }

                m_offScheduleTasks = value;

                if (m_offScheduleTasks != null)
                {
                    foreach (ConnectionProfileTask task in m_offScheduleTasks)
                    {
                        if (task != null)
                            RegisterSchedule(this, task);
                    }
                }
            }
        }
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="Downloader"/> object and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (m_disposed)
            return;

        try
        {
            if (!disposing)
                return;

            AllTasks = null;
            ScheduledTasks = null;
            OffScheduleTasks = null; // This will unregister overridden task schedules

            m_cancellationToken.Cancel();
            UnregisterSchedule(this);

            if (m_rasDialer != null)
            {
                m_rasDialer.Error -= RasDialer_Error;
                m_rasDialer.Dispose();
            }

            StatisticsEngine.Unregister(m_deviceProxy);
            StatisticsEngine.Unregister(this);
        }
        finally
        {
            m_disposed = true;       // Prevent duplicate dispose.
            base.Dispose(disposing); // Call base class Dispose().
        }
    }

    /// <summary>
    /// Initializes <see cref="Downloader"/>.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        ConnectionStringParser<ConnectionStringParameterAttribute> parser = new();
        parser.ParseConnectionString(ConnectionString, this);

        LoadTasks();
        RegisterSchedule(this);

        // Register downloader with the statistics engine
        StatisticsEngine.Register(this, "Downloader", "DLR");
        StatisticsEngine.Register(m_deviceProxy, Name, "Device", "PMU");
    }

    /// <summary>
    /// Attempts to connect to data input source.
    /// </summary>
    protected override void AttemptConnection()
    {
        ConnectionProfileTask[] tasks = AllTasks;

        foreach (ConnectionProfileTask task in tasks)
        {
            ConnectionProfileTaskSettings settings = task.Settings;
            string localPath = settings.LocalPath.ToNonNullString().Trim();

            if (!localPath.StartsWith(@"\\") || string.IsNullOrWhiteSpace(settings.DirectoryAuthUserName) || string.IsNullOrWhiteSpace(settings.DirectoryAuthPassword))
                continue;

            // For any tasks that define credentials for a UNC path, attempt authentication
            string[] userParts = settings.DirectoryAuthUserName.Split('\\');

            try
            {
                if (userParts.Length == 2)
                    FilePath.ConnectToNetworkShare(localPath.Trim(), userParts[1].Trim(), settings.DirectoryAuthPassword.Trim(), userParts[0].Trim());
                else
                    throw new InvalidOperationException($"UNC based local path \"{settings.LocalPath}\" or authentication user name \"{settings.DirectoryAuthUserName}\" is not in the correct format.");
            }
            catch (Exception ex)
            {
                OnProcessException(MessageLevel.Warning, new InvalidOperationException($"Exception while authenticating UNC path \"{settings.LocalPath}\": {ex.Message}", ex));
            }
        }
    }

    /// <summary>
    /// Attempts to disconnect from data input source.
    /// </summary>
    protected override void AttemptDisconnection()
    {
        // Just leaving UNC paths authenticated for the duration of service run-time since multiple
        // devices may share the same root destination path
    }

    /// <summary>
    /// Gets a short one-line status of this adapter.
    /// </summary>
    /// <param name="maxLength">Maximum number of available characters for display.</param>
    /// <returns>
    /// A short one-line summary of the current status of this adapter.
    /// </returns>
    public override string GetShortStatus(int maxLength) =>
        Enabled ?
            $"Downloading enabled for schedule: {Schedule}".CenterText(maxLength) :
            "Downloading for is paused...".CenterText(maxLength);

    /// <summary>
    /// Refreshes progress state notifications for a given client.
    /// </summary>
    /// <param name="clientID">Target client ID.</param>
    public void RefreshProgressStates(string clientID)
    {
        lock (m_trackedProgressUpdates)
        {
            List<ProgressUpdate> updates = ProgressUpdate.Flatten(m_trackedProgressUpdates);
            ProgressUpdated?.Invoke(this, new(clientID, updates));
        }
    }

    /// <summary>
    /// Resets local statistics.
    /// </summary>
    public void ResetStatistics()
    {
        AttemptedConnections = SuccessfulConnections = FailedConnections = 0L;
        LastSuccessfulConnectionTime = LastFailedConnectionTime = null;
        LastFailedConnectionReason = null;
    }

    /// <summary>
    /// Gets all defined connection profile tasks.
    /// </summary>
    public IReadOnlyCollection<ConnectionProfileTask> GetAllTasks => Array.AsReadOnly(AllTasks);

    /// <summary>
    /// Gets connection profile tasks defined with a common primary schedule.
    /// </summary>
    public IReadOnlyCollection<ConnectionProfileTask> GetScheduledTasks => Array.AsReadOnly(ScheduledTasks);

    /// <summary>
    /// Gets connection profile tasks defined with an overridden schedule.
    /// </summary>
    public IReadOnlyCollection<ConnectionProfileTask> GetOffScheduleTasks => Array.AsReadOnly(OffScheduleTasks);

    /// <summary>
    /// Attempts to get <see cref="ConnectionProfileTask"/> by <paramref name="taskName"/>.
    /// </summary>
    /// <param name="taskName">Name of task.</param>
    /// <param name="task">Value will be set to found task, if any; otherwise, value will be set to <c>null</c>.</param>
    /// <returns><c>true</c> if <paramref name="taskName"/> was found; otherwise, <c>false</c>.</returns>
    public bool TryGetConnectionProfileTask(string taskName, out ConnectionProfileTask task) =>
        AllTasks.ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase).TryGetValue(taskName, out task);

    /// <summary>
    /// Queues all tasks for immediate, highest priority, execution to next machine in the distribution pool.
    /// </summary>
    [AdapterCommand("Queues all tasks for immediate, highest priority, execution to next machine in the distribution pool.", "Administrator", "Editor")]
    public void QueueTasks() =>
        // This is for user requested items - these take precedence over all others,
        // call is made via ServiceHost to handle pooled distribution:
        Program.Host.QueueTasks(Name, AllTasksGroupID, QueuePriority.Urgent);

    /// <summary>
    /// Queues all tasks for immediate, highest priority, execution to local machine.
    /// </summary>
    [AdapterCommand("Queues all tasks for immediate, highest priority, execution to local machine.", "Administrator", "Editor")]
    public void QueueTasksLocally() =>
        QueueTasksByID(AllTasksGroupID, QueuePriority.Urgent);

    /// <summary>
    /// Queues all tasks for provided date range for execution.
    /// </summary>
    /// <param name="startDate">Start date for download.</param>
    /// <param name="startTime">Start time for download.</param>
    /// <param name="endDate">End date for download.</param>
    /// <param name="endTime">End time for download.</param>
    [AdapterCommand("Queues all tasks for provided date range for execution.", "Administrator", "Editor")]
    public void QueueTasksByDateRange(string startDate, string startTime, string endDate, string endTime)
    {
        string dateTimeFormat = Program.Host.Model.Global.DateTimeFormat;
        DateTime startTimeConstraint = DateTime.ParseExact($"{startDate} {startTime}", dateTimeFormat, CultureInfo.InvariantCulture);
        DateTime endTimeConstraint = DateTime.ParseExact($"{endDate} {endTime}", dateTimeFormat, CultureInfo.InvariantCulture);
        QueueTasks(AllTasks, QueuePriority.Urgent, startTimeConstraint, endTimeConstraint);
    }

    /// <summary>
    /// Queues group of tasks or individual task, identified by <paramref name="taskID"/>, with specified <paramref name="priority"/>.
    /// </summary>
    /// <param name="taskID">Task identifier, i.e., the group task identifier or specific task name. Value is not case sensitive.</param>
    /// <param name="priority">Priority of task to use when queuing.</param>
    /// <remarks>
    /// When not providing a specific task name to execute in the <paramref name="taskID"/> parameter,
    /// there are three group-based task identifiers available:
    /// <list type="bullet">
    /// <item><description><c><see cref="AllTasksGroupID">_AllTasksGroup_</see></c></description></item>
    /// <item><description><c><see cref="ScheduledTasksGroupID">_ScheduledTasksGroup_</see></c></description></item>
    /// <item><description><c><see cref="OffScheduleTasksGroupID">_OffScheduleTasksGroup_</see></c></description></item>
    /// </list>
    /// The <c>_AllTasksGroup_</c> task identifier will queue all available tasks for execution, whereas, the
    /// <c>_ScheduledTasksGroup_</c> task identifier will only queue the tasks that share a common primary schedule.
    /// The <c>_OffScheduleTasksGroup_</c> task identifier will queue all tasks that have an overridden schedule
    /// defined. Note that when the <paramref name="taskID"/> is one of the specified group task identifiers, the
    /// queued tasks will execute immediately, regardless of any specified schedule, overridden or otherwise.
    /// </remarks>
    public void QueueTasksByID(string taskID, QueuePriority priority)
    {
        switch (taskID.ToUpperInvariant())
        {
            case AllTasksGroupID:
                QueueTasks(AllTasks, priority);
                break;
            case ScheduledTasksGroupID:
                QueueTasks(ScheduledTasks, priority);
                break;
            case OffScheduleTasksGroupID:
                QueueTasks(OffScheduleTasks, priority);
                break;
            default:
                if (TryGetConnectionProfileTask(taskID, out ConnectionProfileTask task))
                {
                    QueueTask(task, priority);
                }
                else
                {
                    string message = $"Failed to find connection profile task \"{taskID}\" associated with downloader instance \"{Name}\", queue operation halted.";

                    OnStatusMessage(MessageLevel.Warning, message);

                    OnProgressUpdated(this, new()
                    {
                        State = ProgressState.Fail,
                        ErrorMessage = message,
                        OverallProgress = 1,
                        OverallProgressTotal = 1
                    });
                }
                break;
        }
    }

    // Queues specified task, with overridden schedule, for execution at specified priority
    private void QueueTask(ConnectionProfileTask task, QueuePriority priority)
    {
        m_taskQueue.QueueAction(() => ExecuteSingleTask(task), priority);

        OnProgressUpdated(this, new()
        {
            State = ProgressState.Queued,
            Message = $"Connection profile task \"{task.Name}\" with overridden schedule queued at \"{priority}\" priority.",
            Progress = 0,
            ProgressTotal = 1,
            OverallProgress = 0,
            OverallProgressTotal = 1
        });
    }

    // Queues specified task array for execution at specified priority
    private void QueueTasks(ConnectionProfileTask[] tasks, QueuePriority priority, DateTime? startTimeConstraint = null, DateTime? endTimeConstraint = null)
    {
        m_taskQueue.QueueAction(() => ExecuteGroupedTasks(tasks, startTimeConstraint, endTimeConstraint), priority);

        OnProgressUpdated(this, new()
        {
            State = ProgressState.Queued,
            Message = $"Connection profile tasks queued at \"{priority}\" priority.",
            Progress = 0,
            ProgressTotal = 1,
            OverallProgress = 0,
            OverallProgressTotal = 1
        });
    }

    private void LoadTasks()
    {
        using AdoDataConnection connection = new("systemSettings");
        TableOperations<Device> deviceTable = new(connection);
        TableOperations<ConnectionProfile> connectionProfileTable = new(connection);
        TableOperations<ConnectionProfileTask> connectionProfileTaskTable = new(connection);
        TableOperations<ConnectionProfileTaskQueue> connectionProfileTaskQueueTable = new(connection);

        ConnectionProfileTaskQueue taskQueue = null;

        if (UseDialUp && !string.IsNullOrWhiteSpace(DialUpEntryName))
        {
            taskQueue = connectionProfileTaskQueueTable.QueryRecordWhere("Name = {0}", DialUpEntryName) ?? new ConnectionProfileTaskQueue { Name = DialUpEntryName };
            taskQueue.MaxThreadCount = 1;
        }

        // Check for manual override specification of connection profile task queue name in downloader connection string.
        // This allows for custom task profile queues, e.g., specifying "MaxThreadCount = 1" when a certain resource should
        // not be multi-threaded, "BenDownloader" external action found in "Tools" folder is an example.
        if (Settings.TryGetValue("connectionProfileTaskQueueName", out string connectionProfileTaskQueueName) && !string.IsNullOrWhiteSpace(connectionProfileTaskQueueName))
            taskQueue = connectionProfileTaskQueueTable.QueryRecordWhere("Name = {0}", connectionProfileTaskQueueName) ?? new ConnectionProfileTaskQueue { Name = connectionProfileTaskQueueName };

        m_deviceRecord = deviceTable.QueryRecordWhere("Acronym = {0}", Name);
        m_connectionProfile = connectionProfileTable.LoadRecord(ConnectionProfileID);

        RecordRestriction filter = ConnectionProfileTask.CreateFilter(ConnectionProfileID);
        ConnectionProfileTask[] tasks = connectionProfileTaskTable.QueryRecords("LoadOrder", filter).ToArray();

        if (taskQueue == null && m_connectionProfile.DefaultTaskQueueID != null)
            taskQueue = connectionProfileTaskQueueTable.QueryRecordWhere("ID = {0}", m_connectionProfile.DefaultTaskQueueID.GetValueOrDefault());

        if (taskQueue == null)
            taskQueue = connectionProfileTaskQueueTable.QueryRecordWhere("Name = {0}", m_connectionProfile.Name) ?? new ConnectionProfileTaskQueue { Name = m_connectionProfile.Name };

        taskQueue.RegisterExceptionHandler(ex => OnProcessException(MessageLevel.Error, ex, "Task Execution"));
        m_taskQueue = taskQueue;

        AllTasks = tasks;
        ScheduledTasks = tasks.Where(task => string.IsNullOrWhiteSpace(task.Settings.Schedule)).ToArray();
        OffScheduleTasks = tasks.Where(task => !string.IsNullOrWhiteSpace(task.Settings.Schedule)).ToArray();
    }

    /// <summary>
    /// Test connection using first defined connection profile task.
    /// </summary>
    /// <returns><c>true</c> if connection succeeded; otherwise, <c>false</c>.</returns>
    public bool TestConnection()
    {
        if (!AdapterIsEnabled())
            return false;

        if (m_cancellationToken.IsCancelled)
            return false;

        ConnectionProfileTask[] allTasks = AllTasks;

        if (allTasks is null || allTasks.Length == 0)
            return false;

        ConnectionProfileTask task = allTasks[0];
        FtpClient ftpClient = null;

        try
        {
            if (string.IsNullOrWhiteSpace(task.Settings.ExternalOperation))
            {
                if (string.IsNullOrWhiteSpace(ConnectionHostName))
                {
                    return false;
                }
                else
                {
                    ftpClient = ConnectFTPClient();
                    return true;
                }
            }
            else
            {
                return TestExternalOperationConnection(task);
            }
        }
        catch
        {
            return false;
        }
        finally
        {
            if (ftpClient != null)
                DisconnectFTPClient(ftpClient);
        }
    }

    #region [ Task Execution Operations ]

    // Execute a single task with an overridden schedule. This method exists in addition
    // to ExecuteGroupedTasks to provide custom feedback for a single task with a defined
    // schedule override. Functional pattern should match that of ExecuteGroupedTasks.
    // This method should "setup" environment needed for execution of tasks, e.g.,
    // making any needed RAS or FTP connections, as well as "cleanup" when complete.
    private void ExecuteSingleTask(ConnectionProfileTask task)
    {
        if (!AdapterIsEnabled())
            return;

        if (m_cancellationToken.IsCancelled)
            return;

        FtpClient ftpClient = null;
        Ticks connectionStartTime = DateTime.UtcNow.Ticks;
        string connectionProfileName = m_connectionProfile?.Name ?? "Undefined";
        bool dialUpConnected = false;

        try
        {
            FilesDownloaded = 0;
            m_overallTasksCompleted = 0;
            m_overallTasksCount = 1;

            OnProgressUpdated(this, new()
            {
                State = ProgressState.Processing,
                Message = $"Beginning execution of task \"{task.Name}\" with overridden schedule for connection profile \"{connectionProfileName}\"...",
                Summary = $"0 Files Downloaded ({TotalFilesDownloaded:N0} Total)"
            });

            if (string.IsNullOrWhiteSpace(task.Settings.ExternalOperation))
            {
                if (string.IsNullOrWhiteSpace(ConnectionHostName))
                {
                    OnStatusMessage(MessageLevel.Warning, "No connection host name provided, skipping connection to FTP server...");
                }
                else
                {
                    try
                    {
                        OnStatusMessage(MessageLevel.Info, $"Attempting connection to FTP server \"{ConnectionUserName}@{ConnectionHostName}\"...");
                        AttemptedConnections++;

                        dialUpConnected = ConnectDialUp();
                        ftpClient = ConnectFTPClient();

                        SuccessfulConnections++;
                        LastSuccessfulConnectionTime = DateTime.UtcNow;

                        OnStatusMessage(MessageLevel.Info, $"Connected to FTP server \"{ConnectionUserName}@{ConnectionHostName}\"");
                    }
                    catch (Exception ex)
                    {
                        FailedConnections++;
                        LastFailedConnectionTime = DateTime.UtcNow;
                        LastFailedConnectionReason = ex.Message;

                        OnProcessException(MessageLevel.Warning, new InvalidOperationException($"Failed to connect to FTP server \"{ConnectionUserName}@{ConnectionHostName}\": {ex.Message}", ex));
                        OnProgressUpdated(this, new() { ErrorMessage = $"Failed to connect to FTP server \"{ConnectionUserName}@{ConnectionHostName}\": {ex.Message}" });
                    }
                }
            }

            if (m_cancellationToken.IsCancelled)
                return;

            OnStatusMessage(MessageLevel.Info, $"Starting \"{connectionProfileName}\" connection profile \"{task.Name}\" task processing on overridden schedule:");
            ExecuteTask(task, ftpClient, DateTime.Now);

            ProgressUpdate finalUpdate = new()
            {
                OverallProgress = 1,
                OverallProgressTotal = 1,
                State = task.Success ? ProgressState.Success : ProgressState.Fail
            };

            OnProgressUpdated(this, finalUpdate);
            LogOutcome(finalUpdate.State.GetValueOrDefault());

            if (FilesDownloaded > 0)
                LogDownload(m_lastDownloadedFileID, connectionStartTime, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            OnProgressUpdated(this, new()
            {
                State = ProgressState.Fail,
                ErrorMessage = $"Unexpected error caused task execution to end prematurely: {ex.Message}",
                OverallProgress = 1,
                OverallProgressTotal = 1
            });

            throw;
        }
        finally
        {
            if (ftpClient != null)
            {
                DisconnectFTPClient(ftpClient);

                if (dialUpConnected)
                    DisconnectDialUp();

                Ticks connectedTime = DateTime.UtcNow.Ticks - connectionStartTime;
                OnStatusMessage(MessageLevel.Info, $"FTP session connected for {connectedTime.ToElapsedTimeString(2)}");
                TotalConnectedTime += connectedTime;
            }
        }
    }

    // Execute a group of tasks with a common schedule. Relevant updates to the execution
    // steps of this method should be reviewed for inclusion in ExecuteSingleTask.
    // This method should "setup" environment needed for execution of tasks, e.g.,
    // making any needed RAS or FTP connections, as well as "cleanup" when complete.
    private void ExecuteGroupedTasks(ConnectionProfileTask[] tasks, DateTime? startTimeConstraint, DateTime? endTimeConstraint)
    {
        if (!AdapterIsEnabled())
            return;

        if (m_cancellationToken.IsCancelled)
            return;

        FtpClient ftpClient = null;
        Ticks connectionStartTime = DateTime.UtcNow.Ticks;
        string connectionProfileName = m_connectionProfile?.Name ?? "Undefined";
        bool dialUpConnected = false;

        try
        {
            if (tasks.Length == 0)
            {
                OnProgressUpdated(this, new()
                {
                    State = ProgressState.Fail,
                    ErrorMessage = $"No connection profile tasks defined for \"{connectionProfileName}\"."
                });

                return;
            }

            FilesDownloaded = 0;
            m_overallTasksCompleted = 0;
            m_overallTasksCount = tasks.Length;

            OnProgressUpdated(this, new()
            {
                State = ProgressState.Processing,
                Message = $"Beginning execution of {m_overallTasksCount:N0} tasks for connection profile \"{connectionProfileName}\"...",
                Summary = $"0 Files Downloaded ({TotalFilesDownloaded:N0} Total)"
            });

            if (tasks.Any(task => string.IsNullOrWhiteSpace(task.Settings.ExternalOperation)))
            {
                if (string.IsNullOrWhiteSpace(ConnectionHostName))
                {
                    OnStatusMessage(MessageLevel.Warning, "No connection host name provided, skipping connection to FTP server...");
                }
                else
                {
                    try
                    {
                        OnStatusMessage(MessageLevel.Info, $"Attempting connection to FTP server \"{ConnectionUserName}@{ConnectionHostName}\"...");
                        AttemptedConnections++;

                        dialUpConnected = ConnectDialUp();
                        ftpClient = ConnectFTPClient();

                        SuccessfulConnections++;
                        LastSuccessfulConnectionTime = DateTime.UtcNow;

                        OnStatusMessage(MessageLevel.Info, $"Connected to FTP server \"{ConnectionUserName}@{ConnectionHostName}\"");
                    }
                    catch (Exception ex)
                    {
                        FailedConnections++;
                        LastFailedConnectionTime = DateTime.UtcNow;
                        LastFailedConnectionReason = ex.Message;

                        OnProcessException(MessageLevel.Warning, new InvalidOperationException($"Failed to connect to FTP server \"{ConnectionUserName}@{ConnectionHostName}\": {ex.Message}", ex));
                        OnProgressUpdated(this, new() { ErrorMessage = $"Failed to connect to FTP server \"{ConnectionUserName}@{ConnectionHostName}\": {ex.Message}" });
                    }
                }
            }

            // Capture local time before task operations to provide consistent timestamp for all tasks
            DateTime localTime = DateTime.Now;

            foreach (ConnectionProfileTask task in tasks)
            {
                if (m_cancellationToken.IsCancelled)
                    return;

                OnStatusMessage(MessageLevel.Info, $"Starting \"{connectionProfileName}\" connection profile \"{task.Name}\" task processing:");

                // Apply any provided start/end time constraints to task settings
                task.Settings.StartTimeConstraint = startTimeConstraint;
                task.Settings.EndTimeConstraint = endTimeConstraint;

                ExecuteTask(task, ftpClient, localTime);

                // Clear any start/end time constraints applied to task settings
                task.Settings.StartTimeConstraint = null;
                task.Settings.EndTimeConstraint = null;
            }

            ProgressUpdate finalUpdate = new()
            {
                OverallProgress = 1,
                OverallProgressTotal = 1
            };

            if (tasks.All(task => task.Success))
                finalUpdate.State = ProgressState.Success;
            else if (tasks.All(task => !task.Success))
                finalUpdate.State = ProgressState.Fail;
            else
                finalUpdate.State = ProgressState.PartialSuccess;

            OnProgressUpdated(this, finalUpdate);
            LogOutcome(finalUpdate.State.GetValueOrDefault());

            if (FilesDownloaded > 0)
                LogDownload(m_lastDownloadedFileID, connectionStartTime, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            OnProgressUpdated(this, new()
            {
                State = ProgressState.Fail,
                ErrorMessage = $"Unexpected error caused task execution to end prematurely: {ex.Message}",
                OverallProgress = 1,
                OverallProgressTotal = 1
            });

            throw;
        }
        finally
        {
            if (ftpClient != null)
            {
                DisconnectFTPClient(ftpClient);

                if (dialUpConnected)
                    DisconnectDialUp();

                Ticks connectedTime = DateTime.UtcNow.Ticks - connectionStartTime;
                OnStatusMessage(MessageLevel.Info, $"FTP session connected for {connectedTime.ToElapsedTimeString(2)}");
                TotalConnectedTime += connectedTime;
            }
        }
    }

    private void ExecuteTask(ConnectionProfileTask task, FtpClient ftpClient, DateTime localTime)
    {
        ConnectionProfileTaskSettings settings = task.Settings;
        bool timeConstraintApplied = settings.StartTimeConstraint.HasValue && settings.EndTimeConstraint.HasValue;

        OnProgressUpdated(this, new() { Message = $"Executing task \"{task.Name}\"..." });

        task.Reset();

        if (string.IsNullOrWhiteSpace(settings.ExternalOperation))
        {
            if (timeConstraintApplied)
            {
                // Execute time constraints on FTP tasks day-by-day to match existing FTP download logic
                DateTime startTimeConstraint = settings.StartTimeConstraint.Value;
                DateTime endTimeConstraint = settings.EndTimeConstraint.Value;
                DateTime topOfDayStartTime = startTimeConstraint.BaselinedTimestamp(BaselineTimeInterval.Day);
                int dayRange = (int)Math.Ceiling((endTimeConstraint - startTimeConstraint).TotalDays);

                for (int i = 0; i < dayRange; i++)
                {
                    settings.StartTimeConstraint = i == 0 ? startTimeConstraint : topOfDayStartTime.AddDays(i);
                    settings.EndTimeConstraint = i == dayRange - 1 ? endTimeConstraint : topOfDayStartTime.AddDays(i + 1).AddTicks(-1);
                    ExecuteFTPTask(task, ftpClient, settings.StartTimeConstraint.Value);
                }
            }
            else
            {
                ExecuteFTPTask(task, ftpClient, localTime);
            }
        }
        else
        {
            ExecuteExternalOperationTask(task, localTime);
        }

        // Handle local file age limit processing, if enabled
        if (settings.DeleteOldLocalFiles && !timeConstraintApplied)
            HandleLocalFileAgeLimitProcessing(task);

        OnProgressUpdated(this, new() { OverallProgress = ++m_overallTasksCompleted, OverallProgressTotal = m_overallTasksCount });

        if (!task.Success)
            LogFailure(task.FailMessage);
    }

    private void HandleLocalFileAgeLimitProcessing(ConnectionProfileTask task)
    {
        ConnectionProfileTaskSettings settings = task.Settings;

        if (string.IsNullOrWhiteSpace(settings.LocalPath) || !Directory.Exists(settings.LocalPath))
        {
            OnProcessException(MessageLevel.Warning, new InvalidOperationException($"Cannot handle local file age limit processing for connection profile task \"{task.Name}\": Local path \"{settings.LocalPath ?? ""}\" does not exist."));
            return;
        }

        OnStatusMessage(MessageLevel.Info, $"Enumerating local files in \"{settings.LocalPath}\"...");

        try
        {
            string[] files = FilePath.GetFileList(Path.Combine(FilePath.GetAbsolutePath(settings.LocalPath), "*\\*.*"));
            long deletedCount = 0;

            OnStatusMessage(MessageLevel.Info, $"Found {files.Length} local files, starting age limit processing...");

            foreach (string file in files)
            {
                // Check file specification restriction
                if (!FilePath.IsFilePatternMatch(settings.FileSpecs, file, true))
                    continue;

                DateTime creationTime = File.GetCreationTime(file);

                if ((DateTime.Now - creationTime).Days > Program.Host.Model.Global.MaxLocalFileAge)
                {
                    OnStatusMessage(MessageLevel.Info, $"Attempting to delete file \"{file}\" created at \"{creationTime:yyyy-MM-dd HH:mm.ss.fff}\"...");

                    try
                    {
                        string rootPathName = FilePath.GetDirectoryName(settings.LocalPath);
                        string directoryName = FilePath.GetDirectoryName(file);

                        FilePath.WaitForWriteLock(file);
                        File.Delete(file);
                        deletedCount++;
                        OnStatusMessage(MessageLevel.Info, $"File \"{file}\" successfully deleted...");

                        if (!directoryName.Equals(rootPathName, StringComparison.OrdinalIgnoreCase))
                        {
                            // Try to remove sub-folder, this will only succeed if folder is empty...
                            try
                            {
                                Directory.Delete(directoryName);
                                OnStatusMessage(MessageLevel.Info, $"Removed empty folder \"{directoryName}\"...");
                            }
                            catch
                            {
                                // Failure is common case, nothing to report
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        OnProcessException(MessageLevel.Warning, new InvalidOperationException($"Failed to delete file \"{file}\": {ex.Message}", ex));
                    }
                }
            }

            OnStatusMessage(MessageLevel.Info, deletedCount > 0 ?
                                $"Deleted {deletedCount} files during local file age limit processing." :
                                "No files deleted during local file age limit processing.");
        }
        catch (Exception ex)
        {
            OnProcessException(MessageLevel.Warning, new InvalidOperationException($"Failed to enumerate local files in \"{settings.LocalPath}\": {ex.Message}", ex));
        }
    }

    private bool AdapterIsEnabled()
    {
        try
        {
            using AdoDataConnection connection = new("systemSettings");
            return connection.ExecuteScalar<bool>("SELECT Enabled FROM Device WHERE Acronym = {0}", Name);
        }
        catch (Exception ex)
        {
            string message = $"Cannot check adapter enabled state in database - task execution skipped: {ex.Message}";

            OnProgressUpdated(this, new()
            {
                State = ProgressState.Fail,
                ErrorMessage = message,
                OverallProgress = 1,
                OverallProgressTotal = 1
            });

            OnStatusMessage(MessageLevel.Warning, message);
            return false;
        }
    }

    private FtpClient ConnectFTPClient()
    {
        FtpClient ftpClient = new(FTPType);

        ftpClient.CommandSent += FtpClient_CommandSent;
        ftpClient.ResponseReceived += FtpClient_ResponseReceived;
        ftpClient.FileTransferProgress += FtpClient_FileTransferProgress;
        ftpClient.FileTransferNotification += FtpClient_FileTransferNotification;

        string[] parts = ConnectionHostName.Split(':');

        if (parts.Length > 1)
        {
            ftpClient.Server = parts[0];
            ftpClient.Port = int.Parse(parts[1]);
        }
        else
        {
            ftpClient.Server = ConnectionHostName;
        }

        ftpClient.Timeout = ConnectionTimeout;
        ftpClient.Passive = PassiveFTP;
        ftpClient.ActiveAddress = ActiveFTPAddress;
        ftpClient.MinActivePort = MinActiveFTPPort;
        ftpClient.MaxActivePort = MaxActiveFTPPort;
        ftpClient.Connect(ConnectionUserName, ConnectionPassword);

        return ftpClient;
    }

    private void DisconnectFTPClient(FtpClient ftpClient)
    {
        ftpClient.CommandSent -= FtpClient_CommandSent;
        ftpClient.ResponseReceived -= FtpClient_ResponseReceived;
        ftpClient.FileTransferProgress -= FtpClient_FileTransferProgress;
        ftpClient.FileTransferNotification -= FtpClient_FileTransferNotification;
        ftpClient.Dispose();
    }

    private bool ConnectDialUp()
    {
        if (!UseDialUp)
            return false;

        m_startDialUpTime = 0;
        DisconnectDialUp();

        try
        {
            if (RasState == RasConnectionState.Connected)
                throw new InvalidOperationException($"Cannot connect to \"{DialUpEntryName}\": already connected.");

            OnStatusMessage(MessageLevel.Info, $"Initiating dial-up for \"{DialUpEntryName}\"...");
            AttemptedDialUps++;

            m_rasDialer.EntryName = DialUpEntryName;
            m_rasDialer.PhoneNumber = DialUpPassword;
            m_rasDialer.Timeout = DialUpTimeout;
            m_rasDialer.Credentials = new(DialUpUserName, DialUpPassword);
            m_rasDialer.Dial();

            m_startDialUpTime = DateTime.UtcNow.Ticks;
            SuccessfulDialUps++;
            OnStatusMessage(MessageLevel.Info, $"Dial-up connected on \"{DialUpEntryName}\"");
            return true;
        }
        catch (Exception ex)
        {
            FailedDialUps++;
            OnProcessException(MessageLevel.Warning, new InvalidOperationException($"Exception while attempting to dial entry \"{DialUpEntryName}\": {ex.Message}", ex));
            DisconnectDialUp();
        }

        return false;
    }

    private void DisconnectDialUp()
    {
        if (!UseDialUp)
            return;

        try
        {
            OnStatusMessage(MessageLevel.Info, $"Initiating hang-up for \"{DialUpEntryName}\"");
            RasConnection.GetActiveConnections().FirstOrDefault(ras => ras.EntryName == DialUpEntryName)?.HangUp();
        }
        catch (Exception ex)
        {
            OnProcessException(MessageLevel.Warning, new InvalidOperationException($"Exception while attempting to hang-up \"{DialUpEntryName}\": {ex.Message}", ex));
        }

        if (m_startDialUpTime <= 0)
            return;

        Ticks dialUpConnectedTime = DateTime.UtcNow.Ticks - m_startDialUpTime;
        OnStatusMessage(MessageLevel.Info, $"Dial-up connected for {dialUpConnectedTime.ToElapsedTimeString(2)}");
        m_startDialUpTime = 0;
        TotalDialUpTime += dialUpConnectedTime;
    }

    private string GetLocalPathDirectory(ConnectionProfileTaskSettings settings, DateTime localTime)
    {
        Dictionary<string, string> substitutions = new()
        {
            { "<YYYY>", $"{localTime.Year}" },
            { "<YY>", $"{localTime.Year.ToString().Substring(2)}" },
            { "<MM>", $"{localTime.Month.ToString().PadLeft(2, '0')}" },
            { "<DD>", $"{localTime.Day.ToString().PadLeft(2, '0')}" },
            { "<DeviceName>", m_deviceRecord.Name ?? "undefined" },
            { "<DeviceAcronym>", m_deviceRecord.Acronym },
            { "<DeviceFolderName>", m_deviceRecord.OriginalSource ?? m_deviceRecord.Acronym },
            { "<ProfileName>", m_connectionProfile.Name ?? "undefined" }
        };

        string subPath = substitutions.Aggregate(settings.DirectoryNamingExpression, (expression, kvp) => expression.Replace(kvp.Key, kvp.Value));

        if (!string.IsNullOrEmpty(settings.LocalPath))
            subPath = subPath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        string directoryName = Path.Combine(settings.LocalPath, subPath);

        if (!Directory.Exists(directoryName))
        {
            try
            {
                Directory.CreateDirectory(directoryName);
            }
            catch (Exception ex)
            {
                OnProcessException(MessageLevel.Warning, new InvalidOperationException($"Failed to create directory \"{directoryName}\": {ex.Message}", ex));
            }
        }

        return directoryName;
    }

    private string GetRemotePathDirectory(ConnectionProfileTaskSettings settings, DateTime localTime)
    {
        Dictionary<string, string> substitutions = new()
        {
            { "<YYYY>", $"{localTime.Year}" },
            { "<YY>", $"{localTime.Year.ToString().Substring(2)}" },
            { "<MM>", $"{localTime.Month.ToString().PadLeft(2, '0')}" },
            { "<DD>", $"{localTime.Day.ToString().PadLeft(2, '0')}" },
            { "<Month MM>", $"Month {localTime.Month.ToString().PadLeft(2, '0')}" },
            { "<Day DD>", $"Day {localTime.Day.ToString().PadLeft(2, '0')}" },
            { "<Day DD-1>", $"Day {localTime.AddDays(-1).Day.ToString().PadLeft(2, '0')}" },
            { "<DeviceName>", m_deviceRecord.Name ?? "undefined" },
            { "<DeviceAcronym>", m_deviceRecord.Acronym },
            { "<DeviceFolderName>", m_deviceRecord.OriginalSource ?? m_deviceRecord.Acronym },
            { "<ProfileName>", m_connectionProfile.Name ?? "undefined" }
        };

        if (settings.RemotePath.Contains("<Day DD-1>"))
        {
            substitutions["<YYYY>"] = $"{localTime.AddDays(-1).Year}";
            substitutions["<YY>"] = $"{localTime.AddDays(-1).Year.ToString().Substring(2)}";
            substitutions["<MM>"] = $"{localTime.AddDays(-1).Month.ToString().PadLeft(2, '0')}";
            substitutions["<Month MM>"] = $"Month {localTime.AddDays(-1).Month.ToString().PadLeft(2, '0')}";
        }

        return substitutions.Aggregate(settings.RemotePath, (path, sub) => path.Replace(sub.Key, sub.Value));
    }

    #endregion

    #region [ FTP Task Operations ]

    private void ExecuteFTPTask(ConnectionProfileTask task, FtpClient ftpClient, DateTime localTime)
    {
        if (ftpClient == null)
        {
            task.Fail();
            return;
        }

        try
        {
            string remotePathDirectory = GetRemotePathDirectory(task.Settings, localTime);
            string localPathDirectory = GetLocalPathDirectory(task.Settings, localTime);
            List<FtpFileWrapper> files = new();

            OnStatusMessage(MessageLevel.Info, $"Ensuring local path \"{localPathDirectory}\" exists.");
            Directory.CreateDirectory(localPathDirectory);

            OnStatusMessage(MessageLevel.Info, $"Building list of files to be downloaded from \"{remotePathDirectory}\".");
            BuildFileList(files, task, ftpClient, remotePathDirectory, localPathDirectory);
            DownloadAllFiles(files, task);
        }
        catch (Exception ex)
        {
            task.Fail(ex.Message);
            OnProcessException(MessageLevel.Error, ex, "ExecuteFTPTask");
        }
    }

    private void BuildFileList(List<FtpFileWrapper> fileList, ConnectionProfileTask task, FtpClient client, string remotePathDirectory, string localPathDirectory)
    {
        ConnectionProfileTaskSettings settings = task.Settings;
        bool timeConstraintApplied = settings.StartTimeConstraint.HasValue && settings.EndTimeConstraint.HasValue;

        if (m_cancellationToken.IsCancelled)
            return;

        OnStatusMessage(MessageLevel.Info, $"Attempting to set remote FTP directory path \"{remotePathDirectory}\"...");
        client.SetCurrentDirectory(remotePathDirectory);

        OnStatusMessage(MessageLevel.Info, $"Enumerating remote files in \"{remotePathDirectory}\"{(timeConstraintApplied ? $" with time constraint from {settings.StartTimeConstraint.Value:yyyy-MM-dd HH:mm.ss.fff} to {settings.EndTimeConstraint.Value:yyyy-MM-dd HH:mm.ss.fff}" : "")}...");

        foreach (FtpFile file in client.CurrentDirectory.Files.Values)
        {
            if (m_cancellationToken.IsCancelled)
                return;

            if (!FilePath.IsFilePatternMatch(settings.FileSpecs, file.Name, true))
                continue;

            if (timeConstraintApplied)
            {
                if (file.Timestamp < settings.StartTimeConstraint.Value || file.Timestamp > settings.EndTimeConstraint.Value)
                    continue;
            }
            else if (settings.LimitRemoteFileDownloadByAge && (DateTime.Now - file.Timestamp).Days > Program.Host.Model.Global.MaxRemoteFileAge)
            {
                OnStatusMessage(MessageLevel.Info, $"File \"{file.Name}\" skipped, timestamp \"{file.Timestamp:yyyy-MM-dd HH:mm.ss.fff}\" is older than {Program.Host.Model.Global.MaxRemoteFileAge} days.");
                OnProgressUpdated(this, new() { Message = $"File \"{file.Name}\" skipped: File is too old." });
                continue;
            }

            if (file.Size > settings.MaximumFileSize * SI2.Mega)
            {
                OnStatusMessage(MessageLevel.Info, $"File \"{file.Name}\" skipped, size of {file.Size / SI2.Mega:N3} MB is larger than {settings.MaximumFileSize:N3} MB configured limit.");
                OnProgressUpdated(this, new() { Message = $"File \"{file.Name}\" skipped: File is too large ({file.Size / (double)SI2.Mega:N3} MB)." });
                continue;
            }

            string localPath = Path.Combine(localPathDirectory, file.Name);

            if (File.Exists(localPath) && settings.SkipDownloadIfUnchanged)
            {
                try
                {
                    FileInfo info = new(localPath);

                    // Compare file sizes and timestamps
                    bool localEqualsRemote =
                        info.Length == file.Size &&
                        (!settings.SynchronizeTimestamps || info.LastWriteTime == file.Timestamp);

                    if (localEqualsRemote)
                    {
                        OnStatusMessage(MessageLevel.Info, $"Skipping file download for remote file \"{file.Name}\": Local file already exists and matches remote file.");
                        OnProgressUpdated(this, new() { Message = $"File \"{file.Name}\" skipped: Local file already exists and matches remote file" });
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    OnProcessException(MessageLevel.Warning, new InvalidOperationException($"Unable to determine whether local file size and time matches remote file size and time due to exception: {ex.Message}", ex));
                }
            }

            fileList.Add(new(localPath, file));
        }

        if (!settings.RecursiveDownload)
            return;

        FtpDirectory[] directories = Array.Empty<FtpDirectory>();

        try
        {
            OnStatusMessage(MessageLevel.Info, $"Enumerating remote directories in \"{remotePathDirectory}\"...");
            directories = client.CurrentDirectory.SubDirectories.Values.ToArray();
        }
        catch (Exception ex)
        {
            task.Fail(ex.Message);
            OnProcessException(MessageLevel.Error, new($"Failed to enumerate remote directories in \"{remotePathDirectory}\" due to exception: {ex.Message}", ex));
            OnProgressUpdated(this, new() { ErrorMessage = $"Failed to enumerate remote directories in \"{remotePathDirectory}\": {ex.Message}" });
        }

        foreach (FtpDirectory directory in directories)
        {
            try
            {
                if (m_cancellationToken.IsCancelled)
                    return;

                string directoryName = directory.Name;

                if (directoryName.StartsWith(".", StringComparison.Ordinal))
                    continue;

                string remoteSubPath = FTPPathCombine(remotePathDirectory, directoryName);
                string localSubPath = Path.Combine(localPathDirectory, directoryName);

                OnStatusMessage(MessageLevel.Info, $"Recursively adding files in \"{remotePathDirectory}\" to file list...");
                BuildFileList(fileList, task, client, remoteSubPath, localSubPath);
            }
            catch (Exception ex)
            {
                task.Fail(ex.Message);
                OnProcessException(MessageLevel.Error, new($"Failed to add remote files from remote directory \"{directory.Name}\" to file list due to exception: {ex.Message}", ex));
                OnProgressUpdated(this, new() { ErrorMessage = $"Failed to build file list for remote directory \"{directory.Name}\": {ex.Message}" });
            }
        }
    }

    private void DownloadAllFiles(List<FtpFileWrapper> files, ConnectionProfileTask task)
    {
        ConnectionProfileTaskSettings settings = task.Settings;
        long progress = 0L;
        long totalBytes = files.Sum(wrapper => wrapper.RemoteFile.Size);

        if (m_cancellationToken.IsCancelled)
            return;

        OnProgressUpdated(this, new() { OverallProgress = m_overallTasksCompleted * totalBytes, OverallProgressTotal = totalBytes * m_overallTasksCount });

        // Group files by destination directory so we can skip whole groups
        // of files if the directory does not exist and cannot be created
        foreach (IGrouping<string, FtpFileWrapper> grouping in files.GroupBy(wrapper => Path.GetDirectoryName(wrapper.LocalPath)))
        {
            if (m_cancellationToken.IsCancelled)
                return;

            try
            {
                Directory.CreateDirectory(grouping.Key);
            }
            catch (Exception ex)
            {
                task.Fail(ex.Message);

                string message = $"Failed to create local directory for {grouping.Count():N0} remote files due to exception: {ex.Message}";
                OnProcessException(MessageLevel.Error, new(message, ex));
                progress += grouping.Sum(wrapper => wrapper.RemoteFile.Size);
                OnProgressUpdated(this, new() { ErrorMessage = message, OverallProgress = m_overallTasksCompleted * totalBytes + progress });
                continue;
            }

            foreach (FtpFileWrapper wrapper in grouping)
            {
                if (m_cancellationToken.IsCancelled)
                    return;

                OnStatusMessage(MessageLevel.Info, $"Verifying logic allows for download of remote file \"{wrapper.RemoteFile.Name}\"...");

                // Update progress in advance in case the transfer fails
                progress += wrapper.RemoteFile.Size;
                TotalProcessedFiles++;

                bool fileUpdated = File.Exists(wrapper.LocalPath) && settings.SkipDownloadIfUnchanged;

                if (File.Exists(wrapper.LocalPath) && settings.ArchiveExistingFilesBeforeDownload)
                {
                    try
                    {
                        string directoryName = Path.Combine(grouping.Key, "Archive\\");
                        string archiveFileName = Path.Combine(directoryName, wrapper.RemoteFile.Name);

                        Directory.CreateDirectory(directoryName);

                        if (File.Exists(archiveFileName))
                            archiveFileName = FilePath.GetUniqueFilePathWithBinarySearch(archiveFileName);

                        OnStatusMessage(MessageLevel.Info, $"Archiving existing file \"{wrapper.LocalPath}\" to \"{archiveFileName}\"...");
                        File.Move(wrapper.LocalPath, archiveFileName);
                    }
                    catch (Exception ex)
                    {
                        OnProcessException(MessageLevel.Warning, new InvalidOperationException($"Failed to archive existing local file \"{wrapper.LocalPath}\" due to exception: {ex.Message}", ex));
                    }
                }

                if (File.Exists(wrapper.LocalPath) && !settings.OverwriteExistingLocalFiles)
                {
                    task.Fail("Local file already exists and settings do not allow overwrite.");
                    OnStatusMessage(MessageLevel.Info, $"Skipping file download for remote file \"{wrapper.RemoteFile.Name}\": Local file already exists and settings do not allow overwrite.");
                    OnProgressUpdated(this, new() { ErrorMessage = $"File \"{wrapper.RemoteFile.Name}\" skipped: Local file already exists", OverallProgress = m_overallTasksCompleted * totalBytes + progress });
                    continue;
                }

                try
                {
                    // Download the file
                    OnStatusMessage(MessageLevel.Info, $"Downloading remote file from \"{wrapper.RemoteFile.FullPath}\" to local path \"{wrapper.LocalPath}\"...");
                    wrapper.Get();

                    if (settings.DeleteRemoteFilesAfterDownload)
                    {
                        try
                        {
                            // Remove the remote file
                            OnStatusMessage(MessageLevel.Info, $"Removing file \"{wrapper.RemoteFile.FullPath}\" from remote server...");
                            wrapper.RemoteFile.Remove();
                        }
                        catch (Exception ex)
                        {
                            task.Fail(ex.Message);

                            string message = $"Failed to remove file \"{wrapper.RemoteFile.FullPath}\" from remote server due to exception: {ex.Message}";
                            OnProcessException(MessageLevel.Warning, new InvalidOperationException(message, ex));
                            OnProgressUpdated(this, new() { ErrorMessage = message });
                        }
                    }

                    // Update these statistics only if
                    // the file download was successful
                    FilesDownloaded++;
                    TotalFilesDownloaded++;
                    BytesDownloaded += wrapper.RemoteFile.Size;

                    OnProgressUpdated(this, new()
                    {
                        Message = $"Successfully downloaded remote file \"{wrapper.RemoteFile.FullPath}\".",
                        Summary = $"{FilesDownloaded} Files Downloaded ({TotalFilesDownloaded} Total)",
                        OverallProgress = m_overallTasksCompleted * totalBytes + progress
                    });

                    // Synchronize local timestamp to that of remote file if requested
                    if (settings.SynchronizeTimestamps)
                    {
                        FileInfo info = new(wrapper.LocalPath);

                        while (info.LastAccessTime != wrapper.RemoteFile.Timestamp || info.LastWriteTime != wrapper.RemoteFile.Timestamp)
                        {
                            info.LastAccessTime = info.LastWriteTime = wrapper.RemoteFile.Timestamp;
                            info.Refresh();
                        }
                    }

                    m_lastDownloadedFileID = LogDownloadedFile(wrapper.LocalPath);
                }
                catch (Exception ex)
                {
                    task.Fail(ex.Message);

                    string message = $"Failed to download remote file \"{wrapper.RemoteFile.FullPath}\" due to exception: {ex.Message}";
                    OnProcessException(MessageLevel.Warning, new InvalidOperationException(message, ex));
                    OnProgressUpdated(this, new() { ErrorMessage = message, OverallProgress = m_overallTasksCompleted * totalBytes + progress });
                }

                // Send e-mail on file update, if requested
                if (fileUpdated && settings.EmailOnFileUpdate)
                {
                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        try
                        {
                            GlobalSettings global = Program.Host.Model.Global;
                            string subject = $"File changed for \"{Name}: {task.Name}\"";
                            string body = $"<b>File Name = {wrapper.LocalPath}</b></br>";

                            if (string.IsNullOrWhiteSpace(global.SmtpUserName))
                                Mail.Send(global.FromAddress, settings.EmailRecipients, subject, body, true, global.SmtpServer);
                            else
                                Mail.Send(global.FromAddress, settings.EmailRecipients, subject, body, true, global.SmtpServer, global.SmtpUserName, global.SmtpPassword);
                        }
                        catch (Exception ex)
                        {
                            OnProcessException(MessageLevel.Warning, new InvalidOperationException($"Failed to send e-mail notification about updated file \"{wrapper.LocalPath}\": {ex.Message}"));
                        }
                    });
                }
            }
        }
    }

    #endregion

    #region [ External Task Operations ]

    private bool TestExternalOperationConnection(ConnectionProfileTask task)
    {
        ConnectionProfileTaskSettings settings = task.Settings;

        Dictionary<string, string> substitutions = new()
        {
            { "<DeviceID>", m_deviceRecord.ID.ToString() },
            { "<DeviceName>", m_deviceRecord.Name ?? "undefined" },
            { "<DeviceAcronym>", m_deviceRecord.Acronym },
            { "<ConnectionHostName>", ConnectionHostName },
            { "<ConnectionUserName>", ConnectionUserName },
            { "<ConnectionPassword>", ConnectionPassword },
            { "<ConnectionTimeout>", ConnectionTimeout.ToString() },
            { "<ProfileName>", m_connectionProfile.Name ?? "undefined" },
            { "<TaskID>", task.ID.ToString() }
        };

        string command = substitutions.Aggregate(settings.ExternalOperation.Trim(), (str, kvp) => str.Replace(kvp.Key, kvp.Value));
        string executable = Arguments.ParseCommand(command)[0];
        string args = $"{TestConnectionParameter} {command.Substring(executable.Length).Trim()}";
        TimeSpan timeout = TimeSpan.FromSeconds(settings.ExternalOperationTimeout ?? ConnectionTimeout / 1000.0D);

        try
        {
            using Process externalOperation = new();
            DateTime lastUpdate = DateTime.UtcNow;
            LogType logType = LogType.Unknown;

            externalOperation.StartInfo.FileName = executable;
            externalOperation.StartInfo.Arguments = args;
            externalOperation.StartInfo.RedirectStandardOutput = true;
            externalOperation.StartInfo.UseShellExecute = false;
            externalOperation.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();

            externalOperation.OutputDataReceived += (sender, processArgs) =>
            {
                if (string.IsNullOrWhiteSpace(processArgs.Data))
                    return;

                if (HandleExternalOperationMessage(processArgs.Data, out logType))
                    return;

                lastUpdate = DateTime.UtcNow;
            };

            externalOperation.Start();
            externalOperation.BeginOutputReadLine();

            while (!externalOperation.WaitForExit(1000))
            {
                if (m_cancellationToken.IsCancelled)
                {
                    TerminateProcessTree(externalOperation.Id);
                    return false;
                }

                if (timeout > TimeSpan.Zero && DateTime.UtcNow - lastUpdate > timeout)
                {
                    TerminateProcessTree(externalOperation.Id);
                    return false;
                }
            }

            return logType == LogType.ConnectionSuccess;
        }
        catch
        {
            return false;
        }
    }

    private void ExecuteExternalOperationTask(ConnectionProfileTask task, DateTime localTime)
    {
        ConnectionProfileTaskSettings settings = task.Settings;
        string localPathDirectory = GetLocalPathDirectory(settings, localTime);
        string dateTimeFormat = Program.Host.Model.Global.DateTimeFormat;

        Dictionary<string, string> substitutions = new()
        {
            { "<DeviceID>", m_deviceRecord.ID.ToString() },
            { "<DeviceName>", m_deviceRecord.Name ?? "undefined" },
            { "<DeviceAcronym>", m_deviceRecord.Acronym },
            { "<DeviceFolderName>", m_deviceRecord.OriginalSource ?? m_deviceRecord.Acronym },
            { "<DeviceFolderPath>", localPathDirectory },
            { "<ConnectionHostName>", ConnectionHostName },
            { "<ConnectionUserName>", ConnectionUserName },
            { "<ConnectionPassword>", ConnectionPassword },
            { "<ConnectionTimeout>", ConnectionTimeout.ToString() },
            { "<ProfileName>", m_connectionProfile.Name ?? "undefined" },
            { "<TaskID>", task.ID.ToString() },
            { "<StartTime>", (settings.StartTimeConstraint ?? DateTime.MinValue).ToString(dateTimeFormat) },
            { "<EndTime>", (settings.EndTimeConstraint ?? DateTime.MaxValue).ToString(dateTimeFormat) }
        };

        string command = substitutions.Aggregate(settings.ExternalOperation.Trim(), (str, kvp) => str.Replace(kvp.Key, kvp.Value));
        string executable = Arguments.ParseCommand(command)[0];
        string args = command.Substring(executable.Length).Trim();
        TimeSpan timeout = TimeSpan.FromSeconds(settings.ExternalOperationTimeout ?? ConnectionTimeout / 1000.0D);

        OnStatusMessage(MessageLevel.Info, $"Executing external operation \"{command}\"...");
        OnProgressUpdated(this, new() { Message = $"Executing external operation command \"{command}\"..." });

        try
        {
            using SafeFileWatcher fileWatcher = new(localPathDirectory);
            using Process externalOperation = new();
            DateTime lastUpdate = DateTime.UtcNow;

            fileWatcher.Created += (sender, fileArgs) => lastUpdate = DateTime.UtcNow;
            fileWatcher.Changed += (sender, fileArgs) => lastUpdate = DateTime.UtcNow;
            fileWatcher.Deleted += (sender, fileArgs) => lastUpdate = DateTime.UtcNow;
            fileWatcher.EnableRaisingEvents = true;

            externalOperation.StartInfo.FileName = executable;
            externalOperation.StartInfo.Arguments = args;
            externalOperation.StartInfo.RedirectStandardOutput = true;
            externalOperation.StartInfo.RedirectStandardError = true;
            externalOperation.StartInfo.UseShellExecute = false;
            externalOperation.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();

            externalOperation.OutputDataReceived += (sender, processArgs) =>
            {
                if (string.IsNullOrWhiteSpace(processArgs.Data))
                    return;

                lastUpdate = DateTime.UtcNow;

                if (HandleExternalOperationMessage(processArgs.Data, out _))
                    return;

                OnStatusMessage(MessageLevel.Info, processArgs.Data);
                OnProgressUpdated(this, new() { Message = processArgs.Data });
            };

            externalOperation.ErrorDataReceived += (sender, processArgs) =>
            {
                if (string.IsNullOrWhiteSpace(processArgs.Data))
                    return;

                task.Fail(processArgs.Data);
                lastUpdate = DateTime.UtcNow;
                OnStatusMessage(MessageLevel.Error, processArgs.Data);
                OnProgressUpdated(this, new() { ErrorMessage = processArgs.Data });
            };

            externalOperation.Start();
            externalOperation.BeginOutputReadLine();
            externalOperation.BeginErrorReadLine();

            while (!externalOperation.WaitForExit(1000))
            {
                if (m_cancellationToken.IsCancelled)
                {
                    task.Fail();
                    TerminateProcessTree(externalOperation.Id);
                    OnProcessException(MessageLevel.Warning, new InvalidOperationException($"External operation \"{command}\" forcefully terminated: downloader was disabled."));
                    OnProgressUpdated(this, new() { ErrorMessage = "External operation forcefully terminated: downloader was disabled." });
                    return;
                }

                if (timeout > TimeSpan.Zero && DateTime.UtcNow - lastUpdate > timeout)
                {
                    task.Fail();
                    TerminateProcessTree(externalOperation.Id);
                    OnProcessException(MessageLevel.Error, new InvalidOperationException($"External operation \"{command}\" forcefully terminated: exceeded timeout ({timeout.TotalSeconds:0.##} seconds)."));
                    OnProgressUpdated(this, new() { ErrorMessage = $"External operation forcefully terminated: exceeded timeout ({timeout.TotalSeconds:0.##} seconds)." });
                    return;
                }
            }

            OnStatusMessage(MessageLevel.Info, $"External operation \"{command}\" completed with status code {externalOperation.ExitCode}.");
            OnProgressUpdated(this, new() { Message = $"External action complete: exit code {externalOperation.ExitCode}." });
        }
        catch (Exception ex)
        {
            task.Fail(ex.Message);
            OnProcessException(MessageLevel.Error, new InvalidOperationException($"Failed to execute external operation \"{command}\": {ex.Message}", ex));
            OnProgressUpdated(this, new() { ErrorMessage = $"Failed to execute external action: {ex.Message}" });
        }
    }

    private bool HandleExternalOperationMessage(string message, out LogType logType)
    {
        Match downloadedFileMatch = Regex.Match(message, LogDownloadedFilePattern);
        string patternMessage;

        if (downloadedFileMatch.Success)
        {
            logType = LogType.Download;
            patternMessage = downloadedFileMatch.Groups["FilePath"].Value;
            m_lastDownloadedFileID = LogDownloadedFile(patternMessage);

            FilesDownloaded++;
            TotalFilesDownloaded++;

            OnProgressUpdated(this, new() { Summary = $"{FilesDownloaded:N0} Files Downloaded ({TotalFilesDownloaded:N0} Total)" });
            return true;
        }

        Match connectionSuccessMatch = Regex.Match(message, LogConnectionSuccessPattern);

        if (connectionSuccessMatch.Success)
        {
            logType = LogType.ConnectionSuccess;
            patternMessage = downloadedFileMatch.Groups["Message"].Value;
            LogOutcome(ProgressState.Processing);

            AttemptedConnections++;
            SuccessfulConnections++;
            LastSuccessfulConnectionTime = DateTime.UtcNow;

            OnProgressUpdated(this, new() { Summary = patternMessage });
            return true;
        }

        Match connectionFailureMatch = Regex.Match(message, LogConnectionFailurePattern);

        if (connectionFailureMatch.Success)
        {
            logType = LogType.ConnectionFailure;
            patternMessage = downloadedFileMatch.Groups["Message"].Value;
            LogFailure(patternMessage);

            AttemptedConnections++;
            FailedConnections++;
            LastFailedConnectionTime = DateTime.UtcNow;
            LastFailedConnectionReason = patternMessage;

            OnProgressUpdated(this, new() { State = ProgressState.Fail, ErrorMessage = patternMessage });
            return true;
        }

        logType = LogType.Unknown;
        return false;
    }

    #endregion

    #region [ Task Logging Handlers ]

    private int LogDownloadedFile(string filePath)
    {
        try
        {
            FileInfo fileInfo = new(filePath);
            using AdoDataConnection connection = new("systemSettings");
            TableOperations<DownloadedFile> downloadedFileTable = new(connection);

            DownloadedFile downloadedFile = new()
            {
                DeviceID = m_deviceRecord.ID,
                FilePath = filePath,
                Timestamp = DateTime.UtcNow,
                CreationTime = fileInfo.CreationTimeUtc,
                LastWriteTime = fileInfo.LastWriteTimeUtc,
                LastAccessTime = fileInfo.LastAccessTimeUtc,
                FileSize = (int)fileInfo.Length
            };

            downloadedFileTable.AddNewRecord(downloadedFile);

            return connection.ExecuteScalar<int>("SELECT ID FROM DownloadedFile WHERE FilePath = {0}", filePath);
        }
        catch (Exception ex)
        {
            OnProcessException(MessageLevel.Error, ex);
            return 0;
        }
    }

    private void LogOutcome(ProgressState outcome)
    {
        try
        {
            using AdoDataConnection connection = new("systemSettings");
            TableOperations<StatusLog> statusLogTable = new(connection);
            StatusLog log = statusLogTable.QueryRecordWhere("DeviceID = {0}", m_deviceRecord.ID) ?? statusLogTable.NewRecord();

            log.DeviceID = m_deviceRecord.ID;
            log.LastOutcome = outcome.ToString();
            log.LastRun = DateTime.UtcNow;

            statusLogTable.AddNewOrUpdateRecord(log);
        }
        catch (Exception ex)
        {
            OnProcessException(MessageLevel.Error, ex);
        }
    }

    private void LogFailure(string message)
    {
        try
        {
            using AdoDataConnection connection = new("systemSettings");
            TableOperations<StatusLog> statusLogTable = new(connection);
            StatusLog log = statusLogTable.QueryRecordWhere("DeviceID = {0}", m_deviceRecord.ID) ?? statusLogTable.NewRecord();

            log.DeviceID = m_deviceRecord.ID;
            log.LastFailure = DateTime.UtcNow;
            log.LastErrorMessage = message;

            statusLogTable.AddNewOrUpdateRecord(log);
        }
        catch (Exception ex)
        {
            OnProcessException(MessageLevel.Error, ex);
        }
    }

    private void LogDownload(int lastDownloadedFileID, DateTime startTime, DateTime endTime)
    {
        try
        {
            using AdoDataConnection connection = new("systemSettings");
            TableOperations<StatusLog> statusLogTable = new(connection);
            StatusLog log = statusLogTable.QueryRecordWhere("DeviceID = {0}", m_deviceRecord.ID) ?? statusLogTable.NewRecord();

            log.DeviceID = m_deviceRecord.ID;
            log.LastDownloadedFileID = lastDownloadedFileID;
            log.LastDownloadStartTime = startTime;
            log.LastDownloadEndTime = endTime;
            log.LastDownloadFileCount = (int)FilesDownloaded;

            statusLogTable.AddNewOrUpdateRecord(log);
        }
        catch (Exception ex)
        {
            OnProcessException(MessageLevel.Error, ex);
        }
    }

    #endregion

    #region [ Event Handlers ]

    private void RasDialer_Error(object sender, ErrorEventArgs e) =>
        OnProcessException(MessageLevel.Warning, e.GetException());

    private void FtpClient_CommandSent(object sender, EventArgs<string> e)
    {
        if (LogConnectionMessages)
            OnStatusMessage(MessageLevel.Info, $"FTP Request: {e.Argument}");
    }

    private void FtpClient_ResponseReceived(object sender, EventArgs<string> e)
    {
        if (LogConnectionMessages)
            OnStatusMessage(MessageLevel.Info, $"FTP Response: {e.Argument}");
    }

    private void FtpClient_FileTransferProgress(object sender, EventArgs<ProcessProgress<long>, TransferDirection> e)
    {
        ProcessProgress<long> progress = e.Argument1;

        OnProgressUpdated(this, new()
        {
            Message = progress.ProgressMessage,
            Progress = progress.Complete,
            ProgressTotal = progress.Total
        });
    }

    private void FtpClient_FileTransferNotification(object sender, EventArgs<FtpTransferResult> e) =>
        OnStatusMessage(MessageLevel.Info, $"FTP File Transfer: {e.Argument.Message}, response code = {e.Argument.ResponseCode}");

    #endregion

    #endregion

    #region [ Static ]

    // Static Fields
    private static readonly ScheduleManager s_scheduleManager;
    private static readonly ConcurrentDictionary<string, Tuple<Downloader, ConnectionProfileTask>> s_taskSchedules;
    private static readonly List<ProgressUpdateWrapper> s_queuedProgressUpdates;
    private static ICancellationToken s_progressUpdateCancellationToken;
    private static readonly string LogDownloadedFilePattern = string.Format(LogDownloadedFileTemplate, "(?<FilePath>.+)");
    private static readonly string LogConnectionSuccessPattern = string.Format(LogConnectionSuccessTemplate, "(?<Message>.+)");
    private static readonly string LogConnectionFailurePattern = string.Format(LogConnectionFailureTemplate, "(?<Message>.+)");

    // Static Events

    /// <summary>
    /// Raised when there is a file transfer progress notification for any downloader instance.
    /// </summary>
    public static event EventHandler<EventArgs<string, List<ProgressUpdate>>> ProgressUpdated;

    // Static Constructor
    static Downloader()
    {
        s_scheduleManager = new();
        s_scheduleManager.ScheduleDue += ScheduleManager_ScheduleDue;

        s_taskSchedules = new();
        s_queuedProgressUpdates = new();

        s_scheduleManager.Start();
    }

    private static void ScheduleManager_ScheduleDue(object sender, EventArgs<Schedule> e)
    {
        if (!s_taskSchedules.TryGetValue(e.Argument.Name, out Tuple<Downloader, ConnectionProfileTask> taskSchedule))
            return;

        Downloader downloader = taskSchedule.Item1;
        ConnectionProfileTask task = taskSchedule.Item2;

        // Queue specific task or group of tasks with device-level schedule for execution,
        // call is made via ServiceHost to handle pooled distribution:
        Program.Host.QueueTasks(downloader.Name, task?.Name ?? ScheduledTasksGroupID, QueuePriority.Normal);
    }

    // Static Methods
    private static void RegisterSchedule(Downloader downloader, ConnectionProfileTask task = null)
    {
        // Do not register any local task schedules if dependent upon remote scheduler
        if (Program.Host.Model.Global.UseRemoteScheduler)
            return;

        // Validate individual task fields
        if (task != null)
        {
            if (string.IsNullOrWhiteSpace(task.Name))
                return;

            if (task.Settings == null)
                return;

            if (string.IsNullOrWhiteSpace(task.Settings.Schedule))
                task.Settings.Schedule = downloader.Schedule;
        }

        string schedule = task == null ? downloader.Schedule : task.Settings.Schedule;

        // Check for a single dash which represents a disabled schedule
        if (schedule?.Trim().Equals("-") ?? true)
            return;

        string scheduleName = task == null ? downloader.Name : $"{downloader.Name}:{task.Name}";

        s_taskSchedules.TryAdd(scheduleName, new(downloader, task));
        s_scheduleManager.AddSchedule(scheduleName, schedule, $"Download schedule for \"{scheduleName}\"", true);
    }

    private static void UnregisterSchedule(Downloader downloader, ConnectionProfileTask task = null)
    {
        // Nothing to unregister when task schedules are dependent upon remote scheduler
        if (Program.Host.Model.Global.UseRemoteScheduler)
            return;

        string scheduleName = task == null ? downloader.Name : $"{downloader.Name}:{task.Name}";

        s_scheduleManager.RemoveSchedule(scheduleName);
        s_taskSchedules.TryRemove(scheduleName, out _);
    }

    private static void OnProgressUpdated(Downloader instance, ProgressUpdate update)
    {
        Action sendProgressUpdates = null;

        sendProgressUpdates = () =>
        {
            List<ProgressUpdateWrapper> queuedProgressUpdates;

            lock (s_queuedProgressUpdates)
            {
                queuedProgressUpdates = new(s_queuedProgressUpdates);
                s_queuedProgressUpdates.Clear();
            }

            foreach (IGrouping<Downloader, ProgressUpdateWrapper> grouping in queuedProgressUpdates.GroupBy(wrapper => wrapper.Instance))
            {
                lock (grouping.Key.m_trackedProgressUpdates)
                {
                    foreach (ProgressUpdateWrapper wrapper in grouping)
                    {
                        if (wrapper.Update.State == ProgressState.Queued)
                            wrapper.Instance.m_trackedProgressUpdates.Clear();

                        wrapper.Instance.m_trackedProgressUpdates.Add(wrapper.Update);
                    }

                    ProgressUpdated?.Invoke(grouping.Key, new(null, ProgressUpdate.Flatten(grouping.Select(wrapper => wrapper.Update).ToList())));
                }
            }

            lock (s_queuedProgressUpdates)
                s_progressUpdateCancellationToken = s_queuedProgressUpdates.Count > 0 ? sendProgressUpdates.DelayAndExecute(100) : null;
        };

        lock (s_queuedProgressUpdates)
        {
            s_queuedProgressUpdates.Add(new(instance, update));

            if (s_progressUpdateCancellationToken == null)
                s_progressUpdateCancellationToken = sendProgressUpdates.DelayAndExecute(100);
        }
    }

    private static void TerminateProcessTree(int ancestorID)
    {
        try
        {
            ManagementObjectSearcher searcher = new($"SELECT * FROM Win32_Process WHERE ParentProcessID = {ancestorID}");
            ManagementObjectCollection descendantIDs = searcher.Get();

            foreach (ManagementBaseObject managementObject in descendantIDs)
            {
                if (managementObject is ManagementObject descendantID)
                    TerminateProcessTree(Convert.ToInt32(descendantID["ProcessID"]));
            }

            try
            {
                using Process ancestor = Process.GetProcessById(ancestorID);
                ancestor.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited
            }
        }
        catch (Exception ex)
        {
            Program.Host.LogException(new InvalidOperationException($"Failed while attempting to terminate process tree with ancestor ID {ancestorID}: {ex.Message}", ex));
        }
    }

    private static string FTPPathCombine(params string[] args)
    {
        const string Separator = "/";
        string path = "";

        foreach (string arg in args)
        {
            if (arg.StartsWith(Separator))
                path = arg;
            else if (path.EndsWith(Separator))
                path += arg;
            else
                path += Separator + arg;
        }

        return path;
    }

    #region [ Statistic Methods ]

#pragma warning disable IDE0051 // These methods are accessed via reflection

    private static double GetDownloaderStatistic_Enabled(object source, string _)
    {
        double statistic = 0.0D;

        if (source is Downloader downloader)
            statistic = downloader.IsConnected ? 1.0D : 0.0D;

        return statistic;
    }

    private static double GetDownloaderStatistic_AttemptedConnections(object source, string _)
    {
        double statistic = 0.0D;

        if (source is Downloader downloader)
            statistic = downloader.AttemptedConnections;

        return statistic;
    }

    private static double GetDownloaderStatistic_SuccessfulConnections(object source, string _)
    {
        double statistic = 0.0D;

        if (source is Downloader downloader)
            statistic = downloader.SuccessfulConnections;

        return statistic;
    }

    private static double GetDownloaderStatistic_FailedConnections(object source, string _)
    {
        double statistic = 0.0D;

        if (source is Downloader downloader)
            statistic = downloader.FailedConnections;

        return statistic;
    }

    private static double GetDownloaderStatistic_AttemptedDialUps(object source, string _)
    {
        double statistic = 0.0D;

        if (source is Downloader downloader)
            statistic = downloader.AttemptedDialUps;

        return statistic;
    }

    private static double GetDownloaderStatistic_SuccessfulDialUps(object source, string _)
    {
        double statistic = 0.0D;

        if (source is Downloader downloader)
            statistic = downloader.SuccessfulDialUps;

        return statistic;
    }

    private static double GetDownloaderStatistic_FailedDialUps(object source, string _)
    {
        double statistic = 0.0D;

        if (source is Downloader downloader)
            statistic = downloader.FailedDialUps;

        return statistic;
    }

    private static double GetDownloaderStatistic_FilesDownloaded(object source, string _)
    {
        double statistic = 0.0D;

        if (source is Downloader downloader)
            statistic = downloader.FilesDownloaded;

        return statistic;
    }

    private static double GetDownloaderStatistic_MegaBytesDownloaded(object source, string _)
    {
        double statistic = 0.0D;

        if (source is Downloader downloader)
            statistic = downloader.BytesDownloaded / (double)SI2.Mega;

        return statistic;
    }

    private static double GetDownloaderStatistic_TotalConnectedTime(object source, string _)
    {
        double statistic = 0.0D;

        if (source is Downloader downloader)
            statistic = ((Ticks)downloader.TotalConnectedTime).ToSeconds();

        return statistic;
    }

    private static double GetDownloaderStatistic_TotalDialUpTime(object source, string _)
    {
        double statistic = 0.0D;

        if (source is Downloader downloader)
            statistic = ((Ticks)downloader.TotalDialUpTime).ToSeconds();

        return statistic;
    }

#pragma warning restore IDE0051

    #endregion

    #endregion
}