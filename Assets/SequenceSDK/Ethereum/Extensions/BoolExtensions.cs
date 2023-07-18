using System;

namespace Sequence.Extensions
{
    public static class BoolExtensions
    {
        public static byte[] ToByteArray(this bool value)
        {
            return BitConverter.GetBytes(value);
        }
    }
}
