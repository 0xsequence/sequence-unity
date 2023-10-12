using System;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace Sequence.Utils
{
    public static class StringExtensions
    {
        public static string ZeroAddress = "0x0";

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
        /// Converts a string that represents a hex value to an int
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static int HexStringToInt(this string hexString)
        {
            hexString = hexString.Replace("0x", "");
            return int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
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
            return (value != null && value.Length == 42 && value.IsHexFormat()) || value == ZeroAddress;
        }

        public static byte[] ToByteArray(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        /// <summary>
        /// Parse a hex string as a bool
        /// RPCs will return a value of 1 as true and 0 as false
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HexStringToBool(this string value)
        {
            BigInteger result = value.HexStringToBigInteger();
            if (result == BigInteger.One)
            {
                return true;
            }
            else if (result == BigInteger.Zero)
            {
                return false;
            }
            else
            {
                throw new ArgumentException($"Cannot decode value: {value} into a bool");
            }
        }

        public static string WithoutHexPrefix(this string value)
        {
            return value.Replace("0x", "");
        }

        public static string NoWhitespace(this string value)
        {
            return Regex.Replace(value, @"\s+", "");
        }
    }
}
