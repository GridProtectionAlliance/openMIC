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
//  12/08/2015 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using DotRas;
using GSF;
using GSF.Configuration;
using GSF.Data.Model;
using GSF.Net.Ftp;
using GSF.IO;
using GSF.Scheduling;
using GSF.Threading;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF.TimeSeries.Statistics;
using GSF.Units;
using GSF.Web.Model;
using openMIC.Model;
using System.Data;
using GSF.Data;
using GSF.Net.Smtp;
using GSF.Parsing;

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

        // Defines connection profile task settings
        private class ConnectionProfileTaskSettings
        {
            private string m_fileExtensions;
            private string[] m_fileSpecs;

            public ConnectionProfileTaskSettings(string name, int id)
            {
                Name = name;
                ID = id;
            }

            public string Name
            {
                get;
            }

            public int ID
            {
                get;
            }

            [ConnectionStringParameter,
            Description("Defines file names or patterns to download."),
            DefaultValue("*.*")]
            public string FileExtensions
            {
                get
                {
                    return m_fileExtensions;
                }
                set
                {
                    m_fileExtensions = value;
                    m_fileSpecs = null;
                }
            }

            public string[] FileSpecs
            {
                get
                {
                    return m_fileSpecs ?? (m_fileSpecs = (m_fileExtensions ?? "*.*").Split(',').Select(pattern => pattern.Trim()).ToArray());
                }
            }

            [ConnectionStringParameter,
            Description("Defines remote path to download files from ."),
            DefaultValue("/")]
            public string RemotePath
            {
                get;
                set;
            }

            [ConnectionStringParameter,
            Description("Defines local path to download files to."),
            DefaultValue("")]
            public string LocalPath
            {
                get;
                set;
            }

            [ConnectionStringParameter,
            Description("Determines if remote folders should scanned for matching downloads - file structure will be replicated locally."),
            DefaultValue(false)]
            public bool RecursiveDownload
            {
                get;
                set;
            }

            [ConnectionStringParameter,
            Description("Determines if remote files should be deleted after download."),
            DefaultValue(false)]
            public bool DeleteRemoteFilesAfterDownload
            {
                get;
                set;
            }

            [ConnectionStringParameter,
            Description("Determines if total remote files to download should be limited by age."),
            DefaultValue(false)]
            public bool LimitRemoteFileDownloadByAge
            {
                get;
                set;
            }

            [ConnectionStringParameter,
            Description("Determines if old local files should be deleted."),
            DefaultValue(false)]
            public bool DeleteOldLocalFiles
            {
                get;
                set;
            }

            [ConnectionStringParameter,
            Description("Determines if download should be skipped if local file already exists and matches remote."),
            DefaultValue(false)]
            public bool SkipDownloadIfUnchanged
            {
                get;
                set;
            }

            [ConnectionStringParameter,
            Description("Determines if existing local files should be overwritten."),
            DefaultValue(false)]
            public bool OverwriteExistingLocalFiles
            {
                get;
                set;
            }

            [ConnectionStringParameter,
            Description("Determines if existing local files should be archived before new ones are downloaded."),
            DefaultValue(false)]
            public bool ArchiveExistingFilesBeforeDownload
            {
                get;
                set;
            }

            [ConnectionStringParameter,
            Description("Determines if downloaded file timestamps should be synchronized to remote file timestamps."),
            DefaultValue(true)]
            public bool SynchronizeTimestamps
            {
                get;
                set;
            }

            [ConnectionStringParameter,
            Description("Defines external operation application."),
            DefaultValue("")]
            public string ExternalOperation
            {
                get;
                set;
            }

            [ConnectionStringParameter,
            Description("Defines maximum file size to download."),
            DefaultValue(1000)]
            public int MaximumFileSize
            {
                get;
                set;
            }

            [ConnectionStringParameter,
            Description("Defines maximum file count to download."),
            DefaultValue(-1)]
            public int MaximumFileCount
            {
                get;
                set;
            }

            [ConnectionStringParameter,
            Description("Defines directory naming expression."),
            DefaultValue("<YYYY><MM>\\<DeviceFolderName>")]
            public string DirectoryNamingExpression
            {
                get;
                set;
            }

            [ConnectionStringParameter,
            Description("Defines directory authentication user name."),
            DefaultValue("")]
            public string DirectoryAuthUserName
            {
                get;
                set;
            }

            [ConnectionStringParameter,
            Description("Defines directory authentication password."),
            DefaultValue("")]
            public string DirectoryAuthPassword
            {
                get;
                set;
            }

            [ConnectionStringParameter,
            Description("Determines if an e-mail should be sent if the downloaded files have been updated."),
            DefaultValue(false)]
            public bool EmailOnFileUpdate
            {
                get;
                set;
            }

            [ConnectionStringParameter,
            Description("Defines the recipient e-mail addresses to use when sending e-mails on file updates."),
            DefaultValue("")]
            public string EmailRecipients
            {
                get;
                set;
            }
        }

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
        }

        // Constants
        private const int NormalPriorty = 1;
        private const int HighPriority = 2;

        // Fields
        private readonly RasDialer m_rasDialer;
        private readonly DeviceProxy m_deviceProxy;
        private readonly object m_connectionProfileLock;
        private Device m_deviceRecord;
        private ConnectionProfile m_connectionProfile;
        private ConnectionProfileTaskSettings[] m_connectionProfileTaskSettings;
        private LogicalThreadOperation m_dialUpOperation;
        private LongSynchronizedOperation m_executeTasks;
        private int m_overallTasksCompleted;
        private int m_overallTasksCount;
        private long m_startDialUpTime;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        public Downloader()
        {
            m_rasDialer = new RasDialer();
            m_rasDialer.Error += m_rasDialer_Error;
            m_deviceProxy = new DeviceProxy(this);
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
        /// Gets or sets total number of files downloaded.
        /// </summary>
        public long FilesDownloaded
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

                // ReloadConfig was requested, take this opportunity to reload connection profile tasks...
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        LoadTasks();
                    }
                    catch (Exception ex)
                    {
                        OnProcessException(new InvalidOperationException($"Failed to reload connection profile tasks: {ex.Message}", ex));
                    }
                });
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

                status.AppendFormat(" Connection profiles tasks: {0}", m_connectionProfileTaskSettings.Length);
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
            ConnectionProfileTaskSettings[] taskSettings;

            lock (m_connectionProfileLock)
                taskSettings = m_connectionProfileTaskSettings;

            foreach (ConnectionProfileTaskSettings settings in taskSettings)
            {
                string localPath = settings.LocalPath.ToNonNullString().Trim();

                if (localPath.StartsWith(@"\\") && !string.IsNullOrWhiteSpace(settings.DirectoryAuthUserName) && !string.IsNullOrWhiteSpace(settings.DirectoryAuthPassword))
                {
                    string[] userParts = settings.DirectoryAuthUserName.Split('\\');
                    string[] pathParts = localPath.Substring(2).Split('\\');

                    try
                    {
                        if (userParts.Length == 2 && pathParts.Length > 1)
                            FilePath.ConnectToNetworkShare($"\\\\{pathParts[0].Trim()}\\{pathParts[1].Trim()}\\", userParts[1].Trim(), settings.DirectoryAuthPassword.Trim(), userParts[0].Trim());
                        else
                            throw new InvalidOperationException($"UNC based local path \"{settings.LocalPath}\" or authentication user name \"{settings.DirectoryAuthUserName}\" is not in the correct format.");
                    }
                    catch (Exception ex)
                    {
                        OnProcessException(new InvalidOperationException($"Exception while authenticating UNC path \"{settings.LocalPath}\": {ex.Message}", ex));
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
            ThreadPool.QueueUserWorkItem(state =>
            {
                AttemptedConnections++;

                if (UseDialUp)
                {
                    m_dialUpOperation.Priority = HighPriority;
                    m_dialUpOperation.RunOnce();
                    m_dialUpOperation.Priority = NormalPriorty;
                }
                else
                {
                    m_executeTasks.RunOnce();
                }
            });
        }

        private void LoadTasks()
        {
            ConnectionStringParser<ConnectionStringParameterAttribute> parser = new ConnectionStringParser<ConnectionStringParameterAttribute>();

            using (DataContext context = new DataContext())
            {
                lock (m_connectionProfileLock)
                {
                    m_deviceRecord = context.Table<Device>().QueryRecords(restriction: new RecordRestriction("Acronym = {0}", Name)).FirstOrDefault();
                    m_connectionProfile = context.Table<ConnectionProfile>().LoadRecord(ConnectionProfileID);
                    IEnumerable<ConnectionProfileTask> tasks = context.Table<ConnectionProfileTask>().QueryRecords(null, new RecordRestriction("ConnectionProfileID={0}", ConnectionProfileID));
                    List<ConnectionProfileTaskSettings> connectionProfileTaskSettings = new List<ConnectionProfileTaskSettings>();

                    foreach (ConnectionProfileTask task in tasks)
                    {
                        ConnectionProfileTaskSettings settings = new ConnectionProfileTaskSettings(task.Name, task.ID);
                        parser.ParseConnectionString(task.Settings, settings);
                        connectionProfileTaskSettings.Add(settings);
                    }

                    m_connectionProfileTaskSettings = connectionProfileTaskSettings.ToArray();
                }
            }
        }

        private void ExecuteTasks()
        {
            using (FtpClient client = new FtpClient())
            {
                client.CommandSent += FtpClient_CommandSent;
                client.ResponseReceived += FtpClient_ResponseReceived;
                client.FileTransferProgress += FtpClient_FileTransferProgress;
                client.FileTransferNotification += FtpClient_FileTransferNotification;

                try
                {
                    if (string.IsNullOrWhiteSpace(ConnectionHostName))
                    {
                        OnStatusMessage("No connection host name provided, skipping connection to FTP server...");
                    }
                    else
                    {
                        OnStatusMessage("Attempting connection to FTP server \"{0}@{1}\"...", ConnectionUserName, ConnectionHostName);

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

                        client.Connect(ConnectionUserName, ConnectionPassword);
                        OnStatusMessage("Connected to FTP server \"{0}@{1}\"", ConnectionUserName, ConnectionHostName);
                    }

                    Ticks connectionStartTime = DateTime.UtcNow.Ticks;
                    SuccessfulConnections++;

                    string connectionProfileName = m_connectionProfile?.Name ?? "Undefined";
                    ConnectionProfileTaskSettings[] taskSettings;

                    lock (m_connectionProfileLock)
                        taskSettings = m_connectionProfileTaskSettings;

                    if (taskSettings.Length > 0)
                    {
                        m_overallTasksCompleted = 0;
                        m_overallTasksCount = taskSettings.Length;
                        OnProgressUpdated(this, new ProgressUpdate(ProgressState.Processing, true, $"Starting \"{connectionProfileName}\" connection profile processing...", m_overallTasksCompleted, m_overallTasksCount));

                        foreach (ConnectionProfileTaskSettings settings in taskSettings)
                        {
                            OnStatusMessage("Starting \"{0}\" connection profile \"{1}\" task processing:", connectionProfileName, settings.Name);

                            if (string.IsNullOrWhiteSpace(settings.ExternalOperation))
                                ProcessFTPTask(settings, client);
                            else
                                ProcessExternalOperationTask(settings);

                            // Handle local file age limit processing, if enabled
                            if (settings.DeleteOldLocalFiles)
                                HandleLocalFileAgeLimitProcessing(settings);

                            OnProgressUpdated(this, new ProgressUpdate(ProgressState.Processing, true, null, ++m_overallTasksCompleted, m_overallTasksCount));
                        }

                        OnProgressUpdated(this, new ProgressUpdate(ProgressState.Succeeded, true, $"Completed \"{connectionProfileName}\" connection profile processing.", m_overallTasksCount, m_overallTasksCount));
                    }
                    else
                    {
                        OnProgressUpdated(this, new ProgressUpdate(ProgressState.Skipped, true, $"Skipped \"{connectionProfileName}\" connection profile processing: No tasks defined.", 0, 1));
                    }

                    Ticks connectedTime = DateTime.UtcNow.Ticks - connectionStartTime;
                    OnStatusMessage("FTP session connected for {0}", connectedTime.ToElapsedTimeString(2));
                    TotalConnectedTime += connectedTime;
                }
                catch (Exception ex)
                {
                    FailedConnections++;
                    OnProcessException(new InvalidOperationException($"Failed to connect to FTP server \"{ConnectionUserName}@{ConnectionHostName}\": {ex.Message}", ex));
                    OnProgressUpdated(this, new ProgressUpdate(ProgressState.Failed, true, $"Failed to connect to FTP server \"{ConnectionUserName}@{ConnectionHostName}\": {ex.Message}", 0, 1));
                }

                client.CommandSent -= FtpClient_CommandSent;
                client.ResponseReceived -= FtpClient_ResponseReceived;
                client.FileTransferProgress -= FtpClient_FileTransferProgress;
                client.FileTransferNotification -= FtpClient_FileTransferNotification;
            }
        }

        private void ProcessFTPTask(ConnectionProfileTaskSettings settings, FtpClient client)
        {
            string remotePath = settings.RemotePath;
            string localSubPath = Path.DirectorySeparatorChar.ToString();

            ProcessFTPTask(settings, client, remotePath, localSubPath);

            if (settings.RecursiveDownload)
                ProcessFTPSubDirectories(settings, client, remotePath, localSubPath);
        }

        private void ProcessFTPSubDirectories(ConnectionProfileTaskSettings settings, FtpClient client, string rootRemotePath, string rootLocalSubPath)
        {        
            client.SetCurrentDirectory(rootRemotePath);

            FtpDirectory[] directories = client.CurrentDirectory.SubDirectories.ToArray();

            foreach (FtpDirectory directory in directories)
            {
                string directoryName = directory.Name;

                if (directoryName.StartsWith(".", StringComparison.Ordinal))
                    continue;

                string remotePath = $"{rootRemotePath}/{directoryName}";
                string localSubPath = Path.Combine(rootLocalSubPath, directoryName);

                ProcessFTPTask(settings, client, remotePath, localSubPath);
                ProcessFTPSubDirectories(settings, client, remotePath, localSubPath);
            }
        }

        private void ProcessFTPTask(ConnectionProfileTaskSettings settings, FtpClient client, string remotePath, string localSubPath)
        {
            if (string.IsNullOrWhiteSpace(remotePath))
            {
                OnProcessException(new InvalidOperationException($"Cannot process connection profile task \"{settings.Name}\", remote FTP directory path is undefined."));
                return;
            }

            OnStatusMessage("Attempting to set remote FTP directory path \"{0}\"...", remotePath);

            try
            {
                client.SetCurrentDirectory(remotePath);

                OnStatusMessage("Enumerating remote files in \"{0}\"...", remotePath);

                try
                {
                    FtpFile[] files = client.CurrentDirectory.Files.ToArray();

                    if (files.Length == 0)
                    {
                        OnStatusMessage("No remote files found, remote file processing terminated.");
                    }
                    else if (files.Length > settings.MaximumFileCount && settings.MaximumFileCount > -1)
                    {
                        OnStatusMessage("WARNING: Skipping remote file processing, there are {0} remote files which exceeds the set {1} file limit.", files.Length, settings.MaximumFileCount);
                    }
                    else
                    {
                        OnStatusMessage("Found {0} remote file{1}, starting file processing...", files.Length, files.Length > 1 ? "s" : "");

                        m_overallTasksCount += files.Length;
                        OnProgressUpdated(this, new ProgressUpdate(ProgressState.Processing, true, null, m_overallTasksCompleted, m_overallTasksCount));

                        foreach (FtpFile file in files)
                        {
                            try
                            {
                                if (!FilePath.IsFilePatternMatch(settings.FileSpecs, file.Name, true))
                                    continue;

                                OnStatusMessage("Processing remote file \"{0}\"...", file.Name);
                                OnProgressUpdated(this, new ProgressUpdate(ProgressState.Processing, false, $"Starting \"{file.Name}\" download...", 0, file.Size));

                                try
                                {
                                    ProcessFile(settings, localSubPath, file);
                                    TotalProcessedFiles++;
                                }
                                catch (Exception ex)
                                {
                                    OnProcessException(new InvalidOperationException($"Failed to process remote file \"{file.Name ?? "undefined"}\" in \"{remotePath}\": {ex.Message}", ex));
                                }
                            }
                            finally
                            {
                                OnProgressUpdated(this, new ProgressUpdate(ProgressState.Processing, true, null, ++m_overallTasksCompleted, m_overallTasksCount));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnProcessException(new InvalidOperationException($"Failed to enumerate remote files in \"{remotePath}\": {ex.Message}", ex));
                }
            }
            catch (Exception ex)
            {
                OnProcessException(new InvalidOperationException($"Failed to set remote FTP directory path \"{remotePath}\": {ex.Message}", ex));
            }
        }

        private void ProcessFile(ConnectionProfileTaskSettings settings, string localSubPath, FtpFile file)
        {
            if (settings.LimitRemoteFileDownloadByAge && (DateTime.Now - file.Timestamp).Days > Program.Host.Model.Global.MaxRemoteFileAge)
            {
                OnStatusMessage("File \"{0}\" skipped, timestamp \"{1:yyyy-MM-dd HH:mm.ss.fff}\" is older than {2} days.", file.Name, file.Timestamp, Program.Host.Model.Global.MaxRemoteFileAge);
                OnProgressUpdated(this, new ProgressUpdate(ProgressState.Skipped, false, $"File \"{file.Name}\" skipped: File is too old.", 0, file.Size));
                return;
            }

            if (file.Size > settings.MaximumFileSize * SI2.Mega)
            {
                OnStatusMessage("File \"{0}\" skipped, size of {1:N3} MB is larger than {2:N3} MB configured limit.", file.Name, file.Size / SI2.Mega, settings.MaximumFileSize);
                OnProgressUpdated(this, new ProgressUpdate(ProgressState.Skipped, false, $"File \"{file.Name}\" skipped: File is too large ({file.Size / (double)SI2.Mega:N3} MB).", 0, file.Size));
                return;
            }

            if (DownloadFile(settings, localSubPath, file) && settings.DeleteRemoteFilesAfterDownload)
            {
                try
                {
                    file.Parent.RemoveFile(file.Name);
                }
                catch (Exception ex)
                {
                    OnProcessException(new InvalidOperationException($"Failed to remove \"{file.Name}\" after download: {ex.Message}", ex));
                }
            }
        }

        private bool DownloadFile(ConnectionProfileTaskSettings settings, string localSubPath, FtpFile file)
        {
            if (string.IsNullOrWhiteSpace(settings.LocalPath) || !Directory.Exists(settings.LocalPath))
            {
                OnProcessException(new InvalidOperationException($"Cannot download file \"{file.Name}\" for connection profile task \"{settings.Name}\": Local path \"{settings.LocalPath ?? ""}\" does not exist."));
                OnProgressUpdated(this, new ProgressUpdate(ProgressState.Failed, false, $"Cannot download file \"{file.Name}\": Local path does not exists", 0, file.Size));
                return false;
            }

            string localFileName = GetLocalFileName(settings, localSubPath, file.Name);
            bool fileChanged = false;

            if (File.Exists(localFileName) && settings.SkipDownloadIfUnchanged)
            {
                try
                {
                    FileInfo info = new FileInfo(localFileName);

                    // Compare file sizes
                    bool localEqualsRemote = info.Length == file.Size;

                    if (localEqualsRemote)
                    {
                        // Compare timestamps, if synchronized
                        if (settings.SynchronizeTimestamps)
                            localEqualsRemote = info.LastWriteTime == file.Timestamp;

                        if (localEqualsRemote)
                        {
                            OnStatusMessage("Skipping file \"{0}\" download for connection profile task \"{1}\": Local file already exists and matches remote file.", file.Name, settings.Name);
                            OnProgressUpdated(this, new ProgressUpdate(ProgressState.Skipped, false, $"File \"{file.Name}\" skipped: Local file already exists and matches remote file", 0, file.Size));
                            return false;
                        }
                    }

                    fileChanged = true;
                }
                catch (Exception ex)
                {
                    OnProcessException(new InvalidOperationException($"Failed to get info for local file \"{localFileName}\" for connection profile task \"{settings.Name}\": {ex.Message}", ex));
                }
            }

            if (File.Exists(localFileName) && settings.ArchiveExistingFilesBeforeDownload)
            {
                try
                {
                    string directoryName = Path.Combine(FilePath.GetDirectoryName(localFileName), "Archive\\");
                    string archiveFileName = Path.Combine(directoryName, FilePath.GetFileName(localFileName));

                    Directory.CreateDirectory(directoryName);

                    if (File.Exists(archiveFileName))
                        archiveFileName = FilePath.GetUniqueFilePathWithBinarySearch(archiveFileName);

                    OnStatusMessage("Archiving existing file \"{0}\" to \"{1}\"...", localFileName, archiveFileName);
                    File.Move(localFileName, archiveFileName);
                }
                catch (Exception ex)
                {
                    OnProcessException(new InvalidOperationException($"Failed to archive existing local file \"{localFileName}\" before download for connection profile task \"{settings.Name}\": {ex.Message}", ex));
                }
            }

            if (File.Exists(localFileName) && !settings.OverwriteExistingLocalFiles)
            {
                OnStatusMessage("Skipping file \"{0}\" download for connection profile task \"{1}\": Local file already exists and settings do not allow overwrite.", file.Name, settings.Name);
                OnProgressUpdated(this, new ProgressUpdate(ProgressState.Skipped, false, $"File \"{file.Name}\" skipped: Local file already exists", 0, file.Size));
                return false;
            }

            OnStatusMessage("Downloading \"{0}\" to \"{1}\"...", file.Name, localFileName);

            try
            {
                file.Parent.GetFile(localFileName, file.Name);
                FilesDownloaded++;
                BytesDownloaded += file.Size;
                OnProgressUpdated(this, new ProgressUpdate(ProgressState.Succeeded, false, $"Download complete for \"{file.Name}\".", file.Size, file.Size));

                // Send e-mail on file update, if requested
                if (fileChanged && settings.EmailOnFileUpdate)
                {
                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        try
                        {
                            GlobalSettings global = Program.Host.Model.Global;
                            string subject = $"File changed for \"{Name}: {settings.Name}\"";
                            string body = $"<b>File Name = {localFileName}</b></br>";

                            if (string.IsNullOrWhiteSpace(global.SmtpUserName))
                                Mail.Send(global.FromAddress, settings.EmailRecipients, subject, body, true, global.SmtpServer);
                            else
                                Mail.Send(global.FromAddress, settings.EmailRecipients, subject, body, true, global.SmtpServer, global.SmtpUserName, global.SmtpPassword);
                        }
                        catch (Exception ex)
                        {
                            OnProcessException(new InvalidOperationException($"Failed to send e-mail notification about updated file \"{localFileName}\": {ex.Message}"));
                        }
                    });
                }

                // Synchronize local timestamp to that of remote file if requested
                if (settings.SynchronizeTimestamps)
                {
                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        try
                        {
                            FileInfo info = new FileInfo(localFileName);
                            info.LastAccessTime = info.LastWriteTime = file.Timestamp;
                        }
                        catch (Exception ex)
                        {
                            OnProcessException(new InvalidOperationException($"Failed to update timestamp of downloaded file \"{localFileName}\": {ex.Message}"));
                        }
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                OnProcessException(new InvalidOperationException($"Failed to download file \"{file.Name}\" for connection profile task \"{settings.Name}\" to \"{localFileName}\": {ex.Message}", ex));
                OnProgressUpdated(this, new ProgressUpdate(ProgressState.Failed, false, $"Failed to download file \"{file.Name}\": {ex.Message}", 0, file.Size));
            }

            return false;
        }

        private string GetLocalFileName(ConnectionProfileTaskSettings settings, string localSubPath, string fileName)
        {
            TemplatedExpressionParser directoryNameExpressionParser = new TemplatedExpressionParser('<', '>', '[', ']');
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

            directoryNameExpressionParser.TemplatedExpression = settings.DirectoryNamingExpression.Replace("\\", "\\\\");

            //         Possible UNC Path                            Sub Directory - duplicate path slashes are removed
            fileName = FilePath.AddPathSuffix(settings.LocalPath) + $"{directoryNameExpressionParser.Execute(substitutions)}{Path.DirectorySeparatorChar}{localSubPath}{Path.DirectorySeparatorChar}{fileName}".RemoveDuplicates(Path.DirectorySeparatorChar.ToString());

            string directoryName = FilePath.GetDirectoryName(fileName);

            if (!Directory.Exists(directoryName))
            {
                try
                {
                    Directory.CreateDirectory(directoryName);
                }
                catch (Exception ex)
                {
                    OnProcessException(new InvalidOperationException($"Failed to create directory \"{directoryName}\": {ex.Message}", ex));
                }
            }

            return fileName;
        }

        private void ProcessExternalOperationTask(ConnectionProfileTaskSettings settings)
        {
            string externalOperationExecutableName = FilePath.GetAbsolutePath(settings.ExternalOperation);

            if (!File.Exists(externalOperationExecutableName))
            {
                OnProcessException(new InvalidOperationException($"Cannot execute external operation \"{settings.ExternalOperation}\" for connection profile task \"{settings.Name}\": Executable file not found."));
                return;
            }

            OnStatusMessage("Executing external operation \"{0}\"...", settings.ExternalOperation);
            OnProgressUpdated(this, new ProgressUpdate(ProgressState.Processing, false, "Starting external action...", 1, 0));

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(externalOperationExecutableName, $"{m_deviceRecord.ID}, {settings.ID}");
                Process externalOperation = Process.Start(startInfo);

                if ((object)externalOperation == null)
                {
                    OnProcessException(new InvalidOperationException($"Failed to start external operation \"{settings.ExternalOperation}\"."));
                    OnProgressUpdated(this, new ProgressUpdate(ProgressState.Failed, false, "Failed to start external action.", 1, 0));
                }
                else
                {
                    externalOperation.WaitForExit();
                    OnStatusMessage("External operation \"{0}\" completed with status code {1}.", settings.ExternalOperation, externalOperation.ExitCode);
                    OnProgressUpdated(this, new ProgressUpdate(ProgressState.Undefined, false, $"External action complete: exit code {externalOperation.ExitCode}.", 1, 1));
                    FilesDownloaded++;
                }
            }
            catch (Exception ex)
            {
                OnProcessException(new InvalidOperationException($"Failed to execute external operation \"{settings.ExternalOperation}\": {ex.Message}", ex));
                OnProgressUpdated(this, new ProgressUpdate(ProgressState.Failed, false, $"Failed to execute external action: {ex.Message}", 1, 0));
            }
        }

        private void HandleLocalFileAgeLimitProcessing(ConnectionProfileTaskSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.LocalPath) || !Directory.Exists(settings.LocalPath))
            {
                OnProcessException(new InvalidOperationException($"Cannot handle local file age limit processing for connection profile task \"{settings.Name}\": Local path \"{settings.LocalPath ?? ""}\" does not exist."));
                return;
            }

            OnStatusMessage("Enumerating local files in \"{0}\"...", settings.LocalPath);

            try
            {
                string[] files = FilePath.GetFileList(Path.Combine(FilePath.GetAbsolutePath(settings.LocalPath), "*\\*.*"));
                long deletedCount = 0;

                OnStatusMessage("Found {0} local files, starting age limit processing...", files.Length);

                foreach (string file in files)
                {
                    // Check file specification restriction
                    if (!FilePath.IsFilePatternMatch(settings.FileSpecs, file, true))
                        continue;

                    DateTime creationTime = File.GetCreationTime(file);

                    if ((DateTime.Now - creationTime).Days > Program.Host.Model.Global.MaxLocalFileAge)
                    {
                        OnStatusMessage("Attempting to delete file \"{0}\" created at \"{1:yyyy-MM-dd HH:mm.ss.fff}\"...", file, creationTime);

                        try
                        {
                            string rootPathName = FilePath.GetDirectoryName(settings.LocalPath);
                            string directoryName = FilePath.GetDirectoryName(file);

                            FilePath.WaitForWriteLock(file);
                            File.Delete(file);
                            deletedCount++;
                            OnStatusMessage("File \"{0}\" successfully deleted...", file);

                            if (!directoryName.Equals(rootPathName, StringComparison.OrdinalIgnoreCase))
                            {
                                // Try to remove sub-folder, this will only succeed if folder is empty...
                                try
                                {
                                    Directory.Delete(directoryName);
                                    OnStatusMessage("Removed empty folder \"{0}\"...", directoryName);
                                }
                                catch
                                {
                                    // Failure is common case, nothing to report
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            OnProcessException(new InvalidOperationException($"Failed to delete file \"{file}\": {ex.Message}", ex));
                        }
                    }
                }

                if (deletedCount > 0)
                    OnStatusMessage("Deleted {0} files during local file age limit processing.", deletedCount);
                else
                    OnStatusMessage("No files deleted during local file age limit processing.");
            }
            catch (Exception ex)
            {
                OnProcessException(new InvalidOperationException($"Failed to enumerate local files in \"{settings.LocalPath}\": {ex.Message}", ex));
            }
        }

        private void FtpClient_CommandSent(object sender, EventArgs<string> e)
        {
            if (LogConnectionMessages)
                OnStatusMessage("FTP Request: {0}", e.Argument);
        }

        private void FtpClient_ResponseReceived(object sender, EventArgs<string> e)
        {
            if (LogConnectionMessages)
                OnStatusMessage("FTP Response: {0}", e.Argument);
        }

        private void FtpClient_FileTransferProgress(object sender, EventArgs<ProcessProgress<long>, TransferDirection> e)
        {
            ProcessProgress<long> progress = e.Argument1;
            OnProgressUpdated(this, new ProgressUpdate(ProgressState.Processing, false, progress.ProgressMessage, progress.Complete, progress.Total));
        }

        private void FtpClient_FileTransferNotification(object sender, EventArgs<FtpAsyncResult> e)
        {
            OnStatusMessage("FTP File Transfer: {0}, response code = {1}", e.Argument.Message, e.Argument.ResponseCode);
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

                OnStatusMessage("Initiating dial-up for \"{0}\"...", DialUpEntryName);
                AttemptedDialUps++;

                m_rasDialer.EntryName = DialUpEntryName;
                m_rasDialer.PhoneNumber = DialUpPassword;
                m_rasDialer.Timeout = DialUpTimeout;
                m_rasDialer.Credentials = new NetworkCredential(DialUpUserName, DialUpPassword);
                m_rasDialer.Dial();

                m_startDialUpTime = DateTime.UtcNow.Ticks;
                SuccessfulDialUps++;
                OnStatusMessage("Dial-up connected on \"{0}\"", DialUpEntryName);
                return true;
            }
            catch (Exception ex)
            {
                FailedDialUps++;
                OnProcessException(new InvalidOperationException($"Exception while attempting to dial entry \"{DialUpEntryName}\": {ex.Message}", ex));
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
                OnStatusMessage("Initiating hang-up for \"{0}\"", DialUpEntryName);
                RasConnection.GetActiveConnections().FirstOrDefault(ras => ras.EntryName == DialUpEntryName)?.HangUp();
            }
            catch (Exception ex)
            {
                OnProcessException(new InvalidOperationException($"Exception while attempting to hang-up \"{DialUpEntryName}\": {ex.Message}", ex));
            }

            if (m_startDialUpTime > 0)
            {
                Ticks dialUpConnectedTime = DateTime.UtcNow.Ticks - m_startDialUpTime;
                OnStatusMessage("Dial-up connected for {0}", dialUpConnectedTime.ToElapsedTimeString(2));
                m_startDialUpTime = 0;
                TotalDialUpTime += dialUpConnectedTime;
            }
        }

        private void m_rasDialer_Error(object sender, ErrorEventArgs e)
        {
            OnProcessException(e.GetException());
        }

        #endregion

        #region [ Static ]

        // Static Fields
        private static readonly ScheduleManager s_scheduleManager;
        private static readonly ConcurrentDictionary<string, Downloader> s_instances;
        private static readonly ConcurrentDictionary<string, LogicalThread> s_dialupScheduler;

        // Static Events

        /// <summary>
        /// Raised when there is a file transfer progress notification for any downloader instance.
        /// </summary>
        public static event EventHandler<EventArgs<ProgressUpdate>> ProgressUpdated;

        // Static Constructor
        static Downloader()
        {
            s_instances = new ConcurrentDictionary<string, Downloader>();
            s_dialupScheduler = new ConcurrentDictionary<string, LogicalThread>();
            s_scheduleManager = new ScheduleManager();
            s_scheduleManager.ScheduleDue += s_scheduleManager_ScheduleDue;
            s_scheduleManager.Start();
        }

        private static void s_scheduleManager_ScheduleDue(object sender, EventArgs<Schedule> e)
        {
            Schedule schedule = e.Argument;
            Downloader instance;

            if (s_instances.TryGetValue(schedule.Name, out instance))
            {
                instance.AttemptedConnections++;

                if (instance.UseDialUp)
                    instance.m_dialUpOperation.RunOnceAsync();
                else
                    instance.m_executeTasks.RunOnceAsync();
            }
        }

        // Static Methods
        private static void RegisterSchedule(Downloader instance)
        {
            s_instances.TryAdd(instance.Name, instance);

            if (instance.UseDialUp)
            {
                // Make sure dial-up's using the same resource (i.e., modem) are executed synchronously
                LogicalThread thread = s_dialupScheduler.GetOrAdd(instance.DialUpEntryName, entryName => new LogicalThread(2));
                WeakReference<Downloader> reference = new WeakReference<Downloader>(instance);

                thread.UnhandledException += (sender, e) =>
                {
                    Downloader downloader;
                    if (reference.TryGetTarget(out downloader))
                        downloader.OnProcessException(e.Argument);
                };

                instance.m_dialUpOperation = new LogicalThreadOperation(thread, () =>
                {
                    if (instance.ConnectDialUp())
                    {
                        instance.ExecuteTasks();
                        instance.DisconnectDialUp();
                    }
                }, NormalPriorty);
            }
            else
            {
                instance.m_executeTasks = new LongSynchronizedOperation(instance.ExecuteTasks, instance.OnProcessException);
            }

            s_scheduleManager.AddSchedule(instance.Name, instance.Schedule, $"Download schedule for \"{instance.Name}\"", true);
        }

        private static void DeregisterSchedule(Downloader instance)
        {
            s_scheduleManager.RemoveSchedule(instance.Name);
            s_instances.TryRemove(instance.Name, out instance);
        }

        private static void OnProgressUpdated(Downloader instance, ProgressUpdate update)
        {
            ProgressUpdated?.Invoke(instance, new EventArgs<ProgressUpdate>(update));
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
