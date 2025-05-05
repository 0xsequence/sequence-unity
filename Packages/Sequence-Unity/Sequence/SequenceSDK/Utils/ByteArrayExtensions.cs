using System;
using System.Numerics;
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
        
        public static byte[] PadLeft(this byte[] input, int totalSize)
        {
            if (input.Length > totalSize)
                throw new ArgumentException("Input is larger than total size");

            byte[] result = new byte[totalSize];
            Buffer.BlockCopy(input, 0, result, totalSize - input.Length, input.Length);
            return result;
        }
        
        public static byte[] ByteArrayFromNumber(this BigInteger value, int? size = null)
        {
            if (value < 0)
                throw new ArgumentException("Value must be non-negative");

            byte[] rawBytes = value.ToByteArray(isUnsigned: true, isBigEndian: true);

            if (size.HasValue)
            {
                if (rawBytes.Length > size.Value)
                    throw new ArgumentException("Value is too large to fit in the specified size");

                return PadLeft(rawBytes, size.Value);
            }

            return rawBytes;
        }
        
        public static byte[] ByteArrayFromNumber(this int value, int size)
        {
            if (size < 1 || size > 4)
                throw new ArgumentOutOfRangeException(nameof(size), "Size must be between 1 and 4 bytes for an int.");

            byte[] bytes = BitConverter.GetBytes(value); 
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return bytes[^size..]; 
        }
        
        public static byte[] ByteArrayFromNumber(this int value)
        {
            byte[] bytes = BitConverter.GetBytes(value); 
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return bytes; 
        }

    }
}