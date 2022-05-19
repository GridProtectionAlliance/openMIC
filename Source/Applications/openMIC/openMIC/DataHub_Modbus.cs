//******************************************************************************************************
//  DataHub_Modbus.cs - Gbtc
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
//  05/19/2021 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Threading.Tasks;
using ModbusAdapters;

namespace openMIC;

public partial class DataHub : IModbusOperations
{
    public Task<bool> ModbusConnect(string connectionString)
    {
        return m_modbusOperations.ModbusConnect(connectionString);
    }

    public void ModbusDisconnect()
    {
        m_modbusOperations.ModbusDisconnect();
    }

    public async Task<bool[]> ReadDiscreteInputs(ushort startAddress, ushort pointCount)
    {
        try
        {
            return await m_modbusOperations.ReadDiscreteInputs(startAddress, pointCount);
        }
        catch (Exception ex)
        {
            LogException(new InvalidOperationException($"Exception while reading discrete inputs starting @ {startAddress}: {ex.Message}", ex));
            return Array.Empty<bool>();
        }
    }

    public async Task<bool[]> ReadCoils(ushort startAddress, ushort pointCount)
    {
        try
        {
            return await m_modbusOperations.ReadCoils(startAddress, pointCount);
        }
        catch (Exception ex)
        {
            LogException(new InvalidOperationException($"Exception while reading coil values starting @ {startAddress}: {ex.Message}", ex));
            return Array.Empty<bool>();
        }
    }

    public async Task<ushort[]> ReadInputRegisters(ushort startAddress, ushort pointCount)
    {
        try
        {
            return await m_modbusOperations.ReadInputRegisters(startAddress, pointCount);
        }
        catch (Exception ex)
        {
            LogException(new InvalidOperationException($"Exception while reading input registers starting @ {startAddress}: {ex.Message}", ex));
            return Array.Empty<ushort>();
        }
    }

    public async Task<ushort[]> ReadHoldingRegisters(ushort startAddress, ushort pointCount)
    {
        try
        {
            return await m_modbusOperations.ReadHoldingRegisters(startAddress, pointCount);
        }
        catch (Exception ex)
        {
            LogException(new InvalidOperationException($"Exception while reading holding registers starting @ {startAddress}: {ex.Message}", ex));
            return Array.Empty<ushort>();
        }
    }

    public async Task WriteCoils(ushort startAddress, bool[] data)
    {
        try
        {
            await m_modbusOperations.WriteCoils(startAddress, data);
        }
        catch (Exception ex)
        {
            LogException(new InvalidOperationException($"Exception while writing coil values starting @ {startAddress}: {ex.Message}", ex));
        }
    }

    public async Task WriteHoldingRegisters(ushort startAddress, ushort[] data)
    {
        try
        {
            await m_modbusOperations.WriteHoldingRegisters(startAddress, data);
        }
        catch (Exception ex)
        {
            LogException(new InvalidOperationException($"Exception while writing holding registers starting @ {startAddress}: {ex.Message}", ex));
        }
    }

    public string DeriveString(ushort[] values)
    {
        return m_modbusOperations.DeriveString(values);
    }

    public float DeriveSingle(ushort highValue, ushort lowValue)
    {
        return m_modbusOperations.DeriveSingle(highValue, lowValue);
    }

    public double DeriveDouble(ushort b3, ushort b2, ushort b1, ushort b0)
    {
        return m_modbusOperations.DeriveDouble(b3, b2, b1, b0);
    }

    public int DeriveInt32(ushort highValue, ushort lowValue)
    {
        return m_modbusOperations.DeriveInt32(highValue, lowValue);
    }

    public uint DeriveUInt32(ushort highValue, ushort lowValue)
    {
        return m_modbusOperations.DeriveUInt32(highValue, lowValue);
    }

    public long DeriveInt64(ushort b3, ushort b2, ushort b1, ushort b0)
    {
        return m_modbusOperations.DeriveInt64(b3, b2, b1, b0);
    }

    public ulong DeriveUInt64(ushort b3, ushort b2, ushort b1, ushort b0)
    {
        return m_modbusOperations.DeriveUInt64(b3, b2, b1, b0);
    }
}