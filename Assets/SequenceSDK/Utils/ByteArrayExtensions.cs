using System;
using System.Text;
using UnityEngine;

namespace Sequence.Utils
{
    public static class ByteArrayExtensions
    {
        public static string ByteArrayToHexStringWithPrefix(this byte[] byteArray)
        {
            return "0x" + byteArray.ByteArrayToHexString();
        }
        
        public static string ByteArrayToHexString(this byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                return "";
            }
            
            try
            {
                StringBuilder hex = new StringBuilder(byteArray.Length * 2);
                foreach (byte b in byteArray)
                    hex.AppendFormat("{0:x2}", b);
                return hex.ToString();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error converting byte array to hexadecimal string: {ex.Message}");
                return string.Empty;
            }
        }

        public static bool HasPrefix(this byte[] b, byte[] prefix) {
            return b.AsSpan().StartsWith(prefix);
        }

        public static byte[] ConcatenateByteArrays(params byte[][] arrays)
        {
            int totalLength = 0;
            int numberOfArrays = arrays.Length;
            for (int i = 0; i < numberOfArrays; i++)
            {
                totalLength += arrays[i].Length;
            }

            int offset = 0;
            byte[] result = new byte[totalLength];
            for (int i = 0; i < numberOfArrays; i++)
            {
                int length = arrays[i].Length;
                Buffer.BlockCopy(arrays[i], 0, result, offset, length);
                offset += length;
            }
            return result;
        }

        public static byte[] BuildArrayWithRepeatedValue(byte[] value, int repetitions)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (repetitions < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(repetitions), "Repetitions must be non-negative.");
            }

            int totalLength = value.Length * repetitions;
            byte[] result = new byte[totalLength];

            for (int i = 0; i < totalLength; i += value.Length)
            {
                Buffer.BlockCopy(value, 0, result, i, value.Length);
            }

            return result;
        }
    }
}