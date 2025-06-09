using System;
using System.Numerics;

namespace Sequence.Utils
{
    public static class BigIntegerExtensions
    {
        public static string BigIntegerToHexString(this BigInteger value)
        {
            string result = value.ToString("x").TrimStart('0');
            if (result == "")
            {
                result = "0";
            }
            return "0x" + result;
        }
        
        public static int MinimumBytesNeeded(this BigInteger value)
        {
            if (value == 0) return 1;

            byte[] bytes = value.ByteArrayFromNumber();
            return bytes.Length;
        }
    }
}
