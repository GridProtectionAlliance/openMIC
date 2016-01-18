//******************************************************************************************************
//  ServiceConnection.cs - Gbtc
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
//  01/15/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Security.Principal;
using System.Threading;
using GSF;
using GSF.Identity;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace openMIC
{
    public class ServiceConnection
    {
        #region [ Members ]

        // Fields
        private readonly IHubConnectionContext<dynamic> m_clients;

        #endregion

        #region [ Constructors ]

        private ServiceConnection(IHubConnectionContext<dynamic> clients)
        {
            Program.Host.UpdatedStatus += m_serviceHost_UpdatedStatus;
            m_clients = clients;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Sends a service command.
        /// </summary>
        /// <param name="command">Command string.</param>
        public void SendCommand(string command)
        {
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(UserInfo.CurrentUserID), new[] { "Administrator" });
            Program.Host.SendRequest(command);
        }

        private void m_serviceHost_UpdatedStatus(object sender, EventArgs<string, UpdateType> e)
        {
            string color;

            switch (e.Argument2)
            {
                case UpdateType.Alarm:
                    color = "red";
                    break;
                case UpdateType.Warning:
                    color = "yellow";
                    break;
                default:
                    color = "white";
                    break;
            }

            BroadcastMessage(e.Argument1, color);
        }

        private void BroadcastMessage(string message, string color)
        {
            if (string.IsNullOrEmpty(color))
                color = "white";

            m_clients.All.broadcastMessage(message, color);
        }

        #endregion

        #region [ Static ]

        // Static Fields
        private static readonly Lazy<ServiceConnection> s_defaultInstance;

        // Static Constructor
        static ServiceConnection()
        {
            s_defaultInstance = new Lazy<ServiceConnection>(() => new ServiceConnection(GlobalHost.ConnectionManager.GetHubContext<ServiceHub>().Clients));
        }

        // Static Properties
        public static ServiceConnection Default => s_defaultInstance.Value;

        #endregion


    }
}
