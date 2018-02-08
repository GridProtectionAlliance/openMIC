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
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using DotRas;
using GSF;
using GSF.Collections;
using GSF.Configuration;
using GSF.Console;
using GSF.Data;
using GSF.Data.Model;
using GSF.Diagnostics;
using GSF.IO;
using GSF.Net.Ftp;
using GSF.Net.Smtp;
using GSF.Scheduling;
using GSF.Threading;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF.TimeSeries.Statistics;
using GSF.Units;
using ModbusAdapters.Model;
using openMIC.Model;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable MemberCanBePrivate.Local
namespace openMIC
{
    /// <summary>
    /// Adapter that implements remote file download capabilities.
    /// </summary>
    [Description("Downloader: Implements remote file download capabilities")]
    [EditorBrowsable(EditorBrowsableState.Advanced)] // Normally defined as an input device protocol
    public class Downloader : InputAdapterBase
    {
        #region [ Members ]

        // Nested Types

        // Define a IDevice implementation for to provide daily reports
        private class DeviceProxy : IDevice
        {
            private readonly Downloader m_parent;

            public DeviceProxy(Downloader parent)
            {
                m_parent = parent;
            }

            // Gets or sets total data quality errors of this <see cref="IDevice"/>.
            public long DataQualityErrors
            {
                get;
                set;
            }

            // Gets or sets total time quality errors of this <see cref="IDevice"/>.
            public long TimeQualityErrors
            {
                get;
                set;
            }

            // Gets or sets total device errors of this <see cref="IDevice"/>.
            public long DeviceErrors
            {
                get;
                set;
            }

            // Gets or sets total measurements received for this <see cref="IDevice"/> - in local context "successful connections" per day.
            public long MeasurementsReceived
            {
                get
                {
                    return m_parent.SuccessfulConnections;
                }
                set
                {
                    // Ignoring updates
                }
            }

            // Gets or sets total measurements expected to have been received for this <see cref="IDevice"/> - in local context "attempted connections" per day.
            public long MeasurementsExpected
            {
                get
                {
                    return m_parent.AttemptedConnections;
                }
                set
                {
                    // Ignoring updates
                }
            }

            // Gets or sets the number of measurements received while this <see cref="IDevice"/> was reporting errors.
            public long MeasurementsWithError
            {
                get;
                set;
            }

            // Gets or sets the number of measurements (per frame) defined for this <see cref="IDevice"/>.
            public long MeasurementsDefined
            {
                get;
                set;
            }
        }

        // Define a wrapper to store information about a
        // remote file and the local path it's destined for
        private class FtpFileWrapper
        {
            public readonly string LocalPath;
            public readonly FtpFile RemoteFile;

            public FtpFileWrapper(string localPath, FtpFile remoteFile)
            {
                LocalPath = localPath;
                RemoteFile = remoteFile;
            }

            public void Get()
            {
                RemoteFile.Get(LocalPath);
            }
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
        private const int NormalPriorty = 1;
        private const int HighPriority = 2;

        // Fields
        private readonly RasDialer m_rasDialer;
        private readonly DeviceProxy m_deviceProxy;
        private readonly List<ProgressUpdate> m_trackedProgressUpdates;
        private readonly object m_connectionProfileLock;
        private readonly ICancellationToken m_cancellationToken;
        private Device m_deviceRecord;
        private ConnectionProfile m_connectionProfile;
        private ConnectionProfileTaskQueue m_connectionProfileTaskQueue;
        private ConnectionProfileTask[] m_connectionProfileTasks;
        private int m_overallTasksCompleted;
        private int m_overallTasksCount;
        private int m_lastDownloadedFileID;
        private long m_startDialUpTime;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        public Downloader()
        {
            m_rasDialer = new RasDialer();
            m_rasDialer.Error += m_rasDialer_Error;
            m_deviceProxy = new DeviceProxy(this);
            m_trackedProgressUpdates = new List<ProgressUpdate>();
            m_cancellationToken = new GSF.Threading.CancellationToken();
            m_connectionProfileLock = new object();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets connection host name or IP for transport.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines connection host name or IP for transport.")]
        public string ConnectionHostName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets connection host user name for transport.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines connection host user name for transport."),
        DefaultValue("anonymous")]
        public string ConnectionUserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets connection password for transport.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines connection password for transport."),
        DefaultValue("anonymous")]
        public string ConnectionPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets connection timeout for transport.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines connection timeout for transport."),
        DefaultValue(30000)]
        public int ConnectionTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets mode of FTP connection.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines mode of FTP connection."),
        DefaultValue(true)]
        public bool PassiveFtp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets mode of FTP connection.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines IP address to send in FTP PORT command."),
        DefaultValue("")]
        public string ActiveFtpAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets mode of FTP connection.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines minimum port in active FTP port range."),
        DefaultValue(0)]
        public int MinActiveFtpPort
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets mode of FTP connection.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines maximum port in active FTP port range."),
        DefaultValue(0)]
        public int MaxActiveFtpPort
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets flag that determines if connection messages should be logged.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines flag that determines if connection messages should be logged."),
        DefaultValue(false)]
        public bool LogConnectionMessages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets connection profile record ID.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines connection profile record ID."),
        DefaultValue(0)]
        public int ConnectionProfileID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets download schedule.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines download schedule."),
        DefaultValue("* * * * *")]
        public string Schedule
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets flag that determines if this connection will use dial-up.
        /// </summary>
        [ConnectionStringParameter,
        Description("Determines if this connection will use dial-up."),
        DefaultValue(false)]
        public bool UseDialUp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets dial-up entry name.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines dial-up entry name."),
        DefaultValue("")]
        public string DialUpEntryName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets dial-up phone number.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines dial-up phone number."),
        DefaultValue("")]
        public string DialUpNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets dial-up user name.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines dial-up user name."),
        DefaultValue("")]
        public string DialUpUserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets dial-up password.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines dial-up password."),
        DefaultValue("")]
        public string DialUpPassword
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets maximum retries for a dial-up connection.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines maximum retries for a dial-up connection."),
        DefaultValue(3)]
        public int DialUpRetries
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets timeout for a dial-up connection.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines timeout for a dial-up connection."),
        DefaultValue(90)]
        public int DialUpTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets total number of attempted connections.
        /// </summary>
        public long AttemptedConnections
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets total number of successful connections.
        /// </summary>
        public long SuccessfulConnections
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets total number of failed connections.
        /// </summary>
        public long FailedConnections
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets total number of processed files.
        /// </summary>
        public long TotalProcessedFiles
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets total number of attempted dial-ups.
        /// </summary>
        public long AttemptedDialUps
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets total number of successful dial-ups.
        /// </summary>
        public long SuccessfulDialUps
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets total number of failed dial-ups.
        /// </summary>
        public long FailedDialUps
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets number of files downloaded during last execution.
        /// </summary>
        public long FilesDownloaded
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets total number of files downloaded.
        /// </summary>
        public long TotalFilesDownloaded
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets total number of bytes downloaded.
        /// </summary>
        public long BytesDownloaded
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets total connected time, in ticks.
        /// </summary>
        public long TotalConnectedTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets total dial-up time, in ticks.
        /// </summary>
        public long TotalDialUpTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets <see cref="DataSet" /> based data source available to this <see cref="AdapterBase" />.
        /// </summary>
        public override DataSet DataSource
        {
            get
            {
                return base.DataSource;
            }
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
                StringBuilder status = new StringBuilder();

                status.Append(base.Status);
                status.AppendFormat("      Connection host name: {0}", ConnectionHostName.ToNonNullNorWhiteSpace("undefined"));
                status.AppendLine();
                status.AppendFormat("      Connection user name: {0} - with {1} password", ConnectionUserName.ToNonNullNorWhiteSpace("undefined"), string.IsNullOrWhiteSpace(ConnectionPassword) ? "no" : "a");
                status.AppendLine();
                status.AppendFormat("     Connection profile ID: {0} - {1}", ConnectionProfileID, m_connectionProfile?.Name ?? "undefined");
                status.AppendLine();
                status.AppendFormat("         Download schedule: {0}", Schedule);
                status.AppendLine();
                status.AppendFormat("   Log connection messages: {0}", LogConnectionMessages);
                status.AppendLine();
                status.AppendFormat("     Attempted connections: {0}", AttemptedConnections);
                status.AppendLine();
                status.AppendFormat("    Successful connections: {0}", SuccessfulConnections);
                status.AppendLine();
                status.AppendFormat("        Failed connections: {0}", FailedConnections);
                status.AppendLine();
                status.AppendFormat("     Total processed files: {0}", TotalProcessedFiles);
                status.AppendLine();
                status.AppendFormat("      Total connected time: {0}", new Ticks(TotalConnectedTime).ToElapsedTimeString(3));
                status.AppendLine();
                status.AppendFormat("               Use dial-up: {0}", UseDialUp);
                status.AppendLine();

                if (UseDialUp)
                {
                    status.AppendFormat("        Dial-up entry name: {0}", DialUpEntryName);
                    status.AppendLine();
                    status.AppendFormat("            Dial-up number: {0}", DialUpNumber);
                    status.AppendLine();
                    status.AppendFormat("         Dial-up user name: {0} - with {1} password", DialUpUserName.ToNonNullNorWhiteSpace("undefined"), string.IsNullOrWhiteSpace(DialUpPassword) ? "no" : "a");
                    status.AppendLine();
                    status.AppendFormat("           Dial-up retries: {0}", DialUpRetries);
                    status.AppendLine();
                    status.AppendFormat("          Dial-up time-out: {0}", DialUpTimeout);
                    status.AppendLine();
                    status.AppendFormat("        Attempted dial-ups: {0}", AttemptedDialUps);
                    status.AppendLine();
                    status.AppendFormat("       Successful dial-ups: {0}", SuccessfulDialUps);
                    status.AppendLine();
                    status.AppendFormat("           Failed dial-ups: {0}", FailedDialUps);
                    status.AppendLine();
                    status.AppendFormat("        Total dial-up time: {0}", new Ticks(TotalDialUpTime).ToElapsedTimeString(3));
                    status.AppendLine();
                }

                status.AppendFormat(" Connection profiles tasks: {0}", m_connectionProfileTasks.Length);
                status.AppendLine();
                status.AppendFormat("          Files downloaded: {0}", FilesDownloaded);
                status.AppendLine();
                status.AppendFormat("          Bytes downloaded: {0:N3} MB", BytesDownloaded / (double)SI2.Mega);
                status.AppendLine();

                return status.ToString();
            }
        }


        // Gets RAS connection state
        private RasConnectionState RasState => RasConnection.GetActiveConnections().FirstOrDefault(ras => ras.EntryName == DialUpEntryName)?.GetConnectionStatus()?.ConnectionState ?? RasConnectionState.Disconnected;

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="Downloader"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        m_cancellationToken.Cancel();
                        DeregisterSchedule(this);

                        if ((object)m_rasDialer != null)
                        {
                            m_rasDialer.Error -= m_rasDialer_Error;
                            m_rasDialer.Dispose();
                        }

                        StatisticsEngine.Unregister(m_deviceProxy);
                        StatisticsEngine.Unregister(this);
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        /// <summary>
        /// Initializes <see cref="Downloader"/>.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            ConnectionStringParser<ConnectionStringParameterAttribute> parser = new ConnectionStringParser<ConnectionStringParameterAttribute>();

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
            ConnectionProfileTask[] tasks;

            lock (m_connectionProfileLock)
                tasks = m_connectionProfileTasks;

            foreach (ConnectionProfileTask task in tasks)
            {
                ConnectionProfileTaskSettings settings = task.Settings;
                string localPath = settings.LocalPath.ToNonNullString().Trim();

                if (localPath.StartsWith(@"\\") && !string.IsNullOrWhiteSpace(settings.DirectoryAuthUserName) && !string.IsNullOrWhiteSpace(settings.DirectoryAuthPassword))
                {
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
        public override string GetShortStatus(int maxLength)
        {
            if (!Enabled)
                return "Downloading for is paused...".CenterText(maxLength);

            return $"Downloading enabled for schedule: {Schedule}".CenterText(maxLength);
        }

        /// <summary>
        /// Queues scheduled tasks for immediate execution.
        /// </summary>
        [AdapterCommand("Queues scheduled tasks for immediate execution.", "Administrator", "Editor")]
        public void QueueTasks()
        {
            if (m_connectionProfileTaskQueue.PrioritizeAction(ExecuteTasks))
            {
                OnProgressUpdated(this, new ProgressUpdate()
                {
                    State = ProgressState.Queued,
                    Message = "Connection profile tasks queued at high priority.",
                    Progress = 0,
                    ProgressTotal = 1,
                    OverallProgress = 0,
                    OverallProgressTotal = 1
                });
            }
        }

        private void LoadTasks()
        {
            ConnectionStringParser<ConnectionStringParameterAttribute> parser = new ConnectionStringParser<ConnectionStringParameterAttribute>();

            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                TableOperations<Device> deviceTable = new TableOperations<Device>(connection);
                TableOperations<ConnectionProfile> connectionProfileTable = new TableOperations<ConnectionProfile>(connection);
                TableOperations<ConnectionProfileTask> connectionProfileTaskTable = new TableOperations<ConnectionProfileTask>(connection);
                TableOperations<ConnectionProfileTaskQueue> connectionProfileTaskQueueTable = new TableOperations<ConnectionProfileTaskQueue>(connection);

                lock (m_connectionProfileLock)
                {
                    string connectionProfileTaskQueueName;
                    ConnectionProfileTaskQueue taskQueue = null;

                    if ((object)taskQueue == null && UseDialUp && !string.IsNullOrWhiteSpace(DialUpEntryName))
                    {
                        taskQueue = connectionProfileTaskQueueTable.QueryRecordWhere("Name = {0}", DialUpEntryName)
                            ?? new ConnectionProfileTaskQueue() { Name = DialUpEntryName };

                        taskQueue.MaxThreadCount = 1;
                    }

                    if (Settings.TryGetValue(nameof(connectionProfileTaskQueueName), out connectionProfileTaskQueueName) && !string.IsNullOrWhiteSpace(connectionProfileTaskQueueName))
                    {
                        taskQueue = connectionProfileTaskQueueTable.QueryRecordWhere("Name = {0}", connectionProfileTaskQueueName)
                            ?? new ConnectionProfileTaskQueue() { Name = connectionProfileTaskQueueName };
                    }

                    m_deviceRecord = deviceTable.QueryRecordWhere("Acronym = {0}", Name);
                    m_connectionProfile = connectionProfileTable.LoadRecord(ConnectionProfileID);

                    connectionProfileTaskTable.RootQueryRestriction[0] = ConnectionProfileID;
                    IEnumerable<ConnectionProfileTask> tasks = connectionProfileTaskTable.QueryRecords("LoadOrder");

                    if ((object)taskQueue == null && (object)m_connectionProfile.DefaultTaskQueueID != null)
                        taskQueue = connectionProfileTaskQueueTable.QueryRecordWhere("ID = {0}", m_connectionProfile.DefaultTaskQueueID.GetValueOrDefault());

                    if ((object)taskQueue == null)
                    {
                        taskQueue = connectionProfileTaskQueueTable.QueryRecordWhere("Name = {0}", m_connectionProfile.Name)
                            ?? new ConnectionProfileTaskQueue() { Name = m_connectionProfile.Name };
                    }

                    taskQueue.RegisterExceptionHandler(ex => OnProcessException(MessageLevel.Error, ex, "Task Execution"));
                    m_connectionProfileTaskQueue = taskQueue;
                    m_connectionProfileTasks = tasks.ToArray();
                }
            }
        }

        private void ExecuteTasks()
        {
            using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
            {
                bool enabled = connection.ExecuteScalar<bool>("SELECT Enabled FROM Device WHERE Acronym = {0}", Name);

                if (!enabled)
                    return;
            }

            if (m_cancellationToken.IsCancelled)
                return;

            FtpClient client = null;
            Ticks connectionStartTime = DateTime.UtcNow.Ticks;
            string connectionProfileName = m_connectionProfile?.Name ?? "Undefined";
            bool dialUpConnected = false;

            try
            {
                ConnectionProfileTask[] tasks;

                lock (m_connectionProfileLock)
                    tasks = m_connectionProfileTasks;

                if (tasks.Length == 0)
                {
                    OnProgressUpdated(this, new ProgressUpdate()
                    {
                        State = ProgressState.Fail,
                        ErrorMessage = $"No connection profile tasks defined for \"{connectionProfileName}\"."
                    });

                    return;
                }

                FilesDownloaded = 0;
                m_overallTasksCompleted = 0;
                m_overallTasksCount = tasks.Length;

                OnProgressUpdated(this, new ProgressUpdate()
                {
                    State = ProgressState.Processing,
                    Message = $"Beginning execution of {m_overallTasksCount} tasks for connection profile \"{connectionProfileName}\"...",
                    Summary = $"0 Files Downloaded ({TotalFilesDownloaded} Total)"
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

                            client = new FtpClient();
                            client.CommandSent += FtpClient_CommandSent;
                            client.ResponseReceived += FtpClient_ResponseReceived;
                            client.FileTransferProgress += FtpClient_FileTransferProgress;
                            client.FileTransferNotification += FtpClient_FileTransferNotification;

                            string[] parts = ConnectionHostName.Split(':');

                            if (parts.Length > 1)
                            {
                                client.Server = parts[0];
                                client.Port = int.Parse(parts[1]);
                            }
                            else
                            {
                                client.Server = ConnectionHostName;
                            }

                            client.Timeout = ConnectionTimeout;
                            client.Passive = PassiveFtp;
                            client.ActiveAddress = ActiveFtpAddress;
                            client.MinActivePort = MinActiveFtpPort;
                            client.MaxActivePort = MaxActiveFtpPort;
                            client.Connect(ConnectionUserName, ConnectionPassword);

                            SuccessfulConnections++;
                            OnStatusMessage(MessageLevel.Info, $"Connected to FTP server \"{ConnectionUserName}@{ConnectionHostName}\"");
                        }
                        catch (Exception ex)
                        {
                            FailedConnections++;
                            OnProcessException(MessageLevel.Warning, new InvalidOperationException($"Failed to connect to FTP server \"{ConnectionUserName}@{ConnectionHostName}\": {ex.Message}", ex));
                            OnProgressUpdated(this, new ProgressUpdate() { ErrorMessage = $"Failed to connect to FTP server \"{ConnectionUserName}@{ConnectionHostName}\": {ex.Message}" });
                        }
                    }
                }

                foreach (ConnectionProfileTask task in tasks)
                {
                    ConnectionProfileTaskSettings settings = task.Settings;

                    if (m_cancellationToken.IsCancelled)
                        return;

                    OnStatusMessage(MessageLevel.Info, $"Starting \"{connectionProfileName}\" connection profile \"{task.Name}\" task processing:");
                    OnProgressUpdated(this, new ProgressUpdate() { Message = $"Executing task \"{task.Name}\"..." });

                    task.Reset();

                    if (string.IsNullOrWhiteSpace(task.Settings.ExternalOperation))
                        ProcessFTPTask(task, client);
                    else
                        ProcessExternalOperationTask(task);

                    // Handle local file age limit processing, if enabled
                    if (settings.DeleteOldLocalFiles)
                        HandleLocalFileAgeLimitProcessing(task);

                    OnProgressUpdated(this, new ProgressUpdate()
                    {
                        OverallProgress = ++m_overallTasksCompleted,
                        OverallProgressTotal = m_overallTasksCount
                    });

                    if (!task.Success)
                        LogFailure(task.FailMessage);
                }

                ProgressUpdate finalUpdate = new ProgressUpdate()
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
                OnProgressUpdated(this, new ProgressUpdate()
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
                if ((object)client != null)
                {
                    client.CommandSent -= FtpClient_CommandSent;
                    client.ResponseReceived -= FtpClient_ResponseReceived;
                    client.FileTransferProgress -= FtpClient_FileTransferProgress;
                    client.FileTransferNotification -= FtpClient_FileTransferNotification;
                    client.Dispose();

                    if (dialUpConnected)
                        DisconnectDialUp();

                    Ticks connectedTime = DateTime.UtcNow.Ticks - connectionStartTime;
                    OnStatusMessage(MessageLevel.Info, $"FTP session connected for {connectedTime.ToElapsedTimeString(2)}");
                    TotalConnectedTime += connectedTime;
                }
            }
        }

        private void ProcessFTPTask(ConnectionProfileTask task, FtpClient client)
        {
            if ((object)client == null)
            {
                task.Fail();
                return;
            }

            try
            {
                string remotePath = GetRemotePathDirectory(task.Settings);
                string localDirectoryPath = GetLocalPathDirectory(task.Settings);
                List<FtpFileWrapper> files = new List<FtpFileWrapper>();

                OnStatusMessage(MessageLevel.Info, $"Ensuring local path \"{localDirectoryPath}\" exists.");
                Directory.CreateDirectory(localDirectoryPath);

                OnStatusMessage(MessageLevel.Info, $"Building list of files to be downloaded from \"{remotePath}\".");
                BuildFileList(files, task, client, remotePath, localDirectoryPath);
                DownloadAllFiles(files, client, task);
            }
            catch (Exception ex)
            {
                task.Fail(ex.Message);
                OnProcessException(MessageLevel.Error, ex, "ProcessFTPTask");
            }
        }

        private void BuildFileList(List<FtpFileWrapper> fileList, ConnectionProfileTask task, FtpClient client, string remotePath, string localDirectoryPath)
        {
            ConnectionProfileTaskSettings settings = task.Settings;

            if (m_cancellationToken.IsCancelled)
                return;

            OnStatusMessage(MessageLevel.Info, $"Attempting to set remote FTP directory path \"{remotePath}\"...");
            client.SetCurrentDirectory(remotePath);

            OnStatusMessage(MessageLevel.Info, $"Enumerating remote files in \"{remotePath}\"...");

            foreach (FtpFile file in client.CurrentDirectory.Files)
            {
                if (m_cancellationToken.IsCancelled)
                    return;

                if (!FilePath.IsFilePatternMatch(settings.FileSpecs, file.Name, true))
                    continue;

                if (settings.LimitRemoteFileDownloadByAge && (DateTime.Now - file.Timestamp).Days > Program.Host.Model.Global.MaxRemoteFileAge)
                {
                    OnStatusMessage(MessageLevel.Info, $"File \"{file.Name}\" skipped, timestamp \"{file.Timestamp:yyyy-MM-dd HH:mm.ss.fff}\" is older than {Program.Host.Model.Global.MaxRemoteFileAge} days.");
                    OnProgressUpdated(this, new ProgressUpdate() { Message = $"File \"{file.Name}\" skipped: File is too old." });
                    continue;
                }

                if (file.Size > settings.MaximumFileSize * SI2.Mega)
                {
                    OnStatusMessage(MessageLevel.Info, $"File \"{file.Name}\" skipped, size of {file.Size / SI2.Mega:N3} MB is larger than {settings.MaximumFileSize:N3} MB configured limit.");
                    OnProgressUpdated(this, new ProgressUpdate() { Message = $"File \"{file.Name}\" skipped: File is too large ({file.Size / (double)SI2.Mega:N3} MB)." });
                    continue;
                }

                string localPath = Path.Combine(localDirectoryPath, file.Name);

                if (File.Exists(localPath) && settings.SkipDownloadIfUnchanged)
                {
                    try
                    {
                        FileInfo info = new FileInfo(localPath);

                        // Compare file sizes and timestamps
                        bool localEqualsRemote =
                            info.Length == file.Size &&
                            (!settings.SynchronizeTimestamps || info.LastWriteTime == file.Timestamp);

                        if (localEqualsRemote)
                        {
                            OnStatusMessage(MessageLevel.Info, $"Skipping file download for remote file \"{file.Name}\": Local file already exists and matches remote file.");
                            OnProgressUpdated(this, new ProgressUpdate() { Message = $"File \"{file.Name}\" skipped: Local file already exists and matches remote file" });
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        OnProcessException(MessageLevel.Warning, new InvalidOperationException($"Unable to determine whether local file size and time matches remote file size and time due to exception: {ex.Message}", ex));
                    }
                }

                fileList.Add(new FtpFileWrapper(localPath, file));
            }

            if (settings.RecursiveDownload)
            {
                FtpDirectory[] directories = new FtpDirectory[0];

                try
                {
                    OnStatusMessage(MessageLevel.Info, $"Enumerating remote directories in \"{remotePath}\"...");
                    directories = client.CurrentDirectory.SubDirectories.ToArray();
                }
                catch (Exception ex)
                {
                    task.Fail(ex.Message);
                    OnProcessException(MessageLevel.Error, new Exception($"Failed to enumerate remote directories in \"{remotePath}\" due to exception: {ex.Message}", ex));
                    OnProgressUpdated(this, new ProgressUpdate() { ErrorMessage = $"Failed to enumerate remote directories in \"{remotePath}\": {ex.Message}" });
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

                        string remoteSubPath = $"{remotePath}/{directoryName}";
                        string localSubPath = Path.Combine(localDirectoryPath, directoryName);

                        OnStatusMessage(MessageLevel.Info, $"Recursively adding files in \"{remotePath}\" to file list...");
                        BuildFileList(fileList, task, client, remoteSubPath, localSubPath);
                    }
                    catch (Exception ex)
                    {
                        task.Fail(ex.Message);
                        OnProcessException(MessageLevel.Error, new Exception($"Failed to add remote files from remote directory \"{directory.Name}\" to file list due to exception: {ex.Message}", ex));
                        OnProgressUpdated(this, new ProgressUpdate() { ErrorMessage = $"Failed to build file list for remote directory \"{directory.Name}\": {ex.Message}" });
                    }
                }
            }
        }

        private void DownloadAllFiles(List<FtpFileWrapper> files, FtpClient client, ConnectionProfileTask task)
        {
            ConnectionProfileTaskSettings settings = task.Settings;

            long progress = 0L;
            long totalBytes = files.Sum(wrapper => wrapper.RemoteFile.Size);

            if (m_cancellationToken.IsCancelled)
                return;

            OnProgressUpdated(this, new ProgressUpdate() { OverallProgress = m_overallTasksCompleted * totalBytes, OverallProgressTotal = totalBytes * m_overallTasksCount });

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

                    string message = $"Failed to create local directory for {grouping.Count()} remote files due to exception: {ex.Message}";
                    OnProcessException(MessageLevel.Error, new Exception(message, ex));
                    progress += grouping.Sum(wrapper => wrapper.RemoteFile.Size);
                    OnProgressUpdated(this, new ProgressUpdate() { ErrorMessage = message, OverallProgress = m_overallTasksCompleted * totalBytes + progress });
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
                        OnProgressUpdated(this, new ProgressUpdate() { ErrorMessage = $"File \"{wrapper.RemoteFile.Name}\" skipped: Local file already exists", OverallProgress = m_overallTasksCompleted * totalBytes + progress });
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
                                OnProgressUpdated(this, new ProgressUpdate() { ErrorMessage = message });
                            }
                        }

                        // Update these statistics only if
                        // the file download was successful
                        FilesDownloaded++;
                        TotalFilesDownloaded++;
                        BytesDownloaded += wrapper.RemoteFile.Size;

                        OnProgressUpdated(this, new ProgressUpdate()
                        {
                            Message = $"Successfully downloaded remote file \"{wrapper.RemoteFile.FullPath}\".",
                            Summary = $"{FilesDownloaded} Files Downloaded ({TotalFilesDownloaded} Total)",
                            OverallProgress = m_overallTasksCompleted * totalBytes + progress
                        });

                        // Synchronize local timestamp to that of remote file if requested
                        if (settings.SynchronizeTimestamps)
                        {
                            FileInfo info = new FileInfo(wrapper.LocalPath);
                            
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
                        OnProgressUpdated(this, new ProgressUpdate() { ErrorMessage = message, OverallProgress = m_overallTasksCompleted * totalBytes + progress });
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

        private string GetLocalPathDirectory(ConnectionProfileTaskSettings settings)
        {
            Dictionary<string, string> substitutions = new Dictionary<string, string>
            {
                { "<YYYY>", $"{DateTime.Now.Year}" },
                { "<YY>", $"{DateTime.Now.Year.ToString().Substring(2)}" },
                { "<MM>", $"{DateTime.Now.Month.ToString().PadLeft(2, '0')}" },
                { "<DD>", $"{DateTime.Now.Day.ToString().PadLeft(2, '0')}" },
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

        private string GetRemotePathDirectory(ConnectionProfileTaskSettings settings)
        {
            Dictionary<string, string> substitutions = new Dictionary<string, string>
            {
                { "<YYYY>", $"{DateTime.Now.Year}" },
                { "<YY>", $"{DateTime.Now.Year.ToString().Substring(2)}" },
                { "<MM>", $"{DateTime.Now.Month.ToString().PadLeft(2, '0')}" },
                { "<DD>", $"{DateTime.Now.Day.ToString().PadLeft(2, '0')}" },
                { "<Month MM>", $"Month {DateTime.Now.Month.ToString().PadLeft(2, '0')}" },
                { "<Day DD>", $"Day {DateTime.Now.Day.ToString().PadLeft(2, '0')}" },
                { "<Day DD-1>", $"Day {DateTime.Now.AddDays(-1).Day.ToString().PadLeft(2, '0')}" },
                { "<DeviceName>", m_deviceRecord.Name ?? "undefined" },
                { "<DeviceAcronym>", m_deviceRecord.Acronym },
                { "<DeviceFolderName>", m_deviceRecord.OriginalSource ?? m_deviceRecord.Acronym },
                { "<ProfileName>", m_connectionProfile.Name ?? "undefined" }
            };

            if(settings.RemotePath.Contains("<Day DD-1>"))
            {
                substitutions["<YYYY>"] = $"{DateTime.Now.AddDays(-1).Year}";
                substitutions["<YY>"] = $"{DateTime.Now.AddDays(-1).Year.ToString().Substring(2)}";
                substitutions["<MM>"] = $"{DateTime.Now.AddDays(-1).Month.ToString().PadLeft(2, '0')}";
                substitutions["<Month MM>"] = $"Month {DateTime.Now.AddDays(-1).Month.ToString().PadLeft(2, '0')}";
            }

            return substitutions.Aggregate(settings.RemotePath, (path, sub) => path.Replace(sub.Key, sub.Value));
        }

        private void ProcessExternalOperationTask(ConnectionProfileTask task)
        {
            ConnectionProfileTaskSettings settings = task.Settings;
            string localPathDirectory = GetLocalPathDirectory(settings);

            Dictionary<string, string> substitutions = new Dictionary<string, string>
            {
                { "<DeviceID>", m_deviceRecord.ID.ToString() },
                { "<DeviceName>", m_deviceRecord.Name ?? "undefined" },
                { "<DeviceAcronym>", m_deviceRecord.Acronym },
                { "<DeviceFolderName>", m_deviceRecord.OriginalSource ?? m_deviceRecord.Acronym },
                { "<DeviceFolderPath>", GetLocalPathDirectory(settings) },
                { "<ConnectionHostName>", ConnectionHostName },
                { "<ConnectionUserName>", ConnectionUserName },
                { "<ConnectionPassword>", ConnectionPassword },
                { "<ConnectionTimeout>", ConnectionTimeout.ToString() },
                { "<ProfileName>", m_connectionProfile.Name ?? "undefined" },
                { "<TaskID>", task.ID.ToString() }
            };

            string command = substitutions.Aggregate(settings.ExternalOperation.Trim(), (str, kvp) => str.Replace(kvp.Key, kvp.Value));
            string executable = Arguments.ParseCommand(command)[0];
            string args = command.Substring(executable.Length).Trim();
            TimeSpan timeout = TimeSpan.FromSeconds(settings.ExternalOperationTimeout ?? ConnectionTimeout / 1000.0D);

            OnStatusMessage(MessageLevel.Info, $"Executing external operation \"{command}\"...");
            OnProgressUpdated(this, new ProgressUpdate() { Message = $"Executing external operation command \"{command}\"..." });

            try
            {
                using (SafeFileWatcher fileWatcher = new SafeFileWatcher(localPathDirectory))
                using (Process externalOperation = new Process())
                {
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

                        if (ProcessExternalOperationMessage(processArgs.Data))
                            return;

                        lastUpdate = DateTime.UtcNow;
                        OnStatusMessage(MessageLevel.Info, processArgs.Data);
                        OnProgressUpdated(this, new ProgressUpdate() { Message = processArgs.Data });
                    };

                    externalOperation.ErrorDataReceived += (sender, processArgs) =>
                    {
                        if (string.IsNullOrWhiteSpace(processArgs.Data))
                            return;

                        task.Fail(processArgs.Data);
                        lastUpdate = DateTime.UtcNow;
                        OnStatusMessage(MessageLevel.Error, processArgs.Data);
                        OnProgressUpdated(this, new ProgressUpdate() { ErrorMessage = processArgs.Data });
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
                            OnProgressUpdated(this, new ProgressUpdate() { ErrorMessage = "External operation forcefully terminated: downloader was disabled." });
                            return;
                        }

                        if (timeout > TimeSpan.Zero && DateTime.UtcNow - lastUpdate > timeout)
                        {
                            task.Fail();
                            TerminateProcessTree(externalOperation.Id);
                            OnProcessException(MessageLevel.Error, new InvalidOperationException($"External operation \"{command}\" forcefully terminated: exceeded timeout ({timeout.TotalSeconds:0.##} seconds)."));
                            OnProgressUpdated(this, new ProgressUpdate() { ErrorMessage = $"External operation forcefully terminated: exceeded timeout ({timeout.TotalSeconds:0.##} seconds)." });
                            return;
                        }
                    }

                    OnStatusMessage(MessageLevel.Info, $"External operation \"{command}\" completed with status code {externalOperation.ExitCode}.");
                    OnProgressUpdated(this, new ProgressUpdate() { Message = $"External action complete: exit code {externalOperation.ExitCode}." });
                }
            }
            catch (Exception ex)
            {
                task.Fail(ex.Message);
                OnProcessException(MessageLevel.Error, new InvalidOperationException($"Failed to execute external operation \"{command}\": {ex.Message}", ex));
                OnProgressUpdated(this, new ProgressUpdate() { ErrorMessage = $"Failed to execute external action: {ex.Message}" });
            }
        }

        private bool ProcessExternalOperationMessage(string message)
        {
            const string LogDownloadedFilePattern = "openMIC :: Log Downloaded File :: (?<FilePath>.+)";
            Match match = Regex.Match(message, LogDownloadedFilePattern);

            if (match.Success)
            {
                m_lastDownloadedFileID = LogDownloadedFile(match.Groups["FilePath"].Value);
                FilesDownloaded++;
                TotalFilesDownloaded++;
                OnProgressUpdated(this, new ProgressUpdate() { Summary = $"{FilesDownloaded} Files Downloaded ({TotalFilesDownloaded} Total)" });
            }

            return match.Success;
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

                if (deletedCount > 0)
                    OnStatusMessage(MessageLevel.Info, $"Deleted {deletedCount} files during local file age limit processing.");
                else
                    OnStatusMessage(MessageLevel.Info, "No files deleted during local file age limit processing.");
            }
            catch (Exception ex)
            {
                OnProcessException(MessageLevel.Warning, new InvalidOperationException($"Failed to enumerate local files in \"{settings.LocalPath}\": {ex.Message}", ex));
            }
        }

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

            OnProgressUpdated(this, new ProgressUpdate()
            {
                Message = progress.ProgressMessage,
                Progress = progress.Complete,
                ProgressTotal = progress.Total
            });
        }

        private void FtpClient_FileTransferNotification(object sender, EventArgs<FtpAsyncResult> e)
        {
            OnStatusMessage(MessageLevel.Info, $"FTP File Transfer: {e.Argument.Message}, response code = {e.Argument.ResponseCode}");
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
                m_rasDialer.Credentials = new NetworkCredential(DialUpUserName, DialUpPassword);
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

            if (m_startDialUpTime > 0)
            {
                Ticks dialUpConnectedTime = DateTime.UtcNow.Ticks - m_startDialUpTime;
                OnStatusMessage(MessageLevel.Info, $"Dial-up connected for {dialUpConnectedTime.ToElapsedTimeString(2)}");
                m_startDialUpTime = 0;
                TotalDialUpTime += dialUpConnectedTime;
            }
        }

        private void m_rasDialer_Error(object sender, ErrorEventArgs e)
        {
            OnProcessException(MessageLevel.Warning, e.GetException());
        }

        public void SendCurrentProgressState(string clientID)
        {
            lock (m_trackedProgressUpdates)
            {
                List<ProgressUpdate> updates = ProgressUpdate.Flatten(m_trackedProgressUpdates);
                ProgressUpdated?.Invoke(this, new EventArgs<string, List<ProgressUpdate>>(clientID, updates));
            }
        }

        private int LogDownloadedFile(string filePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);

                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    TableOperations<DownloadedFile> downloadedFileTable = new TableOperations<DownloadedFile>(connection);
                    DownloadedFile downloadedFile = new DownloadedFile();
                    downloadedFile.DeviceID = m_deviceRecord.ID;
                    downloadedFile.FilePath = filePath;
                    downloadedFile.Timestamp = DateTime.UtcNow;
                    downloadedFile.CreationTime = fileInfo.CreationTimeUtc;
                    downloadedFile.LastWriteTime = fileInfo.LastWriteTimeUtc;
                    downloadedFile.LastAccessTime = fileInfo.LastAccessTimeUtc;
                    downloadedFile.FileSize = (int)fileInfo.Length;
                    downloadedFileTable.AddNewRecord(downloadedFile);

                    return downloadedFileTable.QueryRecordWhere("FilePath = {0}", filePath).ID;
                }
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
                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    TableOperations<StatusLog> statusLogTable = new TableOperations<StatusLog>(connection);
                    StatusLog log = statusLogTable.QueryRecordWhere("DeviceID = {0}", m_deviceRecord.ID) ?? statusLogTable.NewRecord();
                    log.DeviceID = m_deviceRecord.ID;
                    log.LastOutcome = outcome.ToString();
                    log.LastRun = DateTime.UtcNow;
                    statusLogTable.AddNewOrUpdateRecord(log);
                }
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
                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    TableOperations<StatusLog> statusLogTable = new TableOperations<StatusLog>(connection);
                    StatusLog log = statusLogTable.QueryRecordWhere("DeviceID = {0}", m_deviceRecord.ID) ?? statusLogTable.NewRecord();
                    log.DeviceID = m_deviceRecord.ID;
                    log.LastFailure = DateTime.UtcNow;
                    log.LastErrorMessage = message;
                    statusLogTable.AddNewOrUpdateRecord(log);
                }
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
                using (AdoDataConnection connection = new AdoDataConnection("systemSettings"))
                {
                    TableOperations<StatusLog> statusLogTable = new TableOperations<StatusLog>(connection);
                    StatusLog log = statusLogTable.QueryRecordWhere("DeviceID = {0}", m_deviceRecord.ID) ?? statusLogTable.NewRecord();
                    log.DeviceID = m_deviceRecord.ID;
                    log.LastDownloadedFileID = lastDownloadedFileID;
                    log.LastDownloadStartTime = startTime;
                    log.LastDownloadEndTime = endTime;
                    log.LastDownloadFileCount = (int)FilesDownloaded;
                    statusLogTable.AddNewOrUpdateRecord(log);
                }
            }
            catch (Exception ex)
            {
                OnProcessException(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region [ Static ]

        // Static Fields
        private static readonly ScheduleManager s_scheduleManager;
        private static readonly ConcurrentDictionary<string, Downloader> s_instances;
        private static readonly List<ProgressUpdateWrapper> s_queuedProgressUpdates;
        private static readonly int s_maxDownloadThreshold;
        private static readonly int s_maxDownloadThresholdTimeWindow;
        private static readonly string[] s_statusLogInclusions;
        private static readonly string[] s_statusLogExclusions;
        private static ICancellationToken s_progressUpdateCancellationToken;

        // Static Events

        /// <summary>
        /// Raised when there is a file transfer progress notification for any downloader instance.
        /// </summary>
        public static event EventHandler<EventArgs<string, List<ProgressUpdate>>> ProgressUpdated;

        // Static Constructor
        static Downloader()
        {
            const int DefaultMaxDownloadThreshold = 0;
            const int DefaultMaxDownloadThresholdTimeWindow = 24;
            const string DefaultStatusLogInclusions = ".rcd,.d00,.dat,.ctl,.cfg,.pcd";
            const string DefaultStatusLogExclusions = "rms.,trend.";

            CategorizedSettingsElementCollection systemSettings = ConfigurationFile.Current.Settings["systemSettings"];
            systemSettings.Add("MaxDownloadThreshold", DefaultMaxDownloadThreshold, "Maximum downloads a meter can have in a specified time range before disabling the meter, subject to specified StatusLog inclusions and exclusions. Set to 0 to disable.");
            systemSettings.Add("MaxDownloadThresholdTimeWindow", DefaultMaxDownloadThresholdTimeWindow, "Time window for the MaxDownloadThreshold in hours.");
            systemSettings.Add("StatusLogInclusions", DefaultStatusLogInclusions, "Default inclusions to apply when writing updates to StatusLog table and checking MaxDownloadThreshold.");
            systemSettings.Add("StatusLogExclusions", DefaultStatusLogExclusions, "Default exclusions to apply when writing updates to StatusLog table and checking MaxDownloadThreshold.");

            s_instances = new ConcurrentDictionary<string, Downloader>();

            s_queuedProgressUpdates = new List<ProgressUpdateWrapper>();

            s_scheduleManager = new ScheduleManager();
            s_scheduleManager.ScheduleDue += ScheduleManager_ScheduleDue;
            s_scheduleManager.Start();

            s_maxDownloadThreshold = systemSettings["MaxDownloadThreshold"].ValueAsInt32(DefaultMaxDownloadThreshold);
            s_maxDownloadThresholdTimeWindow = systemSettings["MaxDownloadThresholdTimeWindow"].ValueAsInt32(DefaultMaxDownloadThresholdTimeWindow);
            s_statusLogInclusions = systemSettings["StatusLogInclusions"].ValueAs(DefaultStatusLogInclusions).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            s_statusLogExclusions = systemSettings["StatusLogExclusions"].ValueAs(DefaultStatusLogExclusions).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static void ScheduleManager_ScheduleDue(object sender, EventArgs<Schedule> e)
        {
            Schedule schedule = e.Argument;
            Downloader instance;

            if (s_instances.TryGetValue(schedule.Name, out instance))
            {
                if (instance.m_connectionProfileTaskQueue.QueueAction(instance.ExecuteTasks))
                {
                    OnProgressUpdated(instance, new ProgressUpdate()
                    {
                        State = ProgressState.Queued,
                        Message = "Queued tasks at normal priority.",
                        Progress = 0,
                        ProgressTotal = 1,
                        OverallProgress = 0,
                        OverallProgressTotal = 1
                    });
                }
            }
        }

        // Static Methods

        private static void RegisterSchedule(Downloader instance)
        {
            s_instances.TryAdd(instance.Name, instance);
            s_scheduleManager.AddSchedule(instance.Name, instance.Schedule, $"Download schedule for \"{instance.Name}\"", true);
        }

        private static void DeregisterSchedule(Downloader instance)
        {
            s_scheduleManager.RemoveSchedule(instance.Name);
            s_instances.TryRemove(instance.Name, out instance);
        }

        private static void OnProgressUpdated(Downloader instance, ProgressUpdate update)
        {
            Action sendProgressUpdates = null;

            sendProgressUpdates = new Action(() =>
            {
                List<ProgressUpdateWrapper> queuedProgressUpdates;

                lock (s_queuedProgressUpdates)
                {
                    queuedProgressUpdates = new List<ProgressUpdateWrapper>(s_queuedProgressUpdates);
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

                        ProgressUpdated?.Invoke(grouping.Key, new EventArgs<string, List<ProgressUpdate>>(null, ProgressUpdate.Flatten(grouping.Select(wrapper => wrapper.Update).ToList())));
                    }
                }

                lock (s_queuedProgressUpdates)
                {
                    if (s_queuedProgressUpdates.Count > 0)
                        s_progressUpdateCancellationToken = sendProgressUpdates.DelayAndExecute(100);
                    else
                        s_progressUpdateCancellationToken = null;
                }
            });

            lock (s_queuedProgressUpdates)
            {
                s_queuedProgressUpdates.Add(new ProgressUpdateWrapper(instance, update));

                if (s_progressUpdateCancellationToken == null)
                    s_progressUpdateCancellationToken = sendProgressUpdates.DelayAndExecute(100);
            }
        }

        private static void TerminateProcessTree(int ancestorID)
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_Process WHERE ParentProcessID = {ancestorID}");
                ManagementObjectCollection descendantIDs = searcher.Get();

                foreach (ManagementBaseObject managementObject in descendantIDs)
                {
                    ManagementObject descendantID = managementObject as ManagementObject;

                    if ((object)descendantID != null)
                        TerminateProcessTree(Convert.ToInt32(descendantID["ProcessID"]));
                }

                try
                {
                    using (Process ancestor = Process.GetProcessById(ancestorID))
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

        #region [ Statistic Functions ]

        private static double GetDownloaderStatistic_Enabled(object source, string arguments)
        {
            double statistic = 0.0D;
            Downloader downloader = source as Downloader;

            if ((object)downloader != null)
                statistic = downloader.IsConnected ? 1.0D : 0.0D;

            return statistic;
        }

        private static double GetDownloaderStatistic_AttemptedConnections(object source, string arguments)
        {
            double statistic = 0.0D;
            Downloader downloader = source as Downloader;

            if ((object)downloader != null)
                statistic = downloader.AttemptedConnections;

            return statistic;
        }

        private static double GetDownloaderStatistic_SuccessfulConnections(object source, string arguments)
        {
            double statistic = 0.0D;
            Downloader downloader = source as Downloader;

            if ((object)downloader != null)
                statistic = downloader.SuccessfulConnections;

            return statistic;
        }

        private static double GetDownloaderStatistic_FailedConnections(object source, string arguments)
        {
            double statistic = 0.0D;
            Downloader downloader = source as Downloader;

            if ((object)downloader != null)
                statistic = downloader.FailedConnections;

            return statistic;
        }

        private static double GetDownloaderStatistic_AttemptedDialUps(object source, string arguments)
        {
            double statistic = 0.0D;
            Downloader downloader = source as Downloader;

            if ((object)downloader != null)
                statistic = downloader.AttemptedDialUps;

            return statistic;
        }

        private static double GetDownloaderStatistic_SuccessfulDialUps(object source, string arguments)
        {
            double statistic = 0.0D;
            Downloader downloader = source as Downloader;

            if ((object)downloader != null)
                statistic = downloader.SuccessfulDialUps;

            return statistic;
        }

        private static double GetDownloaderStatistic_FailedDialUps(object source, string arguments)
        {
            double statistic = 0.0D;
            Downloader downloader = source as Downloader;

            if ((object)downloader != null)
                statistic = downloader.FailedDialUps;

            return statistic;
        }

        private static double GetDownloaderStatistic_FilesDownloaded(object source, string arguments)
        {
            double statistic = 0.0D;
            Downloader downloader = source as Downloader;

            if ((object)downloader != null)
                statistic = downloader.FilesDownloaded;

            return statistic;
        }

        private static double GetDownloaderStatistic_MegaBytesDownloaded(object source, string arguments)
        {
            double statistic = 0.0D;
            Downloader downloader = source as Downloader;

            if ((object)downloader != null)
                statistic = downloader.BytesDownloaded / (double)SI2.Mega;

            return statistic;
        }

        private static double GetDownloaderStatistic_TotalConnectedTime(object source, string arguments)
        {
            double statistic = 0.0D;
            Downloader downloader = source as Downloader;

            if ((object)downloader != null)
                statistic = ((Ticks)downloader.TotalConnectedTime).ToSeconds();

            return statistic;
        }

        private static double GetDownloaderStatistic_TotalDialUpTime(object source, string arguments)
        {
            double statistic = 0.0D;
            Downloader downloader = source as Downloader;

            if ((object)downloader != null)
                statistic = ((Ticks)downloader.TotalDialUpTime).ToSeconds();

            return statistic;
        }

        #endregion

        #endregion
    }
}
