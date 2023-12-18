using System;

namespace Sequence.Ethereum.Utils
{
    public static class GenericHelpers
    {
        public static T[] Take<T>(this T[] source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count must not be negative.");
            }

            int actualCount = Math.Min(source.Length, count);
            T[] result = new T[actualCount];

            for (int i = 0; i < actualCount; i++)
            {
                result[i] = source[i];
            }

            return result;
        }
        
        public static bool SequenceEqual<T>(this T[] first, T[] second)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }

            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            if (first.Length != second.Length)
            {
                return false;
            }

            for (int i = 0; i < first.Length; i++)
            {
                if (!Equals(first[i], second[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}