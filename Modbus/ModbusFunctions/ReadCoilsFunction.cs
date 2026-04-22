using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read coil functions/requests.
    /// </summary>
    public class ReadCoilsFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
		public ReadCoilsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc/>
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
            ushort quantity = ((ModbusReadCommandParameters)CommandParameters).Quantity;
            int cnt = 0;

            for (int i = 0; i < byteCnt; i++)
            {
                int mask = 1;
                for (int j = 0; j < 8 && cnt < quantity; j++, cnt++)
                {
                    ushort value = (ushort)((response[9 + i] & mask) > 0 ? 1 : 0);
                    mask = mask << 1;
                    Tuple<PointType, ushort> index = Tuple.Create(PointType.DIGITAL_OUTPUT, (ushort)(adress + j));
                    ret_val.Add(index, value);
                }
            }

            return ret_val;
        }
    }
}