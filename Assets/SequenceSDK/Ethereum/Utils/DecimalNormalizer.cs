using System;
using System.Numerics;
using UnityEngine;

namespace SequenceSDK.Ethereum.Utils
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
    }
}