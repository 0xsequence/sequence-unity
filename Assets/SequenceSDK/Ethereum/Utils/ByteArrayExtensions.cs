using System;
using Sequence.ABI;

namespace Sequence.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string ByteArrayToHexStringWithPrefix(this byte[] byteArray)
        {
            return "0x" + SequenceCoder.ByteArrayToHexString(byteArray);
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
    }
}