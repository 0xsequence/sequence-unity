using System;

namespace Sequence.Utils
{
    public static class UInt32Extensions
    {
        /// <summary>
        /// Convert value to a byte[] using Big-Endian encoding
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this UInt32 value)
        {
            byte[] byteArray = new byte[4];
            byteArray[0] = (byte)(value >> 24);
            byteArray[1] = (byte)(value >> 16);
            byteArray[2] = (byte)(value >> 8);
            byteArray[3] = (byte)value;

            return byteArray;
        }
    }
}
