﻿//******************************************************************************************************
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
using GSF;
using GSF.Web.Hubs;

namespace openMIC;

/// <summary>
/// Represents a client instance of a <see cref="ServiceHub"/> for a remote console connection.
/// </summary>
public class ServiceConnectionHubClient : HubClientBase
{
#region [ Members ]

    // Fields
    private bool m_disposed;

#endregion

#region [ Constructors ]

    /// <summary>
    /// Creates a new <see cref="ServiceConnectionHubClient"/> instance.
    /// </summary>
    public ServiceConnectionHubClient()
    {
        Program.Host.UpdatedStatus += ServiceHost_UpdatedStatus;
    }

#endregion

#region [ Methods ]

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="ServiceConnectionHubClient"/> object and optionally releases the managed resources.
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
                    Program.Host.UpdatedStatus -= ServiceHost_UpdatedStatus;

                    if (Guid.TryParse(ConnectionID, out Guid clientID))
                        Program.Host.DisconnectClient(clientID);
                }
            }
            finally
            {
                m_disposed = true;       // Prevent duplicate dispose.
                base.Dispose(disposing); // Call base class Dispose().
            }
        }
    }

    /// <summary>
    /// Sends a service command.
    /// </summary>
    /// <param name="command">Command string.</param>
    public void SendCommand(string command)
    {
        // Note that rights of current thread principle will be used to determine service command rights...
        if (Guid.TryParse(ConnectionID, out Guid clientID))
            Program.Host.SendRequest(command, clientID, HubInstance.Context.User);
    }

    private void ServiceHost_UpdatedStatus(object sender, EventArgs<Guid, string, UpdateType> e)
    {
        if (!Guid.TryParse(ConnectionID, out Guid clientID))
            return;

        // Only show broadcast messages or those destined to this client
        if (e.Argument1 != Guid.Empty && e.Argument1 != clientID)
            return;

        string color = e.Argument3 switch
        {
            UpdateType.Alarm   => "red",
            UpdateType.Warning => "yellow",
            _                  => "white"
        };

        BroadcastMessage(e.Argument2, color);
    }

    private void BroadcastMessage(string message, string color)
    {
            
        if (string.IsNullOrEmpty(color))
            color = "white";
            
        ClientScript.broadcastMessage(message, color);
    }

#endregion
}