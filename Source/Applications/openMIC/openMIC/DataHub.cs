//******************************************************************************************************
//  DataHub.cs - Gbtc
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
//  01/14/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using GSF;
using GSF.ComponentModel.DataAnnotations;
using GSF.Data.Model;
using GSF.Diagnostics;
using GSF.Identity;
using GSF.IO;
using GSF.Web.Hubs;
using GSF.Web.Model.HubOperations;
using GSF.Web.Security;
using GSF.Web.Shared.Model;
using ModbusAdapters;
using ModbusAdapters.Model;
using openMIC.Model;

namespace openMIC
{
    [AuthorizeHubRole]
    public partial class DataHub : RecordOperationsHub<DataHub>, IDataSubscriptionOperations, IDirectoryBrowserOperations
    {
        #region [ Members ]

        // Fields
        private readonly DataSubscriptionOperations m_dataSubscriptionOperations;
        private readonly ModbusOperations m_modbusOperations;

        #endregion

        #region [ Constructors ]

        public DataHub() : base(Program.Host.LogWebHostStatusMessage, Program.Host.LogException)
        {
            void logStatusMessage(string message, UpdateType updateType) => LogStatusMessage(message, updateType);
            void logException(Exception ex) => LogException(ex);

            m_dataSubscriptionOperations = new DataSubscriptionOperations(this, logStatusMessage, logException);
            m_modbusOperations = new ModbusOperations(this, logStatusMessage, logException);
        }

        #endregion

        #region [ Methods ]

        public override Task OnConnected()
        {
            LogStatusMessage($"DataHub connect by {Context.User?.Identity?.Name ?? "Undefined User"} [{Context.ConnectionId}] - count = {ConnectionCount}", UpdateType.Information, false);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled)
            {
                // Dispose any associated hub operations associated with current SignalR client
                m_dataSubscriptionOperations?.EndSession();
                m_modbusOperations?.EndSession();

                LogStatusMessage($"DataHub disconnect by {Context.User?.Identity?.Name ?? "Undefined User"} [{Context.ConnectionId}] - count = {ConnectionCount}", UpdateType.Information, false);
            }

            return base.OnDisconnected(stopCalled);
        }

        #endregion

        #region [ Static ]

        // Static Fields
        private static string s_systemName;
        private static string s_webRootPath;
        private static int s_downloaderProtocolID;
        private static int s_modbusProtocolID;
        private static readonly Func<char, bool> s_isInvalidAcronymChar;
        private static readonly char[] s_digits;
        private static readonly HttpClient s_http;
        private static bool? s_useRemoteScheduler;
        private static string s_remoteSchedulerUri;
        private static HashSet<string> s_allowedSectionMapPaths;
        
        private static readonly string[] s_allowedSectionMaps = { "", "Dranetz", "PQube" };

        private static HashSet<string> AllowedSectionMapPaths => s_allowedSectionMapPaths ??= new HashSet<string>(s_allowedSectionMaps, StringComparer.OrdinalIgnoreCase);

        private static string SystemName => s_systemName ??= Program.Host.Model.Global.SystemName ?? "";

        private static string WebRootPath => s_webRootPath ??= Program.Host.Model.Global.WebRootPath ?? FilePath.GetAbsolutePath("wwwroot");

        private static bool UseRemoteScheduler => s_useRemoteScheduler ?? (s_useRemoteScheduler = Program.Host.Model.Global.UseRemoteScheduler).GetValueOrDefault();

        private static string RemoteSchedulerUri
        {
            get
            {
                if (s_remoteSchedulerUri is null)
                {
                    // At first queued task, remote scheduler address will be available
                    string remoteSchedulerAddress = OperationsController.RemoteSchedulerAddress;

                    if (!(remoteSchedulerAddress is null))
                    {
                        Uri webHostUri = Program.Host.Model.Global.WebHostUri;
                        s_remoteSchedulerUri = $"{webHostUri.Scheme}://{remoteSchedulerAddress}:{webHostUri.Port}";
                    }
                }

                return s_remoteSchedulerUri;
            }
        }

        // Static Constructor
        static DataHub()
        {
            Downloader.ProgressUpdated += HandleProgressUpdated;
            ModbusPoller.ProgressUpdated += (sender, args) => HandleProgressUpdated(sender, new EventArgs<string, List<ProgressUpdate>>(null, new List<ProgressUpdate>(new[] { args.Argument })));

            s_digits = "0123456789".ToCharArray();

            AcronymValidationAttribute acronymValidator = new AcronymValidationAttribute();

            s_isInvalidAcronymChar = testChar =>
            {
                lock (acronymValidator)
                    return !acronymValidator.IsValid(testChar);
            };

            // Create a shared HTTP client instance
            s_http = new HttpClient(new HttpClientHandler { UseCookies = false });
        }

        private static void HandleProgressUpdated(object sender, EventArgs<string, List<ProgressUpdate>> e)
        {
            string deviceName = null;

            switch (sender)
            {
                case Downloader downloader:
                    deviceName = downloader.Name;
                    break;
                case ModbusPoller modbusPoller:
                    deviceName = modbusPoller.Name;
                    break;
            }

            if (deviceName is null)
                return;

            // Inject system name at provided summary updates
            List<ProgressUpdate> progressUpdates = e.Argument2;
            string systemName = string.IsNullOrWhiteSpace(SystemName) ? "" : $"[@{SystemName}] ";

            foreach (ProgressUpdate update in progressUpdates)
            {
                if (!string.IsNullOrWhiteSpace(update.Summary))
                    update.Summary = $"{systemName}{update.Summary}";
            }

            ProgressUpdate(deviceName, e.Argument1, progressUpdates);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static void ProgressUpdate(string deviceName, string clientID, List<ProgressUpdate> progressUpdates)
        {
            IHubConnectionContext<dynamic> clients = GlobalHost.ConnectionManager?.GetHubContext<DataHub>()?.Clients;

            if (!(clients is null))
            {
                List<object> updates = progressUpdates
                    .Select(update => update.AsExpandoObject())
                    .ToList();

                if (clientID is null)
                    clients.All.deviceProgressUpdate(deviceName, updates);
                else
                    clients.Client(clientID).deviceProgressUpdate(deviceName, updates);
            }

            if (UseRemoteScheduler)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        string targetUri = RemoteSchedulerUri;

                        if (!(targetUri is null))
                        {
                            string uri = $"{targetUri}/api/Operations/ProgressUpdate?deviceName={WebUtility.UrlEncode(deviceName)}";
                            string content = JArray.FromObject(progressUpdates).ToString();
                            await s_http.PostAsync(uri, new StringContent(content, Encoding.UTF8, "application/json"));
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.SwallowException(ex);
                    }
                });
            }
        }

        #endregion

        // Client-side script functionality

        #region [ Setting Table Operations ]

        [AuthorizeHubRole("Administrator")]
        [RecordOperation(typeof(Setting), RecordOperation.QueryRecordCount)]
        public int QuerySettingCount(string filterString)
        {
            return DataContext.Table<Setting>().QueryRecordCount(filterString);
        }

        [AuthorizeHubRole("Administrator")]
        [RecordOperation(typeof(Setting), RecordOperation.QueryRecords)]
        public IEnumerable<Setting> QuerySettings(string sortField, bool ascending, int page, int pageSize, string filterString)
        {
            return DataContext.Table<Setting>().QueryRecords(sortField, ascending, page, pageSize, filterString);
        }

        [AuthorizeHubRole("Administrator")]
        [RecordOperation(typeof(Setting), RecordOperation.DeleteRecord)]
        public void DeleteSetting(int id)
        {
            DataContext.Table<Setting>().DeleteRecord(id);
        }

        [AuthorizeHubRole("Administrator")]
        [RecordOperation(typeof(Setting), RecordOperation.CreateNewRecord)]
        public Setting NewSetting()
        {
            return new Setting();
        }

        [AuthorizeHubRole("Administrator")]
        [RecordOperation(typeof(Setting), RecordOperation.AddNewRecord)]
        public void AddNewSetting(Setting record)
        {
            DataContext.Table<Setting>().AddNewRecord(record);
        }

        [AuthorizeHubRole("Administrator, Owner")]
        [RecordOperation(typeof(Setting), RecordOperation.UpdateRecord)]
        public void UpdateSetting(Setting record)
        {
            DataContext.Table<Setting>().UpdateRecord(record);
        }

        #endregion

        #region [ Device Table Operations ]

        private int DownloaderProtocolID => s_downloaderProtocolID != 0 ? s_downloaderProtocolID : s_downloaderProtocolID = DataContext.Connection.ExecuteScalar<int>("SELECT ID FROM Protocol WHERE Acronym='Downloader'");

        private int ModbusProtocolID => s_modbusProtocolID != 0 ? s_modbusProtocolID : s_modbusProtocolID = DataContext.Connection.ExecuteScalar<int>("SELECT ID FROM Protocol WHERE Acronym='Modbus'");

        /// <summary>
        /// Gets protocol ID for "Downloader" adapter.
        /// </summary>
        public int GetDownloaderProtocolID() => DownloaderProtocolID;

        /// <summary>
        /// Gets protocol ID for "ModbusPoller" adapter.
        /// </summary>
        public int GetModbusProtocolID() => ModbusProtocolID;

        [RecordOperation(typeof(Device), RecordOperation.QueryRecordCount)]
        public int QueryDeviceCount(Guid nodeID, string filterText)
        {
            TableOperations<Device> deviceTable = DataContext.Table<Device>();

            RecordRestriction restriction =
                new RecordRestriction("NodeID = {0}", nodeID) +
                deviceTable.GetSearchRestriction(filterText);

            return deviceTable.QueryRecordCount(restriction);
        }

        [RecordOperation(typeof(Device), RecordOperation.QueryRecords)]
        public IEnumerable<Device> QueryDevices(Guid nodeID, string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<Device> deviceTable = DataContext.Table<Device>();

            RecordRestriction restriction =
                new RecordRestriction("NodeID = {0}", nodeID) +
                deviceTable.GetSearchRestriction(filterText);

            return deviceTable.QueryRecords(sortField, ascending, page, pageSize, restriction);
        }

        public IEnumerable<Device> QueryEnabledDevices(Guid nodeID, int limit, string filterText)
        {
            TableOperations<Device> deviceTable = DataContext.Table<Device>();

            RecordRestriction restriction =
                new RecordRestriction("NodeID = {0}", nodeID) +
                new RecordRestriction("Enabled <> 0") +
                deviceTable.GetSearchRestriction(filterText);

            return deviceTable.QueryRecords("Acronym", restriction, limit);
        }

        public Device QueryDevice(string acronym)
        {
            return DataContext.Table<Device>().QueryRecordWhere("Acronym = {0}", acronym) ?? NewDevice();
        }

        public Device QueryDeviceByID(int deviceID)
        {
            return DataContext.Table<Device>().QueryRecordWhere("ID = {0}", deviceID) ?? NewDevice();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Device), RecordOperation.DeleteRecord)]
        public void DeleteDevice(int id)
        {
            TableOperations<Device> deviceTable = DataContext.Table<Device>();            
            
            deviceTable.DeleteRecordWhere("ParentID = {0}", id);
            deviceTable.DeleteRecord(id);

            DataContext.Table<StatusLog>().DeleteRecordWhere("DeviceID = {0}", id);
            DataContext.Table<DownloadedFile>().DeleteRecordWhere("DeviceID = {0}", id);

            ClearCredential(id);
        }

        [RecordOperation(typeof(Device), RecordOperation.CreateNewRecord)]
        public Device NewDevice()
        {
            return DataContext.Table<Device>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Device), RecordOperation.AddNewRecord)]
        public void AddNewDevice(Device device)
        {
            if ((device.ProtocolID ?? 0) == 0)
                device.ProtocolID = DownloaderProtocolID;

            DataContext.Table<Device>().AddNewRecord(device);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Device), RecordOperation.UpdateRecord)]
        public void UpdateDevice(Device device)
        {
            if ((device.ProtocolID ?? 0) == 0)
                device.ProtocolID = DownloaderProtocolID;

            DataContext.Table<Device>().UpdateRecord(device);

            ClearCredential(device.ID);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        public void AddNewOrUpdateDevice(Device device)
        {
            DataContext.Table<Device>().AddNewOrUpdateRecord(device);

            if (device.ID != default)
                ClearCredential(device.ID);
        }

        public Vendor GetDeviceVendor(int? vendorDeviceID)
        {
            if (vendorDeviceID is null)
                return null;

            VendorDevice vendorDevice = DataContext.Table<VendorDevice>().QueryRecordWhere("ID = {0}", vendorDeviceID.Value);
            return vendorDevice is null ? null : DataContext.Table<Vendor>().QueryRecordWhere("ID = {0}", vendorDevice.VendorID);
        }

        #endregion

        #region [ Measurement Table Operations ]

        [RecordOperation(typeof(Measurement), RecordOperation.QueryRecordCount)]
        public int QueryMeasurementCount(string filterText)
        {
            return DataContext.Table<Measurement>().QueryRecordCount(filterText);
        }

        [RecordOperation(typeof(Measurement), RecordOperation.QueryRecords)]
        public IEnumerable<Measurement> QueryMeasurements(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            return DataContext.Table<Measurement>().QueryRecords(sortField, ascending, page, pageSize, filterText);
        }

        public Measurement QueryMeasurement(string signalReference)
        {
            return DataContext.Table<Measurement>().QueryRecordWhere("SignalReference = {0}", signalReference) ?? NewMeasurement();
        }

        public Measurement QueryMeasurementByPointTag(string pointTag)
        {
            return DataContext.Table<Measurement>().QueryRecordWhere("PointTag = {0}", pointTag) ?? NewMeasurement();
        }

        public Measurement QueryMeasurementBySignalID(Guid signalID)
        {
            return DataContext.Table<Measurement>().QueryRecordWhere("SignalID = {0}", signalID) ?? NewMeasurement();
        }

        public IEnumerable<Measurement> QueryDeviceMeasurements(int deviceID)
        {
            return DataContext.Table<Measurement>().QueryRecordsWhere("DeviceID = {0}", deviceID);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Measurement), RecordOperation.DeleteRecord)]
        public void DeleteMeasurement(int id)
        {
            DataContext.Table<Measurement>().DeleteRecord(id);
        }

        [RecordOperation(typeof(Measurement), RecordOperation.CreateNewRecord)]
        public Measurement NewMeasurement()
        {
            return DataContext.Table<Measurement>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Measurement), RecordOperation.AddNewRecord)]
        public void AddNewMeasurement(Measurement measurement)
        {
            DataContext.Table<Measurement>().AddNewRecord(measurement);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Measurement), RecordOperation.UpdateRecord)]
        public void UpdateMeasurement(Measurement measurement)
        {
            DataContext.Table<Measurement>().UpdateRecord(measurement);
        }

        #endregion

        #region [ ConnectionProfile Table Operations ]

        [RecordOperation(typeof(ConnectionProfile), RecordOperation.QueryRecordCount)]
        public int QueryConnectionProfileCount(string filterText)
        {
            return DataContext.Table<ConnectionProfile>().QueryRecordCount(filterText);
        }

        [RecordOperation(typeof(ConnectionProfile), RecordOperation.QueryRecords)]
        public IEnumerable<ConnectionProfile> QueryConnectionProfiles(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            return DataContext.Table<ConnectionProfile>().QueryRecords(sortField, ascending, page, pageSize, filterText);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(ConnectionProfile), RecordOperation.DeleteRecord)]
        public void DeleteConnectionProfile(int id)
        {
            DataContext.Table<ConnectionProfile>().DeleteRecord(id);
        }

        [RecordOperation(typeof(ConnectionProfile), RecordOperation.CreateNewRecord)]
        public ConnectionProfile NewConnectionProfile()
        {
            return DataContext.Table<ConnectionProfile>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(ConnectionProfile), RecordOperation.AddNewRecord)]
        public void AddNewConnectionProfile(ConnectionProfile connectionProfile)
        {
            DataContext.Table<ConnectionProfile>().AddNewRecord(connectionProfile);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(ConnectionProfile), RecordOperation.UpdateRecord)]
        public void UpdateConnectionProfile(ConnectionProfile connectionProfile)
        {
            DataContext.Table<ConnectionProfile>().UpdateRecord(connectionProfile);
        }

        #endregion

        #region [ ConnectionProfileTask Table Operations ]

        public int QueryConnectionProfileTaskCount(int parentID)
        {
            return QueryConnectionProfileTaskCount(parentID, null);
        }

        [RecordOperation(typeof(ConnectionProfileTask), RecordOperation.QueryRecordCount)]
        public int QueryConnectionProfileTaskCount(int parentID, string filterText)
        {
            TableOperations<ConnectionProfileTask> connectionProfileTaskTable = DataContext.Table<ConnectionProfileTask>();
            connectionProfileTaskTable.RootQueryRestriction[0] = parentID;
            return connectionProfileTaskTable.QueryRecordCount(filterText);
        }

        [RecordOperation(typeof(ConnectionProfileTask), RecordOperation.QueryRecords)]
        public IEnumerable<ConnectionProfileTask> QueryConnectionProfileTasks(int parentID, string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<ConnectionProfileTask> connectionProfileTaskTable = DataContext.Table<ConnectionProfileTask>();
            connectionProfileTaskTable.RootQueryRestriction[0] = parentID;
            return connectionProfileTaskTable.QueryRecords(sortField, ascending, page, pageSize, filterText);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(ConnectionProfileTask), RecordOperation.DeleteRecord)]
        public void DeleteConnectionProfileTask(int id)
        {
            DataContext.Table<ConnectionProfileTask>().DeleteRecord(id);
        }

        [RecordOperation(typeof(ConnectionProfileTask), RecordOperation.CreateNewRecord)]
        public ConnectionProfileTask NewConnectionProfileTask()
        {
            return DataContext.Table<ConnectionProfileTask>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(ConnectionProfileTask), RecordOperation.AddNewRecord)]
        public void AddNewConnectionProfileTask(ConnectionProfileTask connectionProfileTask)
        {
            DataContext.Table<ConnectionProfileTask>().AddNewRecord(connectionProfileTask);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(ConnectionProfileTask), RecordOperation.UpdateRecord)]
        public void UpdateConnectionProfileTask(ConnectionProfileTask connectionProfileTask)
        {
            DataContext.Table<ConnectionProfileTask>().UpdateRecord(connectionProfileTask);
        }

        #endregion

        #region [ SignalType Table Operations ]

        [RecordOperation(typeof(SignalType), RecordOperation.QueryRecordCount)]
        public int QuerySignalTypeCount(string filterText)
        {
            return DataContext.Table<SignalType>().QueryRecordCount(filterText);
        }

        [RecordOperation(typeof(SignalType), RecordOperation.QueryRecords)]
        public IEnumerable<SignalType> QuerySignalTypes(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            return DataContext.Table<SignalType>().QueryRecords(sortField, ascending, page, pageSize, filterText);
        }

        public SignalType QuerySignalType(string acronym)
        {
            return DataContext.Table<SignalType>().QueryRecordWhere("Acronym = {0}", acronym);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(SignalType), RecordOperation.DeleteRecord)]
        public void DeleteSignalType(int id)
        {
            DataContext.Table<SignalType>().DeleteRecord(id);
        }

        [RecordOperation(typeof(SignalType), RecordOperation.CreateNewRecord)]
        public SignalType NewSignalType()
        {
            return DataContext.Table<SignalType>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(SignalType), RecordOperation.AddNewRecord)]
        public void AddNewSignalType(SignalType signalType)
        {
            DataContext.Table<SignalType>().AddNewRecord(signalType);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(SignalType), RecordOperation.UpdateRecord)]
        public void UpdateSignalType(SignalType signalType)
        {
            DataContext.Table<SignalType>().UpdateRecord(signalType);
        }

        #endregion

        #region [ RunTime Table Operations ]

        public int QueryRuntimeID(string sourceTable, int sourceID)
        {
            Runtime runtime = DataContext.Table<Runtime>().QueryRecordWhere("SourceTable = {0} AND SourceID = {1}", sourceTable, sourceID);
            return runtime?.ID ?? -1;
        }

        [AuthorizeHubRole("Administrator, Editor")]
        public void DeleteRuntime(string sourceTable, int sourceID)
        {
            DataContext.Table<Runtime>().DeleteRecordWhere("SourceTable = {0} AND SourceID = {1}", sourceTable, sourceID);
        }

        public Runtime NewRuntime()
        {
            return DataContext.Table<Runtime>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        public void AddNewRuntime(Runtime runtime)
        {
            DataContext.Table<Runtime>().AddNewRecord(runtime);
        }

        #endregion

        #region [ Historian Table Operations ]

        [RecordOperation(typeof(Historian), RecordOperation.QueryRecordCount)]
        public int QueryHistorianCount(string filterText)
        {
            return DataContext.Table<Historian>().QueryRecordCount(filterText);
        }

        [RecordOperation(typeof(Historian), RecordOperation.QueryRecords)]
        public IEnumerable<Historian> QueryHistorians(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            return DataContext.Table<Historian>().QueryRecords(sortField, ascending, page, pageSize, filterText);
        }

        public Historian QueryHistorian(string acronym)
        {
            return DataContext.Table<Historian>().QueryRecordWhere("Acronym = {0}", acronym);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Historian), RecordOperation.DeleteRecord)]
        public void DeleteHistorian(int id)
        {
            DataContext.Table<Historian>().DeleteRecord(id);
        }

        [RecordOperation(typeof(Historian), RecordOperation.CreateNewRecord)]
        public Historian NewHistorian()
        {
            return DataContext.Table<Historian>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Historian), RecordOperation.AddNewRecord)]
        public void AddNewHistorian(Historian historian)
        {
            DataContext.Table<Historian>().AddNewRecord(historian);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Historian), RecordOperation.UpdateRecord)]
        public void UpdateHistorian(Historian historian)
        {
            DataContext.Table<Historian>().UpdateRecord(historian);
        }

        /// <summary>
        /// Gets loaded historian adapter instance names.
        /// </summary>
        /// <returns>Historian adapter instance names.</returns>
        public IEnumerable<string> GetInstanceNames()
        {
            return DataContext.Table<Historian>().QueryRecordsWhere("Enabled != 0").Select(historian => historian.Acronym);
        }

        #endregion

        #region [ StatusLog Operations ]

        public StatusLog GetStatusLogForDevice(string deviceName)
        {
            return DataContext.Table<StatusLog>().QueryRecordWhere("DeviceID IN (SELECT ID FROM Device WHERE Acronym LIKE {0})", deviceName) ?? new StatusLog();
        }

        #endregion

        #region [ Data Subscription Operations ]

        // These functions are dependent on subscriptions to data where each client connection can customize the subscriptions, so an instance
        // of the DataHubSubscriptionClient is created per SignalR DataHub client connection to manage the subscription life-cycles.

        public IEnumerable<MeasurementValue> GetMeasurements()
        {
            return m_dataSubscriptionOperations.GetMeasurements();
        }

        public IEnumerable<DeviceDetail> GetDeviceDetails()
        {
            return m_dataSubscriptionOperations.GetDeviceDetails();
        }

        public IEnumerable<MeasurementDetail> GetMeasurementDetails()
        {
            return m_dataSubscriptionOperations.GetMeasurementDetails();
        }

        public IEnumerable<PhasorDetail> GetPhasorDetails()
        {
            return m_dataSubscriptionOperations.GetPhasorDetails();
        }

        public IEnumerable<SchemaVersion> GetSchemaVersion()
        {
            return m_dataSubscriptionOperations.GetSchemaVersion();
        }

        public IEnumerable<MeasurementValue> GetStats()
        {
            return m_dataSubscriptionOperations.GetStats();
        }

        public IEnumerable<StatusLight> GetLights()
        {
            return m_dataSubscriptionOperations.GetLights();
        }

        public void InitializeSubscriptions()
        {
            m_dataSubscriptionOperations.InitializeSubscriptions();
        }

        public void TerminateSubscriptions()
        {
            m_dataSubscriptionOperations.TerminateSubscriptions();
        }

        public void UpdateFilters(string filterExpression)
        {
            m_dataSubscriptionOperations.UpdateFilters(filterExpression);
        }

        public void StatSubscribe(string filterExpression)
        {
            m_dataSubscriptionOperations.StatSubscribe(filterExpression);
        }

        #endregion

        #region [ DirectoryBrowser Operations ]

        public IEnumerable<string> LoadDirectories(string rootFolder, bool showHidden)
        {
            return DirectoryBrowserOperations.LoadDirectories(rootFolder, showHidden);
        }

        public bool IsLogicalDrive(string path)
        {
            return DirectoryBrowserOperations.IsLogicalDrive(path);
        }

        public string ResolvePath(string path)
        {
            return DirectoryBrowserOperations.ResolvePath(path);
        }

        public string CombinePath(string path1, string path2)
        {
            return DirectoryBrowserOperations.CombinePath(path1, path2);
        }

        public void CreatePath(string path)
        {
            DirectoryBrowserOperations.CreatePath(path);
        }

        #endregion

        #region [ Miscellaneous Functions ]

        /// <summary>
        /// Determines if directory exists from server's perspective.
        /// </summary>
        /// <param name="path">Directory path to test for existence.</param>
        /// <returns><c>true</c> if directory exists; otherwise, <c>false</c>.</returns>
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// Determines if file exists from server's perspective.
        /// </summary>
        /// <param name="path">Path and file name to test for existence.</param>
        /// <returns><c>true</c> if file exists; otherwise, <c>false</c>.</returns>
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Requests that the device send the current list of progress updates.
        /// </summary>
        public void QueryDeviceStatus()
        {
            foreach (Downloader downloader in Program.Host.Downloaders)
                downloader.RefreshProgressStates(ConnectionID);
        }

        /// <summary>
        /// Gets current user ID.
        /// </summary>
        /// <returns>Current user ID.</returns>
        public string GetCurrentUserID()
        {
            return Thread.CurrentPrincipal.Identity?.Name ?? UserInfo.CurrentUserID;
        }

        /// <summary>
        /// Gets elapsed time between two dates as a range.
        /// </summary>
        /// <param name="startTime">Start time of query.</param>
        /// <param name="stopTime">Stop time of query.</param>
        /// <returns>Elapsed time between two dates as a range.</returns>
        public Task<string> GetElapsedTimeString(DateTime startTime, DateTime stopTime)
        {
            return Task.Factory.StartNew(() => new Ticks(stopTime - startTime).ToElapsedTimeString(2));
        }

        /// <summary>
        /// Reads section map XML file as JSON.
        /// </summary>
        /// <param name="mapName">Map file name</param>
        /// <returns>Section map XML as JSON.</returns>
        [AuthorizeHubRole("Administrator, Editor")]
        public string GetSectionMap(string mapName)
        {
            // Prevent file access leakage
            if (AllowedSectionMapPaths.Contains(Path.GetDirectoryName(mapName) ?? string.Empty))
                throw new SecurityException("Path access error");

            string mapFileName = Path.Combine(WebRootPath, "SectionMaps", mapName);

            if (!File.Exists(mapFileName))
                throw new FileNotFoundException("Section Map Not Found", mapName);

            XmlDocument mapFile = new XmlDocument();
            mapFile.Load(mapFileName);

            return JsonConvert.SerializeXmlNode(mapFile);
        }

        /// <summary>
        /// Gets XML as JSON.
        /// </summary>
        /// <param name="value">XML source.</param>
        /// <param name="indented"><c>true</c> to indent result JSON; otherwise, <c>false</c>.</param>
        /// <returns>Converted JSON.</returns>
        public string GetXmlAsJson(string value, bool indented)
        {
            XmlDocument document = new();
            document.LoadXml(value);
            return JsonConvert.SerializeXmlNode(document, indented ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None);
        }

        /// <summary>
        /// Gets JSON as XML.
        /// </summary>
        /// <param name="value">JSON source.</param>
        /// <param name="indented"><c>true</c> to indent result XML; otherwise, <c>false</c>.</param>
        /// <returns>Converted XML.</returns>
        public string GetJsonAsXml(string value, bool indented)
        {
            XmlDocument document = JsonConvert.DeserializeXmlNode(value);

            if (document is null)
                return "";

            if (indented)
            {
                StringWriter textWriter = new();
                XmlTextWriter xmlWriter = new(textWriter) { Formatting = System.Xml.Formatting.Indented };
                document.WriteTo(xmlWriter);
                return textWriter.ToString();
            }

            return document.InnerXml;
        }

        /// <summary>
        /// Gets INI as JSON.
        /// </summary>
        /// <param name="value">INI source.</param>
        /// <param name="indented"><c>true</c> to indent result JSON; otherwise, <c>false</c>.</param>
        /// <returns>Converted JSON.</returns>
        public string GetIniAsJson(string value, bool indented) =>
            IniJsonInterop.GetIniAsJson(value, indented, true);

        /// <summary>
        /// Gets JSON as INI.
        /// </summary>
        /// <param name="value">JSON source.</param>
        /// <returns>Converted INI.</returns>
        public string GetJsonAsIni(string value) =>
            IniJsonInterop.GetJsonAsIni(value, true);

        #endregion
    }
}