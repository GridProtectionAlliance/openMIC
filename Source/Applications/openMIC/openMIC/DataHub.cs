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
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using openMIC.Model;

namespace openMIC
{
    public class DataHub : Hub
    {
        #region [ Members ]

        // Fields
        private readonly openMICData m_dataModel;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        public DataHub()
        {
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

        public IEnumerable<Device> QueryDevices(string sortField, bool ascending, int page, int pageSize)
        {
            if (ascending)
                return m_dataModel.Devices.OrderBy(sortField).ToPagedList(page, pageSize);

            return m_dataModel.Devices.OrderByDescending(sortField).ToPagedList(page, pageSize);
        }

        public Vendor FindVendor(int id)
        {
            return m_dataModel.Vendors.Find(id);
        }

        public IEnumerable<int> QueryVendorIDs()
        {
            return m_dataModel.Database.SqlQuery<int>("SELECT ID FROM Vendor");
        }

        public IEnumerable<Vendor> QueryVendors()
        {
            return m_dataModel.Vendors;
        }

        public IEnumerable<Vendor> QueryVendors(string sql)
        {
            return string.IsNullOrEmpty(sql) ? QueryVendors() : QueryVendors(sql, null);
        }

        public IEnumerable<Vendor> QueryVendors(string sql, params object[] parameters)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException(nameof(sql));

            if ((object)parameters == null)
                parameters = new object[0];

            return m_dataModel.Vendors.SqlQuery($"SELECT * FROM Vendor WHERE {sql}", parameters).AsNoTracking().ToArray();
        }

        public VendorDevice FindVendorDevice(int id)
        {
            return m_dataModel.VendorDevices.Find(id);
        }

        public IEnumerable<int> QueryVendorDeviceIDs()
        {
            return m_dataModel.Database.SqlQuery<int>("SELECT ID FROM VendorDevice");
        }

        public IEnumerable<VendorDevice> QueryVendorDevices()
        {
            return m_dataModel.VendorDevices;
        }

        public IEnumerable<VendorDevice> QueryVendorDevices(string sql)
        {
            return string.IsNullOrEmpty(sql) ? QueryVendorDevices() : QueryVendorDevices(sql, null);
        }

        public IEnumerable<VendorDevice> QueryVendorDevices(string sql, params object[] parameters)
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException(nameof(sql));

            if ((object)parameters == null)
                parameters = new object[0];

            return m_dataModel.VendorDevices.SqlQuery($"SELECT * FROM VendorDevice WHERE {sql}", parameters).AsNoTracking().ToArray();
        }

        #endregion

        #region [ Static ]

        // Static Fields
        private static volatile int s_connectCount;

        #endregion
    }
}
