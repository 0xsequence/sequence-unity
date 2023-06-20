using System;
using System.Numerics;

namespace Sequence.Extensions
{
    public static class StringExtensions
    {
        public static BigInteger HexStringToBigInteger(this string hexString)
        {
            hexString = hexString.Replace("0x", "");
            return BigInteger.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
        }

        public static string EnsureHexPrefix(this string value)
        {
            if (value == null) return null;
            if (!value.HasHexPrefix())
                return "0x" + value;
            return value;
        }

        private static bool HasHexPrefix(this string value)
        {
            return value.StartsWith("0x");
        }
    }
}
