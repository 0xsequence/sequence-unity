using System;

namespace Sequence.Core.Signature
{
    public enum SignatureType : UInt32
    {
        Legacy = 0,
        Regular = 1,
        NoChainID = 2,
        Chained = 3
    }

    public static class SignatureTypeExtensions
    {
        public static byte[] ToByteArray(this SignatureType value)
        {
            return new byte[] { (byte)value };
        }
    }
}
