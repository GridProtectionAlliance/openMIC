//******************************************************************************************************
//  ServiceHost.cs - Gbtc
//
//  Copyright © 2015, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  09/15/2015 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CassiniDev;
using GSF;
using GSF.Communication;
using GSF.Configuration;
using GSF.IO;
using GSF.PhasorProtocols;
using GSF.ServiceProcess;
using GSF.Units;
using Microsoft.Win32;

namespace openMIC
{
    /// <summary>
    /// openMIC Service Host
    /// </summary>
    public sealed partial class ServiceHost : ServiceBase
    {
        #region [ Members ]

        // Constants
        private const string ConfigurationFileName = "DeviceSchedules.xml";
        private const int ConfigurationBackups = 5;

        // Fields
        private CassiniDevServer m_httpServer;
        private bool m_webLogEnabled;
        private AutoResetEvent m_configurationLoadComplete;
        private object m_queuedConfigurationLoadPending;
        private readonly ConcurrentDictionary<object, string> m_derivedNameCache;

        #endregion

        #region [ Constructors ]

        public ServiceHost()
        {
            InitializeComponent();

            // Register event handlers.
            m_serviceHelper.ServiceStarting += ServiceHelper_ServiceStarting;
            m_serviceHelper.ServiceStarted += ServiceHelper_ServiceStarted;
            m_serviceHelper.ServiceStopping += ServiceHelper_ServiceStopping;

            if ((object)m_serviceHelper.StatusLog != null)
                m_serviceHelper.StatusLog.LogException += LogExceptionHandler;

            if ((object)m_serviceHelper.ErrorLogger != null && m_serviceHelper.ErrorLogger.ErrorLog != null)
                m_serviceHelper.ErrorLogger.ErrorLog.LogException += LogExceptionHandler;

            // Create a cache for derived proxy connection names
            m_derivedNameCache = new ConcurrentDictionary<object, string>();
        }

        public ServiceHost(IContainer container)
            : this()
        {
            if ((object)container != null)
                container.Add(this);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the related remote console application name.
        /// </summary>
        private string ConsoleApplicationName
        {
            get
            {
                return ServiceName + "Console.exe";
            }
        }

        /// <summary>
        /// Gets access to the <see cref="GSF.ServiceProcess.ServiceHelper"/>.
        /// </summary>
        private ServiceHelper ServiceHelper
        {
            get
            {
                return m_serviceHelper;
            }
        }

        /// <summary>
        /// Gets reference to the <see cref="TcpServer"/> based remoting server.
        /// </summary>
        private TcpServer RemotingServer
        {
            get
            {
                return m_remotingServer;
            }
        }

        #endregion

        #region [ Methods ]

        #region [ Service Event Handlers ]

        private void ServiceHelper_ServiceStarting(object sender, EventArgs<string[]> e)
        {
            // Create a handler for unobserved task exceptions
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            // Initialize system settings
            m_configurationLoadComplete = new AutoResetEvent(true);
            m_queuedConfigurationLoadPending = new object();
        }

        private void ServiceHelper_ServiceStarted(object sender, EventArgs e)
        {
            // Define a line of asterisks for emphasis
            string stars = new string('*', 79);

            // Log startup information
            m_serviceHelper.UpdateStatus(
                UpdateType.Information,
                "\r\n\r\n{0}\r\n\r\n" +
                "     System Time: {1} UTC\r\n\r\n" +
                "    Current Path: {2}\r\n\r\n" +
                "    Machine Name: {3}\r\n\r\n" +
                "      OS Version: {4}\r\n\r\n" +
                "    Product Name: {5}\r\n\r\n" +
                "  Working Memory: {6}\r\n\r\n" +
                "  Execution Mode: {7}-bit\r\n\r\n" +
                "      Processors: {8}\r\n\r\n" +
                "  GC Server Mode: {9}\r\n\r\n" +
                " GC Latency Mode: {10}\r\n\r\n" +
                " Process Account: {11}\\{12}\r\n\r\n" +
                "{13}\r\n",
                stars,
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                FilePath.TrimFileName(FilePath.RemovePathSuffix(FilePath.GetAbsolutePath("")), 61),
                Environment.MachineName,
                Environment.OSVersion.VersionString,
                Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "ProductName", null).ToNonNullString("<Unavailable>"),
                SI2.ToScaledString(Environment.WorkingSet, 3, "B", SI2.IECSymbols),
                IntPtr.Size * 8,
                Environment.ProcessorCount,
                GCSettings.IsServerGC,
                GCSettings.LatencyMode,
                Environment.UserDomainName,
                Environment.UserName,
                stars);

            ConfigurationFile config = ConfigurationFile.Current;
            CategorizedSettingsElementCollection settings = config.Settings["SystemSettings"];

            // Set up heartbeat as a scheduled service
            m_serviceHelper.AddScheduledProcess(ServiceHeartbeatHandler, "ServiceHeartbeat", "* * * * *");

            // Setup custom service commands
            m_serviceHelper.ClientRequestHandlers.Add(new ClientRequestHandler("Restart", "Attempts to restart the host service", RestartServiceHandler));
            m_serviceHelper.ClientRequestHandlers.Add(new ClientRequestHandler("GetStreamProxyStatus", "Gets current status for all stream proxies", GetStreamProxyStatus));
            m_serviceHelper.ClientRequestHandlers.Add(new ClientRequestHandler("ReloadConfig", "Reloads the current configuration", ReloadConfigurationHandler));
            m_serviceHelper.ClientRequestHandlers.Add(new ClientRequestHandler("DownloadConfig", "Provides the current configuration to requester", DownloadConfigurationHandler));
            m_serviceHelper.ClientRequestHandlers.Add(new ClientRequestHandler("UploadConfig", "Deserializes and loads received configuration", UploadConfigurationHandler));
            m_serviceHelper.ClientRequestHandlers.Add(new ClientRequestHandler("UploadConnection", "Applies received proxy connection to running configuration", UploadConnectionHandler));
            m_serviceHelper.ClientRequestHandlers.Add(new ClientRequestHandler("Enumerate", "Enumerates all openMICs", EnumerateSplittersHandler, new[] { "list", "dir", "ls" }));
            m_serviceHelper.ClientRequestHandlers.Add(new ClientRequestHandler("SendCommand", "Sends command to a specific openMIC", SendCommandHandler));

            try
            {
                // Establish web server.
                m_httpServer = new CassiniDevServer();
                m_httpServer.StartServer(FilePath.GetAbsolutePath(settings["WebFilesPath"].Value), settings["ServerPort"].ValueAsInt32(20580), "/", "");

                m_webLogEnabled = settings["WebLogEnabled"].ValueAsBoolean(true);

                if ((object)m_httpServer.Server != null)
                {
                    if (m_webLogEnabled)
                        m_httpServer.Server.RequestComplete += Server_RequestComplete;

                    m_serviceHelper.UpdateStatus(UpdateType.Information, "Started web server on port {0} from {1}\r\n", m_httpServer.Server.Port, m_httpServer.Server.PhysicalPath);
                }
            }
            catch (Exception ex)
            {
                m_serviceHelper.UpdateStatus(UpdateType.Alarm, "Exception: {0}", ex.Message);
                m_serviceHelper.ErrorLogger.Log(ex);
            }

            LoadCurrentConfiguration();
        }

        private void ServiceHelper_ServiceStopping(object sender, EventArgs e)
        {
            try
            {
                // Disconnect web server.
                if ((object)m_httpServer != null)
                {
                    if ((object)m_httpServer.Server != null && m_webLogEnabled)
                        m_httpServer.Server.RequestComplete -= Server_RequestComplete;

                    m_httpServer.StopServer();
                    m_httpServer.Dispose();
                    m_httpServer = null;

                    m_serviceHelper.UpdateStatus(UpdateType.Information, "Stopped web server.");
                }
            }
            catch (Exception ex)
            {
                m_serviceHelper.UpdateStatus(UpdateType.Alarm, "Exception: {0}", ex.Message);
                m_serviceHelper.ErrorLogger.Log(ex);
            }

            //lock (m_streamSplitters)
            //{
            //    foreach (StreamProxy splitter in m_streamSplitters)
            //    {
            //        splitter.Stop();
            //        splitter.Dispose();
            //        m_serviceHelper.ServiceComponents.Remove(splitter);
            //    }
            //}

            m_serviceHelper.ServiceStarting -= ServiceHelper_ServiceStarting;
            m_serviceHelper.ServiceStarted -= ServiceHelper_ServiceStarted;
            m_serviceHelper.ServiceStopping -= ServiceHelper_ServiceStopping;

            if ((object)m_serviceHelper.StatusLog != null)
            {
                m_serviceHelper.StatusLog.Flush();
                m_serviceHelper.StatusLog.LogException -= LogExceptionHandler;
            }

            if ((object)m_serviceHelper.ErrorLogger != null && (object)m_serviceHelper.ErrorLogger.ErrorLog != null)
            {
                m_serviceHelper.ErrorLogger.ErrorLog.Flush();
                m_serviceHelper.ErrorLogger.ErrorLog.LogException -= LogExceptionHandler;
            }

            if ((object)m_configurationLoadComplete != null)
            {
                // Release any waiting threads before disposing wait handle
                m_configurationLoadComplete.Set();
                m_configurationLoadComplete.Dispose();
            }

            m_configurationLoadComplete = null;

            // Unattach from handler for unobserved task exceptions
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
        }

        private void Server_RequestComplete(object sender, RequestEventArgs e)
        {
            try
            {
                string message;

                if (string.IsNullOrEmpty(e.ResponseLog.Exception))
                    message = string.Format("\r\nIdentity: {0}\r\n Request: {1}\r\nResponse: {2}\r\n", e.RequestLog.Identity, e.RequestLog.Url, e.ResponseLog.StatusCode);
                else
                    message = string.Format("\r\nIdentity: {0}\r\n Request: {1}\r\nResponse: {2} - {3}\r\n", e.RequestLog.Identity, e.RequestLog.Url, e.ResponseLog.StatusCode, e.ResponseLog.Exception);

                m_serviceHelper.UpdateStatus(UpdateType.Information, message);
            }
            catch (Exception ex)
            {
                m_serviceHelper.UpdateStatus(UpdateType.Alarm, "Exception: {0}", ex.Message);
                m_serviceHelper.ErrorLogger.Log(ex);
            }
        }

        // Handle task scheduler exceptions
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            foreach (Exception ex in e.Exception.Flatten().InnerExceptions)
            {
                m_serviceHelper.ErrorLogger.Log(ex, false);
            }

            e.SetObserved();
        }

        // Event handler for processing exceptions encountered while writing entries to a log file.
        private void LogExceptionHandler(object sender, EventArgs<Exception> e)
        {
            DisplayStatusMessage("Log file exception: " + e.Argument.Message, UpdateType.Alarm);
        }

        // Handle auto-health display.
        private void ServiceHeartbeatHandler(string s, object[] args)
        {
            const string requestCommand = "Health";
            ClientRequestHandler requestHandler = m_serviceHelper.FindClientRequestHandler(requestCommand);

            // Send a "Health" command to ourselves on heart-beat schedule...
            if ((object)requestHandler != null)
                requestHandler.HandlerMethod(ClientHelper.PretendRequest(requestCommand));
        }

        #endregion

        #region [ Configuration Management ]

        /// <summary>
        /// Loads the current system configuration.
        /// </summary>
        /// <remarks>
        /// This method handles loading of the current system configuration one request at once.
        /// </remarks>
        private void LoadCurrentConfiguration()
        {
            try
            {
                // Queue configuration deserialization
                ThreadPool.QueueUserWorkItem(QueueConfigurationLoad);
            }
            catch (Exception ex)
            {
                string message = string.Format("Failed to queue configuration load due to exception: {0}", ex.Message);
                HandleException(new InvalidOperationException(message, ex));
            }
        }

        // Since configuration deserialization might take a while, we queue-up activity for one-at-a-time processing
        private void QueueConfigurationLoad(object state)
        {
            // Queue up a configuration cache unless another thread has already requested one
            if (!Monitor.TryEnter(m_queuedConfigurationLoadPending, 500))
                return;

            try
            {
                // Queue new configuration load after waiting for any prior cache operation to complete
                if (m_configurationLoadComplete.WaitOne())
                {
                    try
                    {
                        // Queue up task to to execute load of the latest configuration
                        ThreadPool.QueueUserWorkItem(ExecuteConfigurationLoad);
                    }
                    catch (Exception ex)
                    {
                        string message = string.Format("Failed to queue configuration load process due to exception: {0}", ex.Message);
                        HandleException(new InvalidOperationException(message, ex));
                    }
                }
            }
            finally
            {
                Monitor.Exit(m_queuedConfigurationLoadPending);
            }
        }

        // Executes actual deserialization of current configuration
        private void ExecuteConfigurationLoad(object state)
        {
            //try
            //{
            //    Ticks startTime = DateTime.UtcNow.Ticks;
            //    ProxyConnectionCollection currentConfiguration;
            //    string configurationFile = FilePath.GetAbsolutePath(ConfigurationFileName);

            //    // Attempt to load current configuration
            //    currentConfiguration = ProxyConnectionCollection.LoadConfiguration(configurationFile);

            //    DisplayStatusMessage("Loaded {0} connections from \"{1}\".", UpdateType.Information, currentConfiguration.Count, configurationFile);

            //    ProxyConnection[] newConnections;
            //    ProxyConnection[] deletedConnections;
            //    ProxyConnection[] updatedConnections;

            //    if ((object)m_currentConfiguration == null)
            //    {
            //        // If no existing configuration exists, everything is assumed to be new
            //        newConnections = currentConfiguration.ToArray();
            //        deletedConnections = new ProxyConnection[0];
            //        updatedConnections = new ProxyConnection[0];
            //    }
            //    else
            //    {
            //        // Compare loaded configuration to running configuration so needed adjustments can be made
            //        newConnections = currentConfiguration.Except(m_currentConfiguration, ProxyConnectionIDComparer.Default).ToArray();
            //        deletedConnections = m_currentConfiguration.Except(currentConfiguration, ProxyConnectionIDComparer.Default).ToArray();

            //        // Determine if any the loaded configurations have been updated
            //        IEnumerable<Guid> commonConnections = currentConfiguration.Intersect(m_currentConfiguration, ProxyConnectionIDComparer.Default).Select(connection => connection.ID);
            //        IEnumerable<ProxyConnection> currentConnections = m_currentConfiguration.Where(connection => commonConnections.Contains(connection.ID));
            //        IEnumerable<ProxyConnection> loadedConnections = currentConfiguration.Where(connection => commonConnections.Contains(connection.ID));

            //        // Updated proxy connections are any where field values may have changed
            //        updatedConnections = loadedConnections.Except(currentConnections, ProxyConnectionFieldComparer.Default).ToArray();
            //    }

            //    if (newConnections.Length > 0)
            //        DisplayStatusMessage("-- Detected {0} new connections in loaded configuration.", UpdateType.Information, newConnections.Length);

            //    if (deletedConnections.Length > 0)
            //        DisplayStatusMessage("-- Detected {0} connections to be removed in loaded configuration.", UpdateType.Information, deletedConnections.Length);

            //    if (updatedConnections.Length > 0)
            //        DisplayStatusMessage("-- Detected {0} updated connections in loaded configuration.", UpdateType.Information, updatedConnections.Length);

            //    lock (m_streamSplitters)
            //    {
            //        // Handle connection removals
            //        foreach (StreamProxy splitter in deletedConnections.Select(deletedConnection => m_streamSplitters.Find(s => s.ID == deletedConnection.ID)))
            //        {
            //            if ((object)splitter != null)
            //            {
            //                splitter.Stop();
            //                splitter.Dispose();
            //                m_streamSplitters.Remove(splitter);
            //                m_serviceHelper.ServiceComponents.Remove(splitter);
            //            }
            //        }

            //        // Handle connection additions
            //        foreach (ProxyConnection newConnection in newConnections)
            //        {
            //            StreamProxy splitter = new StreamProxy(newConnection);

            //            splitter.StatusMessage += splitter_StatusMessage;
            //            splitter.ProcessException += splitter_ProcessException;

            //            m_streamSplitters.Add(splitter);
            //            m_serviceHelper.ServiceComponents.Add(splitter);
            //        }

            //        // Handle connection updates
            //        foreach (ProxyConnection updatedConnection in updatedConnections)
            //        {
            //            StreamProxy splitter = m_streamSplitters.Find(s => s.ID == updatedConnection.ID);

            //            if ((object)splitter != null)
            //                splitter.ProxyConnection = updatedConnection;
            //        }
            //    }

            //    // Make loaded configuration the current configuration
            //    m_currentConfiguration = currentConfiguration;

            //    double elapsedTime = (DateTime.UtcNow.Ticks - startTime).ToSeconds();
            //    DisplayStatusMessage("Configuration loaded and applied in {0}...", UpdateType.Information, elapsedTime < 0.01D ? "less than a second" : elapsedTime.ToString("0.00") + " seconds");

            //}
            //catch (Exception ex)
            //{
            //    string message = string.Format("Failed to load configuration : {0}", ex.Message);
            //    HandleException(new InvalidOperationException(message, ex));
            //}
            //finally
            //{
            //    // Release any waiting threads
            //    if ((object)m_configurationLoadComplete != null)
            //        m_configurationLoadComplete.Set();
            //}
        }

        private void BackupConfiguration()
        {
            string configurationFile = FilePath.GetAbsolutePath(ConfigurationFileName);

            try
            {
                // Create multiple backup configurations
                for (int i = ConfigurationBackups; i > 0; i--)
                {
                    string origConfigFile = configurationFile + ".backup" + (i == 1 ? "" : (i - 1).ToString());

                    if (File.Exists(origConfigFile))
                    {
                        string nextConfigFile = configurationFile + ".backup" + i;

                        if (File.Exists(nextConfigFile))
                            File.Delete(nextConfigFile);

                        File.Move(origConfigFile, nextConfigFile);
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayStatusMessage("Failed to create extra backup configurations due to exception: {0}", UpdateType.Warning, ex.Message);
                m_serviceHelper.ErrorLogger.Log(ex);
            }

            try
            {
                // Back up current configuration file, if any
                if (File.Exists(configurationFile))
                {
                    string backupConfigFile = configurationFile + ".backup";

                    if (File.Exists(backupConfigFile))
                        File.Delete(backupConfigFile);

                    File.Move(configurationFile, backupConfigFile);
                }
            }
            catch (Exception ex)
            {
                DisplayStatusMessage("Failed to backup last known cached configuration due to exception: {0}", UpdateType.Warning, ex.Message);
                m_serviceHelper.ErrorLogger.Log(ex);
            }
        }

        #endregion

        #region [ Service Command Handlers ]

        // Attempts to restart the hose service.
        private void RestartServiceHandler(ClientRequestInfo requestInfo)
        {
            if (requestInfo.Request.Arguments.ContainsHelpRequest)
            {
                StringBuilder helpMessage = new StringBuilder();

                helpMessage.Append("Attempts to restart the host service.");
                helpMessage.AppendLine();
                helpMessage.AppendLine();
                helpMessage.Append("   Usage:");
                helpMessage.AppendLine();
                helpMessage.Append("       Restart [Options]");
                helpMessage.AppendLine();
                helpMessage.AppendLine();
                helpMessage.Append("   Options:");
                helpMessage.AppendLine();
                helpMessage.Append("       -?".PadRight(20));
                helpMessage.Append("Displays this help message");

                DisplayResponseMessage(requestInfo, helpMessage.ToString());
            }
            else
            {
                DisplayStatusMessage("Attempting to restart host service...", UpdateType.Information);

                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo(ConsoleApplicationName)
                    {
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                        Arguments = ServiceName + " -restart"
                    };

                    using (Process shell = new Process())
                    {
                        shell.StartInfo = psi;
                        shell.Start();

                        if (!shell.WaitForExit(30000))
                            shell.Kill();
                    }

                    SendResponse(requestInfo, true);
                }
                catch (Exception ex)
                {
                    SendResponse(requestInfo, false, "Failed to restart host service: {0}", ex.Message);
                    m_serviceHelper.ErrorLogger.Log(ex);
                }
            }
        }

        private void GetStreamProxyStatus(ClientRequestInfo requestInfo)
        {
            if (requestInfo.Request.Arguments.ContainsHelpRequest)
            {
                StringBuilder helpMessage = new StringBuilder();

                helpMessage.Append("Gets current status for all stream proxies.");
                helpMessage.AppendLine();
                helpMessage.AppendLine();
                helpMessage.Append("   Usage:");
                helpMessage.AppendLine();
                helpMessage.Append("       GetStreamProxyStatus [Options]");
                helpMessage.AppendLine();
                helpMessage.AppendLine();
                helpMessage.Append("   Options:");
                helpMessage.AppendLine();
                helpMessage.Append("       -?".PadRight(20));
                helpMessage.Append("Displays this help message");

                DisplayResponseMessage(requestInfo, helpMessage.ToString());
            }
            else
            {
                //// Send current status all stream proxies
                //StreamProxyStatus[] streamProxies;

                //lock (m_streamSplitters)
                //{
                //    streamProxies = m_streamSplitters.Select(splitter => splitter.StreamProxyStatus).ToArray();
                //}

                //SendResponseWithAttachment(requestInfo, true, streamProxies, null);
            }
        }

        private void EnumerateSplittersHandler(ClientRequestInfo requestInfo)
        {
            if (requestInfo.Request.Arguments.ContainsHelpRequest)
            {
                StringBuilder helpMessage = new StringBuilder();

                helpMessage.Append("Enumerates all openMICs.");
                helpMessage.AppendLine();
                helpMessage.AppendLine();
                helpMessage.Append("   Usage:");
                helpMessage.AppendLine();
                helpMessage.Append("       EnumerateSplitters [index] [Options]");
                helpMessage.AppendLine();
                helpMessage.AppendLine();
                helpMessage.Append("   index:".PadRight(20));
                helpMessage.Append("index of the splitter to display, or all if not specified");
                helpMessage.Append("   Options:");
                helpMessage.AppendLine();
                helpMessage.Append("       -?".PadRight(20));
                helpMessage.Append("Displays this help message");

                DisplayResponseMessage(requestInfo, helpMessage.ToString());
            }
            else
            {
                //    // Send current status all stream proxies                
                //    StringBuilder splitterEnumeration = new StringBuilder();
                //    StreamProxy splitter = null;
                //    int index;

                //    Arguments args = requestInfo.Request.Arguments;

                //    if (args.Exists("OrderedArg1"))
                //    {
                //        lock (m_streamSplitters)
                //        {
                //            if (int.TryParse(args["OrderedArg1"], out index) && index >= 0 && index < m_streamSplitters.Count)
                //            {
                //                splitter = m_streamSplitters[index];
                //            }
                //            else
                //            {
                //                DisplayResponseMessage(requestInfo, "Invalid splitter index specified.");
                //                requestInfo.Request.Arguments = new Arguments("-?");
                //                EnumerateSplittersHandler(requestInfo);
                //            }
                //        }

                //        if ((object)splitter != null)
                //        {
                //            splitterEnumeration.AppendFormat("  {0} - {1}\r\n          {2}\r\n          {3}\r\n\r\n",
                //                    index.ToString().PadLeft(3),
                //                    splitter.Name,
                //                    splitter.ID,
                //                    splitter.StreamProxyStatus.ConnectionState);

                //            splitterEnumeration.Append(splitter.Status);
                //        }
                //    }
                //    else
                //    {
                //        // Show all splitters with associated index
                //        lock (m_streamSplitters)
                //        {
                //            splitterEnumeration.AppendFormat("\r\nIndices for {0} openMICs:\r\n\r\n", m_streamSplitters.Count);

                //            for (int i = 0; i < m_streamSplitters.Count; i++)
                //            {
                //                splitter = m_streamSplitters[i];

                //                splitterEnumeration.AppendFormat("  {0} - {1}\r\n          {2}\r\n          {3}\r\n\r\n",
                //                        i.ToString().PadLeft(3),
                //                        splitter.Name,
                //                        splitter.ID,
                //                        splitter.StreamProxyStatus.ConnectionState);
                //            }
                //        }
                //    }

                //    if (splitterEnumeration.Length > 0)
                //        DisplayResponseMessage(requestInfo, splitterEnumeration.ToString());
            }
        }

        private void SendCommandHandler(ClientRequestInfo requestInfo)
        {
            if (requestInfo.Request.Arguments.ContainsHelpRequest)
            {
                StringBuilder helpMessage = new StringBuilder();

                helpMessage.Append("Sends command to a specific openMIC.");
                helpMessage.AppendLine();
                helpMessage.AppendLine();
                helpMessage.Append("   Usage:");
                helpMessage.AppendLine();
                helpMessage.Append("       SendCommand index command [Options]");
                helpMessage.AppendLine();
                helpMessage.AppendLine();
                helpMessage.Append("       index".PadRight(20));
                helpMessage.AppendLine("Splitter index from EnumerateSplitters command");
                helpMessage.Append("       command".PadRight(20));
                helpMessage.AppendLine("One of the following:");

                string[] commands = Enum.GetNames(typeof(DeviceCommand));
                DeviceCommand command;

                for (int i = 0; i < commands.Length; i++)
                {
                    command = (DeviceCommand)Enum.Parse(typeof(DeviceCommand), commands[i]);
                    helpMessage.Append(new string(' ', 25));
                    helpMessage.AppendFormat("{0} - {1}\r\n", (int)command, command);
                }

                helpMessage.Append("   Options:");
                helpMessage.AppendLine();
                helpMessage.Append("       -?".PadRight(20));
                helpMessage.Append("Displays this help message");

                DisplayResponseMessage(requestInfo, helpMessage.ToString());
            }
            else
            {
                //Arguments args = requestInfo.Request.Arguments;

                //if (args.OrderedArgCount == 2)
                //{
                //    int index;
                //    DeviceCommand command = DeviceCommand.ReservedBits;
                //    StreamProxy splitter = null;

                //    lock (m_streamSplitters)
                //    {
                //        if (int.TryParse(args["OrderedArg1"], out index) && index >= 0 && index < m_streamSplitters.Count)
                //        {
                //            if (Enum.TryParse(args["OrderedArg2"], true, out command))
                //            {
                //                splitter = m_streamSplitters[index];
                //            }
                //            else
                //            {
                //                DisplayResponseMessage(requestInfo, "Invalid device command specified.");
                //                requestInfo.Request.Arguments = new Arguments("-?");
                //                SendCommandHandler(requestInfo);
                //            }
                //        }
                //        else
                //        {
                //            DisplayResponseMessage(requestInfo, "Invalid splitter index specified.");
                //            requestInfo.Request.Arguments = new Arguments("-?");
                //            SendCommandHandler(requestInfo);
                //        }
                //    }

                //    if ((object)splitter != null)
                //    {
                //        splitter.SendCommand(command);
                //        DisplayResponseMessage(requestInfo, "Sent device command \"{0}\" to \"{1}\"", command, splitter.Name);
                //    }
                //}
                //else
                //{
                //    DisplayResponseMessage(requestInfo, "Invalid number of arguments for send command.");
                //    requestInfo.Request.Arguments = new Arguments("-?");
                //    SendCommandHandler(requestInfo);
                //}
            }
        }

        // Reload the current configuration
        private void ReloadConfigurationHandler(ClientRequestInfo requestInfo)
        {
            if (requestInfo.Request.Arguments.ContainsHelpRequest)
            {
                StringBuilder helpMessage = new StringBuilder();

                helpMessage.Append("Reloads the current configuration.");
                helpMessage.AppendLine();
                helpMessage.AppendLine();
                helpMessage.Append("   Usage:");
                helpMessage.AppendLine();
                helpMessage.Append("       ReloadConfig [Options]");
                helpMessage.AppendLine();
                helpMessage.AppendLine();
                helpMessage.Append("   Options:");
                helpMessage.AppendLine();
                helpMessage.Append("       -?".PadRight(20));
                helpMessage.Append("Displays this help message");

                DisplayResponseMessage(requestInfo, helpMessage.ToString());
            }
            else
            {
                LoadCurrentConfiguration();
                SendResponse(requestInfo, true);
            }
        }

        // Send the current configuration to requester
        private void DownloadConfigurationHandler(ClientRequestInfo requestInfo)
        {
            if (requestInfo.Request.Arguments.ContainsHelpRequest)
            {
                StringBuilder helpMessage = new StringBuilder();

                helpMessage.Append("Provides the current configuration to requester.");
                helpMessage.AppendLine();
                helpMessage.AppendLine();
                helpMessage.Append("   Usage:");
                helpMessage.AppendLine();
                helpMessage.Append("       DownloadConfig [Options]");
                helpMessage.AppendLine();
                helpMessage.AppendLine();
                helpMessage.Append("   Options:");
                helpMessage.AppendLine();
                helpMessage.Append("       -?".PadRight(20));
                helpMessage.Append("Displays this help message");

                DisplayResponseMessage(requestInfo, helpMessage.ToString());
            }
            else
            {
                //// Send current running configuration back to requester
                //SendResponseWithAttachment(requestInfo, true, ProxyConnectionCollection.SerializeConfiguration(m_currentConfiguration), null);
                //DisplayStatusMessage("Running configuration sent to requester.", UpdateType.Information);
            }
        }

        // Apply the received configuration to as the running configuration
        private void UploadConfigurationHandler(ClientRequestInfo requestInfo)
        {
            if (requestInfo.Request.Arguments.ContainsHelpRequest)
            {
                StringBuilder helpMessage = new StringBuilder();

                helpMessage.Append("Deserializes and loads received configuration.");
                helpMessage.AppendLine();
                helpMessage.AppendLine();
                helpMessage.Append("   Usage:");
                helpMessage.AppendLine();
                helpMessage.Append("       UploadConfig [Options]");
                helpMessage.AppendLine();
                helpMessage.AppendLine();
                helpMessage.Append("   Options:");
                helpMessage.AppendLine();
                helpMessage.Append("       -?".PadRight(20));
                helpMessage.Append("Displays this help message");

                DisplayResponseMessage(requestInfo, helpMessage.ToString());
            }
            else
            {
                //if (requestInfo.Request.Attachments.Count > 0)
                //{
                //    try
                //    {
                //        // Attempt to deserialize received configuration
                //        ProxyConnectionCollection receivedConfiguration = ProxyConnectionCollection.DeserializeConfiguration(requestInfo.Request.Attachments[0] as byte[]);

                //        // Backup existing service configuration
                //        BackupConfiguration();

                //        // Save new service configuration
                //        ProxyConnectionCollection.SaveConfiguration(receivedConfiguration, FilePath.GetAbsolutePath(ConfigurationFileName));

                //        // Apply changes
                //        LoadCurrentConfiguration();

                //        // Send response that upload succeeded
                //        SendResponse(requestInfo, true);

                //        DisplayStatusMessage("Uploaded configuration applied.", UpdateType.Information);
                //    }
                //    catch (Exception ex)
                //    {
                //        string message = string.Format("Failed to apply new configuration due to exception: {0}", ex.Message);
                //        DisplayStatusMessage(message, UpdateType.Warning);
                //        SendResponse(requestInfo, false, message);
                //        m_serviceHelper.ErrorLogger.Log(ex);
                //    }
                //}
                //else
                //{
                //    const string message = "No configuration was uploaded - could not apply new configuration.";
                //    DisplayStatusMessage(message, UpdateType.Warning);
                //    SendResponse(requestInfo, false, message);
                //}
            }
        }

        // Apply the received proxy connection to the running configuration
        private void UploadConnectionHandler(ClientRequestInfo requestInfo)
        {
            if (requestInfo.Request.Arguments.ContainsHelpRequest)
            {
                StringBuilder helpMessage = new StringBuilder();

                helpMessage.Append("Applies received proxy connection to running configuration.");
                helpMessage.AppendLine();
                helpMessage.AppendLine();
                helpMessage.Append("   Usage:");
                helpMessage.AppendLine();
                helpMessage.Append("       UploadConnection [Options]");
                helpMessage.AppendLine();
                helpMessage.AppendLine();
                helpMessage.Append("   Options:");
                helpMessage.AppendLine();
                helpMessage.Append("       -?".PadRight(20));
                helpMessage.Append("Displays this help message");

                DisplayResponseMessage(requestInfo, helpMessage.ToString());
            }
            else
            {
                //if (requestInfo.Request.Attachments.Count > 0)
                //{
                //    try
                //    {
                //        // Attempt to deserialize received connection
                //        ProxyConnection receivedConfiguration = requestInfo.Request.Attachments[0] as ProxyConnection;

                //        if ((object)receivedConfiguration == null)
                //        {
                //            const string message = "Uploaded proxy connection was empty - could not apply new connection.";
                //            DisplayStatusMessage(message, UpdateType.Warning);
                //            SendResponse(requestInfo, false, message);
                //        }
                //        else
                //        {
                //            string appliedStyle;

                //            lock (m_streamSplitters)
                //            {
                //                StreamProxy splitter = m_streamSplitters.Find(s => s.ID == receivedConfiguration.ID);

                //                if ((object)splitter != null)
                //                {
                //                    // Update existing stream proxy
                //                    splitter.ProxyConnection = receivedConfiguration;
                //                    m_currentConfiguration[receivedConfiguration.ID] = receivedConfiguration;
                //                    appliedStyle = "to existing stream proxy";
                //                }
                //                else
                //                {
                //                    // Create new stream proxy
                //                    splitter = new StreamProxy(receivedConfiguration);

                //                    splitter.StatusMessage += splitter_StatusMessage;
                //                    splitter.ProcessException += splitter_ProcessException;

                //                    m_streamSplitters.Add(splitter);
                //                    m_serviceHelper.ServiceComponents.Add(splitter);
                //                    m_currentConfiguration.Add(receivedConfiguration);
                //                    appliedStyle = "as a new stream proxy";
                //                }
                //            }

                //            // Backup existing service configuration
                //            BackupConfiguration();

                //            // Save updated service configuration
                //            ProxyConnectionCollection.SaveConfiguration(m_currentConfiguration, FilePath.GetAbsolutePath(ConfigurationFileName));

                //            SendResponse(requestInfo, true);
                //            DisplayStatusMessage("Uploaded connection applied {0}.", UpdateType.Information, appliedStyle);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        string message = string.Format("Failed to apply new proxy connection due to exception: {0}", ex.Message);
                //        DisplayStatusMessage(message, UpdateType.Warning);
                //        SendResponse(requestInfo, false, message);
                //        m_serviceHelper.ErrorLogger.Log(ex);
                //    }
                //}
                //else
                //{
                //    const string message = "No proxy connection was uploaded - could not apply new connection.";
                //    DisplayStatusMessage(message, UpdateType.Warning);
                //    SendResponse(requestInfo, false, message);
                //}
            }
        }

        #endregion

        #region [ openMIC Event Handlers ]

        private void splitter_StatusMessage(object sender, EventArgs<string> e)
        {
            DisplayStatusMessage("[{0}] {1}", UpdateType.Information, GetDerivedName(sender), e.Argument);
        }

        private void splitter_ProcessException(object sender, EventArgs<Exception> e)
        {
            HandleException(new InvalidOperationException(string.Format("[{0}] openMIC exception: {1}", GetDerivedName(sender), e.Argument.Message), e.Argument));
        }

        #endregion

        #region [ Broadcast Message Handling ]

        /// <summary>
        /// Sends an actionable response to client.
        /// </summary>
        /// <param name="requestInfo"><see cref="ClientRequestInfo"/> instance containing the client request.</param>
        /// <param name="success">Flag that determines if this response to client request was a success.</param>
        private void SendResponse(ClientRequestInfo requestInfo, bool success)
        {
            SendResponseWithAttachment(requestInfo, success, null, null);
        }

        /// <summary>
        /// Sends an actionable response to client with a formatted message.
        /// </summary>
        /// <param name="requestInfo"><see cref="ClientRequestInfo"/> instance containing the client request.</param>
        /// <param name="success">Flag that determines if this response to client request was a success.</param>
        /// <param name="status">Formatted status message to send with response.</param>
        /// <param name="args">Arguments of the formatted status message.</param>
        private void SendResponse(ClientRequestInfo requestInfo, bool success, string status, params object[] args)
        {
            SendResponseWithAttachment(requestInfo, success, null, status, args);
        }

        /// <summary>
        /// Sends an actionable response to client with a formatted message and attachment.
        /// </summary>
        /// <param name="requestInfo"><see cref="ClientRequestInfo"/> instance containing the client request.</param>
        /// <param name="success">Flag that determines if this response to client request was a success.</param>
        /// <param name="attachment">Attachment to send with response.</param>
        /// <param name="status">Formatted status message to send with response.</param>
        /// <param name="args">Arguments of the formatted status message.</param>
        private void SendResponseWithAttachment(ClientRequestInfo requestInfo, bool success, object attachment, string status, params object[] args)
        {
            try
            {
                // Send actionable response
                m_serviceHelper.SendActionableResponse(requestInfo, success, attachment, status, args);

                if (m_serviceHelper.LogStatusUpdates && m_serviceHelper.StatusLog.IsOpen)
                {
                    string responseType = requestInfo.Request.Command + (success ? ":Success" : ":Failure");
                    string arguments = requestInfo.Request.Arguments.ToString();
                    string message = responseType + (string.IsNullOrWhiteSpace(arguments) ? "" : "(" + arguments + ")");

                    if (status != null)
                    {
                        if (args.Length == 0)
                            message += " - " + status;
                        else
                            message += " - " + string.Format(status, args);
                    }

                    // Log details of client request as well as response
                    m_serviceHelper.StatusLog.WriteTimestampedLine(message);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Failed to send client response due to an exception: {0}", ex.Message);
                HandleException(new InvalidOperationException(message, ex));
            }
        }

        /// <summary>
        /// Displays a response message to client requester.
        /// </summary>
        /// <param name="requestInfo"><see cref="ClientRequestInfo"/> instance containing the client request.</param>
        /// <param name="status">Formatted status message to send to client.</param>
        /// <param name="args">Arguments of the formatted status message.</param>
        private void DisplayResponseMessage(ClientRequestInfo requestInfo, string status, params object[] args)
        {
            try
            {
                m_serviceHelper.UpdateStatus(requestInfo.Sender.ClientID, UpdateType.Information, string.Format("{0}\r\n\r\n", status), args);
            }
            catch (Exception ex)
            {
                string message = string.Format("Failed to update client status \"{0}\" due to an exception: {1}", status.ToNonNullString(), ex.Message);
                HandleException(new InvalidOperationException(message, ex));
            }
        }

        /// <summary>
        /// Displays a broadcast message to all subscribed clients.
        /// </summary>
        /// <param name="status">Status message to send to all clients.</param>
        /// <param name="type"><see cref="UpdateType"/> of message to send.</param>
        private void DisplayStatusMessage(string status, UpdateType type)
        {
            try
            {
                status = status.Replace("{", "{{").Replace("}", "}}");
                m_serviceHelper.UpdateStatus(type, string.Format("{0}\r\n\r\n", status));
            }
            catch (Exception ex)
            {
                string message = string.Format("Failed to update client status \"{0}\" due to an exception: {1}", status.ToNonNullString(), ex.Message);
                HandleException(new InvalidOperationException(message, ex));
            }
        }

        /// <summary>
        /// Displays a broadcast message to all subscribed clients.
        /// </summary>
        /// <param name="status">Formatted status message to send to all clients.</param>
        /// <param name="type"><see cref="UpdateType"/> of message to send.</param>
        /// <param name="args">Arguments of the formatted status message.</param>
        private void DisplayStatusMessage(string status, UpdateType type, params object[] args)
        {
            try
            {
                DisplayStatusMessage(string.Format(status, args), type);
            }
            catch (Exception ex)
            {
                string message = string.Format("Failed to update client status \"{0}\" due to an exception: {1}", status.ToNonNullString(), ex.Message);
                HandleException(new InvalidOperationException(message, ex));
            }
        }

        #endregion

        #region[ Common Methods ]

        // Gets derived name of specified object.
        private string GetDerivedName(object sender)
        {
            return m_derivedNameCache.GetOrAdd(sender, key =>
            {
                string name;
                IProvideStatus statusProvider = key as IProvideStatus;

                if ((object)statusProvider != null)
                    name = statusProvider.Name;
                else
                    name = key as string;

                if (string.IsNullOrWhiteSpace(name))
                    name = key.GetType().Name;

                return name;
            });
        }

        // Send the error to the service helper and error logger
        private void HandleException(Exception ex)
        {
            string newLines = string.Format("{0}{0}", Environment.NewLine);

            m_serviceHelper.ErrorLogger.Log(ex);
            m_serviceHelper.UpdateStatus(UpdateType.Alarm, ex.Message + newLines);
        }

        #endregion

        #endregion
    }
}
