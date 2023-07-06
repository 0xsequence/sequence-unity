using System;
using System.Numerics;

namespace Sequence.Extensions
{
    public static class BigIntegerExtensions
    {
        public static string BigIntegerToHexString(this BigInteger value)
        {
            return "0x" + value.ToString("x");
        }
    }
}
