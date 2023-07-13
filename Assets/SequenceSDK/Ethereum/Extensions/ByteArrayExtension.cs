using System;
using System.Numerics;
using System.Text;

namespace Sequence.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string ByteArrayToHexString(this byte[] byteArray)
        {
            StringBuilder hexBuilder = new StringBuilder(byteArray.Length * 2);
            for (int i = 0; i < byteArray.Length; i++)
            {
                hexBuilder.Append(byteArray[i].ToString("X2"));
            }
            string hexString = hexBuilder.ToString();
            return "0x" + hexString;
        }

        public static bool HasPrefix(this byte[] b, byte[] prefix) {
            return b.AsSpan().StartsWith(prefix);
        }
    }
}
