using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sequence.Utils;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    public class ParameterRule
    {
        public static readonly byte[] SelectorMask = "0xffffffff".HexStringToByteArray().PadRight(32);
        public static readonly byte[] Uint256Mask = Enumerable.Repeat((byte)0xff, 32).ToArray().PadLeft(32);
        
        public bool cumulative;
        public ParameterOperation operation;
        public byte[] value; 
        public BigInteger offset;
        public byte[] mask;

        public object ToJson()
        {
            return new
            {
                cumulative = cumulative,
                operation = (int)operation,
                value = new Dictionary<string, object>
                {
                    {"_isUint8Array", true},
                    {"data", value.ByteArrayToHexStringWithPrefix()}
                },
                offset = offset.ToString(),
                mask = new Dictionary<string, object>
                {
                    {"_isUint8Array", true},
                    {"data", mask.ByteArrayToHexStringWithPrefix()}
                }
            };
        }

        public byte[] Encode()
        {
            byte operationCumulative = (byte)(((byte)operation << 1) | (cumulative ? 1 : 0));
            List<byte> result = new() { operationCumulative };
            result.AddRange(value.PadLeft(32));
            result.AddRange(offset.ToByteArray().PadLeft(32));
            result.AddRange(mask.PadLeft(32));
            return result.ToArray();
        }
        
        public static ParameterRule FromJson(string json)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            return new()
            {
                cumulative = (bool)data["cumulative"],
                operation = (ParameterOperation)Convert.ToInt32(data["operation"]),
                value = data["value"].ToString().HexStringToByteArray(),
                offset = BigInteger.Parse(data["offset"].ToString()),
                mask = data["mask"].ToString().HexStringToByteArray()
            };
        }

        public static ParameterRule Decode(byte[] data)
        {
            if (data.Length != 97)
                throw new Exception("Invalid parameter rule length");

            var operationCumulative = data[0];
            var cumulative = (operationCumulative & 1) == 1;
            var operation = (ParameterOperation)(operationCumulative >> 1);

            var value = data.AsSpan(1, 32).ToArray();
            var offset = new BigInteger(data.AsSpan(33, 32).ToArray(), isUnsigned: true, isBigEndian: true);
            var mask = data.AsSpan(65, 32).ToArray();

            return new ParameterRule
            {
                cumulative = cumulative,
                operation = operation,
                value = value,
                offset = offset,
                mask = mask
            };
        }
    }
}