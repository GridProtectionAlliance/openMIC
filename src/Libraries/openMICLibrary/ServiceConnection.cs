//******************************************************************************************************
//  ServiceConnection.cs - Gbtc
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
using System.Threading;
using GSF;
using GSF.Communication;
using GSF.ServiceProcess;

namespace openMIC
{
    /// <summary>
    /// Represents a remote connection to a service.
    /// </summary>
    public class ServiceConnection : IDisposable
    {
        #region [ Members ]

        // Events

        /// <summary>
        /// Reports a new status message from the service.
        /// </summary>
        public event EventHandler<EventArgs<UpdateType, string>> StatusMessage;

        /// <summary>
        /// Reports a new response from the service.
        /// </summary>
        public event EventHandler<EventArgs<ServiceResponse, string, bool>> ServiceResponse;

        /// <summary>
        /// Reports a change connection state.
        /// </summary>
        public event EventHandler<EventArgs<bool>> ConnectionState;

        // Fields
        private ClientHelper m_clientHelper;
        private TcpClient m_remotingClient;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="ServiceConnection"/> class.
        /// </summary>
        public ServiceConnection()
        {
            m_clientHelper = new ClientHelper();
            m_remotingClient = new TcpClient();

            // Setup remoting client
            m_remotingClient.ConnectionEstablished += m_remotingClient_ConnectionEstablished;
            m_remotingClient.ConnectionException += m_remotingClient_ConnectionException;
            m_remotingClient.ConnectionTerminated += m_remotingClient_ConnectionTerminated;
            m_remotingClient.ConnectionString = "server=localhost:8890";
            m_remotingClient.SettingsCategory = "RemotingClient";
            m_remotingClient.AllowDualStackSocket = true;
            m_remotingClient.IntegratedSecurity = false;
            m_remotingClient.PayloadAware = true;
            m_remotingClient.PersistSettings = true;

            // Setup client helper
            m_clientHelper.ReceivedServiceUpdate += m_clientHelper_ReceivedServiceUpdate;
            m_clientHelper.ReceivedServiceResponse += m_clientHelper_ReceivedServiceResponse;
            m_clientHelper.RemotingClient = m_remotingClient;
        }

        /// <summary>
        /// Releases the unmanaged resources before the <see cref="ServiceConnection"/> object is reclaimed by <see cref="GC"/>.
        /// </summary>
        ~ServiceConnection()
        {
            Dispose(false);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets enabled state of <see cref="ServiceConnection"/>.
        /// </summary>
        public bool Enabled
        {
            get
            {
                if ((object)m_clientHelper != null)
                    return m_clientHelper.Enabled;

                return false;
            }
            set
            {
                if ((object)m_clientHelper != null)
                    m_clientHelper.Enabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of times the service connection will attempt to connect to the service.
        /// </summary>
        /// <remarks>
        /// Set <see cref="MaxConnectionAttempts"/> to -1 for infinite connection attempts.
        /// </remarks>
        public int MaxConnectionAttempts
        {
            get
            {
                if ((object)m_remotingClient != null)
                    return m_remotingClient.MaxConnectionAttempts;

                return -1;
            }
            set
            {
                if ((object)m_remotingClient != null)
                    m_remotingClient.MaxConnectionAttempts = value;
            }
        }

        /// <summary>
        /// Gets or sets the connection string for the service connection used to attempt to connect to the service.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                if ((object)m_remotingClient != null)
                    return m_remotingClient.ConnectionString;

                return null;
            }
            set
            {
                if ((object)m_remotingClient != null)
                    m_remotingClient.ConnectionString = value;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases all the resources used by the <see cref="ServiceConnection"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ServiceConnection"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                    {
                        if ((object)m_clientHelper != null)
                        {
                            m_clientHelper.ReceivedServiceUpdate -= m_clientHelper_ReceivedServiceUpdate;
                            m_clientHelper.ReceivedServiceResponse -= m_clientHelper_ReceivedServiceResponse;
                            m_clientHelper.Dispose();
                            m_clientHelper = null;
                        }

                        if ((object)m_remotingClient != null)
                        {
                            m_remotingClient.ConnectionEstablished -= m_remotingClient_ConnectionEstablished;
                            m_remotingClient.ConnectionException -= m_remotingClient_ConnectionException;
                            m_remotingClient.ConnectionTerminated -= m_remotingClient_ConnectionTerminated;
                            m_remotingClient.Dispose();
                            m_remotingClient = null;
                        }
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        /// <summary>
        /// Starts (or restarts) connection cycle for service connection.
        /// </summary>
        public void Connect()
        {
            try
            {
                if ((object)m_clientHelper != null && (object)m_remotingClient != null)
                {
                    // Disconnect if currently connected to allow actual "reconnect"
                    if (m_remotingClient.CurrentState == ClientState.Connected)
                    {
                        m_clientHelper.Disconnect();
                        OnConnectionState(false);
                        Thread.Sleep(1000);
                    }

                    if (m_remotingClient.CurrentState == ClientState.Disconnected)
                        m_clientHelper.Connect();
                }
            }
            catch (Exception ex)
            {
                OnStatusMessage(UpdateType.Alarm, "Failed to connect due to exception: " + ex.Message);
            }
        }

        /// <summary>
        /// Starts (or restarts) connection cycle for service connection.
        /// </summary>
        public void ConnectAsync()
        {
            ThreadPool.QueueUserWorkItem(state => Connect());
        }

        /// <summary>
        /// Sends a command request to the service.
        /// </summary>
        /// <param name="command">Command to send.</param>
        /// <param name="attachments">Optional attachments to send with command.</param>
        public void SendCommand(string command, params object[] attachments)
        {
            if ((object)m_clientHelper == null || (object)m_remotingClient == null)
                throw new NullReferenceException("Client helper is not established - cannot send service command.");

            if (m_remotingClient.CurrentState == ClientState.Connected)
            {
                ClientRequest request = ClientRequest.Parse(command);

                // Add any attachments to the client request
                if ((object)attachments != null && attachments.Length > 0)
                    request.Attachments.AddRange(attachments);

                m_clientHelper.SendRequest(request);
            }
        }

        /// <summary>
        /// Raises the <see cref="StatusMessage"/> event.
        /// </summary>
        /// <param name="updateType">Update type of the message.</param>
        /// <param name="message">Status message.</param>
        protected virtual void OnStatusMessage(UpdateType updateType, string message)
        {
            if ((object)StatusMessage != null)
                StatusMessage(this, new EventArgs<UpdateType, string>(updateType, message));
        }

        /// <summary>
        /// Raises the <see cref="openMIC.ServiceConnection.ServiceResponse"/> event.
        /// </summary>
        /// <param name="serviceResponse">Reported <see cref="GSF.ServiceProcess.ServiceResponse"/> object.</param>
        /// <param name="sourceCommand">Original command.</param>
        /// <param name="responseSuccess">Response success flag.</param>
        protected virtual void OnServiceResponse(ServiceResponse serviceResponse, string sourceCommand, bool responseSuccess)
        {
            if ((object)ServiceResponse != null)
                ServiceResponse(this, new EventArgs<ServiceResponse, string, bool>(serviceResponse, sourceCommand, responseSuccess));
        }

        /// <summary>
        /// Raises the <see cref="ConnectionState"/> event.
        /// </summary>
        /// <param name="connected">Connected flag.</param>
        protected virtual void OnConnectionState(bool connected)
        {
            if ((object)ConnectionState != null)
                ConnectionState(this, new EventArgs<bool>(connected));
        }

        // Client helper service update reception handler
        private void m_clientHelper_ReceivedServiceUpdate(object sender, EventArgs<UpdateType, string> e)
        {
            OnStatusMessage(e.Argument1, e.Argument2);
        }

        // Client helper service response reception handler
        private void m_clientHelper_ReceivedServiceResponse(object sender, EventArgs<ServiceResponse> e)
        {
            string sourceCommand;
            bool responseSuccess;

            if (ClientHelper.TryParseActionableResponse(e.Argument, out sourceCommand, out responseSuccess))
                OnServiceResponse(e.Argument, sourceCommand, responseSuccess);
        }

        // Remoting client connection established handler
        private void m_remotingClient_ConnectionEstablished(object sender, EventArgs e)
        {
            OnConnectionState(true);
        }

        // Remoting client connection terminated handler
        private void m_remotingClient_ConnectionTerminated(object sender, EventArgs e)
        {
            OnConnectionState(false);
        }

        // Remoting client connection exception handler
        private void m_remotingClient_ConnectionException(object sender, EventArgs<Exception> e)
        {
            OnConnectionState(false);
        }

        #endregion
    }
}
