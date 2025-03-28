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
            string numberStr = BigInteger.Abs(x).ToString();

            while (numberStr.Length <= decimals)
            {
                numberStr = "0" + numberStr;
            }

            int decimalPointIndex = numberStr.Length - decimals;
            string resultStr = numberStr.Insert(decimalPointIndex, ".");
            if (resultStr.Contains('.'))
            {
                resultStr = resultStr.TrimEnd('0');
            }
            return resultStr;
        }

        public static float ReturnToNormal(BigInteger x, int decimals = 18)
        {
            return float.Parse(ReturnToNormalString(x, decimals));
        }
        
        public static decimal ReturnToNormalPrecise(BigInteger x, int decimals = 18)
        {
            return decimal.Parse(ReturnToNormalString(x, decimals));
        }
    }
}