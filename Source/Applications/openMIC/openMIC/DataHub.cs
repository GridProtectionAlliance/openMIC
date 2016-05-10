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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using GSF;
using GSF.Data.Model;
using GSF.Identity;
using GSF.Web.Model;
using GSF.Web.Security;
using Microsoft.AspNet.SignalR;
using openMIC.Model;
using RecordRestriction = GSF.Data.Model.RecordRestriction;

namespace openMIC
{
    [AuthorizeHubRole]
    public class DataHub : Hub, IRecordOperationsHub
    {
        #region [ Members ]

        // Fields
        private readonly DataContext m_dataContext;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        public DataHub()
        {
            m_dataContext = new DataContext();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets <see cref="IRecordOperationsHub.RecordOperationsCache"/> for SignalR hub.
        /// </summary>
        public RecordOperationsCache RecordOperationsCache => s_recordOperationsCache;

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="DataHub"/> object and optionally releases the managed resources.
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
                        m_dataContext?.Dispose();
                    }
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
                }
            }
        }

        public override Task OnConnected()
        {
            s_connectCount++;
            Program.Host.LogStatusMessage($"DataHub connect by {Context.User?.Identity?.Name ?? "Undefined User"} [{Context.ConnectionId}] - count = {s_connectCount}");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled)
            {
                s_connectCount--;
                Program.Host.LogStatusMessage($"DataHub disconnect by {Context.User?.Identity?.Name ?? "Undefined User"} [{Context.ConnectionId}] - count = {s_connectCount}");
            }

            return base.OnDisconnected(stopCalled);
        }

        #endregion

        #region [ Static ]

        // Static Properties

        /// <summary>
        /// Gets the hub connection ID for the current thread.
        /// </summary>
        public static string CurrentConnectionID => s_connectionID.Value;

        // Static Fields
        private static volatile int s_connectCount;
        private static int s_downloaderProtocolID;
        private static readonly ThreadLocal<string> s_connectionID = new ThreadLocal<string>();
        private static readonly RecordOperationsCache s_recordOperationsCache;

        // Static Methods

        /// <summary>
        /// Gets statically cached instance of <see cref="RecordOperationsCache"/> for <see cref="DataHub"/> instances.
        /// </summary>
        /// <returns>Statically cached instance of <see cref="RecordOperationsCache"/> for <see cref="DataHub"/> instances.</returns>
        public static RecordOperationsCache GetRecordOperationsCache() => s_recordOperationsCache;

        // Static Constructor
        static DataHub()
        {
            // Analyze and cache record operations of security hub
            s_recordOperationsCache = new RecordOperationsCache(typeof(DataHub));
        }

        #endregion

        // Client-side script functionality

        #region [ Device Table Operations ]

        /// <summary>
        /// Gets protocol ID for "Downloader" adapter.
        /// </summary>
        public int DownloaderProtocolID => s_downloaderProtocolID != 0 ? s_downloaderProtocolID : (s_downloaderProtocolID = m_dataContext.Connection.ExecuteScalar<int>("SELECT ID FROM Protocol WHERE Acronym='Downloader'"));

        public IEnumerable<Device> QueryDevices(string sortField, bool ascending, int page, int pageSize)
        {
            return m_dataContext.Table<Device>().QueryRecords(sortField, ascending, page, pageSize);
        }

        public int QueryDeviceCount()
        {
            return m_dataContext.Table<Device>().QueryRecordCount();
        }

        public void DeleteDevice(int id)
        {
            m_dataContext.Table<Device>().DeleteRecord(id);
        }

        public Device NewDevice()
        {
            return new Device();
        }

        public void AddNewDevice(Device device)
        {
            device.NodeID = Program.Host.Model.NodeID;
            device.ProtocolID = DownloaderProtocolID;
            device.CreatedBy = UserInfo.CurrentUserID;
            device.CreatedOn = DateTime.UtcNow;
            device.UpdatedBy = device.CreatedBy;
            device.UpdatedOn = device.CreatedOn;

            m_dataContext.Table<Device>().AddNewRecord(device);
        }

        public void UpdateDevice(Device device)
        {
            device.ProtocolID = DownloaderProtocolID;
            m_dataContext.Table<Device>().UpdateRecord(device);
        }

        #endregion

        #region [ ConnectionProfile Table Operations ]

        public IEnumerable<ConnectionProfile> QueryConnectionProfiles(string sortField, bool ascending, int page, int pageSize)
        {
            return m_dataContext.Table<ConnectionProfile>().QueryRecords(sortField, ascending, page, pageSize);
        }

        public int QueryConnectionProfileCount()
        {
            return m_dataContext.Table<ConnectionProfile>().QueryRecordCount();
        }

        public void DeleteConnectionProfile(int id)
        {
            m_dataContext.Table<ConnectionProfile>().DeleteRecord(id);
        }

        public ConnectionProfile NewConnectionProfile()
        {
            return new ConnectionProfile();
        }

        public void AddNewConnectionProfile(ConnectionProfile connectionProfile)
        {
            connectionProfile.CreatedBy = UserInfo.CurrentUserID;
            connectionProfile.CreatedOn = DateTime.UtcNow;
            connectionProfile.UpdatedBy = connectionProfile.CreatedBy;
            connectionProfile.UpdatedOn = connectionProfile.CreatedOn;

            m_dataContext.Table<ConnectionProfile>().AddNewRecord(connectionProfile);
        }

        public void UpdateConnectionProfile(ConnectionProfile connectionProfile)
        {
            m_dataContext.Table<ConnectionProfile>().UpdateRecord(connectionProfile);
        }

        #endregion

        #region [ ConnectionProfileTask Table Operations ]

        public IEnumerable<ConnectionProfileTask> QueryConnectionProfileTasks(int parentID, string sortField, bool ascending, int page, int pageSize)
        {
            return m_dataContext.Table<ConnectionProfileTask>().QueryRecords(sortField, ascending, page, pageSize, new RecordRestriction
            {
                FilterExpression = "ConnectionProfileID = {0}",
                Parameters = new object[] { parentID }
            });
        }

        public int QueryConnectionProfileTaskCount(int parentID)
        {
            return m_dataContext.Table<ConnectionProfileTask>().QueryRecordCount(new RecordRestriction
            {
                FilterExpression = "ConnectionProfileID = {0}",
                Parameters = new object[] { parentID }
            });
        }

        public void DeleteConnectionProfileTask(int id)
        {
            m_dataContext.Table<ConnectionProfileTask>().DeleteRecord(id);
        }

        public ConnectionProfileTask NewConnectionProfileTask()
        {
            return new ConnectionProfileTask();
        }

        public void AddNewConnectionProfileTask(ConnectionProfileTask connectionProfileTask)
        {
            connectionProfileTask.CreatedBy = UserInfo.CurrentUserID;
            connectionProfileTask.CreatedOn = DateTime.UtcNow;
            connectionProfileTask.UpdatedBy = connectionProfileTask.CreatedBy;
            connectionProfileTask.UpdatedOn = connectionProfileTask.CreatedOn;

            m_dataContext.Table<ConnectionProfileTask>().AddNewRecord(connectionProfileTask);
        }

        public void UpdateConnectionProfileTask(ConnectionProfileTask connectionProfileTask)
        {
            m_dataContext.Table<ConnectionProfileTask>().UpdateRecord(connectionProfileTask);
        }

        #endregion

        #region [ Company Table Operations ]

        public int QueryCompanyCount()
        {
            return m_dataContext.Table<Company>().QueryRecordCount();
        }

        public IEnumerable<Company> QueryCompanies(string sortField, bool ascending, int page, int pageSize)
        {
            return m_dataContext.Table<Company>().QueryRecords(sortField, ascending, page, pageSize);
        }

        public void DeleteCompany(int id)
        {
            m_dataContext.Table<Company>().DeleteRecord(id);
        }

        public Company NewCompany()
        {
            return new Company();
        }

        public void AddNewCompany(Company company)
        {
            company.CreatedBy = UserInfo.CurrentUserID;
            company.CreatedOn = DateTime.UtcNow;
            company.UpdatedBy = company.CreatedBy;
            company.UpdatedOn = company.CreatedOn;

            m_dataContext.Table<Company>().AddNewRecord(company);
        }

        public void UpdateCompany(Company company)
        {
            m_dataContext.Table<Company>().UpdateRecord(company);
        }

        #endregion

        #region [ Vendor Table Operations ]

        public int QueryVendorCount()
        {
            return m_dataContext.Table<Vendor>().QueryRecordCount();
        }

        public IEnumerable<Vendor> QueryVendors(string sortField, bool ascending, int page, int pageSize)
        {
            return m_dataContext.Table<Vendor>().QueryRecords(sortField, ascending, page, pageSize);
        }

        public void DeleteVendor(int id)
        {
            m_dataContext.Table<Vendor>().DeleteRecord(id);
        }

        public Vendor NewVendor()
        {
            return new Vendor();
        }

        public void AddNewVendor(Vendor vendor)
        {
            vendor.CreatedBy = UserInfo.CurrentUserID;
            vendor.CreatedOn = DateTime.UtcNow;
            vendor.UpdatedBy = vendor.CreatedBy;
            vendor.UpdatedOn = vendor.CreatedOn;

            m_dataContext.Table<Vendor>().AddNewRecord(vendor);
        }

        public void UpdateVendor(Vendor vendor)
        {
            m_dataContext.Table<Vendor>().UpdateRecord(vendor);
        }

        #endregion

        #region [ VendorDevice Table Operations ]

        public int QueryVendorDeviceCount()
        {
            return m_dataContext.Table<VendorDevice>().QueryRecordCount();
        }

        public IEnumerable<VendorDevice> QueryVendorDevices(string sortField, bool ascending, int page, int pageSize)
        {
            return m_dataContext.Table<VendorDevice>().QueryRecords(sortField, ascending, page, pageSize);
        }

        public void DeleteVendorDevice(int id)
        {
            m_dataContext.Table<VendorDevice>().DeleteRecord(id);
        }

        public VendorDevice NewVendorDevice()
        {
            return new VendorDevice();
        }

        public void AddNewVendorDevice(VendorDevice vendorDevice)
        {
            vendorDevice.CreatedBy = UserInfo.CurrentUserID;
            vendorDevice.CreatedOn = DateTime.UtcNow;
            vendorDevice.UpdatedBy = vendorDevice.CreatedBy;
            vendorDevice.UpdatedOn = vendorDevice.CreatedOn;

            m_dataContext.Table<VendorDevice>().AddNewRecord(vendorDevice);
        }

        public void UpdateVendorDevice(VendorDevice vendorDevice)
        {
            m_dataContext.Table<VendorDevice>().UpdateRecord(vendorDevice);
        }

        #endregion
    }
}
