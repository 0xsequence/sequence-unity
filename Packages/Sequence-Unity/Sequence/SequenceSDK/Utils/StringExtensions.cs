using System;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

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
            hexString = hexString.WithoutHexPrefix();
            return BigInteger.Parse("0" + hexString, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a string that represents a hex value to an int
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static int HexStringToInt(this string hexString)
        {
            hexString = hexString.Replace("0x", "");
            hexString.TrimStart('0');
            return int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Converts a string that represents a hex value to a human-readable string
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static string HexStringToHumanReadable(this string hexString)
        {
            byte[] bytes = HexStringToByteArray(hexString);
            string result = Encoding.UTF8.GetString(bytes);
            string cleaned = RemoveControlCharactersExceptNewline(result); // Unity's encoding/decoding is a bit wonky and adds a bunch of \0 (null terminators) to the beginning and end of the string
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
             cleaned = cleaned.Replace("\n", "\r\n");
#endif
            return cleaned;
        }

        private static string RemoveControlCharactersExceptNewline(string value)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in value)
            {
                if (!char.IsControl(c) || c == '\n')
                {
                    result.Append(c);
                }
            }
            return result.ToString();
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
        
        public static string RemoveZeroPadding(this string value)
        {
            if (value == null)
            {
                return null;
            }
            if (value.Length == 0)
            {
                return value;
            }
            string hexValue = value.WithoutHexPrefix();

            int firstNonZeroIndex = 0;
            while (firstNonZeroIndex < hexValue.Length && hexValue[firstNonZeroIndex] == '0')
            {
                firstNonZeroIndex++;
            }

            string result = hexValue.Substring(firstNonZeroIndex).EnsureHexPrefix();
            return result;
        }

        public static byte[] HexStringToByteArray(this string value)
        {
            value = value.WithoutHexPrefix();
            if (value.Length % 2 != 0)
            {
                value = "0" + value;
            }
            
            byte[] result = new byte[value.Length / 2];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
            }

            return result;
        }
        
        public static byte[] HexStringToByteArray(this string hex, int expectedSize)
        {
            if (hex.StartsWith("0x"))
            {
                hex = hex.Substring(2);
            }

            int byteCount = hex.Length / 2;
            byte[] rawBytes = new byte[byteCount];

            for (int i = 0; i < byteCount; i++)
            {
                string byteValue = hex.Substring(i * 2, 2);
                rawBytes[i] = Convert.ToByte(byteValue, 16);
            }

            if (rawBytes.Length == expectedSize)
            {
                return rawBytes;
            }

            if (rawBytes.Length > expectedSize)
            {
                byte[] trimmed = new byte[expectedSize];
                Buffer.BlockCopy(rawBytes, rawBytes.Length - expectedSize, trimmed, 0, expectedSize);
                return trimmed;
            }

            byte[] padded = new byte[expectedSize];
            Buffer.BlockCopy(rawBytes, 0, padded, expectedSize - rawBytes.Length, rawBytes.Length);
            return padded;
        }

        public static string StringToBase64(this string value)
        {
            byte[] asBytes = value.ToByteArray();
            string base64 = Convert.ToBase64String(asBytes);
            return base64;
        }
    }
}
