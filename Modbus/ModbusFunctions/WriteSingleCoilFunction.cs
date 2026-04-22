using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write coil functions/requests.
    /// </summary>
    public class WriteSingleCoilFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleCoilFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            ModbusWriteCommandParameters writeParams = CommandParameters as ModbusWriteCommandParameters;
            byte[] ret_val = new byte[12];
            ret_val[1] = (byte)(writeParams.TransactionId);
            ret_val[0] = (byte)(writeParams.TransactionId >> 8);
            ret_val[3] = (byte)(writeParams.ProtocolId);
            ret_val[2] = (byte)(writeParams.ProtocolId >> 8);
            ret_val[5] = (byte)(writeParams.Length);
            ret_val[4] = (byte)(writeParams.Length >> 8);
            ret_val[6] = writeParams.UnitId;
            ret_val[7] = writeParams.FunctionCode;
            ret_val[9] = (byte)(writeParams.OutputAddress);
            ret_val[8] = (byte)(writeParams.OutputAddress >> 8);
            ret_val[11] = (byte)(writeParams.Value);
            ret_val[10] = (byte)(writeParams.Value >> 8);
            return ret_val;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            Dictionary<Tuple<PointType, ushort>, ushort> ret_val = new Dictionary<Tuple<PointType, ushort>, ushort>();

            byte regAddres1 = response[8];
            byte regAddres2 = response[9];

            byte val1 = response[10];
            byte val2 = response[11];

            ushort adress = (ushort)((ushort)(regAddres1 << 8) + regAddres2);
            ushort value = (ushort)((ushort)(val1 << 8) + val2);
            Tuple<PointType, ushort> index = Tuple.Create(PointType.DIGITAL_OUTPUT, adress);
            ret_val.Add(index, value);
            return ret_val;
        }
    }
}