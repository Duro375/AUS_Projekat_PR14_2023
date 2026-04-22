using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read input registers functions/requests.
    /// </summary>
    public class ReadInputRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadInputRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadInputRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            ModbusReadCommandParameters readParams = CommandParameters as ModbusReadCommandParameters;
            byte[] ret_val = new byte[12];
            ret_val[1] = (byte)(readParams.TransactionId);
            ret_val[0] = (byte)(readParams.TransactionId >> 8);
            ret_val[3] = (byte)(readParams.ProtocolId);
            ret_val[2] = (byte)(readParams.ProtocolId >> 8);
            ret_val[5] = (byte)(readParams.Length);
            ret_val[4] = (byte)(readParams.Length >> 8);
            ret_val[6] = readParams.UnitId;
            ret_val[7] = readParams.FunctionCode;
            ret_val[9] = (byte)(readParams.StartAddress);
            ret_val[8] = (byte)(readParams.StartAddress >> 8);
            ret_val[11] = (byte)(readParams.Quantity);
            ret_val[10] = (byte)(readParams.Quantity >> 8);
            return ret_val;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            Dictionary<Tuple<PointType, ushort>, ushort> ret_val = new Dictionary<Tuple<PointType, ushort>, ushort>();

            int byteCnt = response[8];

            ushort adress = ((ModbusReadCommandParameters)CommandParameters).StartAddress;

            for (int i = 0; i < byteCnt / 2; i += 2)
            {
                ushort value = (ushort)((ushort)(response[9 + i] << 8) + response[10 + i]);
                Tuple<PointType, ushort> index = Tuple.Create(PointType.ANALOG_INPUT, (ushort)(adress + i));
                ret_val.Add(index, value);
            }

            return ret_val;
        }
    }
}