using System;
using System.Numerics;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Primitives
{
    public class RecoveryLeaf : ILeaf
    {
        private static byte[] HashPrefix = "Sequence recovery leaf:\n".ToByteArray();
        
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
            return ByteArrayExtensions.ConcatenateByteArrays(
                RecoveryTopology.FlagLeaf.ByteArrayFromNumber(1),
                signer.Value.HexStringToByteArray(20),
                requiredDeltaTime.ByteArrayFromNumber(3),
                minTimestamp.ByteArrayFromNumber(8));
        }

        public byte[] EncodeRaw()
        {
            return ByteArrayExtensions.ConcatenateByteArrays(
                HashPrefix,
                signer.Value.HexStringToByteArray(20),
                requiredDeltaTime.ByteArrayFromNumber(32),
                minTimestamp.ByteArrayFromNumber(32));
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
                signer = new Address(address.ToLower()),
                requiredDeltaTime = BigInteger.Parse(requiredDeltaTimeStr),
                minTimestamp = BigInteger.Parse(minTimestampStr)
            };
        }
    }
}