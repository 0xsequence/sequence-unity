using System;

namespace Sequence.Utils
{
    public static class BoolExtensions
    {
        public static byte[] ToByteArray(this bool value)
        {
            return BitConverter.GetBytes(value);
        }
    }
}
