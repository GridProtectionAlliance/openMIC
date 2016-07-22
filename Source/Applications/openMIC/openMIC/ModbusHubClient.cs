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
using System.Collections.Generic;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
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
        private byte m_unitID;
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

        public Task<bool> ModbusConnect(string connectionString)
        {
            return Task.Factory.StartNew(() => {
                try
                {
                    Dictionary<string, string> parameters = connectionString.ParseKeyValuePairs();

                    string frameFormat, transport, setting;

                    if (!parameters.TryGetValue("frameFormat", out frameFormat) || string.IsNullOrWhiteSpace(frameFormat))
                        throw new ArgumentException("Connection string is missing \"frameFormat\".");

                    if (!parameters.TryGetValue("transport", out transport) || string.IsNullOrWhiteSpace(transport))
                        throw new ArgumentException("Connection string is missing \"transport\".");

                    if (!parameters.TryGetValue("unitID", out setting) || !byte.TryParse(setting, out m_unitID))
                        throw new ArgumentException("Connection string is missing \"unitID\" or value is invalid.");

                    bool useIP = false;
                    bool useRTU = false;

                    switch (frameFormat.ToUpperInvariant())
                    {
                        case "RTU":
                            useRTU = true;
                            break;
                        case "TCP":
                            useIP = true;
                            break;
                    }

                    if (useIP)
                    {
                        int port;

                        if (!parameters.TryGetValue("port", out setting) || !int.TryParse(setting, out port))
                            throw new ArgumentException("Connection string is missing \"port\" or value is invalid.");

                        if (transport.ToUpperInvariant() == "TCP")
                        {
                            string hostName;

                            if (!parameters.TryGetValue("hostName", out hostName) || string.IsNullOrWhiteSpace(hostName))
                                throw new ArgumentException("Connection string is missing \"hostName\".");

                            return ConnectTcpModbusMaster(hostName, port);
                        }

                        string interfaceIP;

                        if (!parameters.TryGetValue("interface", out interfaceIP))
                            interfaceIP = "0.0.0.0";

                        return ConnectUdpModbusMaster(interfaceIP, port);
                    }

                    string portName;
                    int baudRate;
                    int dataBits;
                    Parity parity;
                    StopBits stopBits;

                    if (!parameters.TryGetValue("portName", out portName) || string.IsNullOrWhiteSpace(portName))
                        throw new ArgumentException("Connection string is missing \"portName\".");

                    if (!parameters.TryGetValue("baudRate", out setting) || !int.TryParse(setting, out baudRate))
                        throw new ArgumentException("Connection string is missing \"baudRate\" or value is invalid.");

                    if (!parameters.TryGetValue("dataBits", out setting) || !int.TryParse(setting, out dataBits))
                        throw new ArgumentException("Connection string is missing \"dataBits\" or value is invalid.");

                    if (!parameters.TryGetValue("parity", out setting) || !Enum.TryParse(setting, out parity))
                        throw new ArgumentException("Connection string is missing \"parity\" or value is invalid.");

                    if (!parameters.TryGetValue("stopBits", out setting) || !Enum.TryParse(setting, out stopBits))
                        throw new ArgumentException("Connection string is missing \"stopBits\" or value is invalid.");

                    return ConnectSerialModbusMaster(useRTU, portName, baudRate, dataBits, parity, stopBits);
                }
                catch (Exception ex)
                {
                    m_hubClient.connectionFailed();
                    UpdateStatus($"Failed to connect to device: {ex.Message}", UpdateType.Alarm);
                }

                return false;
            });
        }

        private bool ConnectTcpModbusMaster(string hostName, int port)
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

        private bool ConnectUdpModbusMaster(string interfaceIP, int port)
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

        private bool ConnectSerialModbusMaster(bool useRTU, string portName, int baudRate, int dataBits, Parity parity, StopBits stopBits)
        {
            DisposeConnections();

            try
            {
                AttemptingConnection("Serial");

                m_serialClient = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
                m_modbusConnection = useRTU ? ModbusSerialMaster.CreateRtu(m_serialClient) : ModbusSerialMaster.CreateAscii(m_serialClient);

                ConnectionSucceeded("Serial");
                return true;
            }
            catch (Exception ex)
            {
                ConnectionFailed("Serial", ex.Message);
            }

            return false;
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

        public async Task<bool[]> ReadDiscreteInputs(ushort startAddress, ushort pointCount)
        {
            return await m_modbusConnection.ReadInputsAsync(m_unitID, startAddress, pointCount);
        }

        public async Task<bool[]> ReadCoils(ushort startAddress, ushort pointCount)
        {
            return await m_modbusConnection.ReadCoilsAsync(m_unitID, startAddress, pointCount);
        }

        public async Task<ushort[]> ReadInputRegisters(ushort startAddress, ushort pointCount)
        {
            return await m_modbusConnection.ReadInputRegistersAsync(m_unitID, startAddress, pointCount);
        }

        public async Task<ushort[]> ReadHoldingRegisters(ushort startAddress, ushort pointCount)
        {
            return await m_modbusConnection.ReadHoldingRegistersAsync(m_unitID, startAddress, pointCount);
        }

        public async Task WriteCoils(ushort startAddress, bool[] data)
        {
            await m_modbusConnection.WriteMultipleCoilsAsync(m_unitID, startAddress, data);
        }

        public async Task WriteHoldingRegisters(ushort startAddress, ushort[] data)
        {
            await m_modbusConnection.WriteMultipleRegistersAsync(m_unitID, startAddress, data);
        }

        #endregion
    }
}
