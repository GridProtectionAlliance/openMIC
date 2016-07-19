//******************************************************************************************************
//  ModbusHubClient.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  07/18/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using GSF;
using Modbus.Device;

namespace openMIC
{
    /// <summary>
    /// Represents a client instance of a <see cref="DataHub"/> for Modbus connections.
    /// </summary>
    public class ModbusHubClient : IDisposable
    {
        #region [ Members ]

        // Fields
        private readonly dynamic m_hubClient;
        private IModbusMaster m_modbusConnection;
        private TcpClient m_tcpClient;
        private UdpClient m_udpClient;
        private SerialPort m_serialClient;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="ModbusHubClient"/> instance.
        /// </summary>
        /// <param name="hubClient"></param>
        public ModbusHubClient(dynamic hubClient)
        {
            m_hubClient = hubClient;
        }


        #endregion

        #region [ Properties ]

        //

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases all the resources used by the <see cref="ModbusHubClient"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ModbusHubClient"/> object and optionally releases the managed resources.
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
                        DisposeConnections();
                    }
                }
                finally
                {
                    m_disposed = true;  // Prevent duplicate dispose.
                }
            }
        }

        private void DisposeConnections()
        {
            m_tcpClient?.Dispose();
            m_udpClient?.Dispose();
            m_serialClient?.Dispose();
            m_modbusConnection?.Dispose();
        }

        private void AttemptingConnection(string type)
        {
            m_hubClient.attemptingConnection();
            UpdateStatus($"Attempting to connect to device using {type}...", UpdateType.Information);
        }

        private void ConnectionSucceeded(string type)
        {
            m_hubClient.connectionSucceeded();
            UpdateStatus($"Connected to device using {type}", UpdateType.Information);
        }

        private void ConnectionFailed(string type, string exceptionMessage)
        {
            m_hubClient.connectionFailed();
            UpdateStatus($"Failed to connect to device using {type}: {exceptionMessage}", UpdateType.Alarm);
        }

        public bool ConnectTcpModbusMaster(string hostName, int port)
        {
            DisposeConnections();

            try
            {
                AttemptingConnection("TCP");

                m_tcpClient = new TcpClient(hostName, port);
                m_modbusConnection = ModbusIpMaster.CreateIp(m_tcpClient);

                ConnectionSucceeded("TCP");
                return true;
            }
            catch (Exception ex)
            {
                ConnectionFailed("TCP", ex.Message);
            }

            return false;
        }

        public bool ConnectUdpModbusMaster(string interfaceIP, int port)
        {
            DisposeConnections();

            try
            {
                AttemptingConnection("UDP");

                m_udpClient = new UdpClient(new IPEndPoint(IPAddress.Parse(interfaceIP), port));
                m_modbusConnection = ModbusIpMaster.CreateIp(m_udpClient);

                ConnectionSucceeded("UDP");
                return true;
            }
            catch (Exception ex)
            {
                ConnectionFailed("UDP", ex.Message);
            }

            return false;
        }

        public bool ConnectSerialModbusMaster(bool isRTU, string portName, int baudRate, int dataBits, Parity parity, StopBits stopBits)
        {
            DisposeConnections();

            try
            {
                AttemptingConnection("Serial");

                m_serialClient = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
                m_modbusConnection = isRTU ? ModbusSerialMaster.CreateRtu(m_serialClient) : ModbusSerialMaster.CreateAscii(m_serialClient);

                ConnectionSucceeded("Serial");
                return true;
            }
            catch (Exception ex)
            {
                ConnectionFailed("Serial", ex.Message);
            }

            return false;
        }

        private void UpdateStatus(string message, UpdateType type)
        {
            string color = "white";

            switch (type)
            {
                case UpdateType.Warning:
                    color = "yellow";
                    break;
                case UpdateType.Alarm:
                    color = "red";
                    break;
            }

            m_hubClient.connectionStatusUpdate(message, color);
        }

        #endregion
    }
}
