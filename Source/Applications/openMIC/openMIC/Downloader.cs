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

namespace openMIC
{
    public enum DirectoryNamingConvention
    {
        [Description("Top Folder")]
        Root,
        [Description("Year Sub-folder")]
        Year,
        [Description("YYYYMM Sub-folder")]
        YearMonth,
        [Description("Year/Month Sub-folders")]
        YearThenMonth
    }

    [Description("Downloader: Implements remote file download capabilities")]
    [EditorBrowsable(EditorBrowsableState.Advanced)] // Normally defined as an input device protocol
    public class Downloader : InputAdapterBase, IDevice
    {
        #region [ Members ]

        // Nested Types

        // Defines connection profile task settings
        public class ConnectionProfileTaskSettings
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
            Description("Determines if existing local files should be overwritten."),
            DefaultValue(false)]
            public bool OverwriteExistingLocalFiles
            {
                get;
                set;
            }

            [ConnectionStringParameter,
            Description("Determines if existing local files should be archived before they are overwritten."),
            DefaultValue(false)]
            public bool ArchiveLocalFilesBeforeOverwrite
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
            Description("Defines directory naming convention."),
            DefaultValue("Root")]
            public string DirectoryNamingConvention
            {
                get;
                set;
            }

            public DirectoryNamingConvention GetDirectoryNamingConvention()
            {
                DirectoryNamingConvention convention;

                if (Enum.TryParse(DirectoryNamingConvention, out convention))
                    return convention;

                return openMIC.DirectoryNamingConvention.Root;
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
        }

        // Constants
        private const int NormalPriorty = 1;
        private const int HighPriority = 2;
        private const string DailyCounterResetSchedule = "Downloader!DailyCounterReset";

        // Fields
        private readonly RasDialer m_rasDialer;
        private object m_connectionProfileLock;
        private ConnectionProfile m_connectionProfile;
        private ConnectionProfileTaskSettings[] m_connectionProfileTaskSettings;
        private LogicalThreadOperation m_dialUpOperation;
        private LongSynchronizedOperation m_executeTasks;
        private long m_startDialUpTime;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        public Downloader()
        {
            m_rasDialer = new RasDialer();
            m_rasDialer.Error += m_rasDialer_Error;
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
        /// Gets or sets total data quality errors of this <see cref="IDevice"/>.
        /// </summary>
        public long DataQualityErrors
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets total time quality errors of this <see cref="IDevice"/>.
        /// </summary>
        public long TimeQualityErrors
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets total device errors of this <see cref="IDevice"/>.
        /// </summary>
        public long DeviceErrors
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets total measurements received for this <see cref="IDevice"/> - in local context "successful connections" per day.
        /// </summary>
        public long MeasurementsReceived
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets total measurements expected to have been received for this <see cref="IDevice"/> - in local context "expected connections" per day.
        /// </summary>
        public long MeasurementsExpected
        {
            get
            {
                Schedule schedule = new Schedule(Schedule);

                // Check for scheduled days of week
                if (schedule.DaysOfWeekPart.Values.Contains((int)DateTime.UtcNow.DayOfWeek))
                {
                    // Check for scheduled months
                    if (schedule.MonthPart.Values.Contains(DateTime.UtcNow.Month))
                    {
                        // Check for scheduled days of month
                        if (schedule.DayPart.Values.Contains(DateTime.UtcNow.Day))
                        {
                            // Return expected downloads per day
                            return schedule.HourPart.Values.Count * schedule.MinutePart.Values.Count;
                        }

                        // Not a matching day of month - no downloads expected for today
                        return 0;
                    }

                    // Not a matching month - no downloads expected for today
                    return 0;
                }

                // Not a matching day of week - no downloads expected for today
                return 0;
            }
            set
            {
                // Ignoring updates
            }
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
                status.AppendFormat("     Completed connections: {0} - for today only", MeasurementsReceived);
                status.AppendLine();
                status.AppendFormat("      Expected connections: {0} - for today only", MeasurementsExpected);
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
            AttemptedConnections++;

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
                        client.Server = ConnectionHostName;
                        client.Connect(ConnectionUserName, ConnectionPassword);
                        OnStatusMessage("Connected to FTP server \"{0}@{1}\"", ConnectionUserName, ConnectionHostName);
                    }

                    Ticks connectionStartTime = DateTime.UtcNow.Ticks;
                    SuccessfulConnections++;
                    MeasurementsReceived++;

                    ConnectionProfileTaskSettings[] taskSettings;

                    lock (m_connectionProfileLock)
                        taskSettings = m_connectionProfileTaskSettings;

                    foreach (ConnectionProfileTaskSettings settings in taskSettings)
                    {
                        OnStatusMessage("Starting \"{0}\" connection profile \"{1}\" task processing:", m_connectionProfile?.Name ?? "Undefined", settings.Name);

                        if (string.IsNullOrWhiteSpace(settings.ExternalOperation))
                            ProcessFTPTask(settings, client);
                        else
                            ProcessExternalOperationTask(settings);

                        // Handle local file age limit processing, if enabled
                        if (settings.DeleteOldLocalFiles)
                            HandleLocalFileAgeLimitProcessing(settings);
                    }

                    Ticks connectedTime = DateTime.UtcNow.Ticks - connectionStartTime;
                    OnStatusMessage("FTP session connected for {0}", connectedTime.ToElapsedTimeString(2));
                    TotalConnectedTime += connectedTime;
                }
                catch (Exception ex)
                {
                    FailedConnections++;
                    OnProcessException(new InvalidOperationException($"Failed to connect to FTP server \"{ConnectionUserName}@{ConnectionHostName}\": {ex.Message}", ex));
                }

                client.CommandSent -= FtpClient_CommandSent;
                client.ResponseReceived -= FtpClient_ResponseReceived;
                client.FileTransferProgress -= FtpClient_FileTransferProgress;
                client.FileTransferNotification -= FtpClient_FileTransferNotification;
            }
        }

        private void ProcessFTPTask(ConnectionProfileTaskSettings settings, FtpClient client)
        {
            if (string.IsNullOrWhiteSpace(settings.RemotePath))
            {
                OnProcessException(new InvalidOperationException($"Cannot process connection profile task \"{settings.Name}\", remote FTP directory path is undefined."));
                return;
            }

            OnStatusMessage("Attempting to set remote FTP directory path \"{0}\"...", settings.RemotePath);

            try
            {
                client.SetCurrentDirectory(settings.RemotePath);

                OnStatusMessage("Enumerating remote files in \"{0}\"...", settings.RemotePath);

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

                        foreach (FtpFile file in files)
                        {
                            if (!FilePath.IsFilePatternMatch(settings.FileSpecs, file.Name, true))
                                continue;

                            OnStatusMessage("Processing remote file \"{0}\"...", file.Name);

                            try
                            {
                                ProcessFile(settings, file);
                                TotalProcessedFiles++;
                            }
                            catch (Exception ex)
                            {
                                OnProcessException(new InvalidOperationException($"Failed to process remote file \"{file.Name ?? "undefined"}\" in \"{settings.RemotePath}\": {ex.Message}", ex));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnProcessException(new InvalidOperationException($"Failed to enumerate remote files in \"{settings.RemotePath}\": {ex.Message}", ex));
                }
            }
            catch (Exception ex)
            {
                OnProcessException(new InvalidOperationException($"Failed to set remote FTP directory path \"{settings.RemotePath ?? "undefined"}\": {ex.Message}", ex));
            }
        }

        private void ProcessFile(ConnectionProfileTaskSettings settings, FtpFile file)
        {
            if (settings.LimitRemoteFileDownloadByAge && (DateTime.Now - file.Timestamp).Days > Program.Host.Model.Global.MaxRemoteFileAge)
            {
                OnStatusMessage("File \"{0}\" skipped, timestamp \"{1:yyyy-MM-dd HH:mm.ss.fff}\" is older than {2} days.", file.Name, file.Timestamp, Program.Host.Model.Global.MaxRemoteFileAge);
                return;
            }

            if (file.Size > settings.MaximumFileSize * SI2.Mega)
            {
                OnStatusMessage("File \"{0}\" skipped, size of {1:N3} MB is larger than {2:N3} MB configured limit.", file.Name, file.Size / SI2.Mega, settings.MaximumFileSize);
                return;
            }

            if (DownloadFile(settings, file) && settings.DeleteRemoteFilesAfterDownload)
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

        private bool DownloadFile(ConnectionProfileTaskSettings settings, FtpFile file)
        {
            if (string.IsNullOrWhiteSpace(settings.LocalPath) || !Directory.Exists(settings.LocalPath))
            {
                OnProcessException(new InvalidOperationException($"Cannot download file \"{file.Name}\" for connection profile task \"{settings.Name}\": Local path \"{settings.LocalPath ?? ""}\" does not exist."));
                return false;
            }

            string localFileName = GetLocalFileName(settings, file.Name);

            if (File.Exists(localFileName) && settings.ArchiveLocalFilesBeforeOverwrite)
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
                OnProcessException(new InvalidOperationException($"Skipping file \"{file.Name}\" download for connection profile task \"{settings.Name}\": Local file already exists and settings do not allow overwrite."));
                return false;
            }

            OnStatusMessage("Downloading \"{0}\" to \"{1}\"...", file.Name, localFileName);

            try
            {
                using (FtpInputDataStream remoteFile = file.GetInputStream())
                using (FileStream localFile = File.Create(localFileName))
                {
                    remoteFile.CopyTo(localFile);
                    FilesDownloaded++;
                    BytesDownloaded += file.Size;
                }

                return true;
            }
            catch (Exception ex)
            {
                OnProcessException(new InvalidOperationException($"Failed to download file \"{file.Name}\" for connection profile task \"{settings.Name}\" to \"{localFileName}\": {ex.Message}", ex));
            }

            return false;
        }

        private string GetLocalFileName(ConnectionProfileTaskSettings settings, string fileName)
        {
            switch (settings.GetDirectoryNamingConvention())
            {
                case DirectoryNamingConvention.Root:
                    fileName = Path.Combine(settings.LocalPath, fileName);
                    break;
                case DirectoryNamingConvention.Year:
                    fileName = Path.Combine(settings.LocalPath, $"{DateTime.Now.Year}\\", fileName);
                    break;
                case DirectoryNamingConvention.YearMonth:
                    fileName = Path.Combine(settings.LocalPath, $"{DateTime.Now.Year}{DateTime.Now.Month.ToString().PadLeft(2, '0')}\\", fileName);
                    break;
                case DirectoryNamingConvention.YearThenMonth:
                    fileName = Path.Combine(settings.LocalPath, $"{DateTime.Now.Year}\\{DateTime.Now.Month.ToString().PadLeft(2, '0')}\\", fileName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

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

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(externalOperationExecutableName, settings.ID.ToString());
                Process externalOperation = Process.Start(startInfo);

                if ((object)externalOperation == null)
                {
                    OnProcessException(new InvalidOperationException($"Failed to start external operation \"{settings.ExternalOperation}\"."));
                }
                else
                {
                    externalOperation.WaitForExit();
                    OnStatusMessage("External operation \"{0}\" completed with status code {1}.", settings.ExternalOperation, externalOperation.ExitCode);
                }
            }
            catch (Exception ex)
            {
                OnProcessException(new InvalidOperationException($"Failed to execute external operation \"{settings.ExternalOperation}\": {ex.Message}", ex));
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
                bool managingSubfolders = settings.GetDirectoryNamingConvention() != DirectoryNamingConvention.Root;
                string filePattern = managingSubfolders ? "*\\*.*" : "*.*";
                string[] files = FilePath.GetFileList(Path.Combine(FilePath.GetAbsolutePath(settings.LocalPath), filePattern));
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
                            FilePath.WaitForWriteLock(file);
                            File.Delete(file);
                            deletedCount++;
                            OnStatusMessage("File \"{0}\" successfully deleted...", file);

                            if (managingSubfolders)
                            {
                                string rootPathName = FilePath.GetDirectoryName(settings.LocalPath);
                                string directoryName = FilePath.GetDirectoryName(file);

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
            OnFileTransferProgress(this, e.Argument1);
        }

        private void FtpClient_FileTransferNotification(object sender, EventArgs<FtpAsyncResult> e)
        {
            OnStatusMessage("FTP File Transfer: {0}, response code = {1}", e.Argument.Message, e.Argument.ResponseCode);

            if (e.Argument.IsSuccess)
                FilesDownloaded++;
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
        public static event EventHandler<EventArgs<ProcessProgress<long>>> FileTransferProgress;

        // Static Constructor
        static Downloader()
        {
            s_instances = new ConcurrentDictionary<string, Downloader>();
            s_dialupScheduler = new ConcurrentDictionary<string, LogicalThread>();
            s_scheduleManager = new ScheduleManager();
            s_scheduleManager.ScheduleDue += s_scheduleManager_ScheduleDue;
            s_scheduleManager.AddSchedule(DailyCounterResetSchedule, "0 0 * * *", "Resets daily counters", true);
            s_scheduleManager.Start();
        }

        private static void s_scheduleManager_ScheduleDue(object sender, EventArgs<Schedule> e)
        {
            Schedule schedule = e.Argument;

            if (schedule.Name.Equals(DailyCounterResetSchedule))
            {
                // Reset daily IDevice counter for reporting
                foreach (Downloader downloader in s_instances.Values)
                    downloader.MeasurementsReceived = 0;

                return;
            }

            Downloader instance;

            if (s_instances.TryGetValue(schedule.Name, out instance))
            {
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

        private static void OnFileTransferProgress(Downloader instance, ProcessProgress<long> progress)
        {
            FileTransferProgress?.Invoke(instance, new EventArgs<ProcessProgress<long>>(progress));
        }

        #region [ Statistic Functions ]

        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedParameter.Local

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
