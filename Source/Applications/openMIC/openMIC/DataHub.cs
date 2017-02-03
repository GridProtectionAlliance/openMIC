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
using GSF;
using GSF.Configuration;
using GSF.Data.Model;
using GSF.Identity;
using GSF.IO;
using GSF.Web.Hubs;
using GSF.Web.Model.HubOperations;
using GSF.Web.Security;
using Microsoft.AspNet.SignalR;
using openMIC.Model;
using RecordRestriction = GSF.Data.Model.RecordRestriction;

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

        // Static Properties

        // Static Fields
        private static int s_downloaderProtocolID;
        private static int s_modbusProtocolID;
        private static string s_configurationCachePath;

        // Static Constructor
        static DataHub()
        {
            Downloader.ProgressUpdated += ProgressUpdated;
            ModbusPoller.ProgressUpdated += ProgressUpdated;
        }

        private static void ProgressUpdated(object sender, EventArgs<ProgressUpdate> e)
        {
            ProgressUpdate update = e.Argument;

            Downloader downloader = sender as Downloader;

            if ((object)downloader != null)
            {
                update.DeviceName = downloader.Name;
                update.FilesDownloaded = downloader.FilesDownloaded;
            }

            ModbusPoller modbusPoller = sender as ModbusPoller;

            if ((object)modbusPoller != null)
            {
                update.DeviceName = modbusPoller.Name;
                update.ValuesProcessed = modbusPoller.MeasurementsReceived;
            }

            if (!string.IsNullOrEmpty(update.ProgressMessage))
                update.ProgressMessage += $"\r\n\r\n[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}]";
            
            GlobalHost.ConnectionManager.GetHubContext<DataHub>().Clients.All.deviceProgressUpdate(e.Argument);
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
            TableOperations<Device> tableOperations = DataContext.Table<Device>();
            RecordRestriction restriction = tableOperations.GetSearchRestriction(filterText);
            return tableOperations.QueryRecordCount(restriction);
        }

        [RecordOperation(typeof(Device), RecordOperation.QueryRecords)]
        public IEnumerable<Device> QueryDevices(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<Device> tableOperations = DataContext.Table<Device>();
            RecordRestriction restriction = tableOperations.GetSearchRestriction(filterText);
            return tableOperations.QueryRecords(sortField, ascending, page, pageSize, restriction);
        }

        public IEnumerable<Device> QueryEnabledDevices(int limit, string filterText)
        {
            return DataContext.Table<Device>().QueryRecords("Acronym", new RecordRestriction("Enabled <> 0 AND (Acronym LIKE {0} OR Name LIKE {0})", $"%{filterText}%"), limit);
        }

        public Device QueryDevice(string acronym)
        {
            return DataContext.Table<Device>().QueryRecords("Acronym", new RecordRestriction("Acronym = {0}", acronym)).FirstOrDefault() ?? NewDevice();
        }

        public Device QueryDeviceByID(int deviceID)
        {
            return DataContext.Table<Device>().QueryRecords("ID", new RecordRestriction("ID = {0}", deviceID)).FirstOrDefault() ?? NewDevice();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Device), RecordOperation.DeleteRecord)]
        public void DeleteDevice(int id)
        {
            DataContext.Table<Device>().DeleteRecord(id);
        }

        [RecordOperation(typeof(Device), RecordOperation.CreateNewRecord)]
        public Device NewDevice()
        {
            return new Device
            {
                FramesPerSecond = 1
            };
        }


        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Device), RecordOperation.DeleteRecord)]
        public void DisableDevice(int id, bool truth)
        {
            DataContext.Connection.ExecuteNonQuery("Update Device SET Enabled = {0} WHERE ID = {1}", truth, id);
        }


        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Device), RecordOperation.AddNewRecord)]
        public void AddNewDevice(Device device)
        {
            //device.NodeID = Program.Host.Model.Global.NodeID;
            device.UniqueID = Guid.NewGuid();

            if ((device.ProtocolID ?? 0) == 0)
                device.ProtocolID = DownloaderProtocolID;

            device.CreatedBy = GetCurrentUserID();
            device.CreatedOn = DateTime.UtcNow;
            device.UpdatedBy = device.CreatedBy;
            device.UpdatedOn = device.CreatedOn;

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

            device.UpdatedBy = GetCurrentUserID();
            device.UpdatedOn = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(device.OriginalSource))
                device.OriginalSource = device.Acronym;

            DataContext.Table<Device>().UpdateRecord(device);
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
            TableOperations<Measurement> tableOperations = DataContext.Table<Measurement>();
            RecordRestriction restriction = tableOperations.GetSearchRestriction(filterText);
            return tableOperations.QueryRecordCount(restriction);
        }

        [RecordOperation(typeof(Measurement), RecordOperation.QueryRecords)]
        public IEnumerable<Measurement> QueryMeasurements(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<Measurement> tableOperations = DataContext.Table<Measurement>();
            RecordRestriction restriction = tableOperations.GetSearchRestriction(filterText);
            return tableOperations.QueryRecords(sortField, ascending, page, pageSize, restriction);
        }

        public Measurement QueryMeasurement(string signalReference)
        {
            return DataContext.Table<Measurement>().QueryRecords("SignalReference", new RecordRestriction("SignalReference = {0}", signalReference)).FirstOrDefault() ?? NewMeasurement();
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
            return new Measurement
            {
                Adder = 0.0D,
                Multiplier = 1.0D
            };
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Measurement), RecordOperation.AddNewRecord)]
        public void AddNewMeasurement(Measurement measurement)
        {
            measurement.SignalID = Guid.NewGuid();
            measurement.CreatedBy = GetCurrentUserID();
            measurement.CreatedOn = DateTime.UtcNow;
            measurement.UpdatedBy = measurement.CreatedBy;
            measurement.UpdatedOn = measurement.CreatedOn;

            DataContext.Table<Measurement>().AddNewRecord(measurement);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Measurement), RecordOperation.UpdateRecord)]
        public void UpdateMeasurement(Measurement measurement)
        {
            measurement.UpdatedBy = GetCurrentUserID();
            measurement.UpdatedOn = DateTime.UtcNow;

            DataContext.Table<Measurement>().UpdateRecord(measurement);
        }

        #endregion

        #region [ ConnectionProfile Table Operations ]

        [RecordOperation(typeof(ConnectionProfile), RecordOperation.QueryRecordCount)]
        public int QueryConnectionProfileCount(string filterText)
        {
            return DataContext.Table<ConnectionProfile>().QueryRecordCount();
        }

        [RecordOperation(typeof(ConnectionProfile), RecordOperation.QueryRecords)]
        public IEnumerable<ConnectionProfile> QueryConnectionProfiles(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            return DataContext.Table<ConnectionProfile>().QueryRecords(sortField, ascending, page, pageSize);
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
            return new ConnectionProfile();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(ConnectionProfile), RecordOperation.AddNewRecord)]
        public void AddNewConnectionProfile(ConnectionProfile connectionProfile)
        {
            connectionProfile.CreatedBy = GetCurrentUserID();
            connectionProfile.CreatedOn = DateTime.UtcNow;
            connectionProfile.UpdatedBy = connectionProfile.CreatedBy;
            connectionProfile.UpdatedOn = connectionProfile.CreatedOn;

            DataContext.Table<ConnectionProfile>().AddNewRecord(connectionProfile);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(ConnectionProfile), RecordOperation.UpdateRecord)]
        public void UpdateConnectionProfile(ConnectionProfile connectionProfile)
        {
            connectionProfile.UpdatedBy = GetCurrentUserID();
            connectionProfile.UpdatedOn = DateTime.UtcNow;

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
            return DataContext.Table<ConnectionProfileTask>().QueryRecordCount(new RecordRestriction
            {
                FilterExpression = "ConnectionProfileID = {0}",
                Parameters = new object[] { parentID }
            });
        }

        [RecordOperation(typeof(ConnectionProfileTask), RecordOperation.QueryRecords)]
        public IEnumerable<ConnectionProfileTask> QueryConnectionProfileTasks(int parentID, string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            return DataContext.Table<ConnectionProfileTask>().QueryRecords(sortField, ascending, page, pageSize, new RecordRestriction
            {
                FilterExpression = "ConnectionProfileID = {0}",
                Parameters = new object[] { parentID }
            });
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
            return new ConnectionProfileTask();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(ConnectionProfileTask), RecordOperation.AddNewRecord)]
        public void AddNewConnectionProfileTask(ConnectionProfileTask connectionProfileTask)
        {
            connectionProfileTask.CreatedBy = GetCurrentUserID();
            connectionProfileTask.CreatedOn = DateTime.UtcNow;
            connectionProfileTask.UpdatedBy = connectionProfileTask.CreatedBy;
            connectionProfileTask.UpdatedOn = connectionProfileTask.CreatedOn;

            DataContext.Table<ConnectionProfileTask>().AddNewRecord(connectionProfileTask);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(ConnectionProfileTask), RecordOperation.UpdateRecord)]
        public void UpdateConnectionProfileTask(ConnectionProfileTask connectionProfileTask)
        {
            connectionProfileTask.UpdatedBy = GetCurrentUserID();
            connectionProfileTask.UpdatedOn = DateTime.UtcNow;

            DataContext.Table<ConnectionProfileTask>().UpdateRecord(connectionProfileTask);
        }

        #endregion

        #region [ Company Table Operations ]

        [RecordOperation(typeof(Company), RecordOperation.QueryRecordCount)]
        public int QueryCompanyCount(string filterText)
        {
            TableOperations<Company> tableOperations = DataContext.Table<Company>();
            RecordRestriction restriction = tableOperations.GetSearchRestriction(filterText);
            return tableOperations.QueryRecordCount(restriction);
        }

        [RecordOperation(typeof(Company), RecordOperation.QueryRecords)]
        public IEnumerable<Company> QueryCompanies(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<Company> tableOperations = DataContext.Table<Company>();
            RecordRestriction restriction = tableOperations.GetSearchRestriction(filterText);
            return tableOperations.QueryRecords(sortField, ascending, page, pageSize, restriction);
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
            return new Company();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Company), RecordOperation.AddNewRecord)]
        public void AddNewCompany(Company company)
        {
            company.CreatedBy = GetCurrentUserID();
            company.CreatedOn = DateTime.UtcNow;
            company.UpdatedBy = company.CreatedBy;
            company.UpdatedOn = company.CreatedOn;

            DataContext.Table<Company>().AddNewRecord(company);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Company), RecordOperation.UpdateRecord)]
        public void UpdateCompany(Company company)
        {
            company.UpdatedBy = GetCurrentUserID();
            company.UpdatedOn = DateTime.UtcNow;

            DataContext.Table<Company>().UpdateRecord(company);
        }

        #endregion

        #region [ Vendor Table Operations ]

        [RecordOperation(typeof(Vendor), RecordOperation.QueryRecordCount)]
        public int QueryVendorCount(string filterText)
        {
            TableOperations<Vendor> tableOperations = DataContext.Table<Vendor>();
            RecordRestriction restriction = tableOperations.GetSearchRestriction(filterText);
            return tableOperations.QueryRecordCount(restriction);
        }

        [RecordOperation(typeof(Vendor), RecordOperation.QueryRecords)]
        public IEnumerable<Vendor> QueryVendors(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<Vendor> tableOperations = DataContext.Table<Vendor>();
            RecordRestriction restriction = tableOperations.GetSearchRestriction(filterText);
            return tableOperations.QueryRecords(sortField, ascending, page, pageSize, restriction);
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
            return new Vendor();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Vendor), RecordOperation.AddNewRecord)]
        public void AddNewVendor(Vendor vendor)
        {
            vendor.CreatedBy = GetCurrentUserID();
            vendor.CreatedOn = DateTime.UtcNow;
            vendor.UpdatedBy = vendor.CreatedBy;
            vendor.UpdatedOn = vendor.CreatedOn;

            DataContext.Table<Vendor>().AddNewRecord(vendor);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(Vendor), RecordOperation.UpdateRecord)]
        public void UpdateVendor(Vendor vendor)
        {
            vendor.UpdatedBy = GetCurrentUserID();
            vendor.UpdatedOn = DateTime.UtcNow;

            DataContext.Table<Vendor>().UpdateRecord(vendor);
        }

        #endregion

        #region [ VendorDevice Table Operations ]

        [RecordOperation(typeof(VendorDevice), RecordOperation.QueryRecordCount)]
        public int QueryVendorDeviceCount(string filterText)
        {
            TableOperations<VendorDevice> tableOperations = DataContext.Table<VendorDevice>();
            RecordRestriction restriction = tableOperations.GetSearchRestriction(filterText);
            return tableOperations.QueryRecordCount(restriction);
        }

        [RecordOperation(typeof(VendorDevice), RecordOperation.QueryRecords)]
        public IEnumerable<VendorDevice> QueryVendorDevices(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<VendorDevice> tableOperations = DataContext.Table<VendorDevice>();
            RecordRestriction restriction = tableOperations.GetSearchRestriction(filterText);
            return tableOperations.QueryRecords(sortField, ascending, page, pageSize, restriction);
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
            return new VendorDevice();
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(VendorDevice), RecordOperation.AddNewRecord)]
        public void AddNewVendorDevice(VendorDevice vendorDevice)
        {
            vendorDevice.CreatedBy = GetCurrentUserID();
            vendorDevice.CreatedOn = DateTime.UtcNow;
            vendorDevice.UpdatedBy = vendorDevice.CreatedBy;
            vendorDevice.UpdatedOn = vendorDevice.CreatedOn;

            DataContext.Table<VendorDevice>().AddNewRecord(vendorDevice);
        }

        [AuthorizeHubRole("Administrator, Editor")]
        [RecordOperation(typeof(VendorDevice), RecordOperation.UpdateRecord)]
        public void UpdateVendorDevice(VendorDevice vendorDevice)
        {
            vendorDevice.UpdatedBy = GetCurrentUserID();
            vendorDevice.UpdatedOn = DateTime.UtcNow;

            DataContext.Table<VendorDevice>().UpdateRecord(vendorDevice);
        }

        #endregion

        #region [ SignalType Table Operations ]

        [RecordOperation(typeof(SignalType), RecordOperation.QueryRecordCount)]
        public int QuerySignalTypeCount(string filterText)
        {
            TableOperations<SignalType> tableOperations = DataContext.Table<SignalType>();
            RecordRestriction restriction = tableOperations.GetSearchRestriction(filterText);
            return tableOperations.QueryRecordCount(restriction);
        }

        [RecordOperation(typeof(SignalType), RecordOperation.QueryRecords)]
        public IEnumerable<SignalType> QuerySignalTypes(string sortField, bool ascending, int page, int pageSize, string filterText)
        {
            TableOperations<SignalType> tableOperations = DataContext.Table<SignalType>();
            RecordRestriction restriction = tableOperations.GetSearchRestriction(filterText);
            return tableOperations.QueryRecords(sortField, ascending, page, pageSize, restriction);
        }

        public SignalType QuerySignalType(string acronym)
        {
            return DataContext.Table<SignalType>().QueryRecords("Acronym", new RecordRestriction("Acronym = {0}", acronym)).FirstOrDefault();
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
            return new SignalType();
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

        #region [StatusLog Operations]

        public StatusLog GetStatusLogForDevice(string deviceName)
        {
            IEnumerable<StatusLog> logs = DataContext.Table<StatusLog>().QueryRecords(restriction: new RecordRestriction("DeviceID IN (SELECT ID FROM Device WHERE Acronym LIKE {0})",deviceName));

            if (logs.Any())
                return logs.First();
            else
                return new StatusLog();
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
