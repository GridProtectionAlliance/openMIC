//******************************************************************************************************
//  ModbusPoller.cs - Gbtc
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
//  07/26/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GSF;
using GSF.Configuration;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF.TimeSeries.Statistics;
using Modbus.Device;
using Modbus.Utility;

namespace openMIC
{
    [Description("Modbus: Implements Modbus polling capabilities")]
    [EditorBrowsable(EditorBrowsableState.Advanced)] // Normally defined as an input device protocol
    public class ModbusPoller : InputAdapterBase
    {
        #region [ Members ]

        // Nested Types

        // Define a IDevice implementation for to provide daily reports
        private class DeviceProxy : IDevice
        {
            private readonly ModbusPoller m_parent;

            public DeviceProxy(ModbusPoller parent)
            {
                m_parent = parent;
            }

            // Gets or sets total data quality errors of this <see cref="IDevice"/>.
            public long DataQualityErrors
            {
                get;
                set;
            }

            // Gets or sets total time quality errors of this <see cref="IDevice"/>.
            public long TimeQualityErrors
            {
                get;
                set;
            }

            // Gets or sets total device errors of this <see cref="IDevice"/>.
            public long DeviceErrors
            {
                get
                {
                    return m_parent.m_deviceErrors;
                }
                set
                {
                    // Ignoring updates
                }
            }

            // Gets or sets total measurements received for this <see cref="IDevice"/> - in local context "successful connections" per day.
            public long MeasurementsReceived
            {
                get
                {
                    return m_parent.m_measurementsReceived;
                }
                set
                {
                    // Ignoring updates
                }
            }

            // Gets or sets total measurements expected to have been received for this <see cref="IDevice"/> - in local context "attempted connections" per day.
            public long MeasurementsExpected
            {
                get
                {
                    return m_parent.m_measurementsExpected;
                }
                set
                {
                    // Ignoring updates
                }
            }
        }

        // Constants
        private const byte DefaultUnitID = 255;
        private const int DefaultPollingRate = 2000;
        private const int DefaultInterSequenceGroupPollDelay = 250;

        // Fields
        private readonly DeviceProxy m_deviceProxy;
        private IModbusMaster m_modbusConnection;
        private TcpClient m_tcpClient;
        private UdpClient m_udpClient;
        private SerialPort m_serialClient;
        private byte m_unitID;
        private int m_pollingRate;
        private int m_interSequenceGroupPollDelay;
        private long m_deviceErrors;
        private long m_measurementsReceived;
        private long m_measurementsExpected;
        private bool m_disposed;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="ModbusPoller"/>.
        /// </summary>
        public ModbusPoller()
        {
            m_deviceProxy = new DeviceProxy(this);
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets unit ID for Modbus connection.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines unit ID for Modbus connection."),
        DefaultValue(DefaultUnitID)]
        public byte UnitID
        {
            get
            {
                return m_unitID;
            }
            set
            {
                m_unitID = value;
            }
        }

        /// <summary>
        /// Gets or sets polling rate, in milliseconds, for Modbus connection.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines overall polling rate, in milliseconds, for Modbus connection."),
        DefaultValue(DefaultPollingRate)]
        public int PollingRate
        {
            get
            {
                return m_pollingRate;
            }
            set
            {
                m_pollingRate = value;
            }
        }

        /// <summary>
        /// Gets or sets inter sequence-group poll delay, in milliseconds, for Modbus connection.
        /// </summary>
        [ConnectionStringParameter,
        Description("Defines inter sequence-group poll delay, in milliseconds, for Modbus connection."),
        DefaultValue(DefaultInterSequenceGroupPollDelay)]
        public int InterSequenceGroupPollDelay
        {
            get
            {
                return m_interSequenceGroupPollDelay;
            }
            set
            {
                m_interSequenceGroupPollDelay = value;
            }
        }

        /// <summary>
        /// Gets flag that determines if the data input connects asynchronously.
        /// </summary>
        /// <remarks>
        /// Derived classes should return true when data input source is connects asynchronously, otherwise return false.
        /// </remarks>
        protected override bool UseAsyncConnect => false;

        /// <summary>
        /// Gets the flag indicating if this adapter supports temporal processing.
        /// </summary>
        public override bool SupportsTemporalProcessing => false;

        /// <summary>
        /// Returns the detailed status of the data input source.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();

                status.Append(base.Status);
                status.AppendFormat("                   Unit ID: {0}", UnitID);
                status.AppendLine();
                status.AppendFormat("              Polling Rate: {0}ms", PollingRate);
                status.AppendLine();
                status.AppendFormat("Inter Seq-Group Poll Delay: {0}ms", InterSequenceGroupPollDelay);
                status.AppendLine();
                status.AppendFormat("             Device Errors: {0}", m_deviceErrors);
                status.AppendLine();
                status.AppendFormat("     Measurements Received: {0}", m_measurementsReceived);
                status.AppendLine();
                status.AppendFormat("     Measurements Expected: {0}", m_measurementsExpected);
                status.AppendLine();

                return status.ToString();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ModbusPoller"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                try
                {
                    if (disposing)
                        DisposeConnections();
                }
                finally
                {
                    m_disposed = true;          // Prevent duplicate dispose.
                    base.Dispose(disposing);    // Call base class Dispose().
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

        /// <summary>
        /// Initializes <see cref="ModbusPoller" />.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            ConnectionStringParser<ConnectionStringParameterAttribute> parser = new ConnectionStringParser<ConnectionStringParameterAttribute>();
            parser.ParseConnectionString(ConnectionString, this);

            // Register downloader with the statistics engine
            StatisticsEngine.Register(this, "Modbus", "MOD");
            StatisticsEngine.Register(m_deviceProxy, Name, "Device", "PMU");
        }

        /// <summary>
        /// Attempts to connect to data input source.
        /// </summary>
        /// <remarks>
        /// Derived classes should attempt connection to data input source here.  Any exceptions thrown
        /// by this implementation will result in restart of the connection cycle.
        /// </remarks>
        protected override void AttemptConnection()
        {
            Dictionary<string, string> parameters = Settings;

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

                    m_tcpClient = new TcpClient(hostName, port);
                    m_modbusConnection = ModbusIpMaster.CreateIp(m_tcpClient);
                    return;
                }

                string interfaceIP;

                if (!parameters.TryGetValue("interface", out interfaceIP))
                    interfaceIP = "0.0.0.0";

                m_udpClient = new UdpClient(new IPEndPoint(IPAddress.Parse(interfaceIP), port));
                m_modbusConnection = ModbusIpMaster.CreateIp(m_udpClient);
                return;
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

            m_serialClient = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            m_modbusConnection = useRTU ? ModbusSerialMaster.CreateRtu(m_serialClient) : ModbusSerialMaster.CreateAscii(m_serialClient);
        }

        /// <summary>
        /// Attempts to disconnect from data input source.
        /// </summary>
        /// <remarks>
        /// Derived classes should attempt disconnect from data input source here.  Any exceptions thrown
        /// by this implementation will be reported to host via <see cref="E:GSF.TimeSeries.Adapters.AdapterBase.ProcessException" /> event.
        /// </remarks>
        protected override void AttemptDisconnection()
        {
            DisposeConnections();
            OnStatusMessage("Device disconnected.");
        }

        private string[] getDerivedValueAddresses(string expression)
        {
            int indexOfParen = expression.IndexOf('(');
            string addressRefs = expression.Substring(indexOfParen + 1, expression.Length - 1);
            return addressRefs.Split(',');
        }

        private void parseDerivedValueAddressExpression(string sequence, string expression)
        {
            int indexOfParen = expression.IndexOf('(');
            string derivedType = expression.Substring(0, indexOfParen).ToUpper();
            string[] addressList = getDerivedValueAddresses(expression);

            //const parsedValue = {
            //    type: derivedType,
            //    values: []
            //};

            //for (int i = 0; i < addressList.Length; i++) {
            //    string addressID = addressList[i].Trim();
            //    string recordType = addressID.substring(0, 2).toUpperCase();
            //    int address = parseInt(addressID.substring(2, addressID.length));
            //    var record = null;

            //    switch (recordType) {
            //        case "IR":
            //            record = sequence.findSequenceRecord(RecordType.InputRegister, address);
            //            break;
            //        case "HR":
            //            record = sequence.findSequenceRecord(RecordType.HoldingRegister, address);
            //            break;
            //    }

            //    if (record) {

            //            parsedValue.values.push(parseInt(record.dataValue()));
            //    }
            //}

            //return parsedValue;
        }

        /// <summary>
        /// Gets a short one-line status of this adapter.
        /// </summary>
        /// <param name="maxLength">Maximum number of available characters for display.</param>
        /// <returns>
        /// A short one-line summary of the current status of this adapter.
        /// </returns>
        public override string GetShortStatus(int maxLength)
        {
            if (!Enabled)
                return "Polling for is disabled...".CenterText(maxLength);

            return $"Polling enabled for every {PollingRate:N0}ms".CenterText(maxLength);
        }

        private bool[] ReadDiscreteInputs(ushort startAddress, ushort pointCount)
        {
            return m_modbusConnection.ReadInputs(m_unitID, startAddress, pointCount);
        }

        private bool[] ReadCoils(ushort startAddress, ushort pointCount)
        {
            return m_modbusConnection.ReadCoils(m_unitID, startAddress, pointCount);
        }

        private ushort[] ReadInputRegisters(ushort startAddress, ushort pointCount)
        {
            return m_modbusConnection.ReadInputRegisters(m_unitID, startAddress, pointCount);
        }

        private ushort[] ReadHoldingRegisters(ushort startAddress, ushort pointCount)
        {
            return m_modbusConnection.ReadHoldingRegisters(m_unitID, startAddress, pointCount);
        }

        private void WriteCoils(ushort startAddress, bool[] data)
        {
            m_modbusConnection.WriteMultipleCoilsAsync(m_unitID, startAddress, data);
        }

        private void WriteHoldingRegisters(ushort startAddress, ushort[] data)
        {
            m_modbusConnection.WriteMultipleRegistersAsync(m_unitID, startAddress, data);
        }

        private string DeriveString(ushort[] values)
        {
            return Encoding.Default.GetString(values.Select(BigEndianOrder.Default.GetBytes).SelectMany(bytes => bytes).ToArray());
        }

        private float DeriveSingle(ushort highValue, ushort lowValue)
        {
            return ModbusUtility.GetSingle(highValue, lowValue);
        }

        private double DeriveDouble(ushort b3, ushort b2, ushort b1, ushort b0)
        {
            return ModbusUtility.GetDouble(b3, b2, b1, b0);
        }

        private int DeriveInt32(ushort highValue, ushort lowValue)
        {
            return (int)ModbusUtility.GetUInt32(highValue, lowValue);
        }

        private uint DeriveUInt32(ushort highValue, ushort lowValue)
        {
            return ModbusUtility.GetUInt32(highValue, lowValue);
        }

        private long DeriveInt64(ushort b3, ushort b2, ushort b1, ushort b0)
        {
            return (long)DeriveUInt64(b3, b2, b1, b0);
        }

        private ulong DeriveUInt64(ushort b3, ushort b2, ushort b1, ushort b0)
        {
            return Word.MakeQuadWord(ModbusUtility.GetUInt32(b3, b2), ModbusUtility.GetUInt32(b1, b0));
        }

        #endregion
    }
}
