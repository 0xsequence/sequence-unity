using System;
using System.Numerics;

namespace Sequence.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string that represents a hex value to a BigInteger
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static BigInteger HexStringToBigInteger(this string hexString)
        {
            hexString = hexString.Replace("0x", "");
            return BigInteger.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Appends '0x' to the beginning of a string if it is not already present
        /// Useful for representing strings as hex values
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EnsureHexPrefix(this string value)
        {
            if (value == null)
            {
                return null;
            }
            if (!value.HasHexPrefix())
            {
                return "0x" + value;
            }
            return value;
        }

        private static bool HasHexPrefix(this string value)
        {
            return value.StartsWith("0x");
        }
    }
}
