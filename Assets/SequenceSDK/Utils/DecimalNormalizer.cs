using System;
using System.Numerics;
using UnityEngine;

namespace Sequence.Utils
{
    public static class DecimalNormalizer
    {
        public static string Normalize(float x, int decimals = 18)
        {
            BigInteger result = NormalizeAsBigInteger(x, decimals);
            return result.ToString();
        }
        
        public static BigInteger NormalizeAsBigInteger(float x, int decimals = 18)
        {
            x = Math.Abs(x);
            double normalized = x * Math.Pow(10, decimals);
            BigInteger result = (BigInteger) normalized;
            return result;
        }

        public static string ReturnToNormalString(BigInteger x, int decimals = 18)
        {
            float result = ReturnToNormal(x, decimals);
            return result.ToString();
        }

        public static float ReturnToNormal(BigInteger x, int decimals = 18)
        {
            x = Math.Abs((long)x);
            double normalized = (long)x / Math.Pow(10, decimals);
            float result = (float) normalized;
            return result;
        }
    }
}