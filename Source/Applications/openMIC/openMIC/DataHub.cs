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
//  01/14/2016 - Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using GSF;
using GSF.ComponentModel.DataAnnotations;
using GSF.Configuration;
using GSF.Data.Model;
using GSF.Identity;
using GSF.IO;
using GSF.Web.Hubs;
using GSF.Web.Model.HubOperations;
using GSF.Web.Security;
using Microsoft.AspNet.SignalR;
using openMIC.Model;

namespace openMIC
{
    [AuthorizeHubRole]
    public class DataHub : RecordOperationsHub<DataHub>, IDataSubscriptionOperations, IModbusOperations, IDirectoryBrowserOperations
    {
        #region [ Members ]

        // Fields
        private readonly DataSubscriptionOperations m_dataSubscriptionOperations;
        private readonly ModbusOperations m_modbusOperations;

        #endregion

        #region [ Constructors ]

        public DataHub() : base(Program.Host.LogStatusMessage, Program.Host.LogException)
        {
            Action<string, UpdateType> logStatusMessage = (message, updateType) => LogStatusMessage(message, updateType);
            Action<Exception> logException = ex => LogException(ex);

            m_dataSubscriptionOperations = new DataSubscriptionOperations(this, logStatusMessage, logException);
            m_modbusOperations = new ModbusOperations(this, logStatusMessage, logException);
        }

        #endregion

        #region [ Methods ]

        public override Task OnConnected()
        {
            Program.Host.LogStatusMessage($"DataHub connect by {Context.User?.Identity?.Name ?? "Undefined User"} [{Context.ConnectionId}] - count = {ConnectionCount}");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled)
            {
                // Dispose any associated hub operations associated with current SignalR client
                m_dataSubscriptionOperations?.EndSession();
                m_modbusOperations?.EndSession();

                Program.Host.LogStatusMessage($"DataHub disconnect by {Context.User?.Identity?.Name ?? "Undefined User"} [{Context.ConnectionId}] - count = {ConnectionCount}");
            }

            return base.OnDisconnected(stopCalled);
        }

        #endregion

        #region [ Static ]

        // Static Fields
        private static int s_downloaderProtocolID;
        private static int s_modbusProtocolID;
        private static string s_configurationCachePath;
        private static readonly Func<char, bool> s_isInvalidAcronymChar;
        private static readonly char[] s_digits;

        // Static Constructor
        static DataHub()
        {
            Downloader.ProgressUpdated += ProgressUpdated;
            ModbusPoller.ProgressUpdated += (sender, args) => ProgressUpdated(sender, new EventArgs<string, List<ProgressUpdate>>(null, new List<ProgressUpdate>() { args.Argument }));

            s_digits = "0123456789".ToCharArray();

            AcronymValidationAttribute acronymValidator = new AcronymValidationAttribute();

            s_isInvalidAcronymChar = testChar =>
            {
                lock (acronymValidator)
                    return !acronymValidator.IsValid(testChar);
            };
        }

        private static void ProgressUpdated(object sender, EventArgs<string, List<ProgressUpdate>> e)
        {
            Downloader downloader = sender as Downloader;
            string deviceName = null;

            if ((object)downloader != null)
                deviceName = downloader.Name;

            ModbusPoller modbusPoller = sender as ModbusPoller;

            if ((object)modbusPoller != null)
                deviceName = modbusPoller.Name;

            if ((object)deviceName == null)
                return;

            string clientID = e.Argument1;

            List<object> updates = e.Argument2
                .Select(update => update.AsExpandoObject())
                .ToList();

            if ((object)clientID != null)
                GlobalHost.ConnectionManager.GetHubContext<DataHub>().Clients.Client(clientID).deviceProgressUpdate(deviceName, updates);
            else
                GlobalHost.ConnectionManager.GetHubContext<DataHub>().Clients.All.deviceProgressUpdate(deviceName, updates);
        }

        #endregion

        // Client-side script functionality

        #region [ Device Table Operations ]

        private int DownloaderProtocolID => s_downloaderProtocolID != 0 ? s_downloaderProtocolID : (s_downloaderProtocolID = DataContext.Connection.ExecuteScalar<int>("SELECT ID FROM Protocol WHERE Acronym='Downloader'"));

        private int ModbusProtocolID => s_modbusProtocolID != 0 ? s_modbusProtocolID : (s_modbusProtocolID = DataContext.Connection.ExecuteScalar<int>("SELECT ID FROM Protocol WHERE Acronym='Modbus'"));

        /// <summary>
        /// Gets protocol ID for "Downloader" adapter.
        /// </summary>
        public int GetDownloaderProtocolID() => DownloaderProtocolID;

        /// <summary>
        /// Gets protocol ID for "ModbusPoller" adapter.
        /// </summary>
        public int GetModbusProtocolID() => ModbusProtocolID;

        [RecordOperation(typeof(Device), RecordOperation.QueryRecordCount)]
        public int QueryDeviceCount(string filterText)
        {
            return DataContext.Table<Device>().QueryRecordCount(filterText);
        }

        [RecordOperation(typeof(Device), RecordOperation.QueryRecords)]
        public IEnumerable<Device> QueryDevices(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            return DataContext.Table<Device>().QueryRecords(sortField, ascending, page, pageSize, filterText);
        }

        public IEnumerable<Device> QueryEnabledDevices(int limit, string filterText)
        {
            TableOperations<Device> deviceTable = DataContext.Table<Device>();
            return deviceTable.QueryRecords("Acronym", "Enabled <> 0" + deviceTable.GetSearchRestriction(filterText), limit);
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
            DataContext.Table<Device>().DeleteRecord(id);
            DataContext.Table<StatusLog>().DeleteRecordWhere("DeviceID = {0}", id);
            DataContext.Table<DownloadedFile>().DeleteRecordWhere("DeviceID = {0}", id);
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

            if (string.IsNullOrWhiteSpace(device.OriginalSource))
                device.OriginalSource = device.Acronym;

            DataContext.Table<Device>().AddNewRecord(device);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Device), RecordOperation.UpdateRecord)]
        public void UpdateDevice(Device device)
        {
            if ((device.ProtocolID ?? 0) == 0)
                device.ProtocolID = DownloaderProtocolID;

            if (string.IsNullOrWhiteSpace(device.OriginalSource))
                device.OriginalSource = device.Acronym;

            DataContext.Table<Device>().UpdateRecord(device);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        public void AddNewOrUpdateDevice(Device device)
        {
            DataContext.Table<Device>().AddNewOrUpdateRecord(device);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        public void SaveDeviceConfiguration(string acronym, string configuration)
        {
            File.WriteAllText(GetConfigurationCacheFileName(acronym), configuration);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        public string LoadDeviceConfiguration(string acronym)
        {
            string fileName = GetConfigurationCacheFileName(acronym);
            return File.Exists(fileName) ? File.ReadAllText(fileName) : "";
        }

        [AuthorizeHubRole("Administrator, Editor")]
        public string GetConfigurationCacheFileName(string acronym)
        {
            return Path.Combine(ConfigurationCachePath, $"{acronym}.configuration.json");
        }

        [AuthorizeHubRole("Administrator, Editor")]
        public string GetConfigurationCachePath()
        {
            return ConfigurationCachePath;
        }

        private static string ConfigurationCachePath
        {
            get
            {
                // This property will not change during system life-cycle so we cache if for future use
                if (string.IsNullOrEmpty(s_configurationCachePath))
                {
                    // Define default configuration cache directory relative to path of host application
                    s_configurationCachePath = string.Format("{0}{1}ConfigurationCache{1}", FilePath.GetAbsolutePath(""), Path.DirectorySeparatorChar);

                    // Make sure configuration cache path setting exists within system settings section of config file
                    ConfigurationFile configFile = ConfigurationFile.Current;
                    CategorizedSettingsElementCollection systemSettings = configFile.Settings["systemSettings"];
                    systemSettings.Add("ConfigurationCachePath", s_configurationCachePath, "Defines the path used to cache serialized phasor protocol configurations");

                    // Retrieve configuration cache directory as defined in the config file
                    s_configurationCachePath = FilePath.AddPathSuffix(systemSettings["ConfigurationCachePath"].Value);

                    // Make sure configuration cache directory exists
                    if (!Directory.Exists(s_configurationCachePath))
                        Directory.CreateDirectory(s_configurationCachePath);
                }

                return s_configurationCachePath;
            }
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
            return connectionProfileTaskTable.QueryRecordCount(new RecordRestriction("ConnectionProfileID = {0}", parentID) + connectionProfileTaskTable.GetSearchRestriction(filterText));
        }

        [RecordOperation(typeof(ConnectionProfileTask), RecordOperation.QueryRecords)]
        public IEnumerable<ConnectionProfileTask> QueryConnectionProfileTasks(int parentID, string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<ConnectionProfileTask> connectionProfileTaskTable = DataContext.Table<ConnectionProfileTask>();
            return connectionProfileTaskTable.QueryRecords(sortField, ascending, page, pageSize, new RecordRestriction("ConnectionProfileID = {0}", parentID) + connectionProfileTaskTable.GetSearchRestriction(filterText));
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

        #region [ Company Table Operations ]

        [RecordOperation(typeof(Company), RecordOperation.QueryRecordCount)]
        public int QueryCompanyCount(string filterText)
        {
            return DataContext.Table<Company>().QueryRecordCount(filterText);
        }

        [RecordOperation(typeof(Company), RecordOperation.QueryRecords)]
        public IEnumerable<Company> QueryCompanies(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            return DataContext.Table<Company>().QueryRecords(sortField, ascending, page, pageSize, filterText);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Company), RecordOperation.DeleteRecord)]
        public void DeleteCompany(int id)
        {
            DataContext.Table<Company>().DeleteRecord(id);
        }

        [RecordOperation(typeof(Company), RecordOperation.CreateNewRecord)]
        public Company NewCompany()
        {
            return DataContext.Table<Company>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Company), RecordOperation.AddNewRecord)]
        public void AddNewCompany(Company company)
        {
            DataContext.Table<Company>().AddNewRecord(company);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Company), RecordOperation.UpdateRecord)]
        public void UpdateCompany(Company company)
        {
            DataContext.Table<Company>().UpdateRecord(company);
        }

        #endregion

        #region [ Vendor Table Operations ]

        [RecordOperation(typeof(Vendor), RecordOperation.QueryRecordCount)]
        public int QueryVendorCount(string filterText)
        {
            return DataContext.Table<Vendor>().QueryRecordCount(filterText);
        }

        [RecordOperation(typeof(Vendor), RecordOperation.QueryRecords)]
        public IEnumerable<Vendor> QueryVendors(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            return DataContext.Table<Vendor>().QueryRecords(sortField, ascending, page, pageSize, filterText);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Vendor), RecordOperation.DeleteRecord)]
        public void DeleteVendor(int id)
        {
            DataContext.Table<Vendor>().DeleteRecord(id);
        }

        [RecordOperation(typeof(Vendor), RecordOperation.CreateNewRecord)]
        public Vendor NewVendor()
        {
            return DataContext.Table<Vendor>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Vendor), RecordOperation.AddNewRecord)]
        public void AddNewVendor(Vendor vendor)
        {
            DataContext.Table<Vendor>().AddNewRecord(vendor);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Vendor), RecordOperation.UpdateRecord)]
        public void UpdateVendor(Vendor vendor)
        {
            DataContext.Table<Vendor>().UpdateRecord(vendor);
        }

        #endregion

        #region [ VendorDevice Table Operations ]

        [RecordOperation(typeof(VendorDevice), RecordOperation.QueryRecordCount)]
        public int QueryVendorDeviceCount(string filterText)
        {
            return DataContext.Table<VendorDevice>().QueryRecordCount(filterText);
        }

        [RecordOperation(typeof(VendorDevice), RecordOperation.QueryRecords)]
        public IEnumerable<VendorDevice> QueryVendorDevices(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            return DataContext.Table<VendorDevice>().QueryRecords(sortField, ascending, page, pageSize, filterText);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(VendorDevice), RecordOperation.DeleteRecord)]
        public void DeleteVendorDevice(int id)
        {
            DataContext.Table<VendorDevice>().DeleteRecord(id);
        }

        [RecordOperation(typeof(VendorDevice), RecordOperation.CreateNewRecord)]
        public VendorDevice NewVendorDevice()
        {
            return DataContext.Table<VendorDevice>().NewRecord();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(VendorDevice), RecordOperation.AddNewRecord)]
        public void AddNewVendorDevice(VendorDevice vendorDevice)
        {
            DataContext.Table<VendorDevice>().AddNewRecord(vendorDevice);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(VendorDevice), RecordOperation.UpdateRecord)]
        public void UpdateVendorDevice(VendorDevice vendorDevice)
        {
            DataContext.Table<VendorDevice>().UpdateRecord(vendorDevice);
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

        #region [ Modbus Operations ]

        public Task<bool> ModbusConnect(string connectionString)
        {
            return m_modbusOperations.ModbusConnect(connectionString);
        }

        public void ModbusDisconnect()
        {
            m_modbusOperations.ModbusDisconnect();
        }

        public async Task<bool[]> ReadDiscreteInputs(ushort startAddress, ushort pointCount)
        {
            try
            {
                return await m_modbusOperations.ReadDiscreteInputs(startAddress, pointCount);
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Exception while reading discrete inputs starting @ {startAddress}: {ex.Message}", ex));
                return new bool[0];
            }
        }

        public async Task<bool[]> ReadCoils(ushort startAddress, ushort pointCount)
        {
            try
            {
                return await m_modbusOperations.ReadCoils(startAddress, pointCount);
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Exception while reading coil values starting @ {startAddress}: {ex.Message}", ex));
                return new bool[0];
            }
        }

        public async Task<ushort[]> ReadInputRegisters(ushort startAddress, ushort pointCount)
        {
            try
            {
                return await m_modbusOperations.ReadInputRegisters(startAddress, pointCount);
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Exception while reading input registers starting @ {startAddress}: {ex.Message}", ex));
                return new ushort[0];
            }
        }

        public async Task<ushort[]> ReadHoldingRegisters(ushort startAddress, ushort pointCount)
        {
            try
            {
                return await m_modbusOperations.ReadHoldingRegisters(startAddress, pointCount);
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Exception while reading holding registers starting @ {startAddress}: {ex.Message}", ex));
                return new ushort[0];
            }
        }

        public async Task WriteCoils(ushort startAddress, bool[] data)
        {
            try
            {
                await m_modbusOperations.WriteCoils(startAddress, data);
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Exception while writing coil values starting @ {startAddress}: {ex.Message}", ex));
            }
        }

        public async Task WriteHoldingRegisters(ushort startAddress, ushort[] data)
        {
            try
            {
                await m_modbusOperations.WriteHoldingRegisters(startAddress, data);
            }
            catch (Exception ex)
            {
                LogException(new InvalidOperationException($"Exception while writing holding registers starting @ {startAddress}: {ex.Message}", ex));
            }
        }

        public string DeriveString(ushort[] values)
        {
            return m_modbusOperations.DeriveString(values);
        }

        public float DeriveSingle(ushort highValue, ushort lowValue)
        {
            return m_modbusOperations.DeriveSingle(highValue, lowValue);
        }

        public double DeriveDouble(ushort b3, ushort b2, ushort b1, ushort b0)
        {
            return m_modbusOperations.DeriveDouble(b3, b2, b1, b0);
        }

        public int DeriveInt32(ushort highValue, ushort lowValue)
        {
            return m_modbusOperations.DeriveInt32(highValue, lowValue);
        }

        public uint DeriveUInt32(ushort highValue, ushort lowValue)
        {
            return m_modbusOperations.DeriveUInt32(highValue, lowValue);
        }

        public long DeriveInt64(ushort b3, ushort b2, ushort b1, ushort b0)
        {
            return m_modbusOperations.DeriveInt64(b3, b2, b1, b0);
        }

        public ulong DeriveUInt64(ushort b3, ushort b2, ushort b1, ushort b0)
        {
            return m_modbusOperations.DeriveUInt64(b3, b2, b1, b0);
        }

        #endregion

        #region [ I-Grid Operations ] 

        public const string DefaultIGridConnectionProfileName = "I-Grid Connection Profile";
        public const string DefaultIGridConnectionProfileTaskQueueName = "I-Grid Connection Profile Task Queue";

        public int GetDefaultIGridProfileID() => DataContext.Connection.ExecuteScalar<int?>("SELECT ID FROM ConnectionProfile WHERE Name={0}", DefaultIGridConnectionProfileName) ?? 0;

        public IEnumerable<IGridDevice> QueryIGridDevices(string baseURL)
        {
            TableOperations<Device> deviceTable = DataContext.Table<Device>();
            XDocument document = XDocument.Load($"{baseURL}&action=getMonitorList");

            foreach (XElement monitor in document.Descendants("monitor"))
            {
                decimal longitude, latitude;
                XElement location = monitor.Element("location");
                XElement identification = monitor.Element("identification");
                XElement model = monitor.Element("model");

                string serialNumber = identification?.Element("serialNumber")?.Value;
                string modelNumber = model?.Element("number")?.Value;
                string monitorName = identification?.Element("monitorName")?.Value;

                if (string.IsNullOrWhiteSpace(serialNumber) || string.IsNullOrWhiteSpace(modelNumber))
                    continue;

                Device deviceRecord = deviceTable.QueryRecordWhere("OriginalSource = {0}", serialNumber);

                if ((object)deviceRecord == null)
                {
                    deviceRecord = deviceTable.NewRecord();

                    string acronym = monitorName ?? modelNumber;

                    // Get a clean acronym
                    acronym = acronym
                        .ToUpperInvariant()
                        .ReplaceCharacters(' ', s_isInvalidAcronymChar)
                        .RemoveDuplicateWhiteSpace();

                    // Truncate at any numbers in the acronym, note this is based on sample data naming convention
                    int index = acronym.IndexOfAny(s_digits);

                    if (index > 0)
                        acronym = acronym.Substring(0, index);

                    acronym = acronym.Trim().ReplaceWhiteSpace('-');

                    deviceRecord.Acronym = $"{acronym}${serialNumber}";
                }

                decimal.TryParse(location?.Element("longitude")?.Value, out longitude);
                decimal.TryParse(location?.Element("latitude")?.Value, out latitude);

                yield return new IGridDevice
                {
                    DeviceID = deviceRecord.ID,
                    Acronym = deviceRecord.Acronym,
                    SerialNumber = serialNumber,
                    Name = monitorName,
                    Description = model.Element("description")?.Value,
                    ModelNumber = modelNumber,
                    Longitude = longitude,
                    Latitude = latitude,
                    Selected = deviceRecord.ID == 0
                };
            }
        }

        public int GetDefaultIGridConnectionProfileID()
        {
            TableOperations<ConnectionProfileTaskQueue> profileTaskQueueTable = DataContext.Table<ConnectionProfileTaskQueue>();
            TableOperations<ConnectionProfile> profileTable = DataContext.Table<ConnectionProfile>();
            ConnectionProfile profile = profileTable.QueryRecordWhere("Name = {0}", DefaultIGridConnectionProfileName);

            if ((object)profile == null)
            {
                ConnectionProfileTaskQueue profileTaskQueue = profileTaskQueueTable.NewRecord();
                profileTaskQueue.Name = DefaultIGridConnectionProfileTaskQueueName;
                profileTaskQueue.MaxThreadCount = 1;
                profileTaskQueue.UseBackgroundThreads = false;
                profileTaskQueue.Description = "Connection Profile Task for I-Grid Devices";
                profileTaskQueueTable.AddNewRecord(profileTaskQueue);
                profileTaskQueue = profileTaskQueueTable.QueryRecordWhere("Name = {0}", DefaultIGridConnectionProfileTaskQueueName);

                profile = profileTable.NewRecord();
                profile.Name = DefaultIGridConnectionProfileName;
                profile.Description = "Connection Profile for I-Grid Devices";
                profile.DefaultTaskQueueID = profileTaskQueue.ID;
                profileTable.AddNewRecord(profile);
                profile.ID = GetDefaultIGridConnectionProfileID();

                TableOperations<ConnectionProfileTask> profileTaskTable = DataContext.Table<ConnectionProfileTask>();
                int taskCount = profileTaskTable.QueryRecordCountWhere("ConnectionProfileID = {0}", profile.ID);

                if (taskCount == 0)
                {
                    ConnectionProfileTask profileTask = profileTaskTable.NewRecord();
                    ConnectionProfileTaskSettings profileTaskSettings = profileTask.Settings;

                    profileTask.ConnectionProfileID = profile.ID;
                    profileTask.Name = "I-Grid Default Downloader Task";

                    profileTaskSettings.FileExtensions = "*.*";
                    profileTaskSettings.RemotePath = "/";
                    profileTaskSettings.LocalPath = Program.Host.Model.Global.DefaultLocalPath;
                    profileTaskSettings.DeleteOldLocalFiles = true;
                    profileTaskSettings.SkipDownloadIfUnchanged = true;
                    profileTaskSettings.OverwriteExistingLocalFiles = false;
                    profileTaskSettings.ArchiveExistingFilesBeforeDownload = false;
                    profileTaskSettings.SynchronizeTimestamps = true;
                    profileTaskSettings.ExternalOperation = "IGridDownloader.exe <DeviceID> <TaskID>";
                    profileTaskSettings.DirectoryNamingExpression = @"<YYYY><MM>\<DeviceFolderName>";

                    profileTaskTable.AddNewRecord(profileTask);
                }
            }

            return profile.ID;
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
        /// Requests that the device send the current list of progress updates.
        /// </summary>
        public void QueryDeviceStatus()
        {
            foreach (Downloader downloader in Program.Host.Downloaders)
                downloader.SendAllProgressUpdates(ConnectionID);
        }

        /// <summary>
        /// Gets current user ID.
        /// </summary>
        /// <returns>Current user ID.</returns>
        public string GetCurrentUserID()
        {
            return Thread.CurrentPrincipal.Identity?.Name ?? UserInfo.CurrentUserID;
        }

        #endregion
    }
}
