using System;
using System.Numerics;
using Sequence.ABI;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    internal static class IndividualStringEncoder
    {
        public static string EncodeString(this string value)
        {
            byte[] encoded = EncodeStringAsBytes(value);
            return encoded.ByteArrayToHexString();
        }

        public static byte[] EncodeStringAsBytes(this string value)
        {
            byte[] asBytes = value.ToByteArray().PadLeft(32);
            byte[] lengthBytes = ((BigInteger)asBytes.Length).ByteArrayFromNumber(32);
            return ByteArrayExtensions.ConcatenateByteArrays(lengthBytes, asBytes);
        }
    }
}