using System;
using System.Collections.Generic;
using System.Numerics;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    public class ParameterRule
    {
        public bool cumulative;
        public ParameterOperation operation;
        public byte[] value; 
        public BigInteger offset;
        public byte[] mask;

        public byte[] Encode()
        {
            byte operationCumulative = (byte)(((byte)operation << 1) | (cumulative ? 1 : 0));
            List<byte> result = new() { operationCumulative };
            result.AddRange(value.PadLeft(32));
            result.AddRange(offset.ToByteArray().PadLeft(32));
            result.AddRange(mask.PadLeft(32));
            return result.ToArray();
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