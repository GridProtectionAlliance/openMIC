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
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GSF.Data;
using GSF.Identity;
using Microsoft.AspNet.SignalR;
using openMIC.Model;

namespace openMIC
{
    public class DataHub : Hub
    {
        #region [ Members ]

        // Fields
        private readonly AdoDataConnection m_dataConnection;
        private readonly openMICData m_dataModel;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        public DataHub()
        {
            m_dataConnection = new AdoDataConnection("systemSettings");
            m_dataModel = new openMICData();
        }

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
                        m_dataConnection?.Dispose();
                        m_dataModel?.Dispose();
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

        #region [ Device Table Operations ]

        private Sql<Device> m_devices = new Sql<Device>();

        public IEnumerable<Device> QueryDevices(string sortField, bool ascending, int page, int pageSize)
        {
            return m_devices.QueryRecords(m_dataConnection, sortField, ascending, page, pageSize);
        }

        public int QueryDeviceCount()
        {
            return Sql<Device>.QueryRecordCount(m_dataConnection);
        }

        public void DeleteDevice(int id)
        {
            Sql<Device>.DeleteRecord(m_dataConnection, id);
        }

        public Device NewDevice()
        {
            return new Device();
        }

        public void AddNewDevice(Device device)
        {
            device.NodeID = Program.Host.Model.NodeID;
            device.CreatedBy = UserInfo.CurrentUserID;
            device.CreatedOn = DateTime.UtcNow;
            device.UpdatedBy = device.CreatedBy;
            device.UpdatedOn = device.CreatedOn;

            Sql<Device>.AddNewRecord(m_dataConnection, device);
        }

        public void UpdateDevice(Device device)
        {
            Sql<Device>.UpdateRecord(m_dataConnection, device);
        }

        #endregion

        #region [ Vendor Table Operations ]

        public int QueryVendorCount()
        {
            return m_dataModel.Database.SqlQuery<int>("SELECT COUNT(*) FROM Vendor").FirstOrDefault();
        }

        public IEnumerable<Vendor> QueryVendors(string sortField, bool ascending, int page, int pageSize)
        {
            if (ascending)
                return m_dataModel.Vendors.OrderBy(sortField).ToPagedList(page, pageSize);

            return m_dataModel.Vendors.OrderByDescending(sortField).ToPagedList(page, pageSize);
        }

        public void DeleteVendor(int id)
        {
            m_dataModel.Database.ExecuteSqlCommand("DELETE FROM Vendor WHERE ID = @p0", id);
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
            m_dataModel.Vendors.Add(vendor);
            m_dataModel.SaveChanges();
        }

        public void UpdateVendor(Vendor vendor)
        {
            Vendor source = m_dataModel.Vendors.Find(vendor.ID);

            if (source != null)
            {
                vendor.CopyProperties(source);
                m_dataModel.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Record not found, cannot save update.");
            }
        }

        //public Vendor FindVendor(int id)
        //{
        //    return m_dataModel.Vendors.Find(id);
        //}

        //public IEnumerable<int> QueryVendorIDs()
        //{
        //    return m_dataModel.Database.SqlQuery<int>("SELECT ID FROM Vendor");
        //}

        //public IEnumerable<Vendor> QueryVendors()
        //{
        //    return m_dataModel.Vendors;
        //}

        //public IEnumerable<Vendor> QueryVendors(string sql)
        //{
        //    return string.IsNullOrEmpty(sql) ? QueryVendors() : QueryVendors(sql, null);
        //}

        //public IEnumerable<Vendor> QueryVendors(string sql, params object[] parameters)
        //{
        //    if (string.IsNullOrEmpty(sql))
        //        throw new ArgumentNullException(nameof(sql));

        //    if ((object)parameters == null)
        //        parameters = new object[0];

        //    return m_dataModel.Vendors.SqlQuery($"SELECT * FROM Vendor WHERE {sql}", parameters).AsNoTracking().ToArray();
        //}

        #endregion

        #region [ VendorDevice Table Operations ]

        public int QueryVendorDeviceCount()
        {
            return m_dataModel.Database.SqlQuery<int>("SELECT COUNT(*) FROM VendorDevice").FirstOrDefault();
        }

        public IEnumerable<VendorDevice> QueryVendorDevices(string sortField, bool ascending, int page, int pageSize)
        {
            if (ascending)
                return m_dataModel.VendorDevices.OrderBy(sortField).ToPagedList(page, pageSize);

            return m_dataModel.VendorDevices.OrderByDescending(sortField).ToPagedList(page, pageSize);
        }

        public void DeleteVendorDevice(int id)
        {
            m_dataModel.Database.ExecuteSqlCommand("DELETE FROM VendorDevice WHERE ID = @p0", id);
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
            m_dataModel.VendorDevices.Add(vendorDevice);
            m_dataModel.SaveChanges();
        }

        public void UpdateVendorDevice(VendorDevice vendorDevice)
        {
            VendorDevice source = m_dataModel.VendorDevices.Find(vendorDevice.ID);

            if (source != null)
            {
                vendorDevice.CopyProperties(source);
                m_dataModel.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Record not found, cannot save update.");
            }
        }

        //public VendorDevice FindVendorDevice(int id)
        //{
        //    return m_dataModel.VendorDevices.Find(id);
        //}

        //public IEnumerable<int> QueryVendorDeviceIDs()
        //{
        //    return m_dataModel.Database.SqlQuery<int>("SELECT ID FROM VendorDevice");
        //}

        //public IEnumerable<VendorDevice> QueryVendorDevices()
        //{
        //    return m_dataModel.VendorDevices;
        //}

        //public IEnumerable<VendorDevice> QueryVendorDevices(string sql)
        //{
        //    return string.IsNullOrEmpty(sql) ? QueryVendorDevices() : QueryVendorDevices(sql, null);
        //}

        //public IEnumerable<VendorDevice> QueryVendorDevices(string sql, params object[] parameters)
        //{
        //    if (string.IsNullOrEmpty(sql))
        //        throw new ArgumentNullException(nameof(sql));

        //    if ((object)parameters == null)
        //        parameters = new object[0];

        //    return m_dataModel.VendorDevices.SqlQuery($"SELECT * FROM VendorDevice WHERE {sql}", parameters).AsNoTracking().ToArray();
        //}

        #endregion

        #endregion

        #region [ Static ]

        // Static Fields
        private static volatile int s_connectCount;

        #endregion
    }
}
