using System;

namespace Sequence.Utils
{
    public static class UInt16Extensions
    {
        /// <summary>
        /// Convert value to a byte[] using Big-Endian encoding
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this UInt16 value)
        {
            byte[] byteArray = new byte[2];
            byteArray[0] = (byte)(value >> 8);
            byteArray[1] = (byte)value;

            return byteArray;
        }
    }
}
