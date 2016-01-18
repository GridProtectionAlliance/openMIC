//******************************************************************************************************
//  ConsoleHub.cs - Gbtc
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
//  01/12/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using GSF;
using GSF.Identity;
using Microsoft.AspNet.SignalR;

namespace openMIC
{
    public class ServiceHub : Hub
    {
        public override Task OnConnected()
        {
            Program.Host.UpdatedStatus += m_serviceHost_UpdatedStatus;
            s_connectCount++;
            Program.Host.LogStatusMessage($"ServiceHub connect - count = {s_connectCount}");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled)
            {
                Program.Host.UpdatedStatus -= m_serviceHost_UpdatedStatus;
                s_connectCount--;
                Program.Host.LogStatusMessage($"ServiceHub disconnect - count = {s_connectCount}");
            }

            return base.OnDisconnected(stopCalled);
        }

        /// <summary>
        /// Gets the current server time.
        /// </summary>
        /// <returns>Current server time.</returns>
        public DateTime GetServerTime() => DateTime.UtcNow;

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
            string color = null;

            switch (e.Argument2)
            {
                case UpdateType.Alarm:
                    color = "red";
                    break;
                case UpdateType.Warning:
                    color = "yellow";
                    break;
            }

            BroadcastMessage(e.Argument1, color);
        }

        private void BroadcastMessage(string message, string color)
        {
            if (string.IsNullOrEmpty(color))
                color = "white";

            Clients.All.broadcastMessage(message, color);
        }

        // Static Fields
        private static volatile int s_connectCount;
    }
}
