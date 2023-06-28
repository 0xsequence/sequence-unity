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
            if (value == null)
            {
                return false;
            }
            return value.StartsWith("0x");
        }

        public static bool IsHexFormat(this string value)
        {
            if (!value.HasHexPrefix())
            {
                return false;
            }

            value = value.Replace("0x", "");

            int length = value.Length;
            for (int i = 0; i < length; i++)
            {
                if (!((value[i] >= '0' && value[i] <= '9') ||
                    (value[i] >= 'a' && value[i] <= 'f') ||
                    (value[i] >= 'A' && value[i] <= 'F'))) {
                    return false;
                }
            }
            return true;
        }

        public static bool IsAddress(this string value)
        {
            return value.Length == 42 && value.IsHexFormat();
        }
    }
}
