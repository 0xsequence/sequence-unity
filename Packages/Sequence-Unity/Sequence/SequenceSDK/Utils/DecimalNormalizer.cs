using System;
using System.Globalization;
using System.Numerics;
using UnityEngine;

namespace Sequence.Utils
{
    public static class DecimalNormalizer
    {
        /// <summary>
        /// Normalize a float value into EVM-readable format
        /// </summary>
        /// <param name="x"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static string Normalize(float x, int decimals = 18)
        {
            BigInteger result = NormalizeAsBigInteger(x, decimals);
            return result.ToString();
        }
        
        /// <summary>
        /// Normalize a float value into EVM-readable format
        /// </summary>
        /// <param name="x"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static BigInteger NormalizeAsBigInteger(float x, int decimals = 18)
        {
            x = Math.Abs(x);
            double normalized = x * Math.Pow(10, decimals);
            BigInteger result = (BigInteger) normalized;
            return result;
        }
        
        /// <summary>
        /// Return the value back into human-readable format
        /// Gives a string value - recommended for use in UI or anywhere where precision matters
        /// </summary>
        /// <param name="x"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
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
            if (resultStr.EndsWith("."))
            {
                resultStr = resultStr.TrimEnd('.');
            }
            return resultStr;
        }

        /// <summary>
        /// Return the value back into human-readable format
        /// Gives a float value - recommended for use during gameplay or anywhere where performance matters
        /// </summary>
        /// <param name="x"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static float ReturnToNormal(BigInteger x, int decimals = 18)
        {
            return float.Parse(ReturnToNormalString(x, decimals));
        }
        
        /// <summary>
        /// Return the value back into human-readable format
        /// Gives a decimal value - recommended for use in UI or anywhere where precision matters
        /// </summary>
        /// <param name="x"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static decimal ReturnToNormalPrecise(BigInteger x, int decimals = 18)
        {
            return decimal.Parse(ReturnToNormalString(x, decimals), CultureInfo.InvariantCulture);
        }
    }
}