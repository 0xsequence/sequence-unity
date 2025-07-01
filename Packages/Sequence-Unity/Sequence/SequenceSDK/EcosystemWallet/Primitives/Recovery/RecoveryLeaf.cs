using System;
using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    public class RecoveryLeaf : ILeaf
    {
        public Address signer;
        public BigInteger requiredDeltaTime;
        public BigInteger minTimestamp;

        public object ToJsonObject()
        {
            return new
            {
                signer = signer.Value,
                requiredDeltaTime = requiredDeltaTime.ToString(),
                minTimestamp = minTimestamp.ToString()
            };
        }

        public byte[] Encode()
        {
            throw new NotImplementedException();
        }

        public byte[] EncodeRaw()
        {
            return Encode();
        }
        
        public static RecoveryLeaf FromInput(string input)
        {
            var parts = input.Split(':');
            if (parts.Length != 4 || parts[0] != "signer")
                throw new ArgumentException($"Invalid leaf format: {input}");

            var address = parts[1];
            var requiredDeltaTimeStr = parts[2];
            var minTimestampStr = parts[3];

            if (string.IsNullOrEmpty(requiredDeltaTimeStr) || string.IsNullOrEmpty(minTimestampStr))
                throw new ArgumentException($"Invalid leaf format: {input}");

            return new RecoveryLeaf
            {
                signer = new Address(address),
                requiredDeltaTime = BigInteger.Parse(requiredDeltaTimeStr),
                minTimestamp = BigInteger.Parse(minTimestampStr)
            };
        }
    }
}