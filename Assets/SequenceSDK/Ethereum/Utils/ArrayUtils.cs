using System;

namespace Sequence.Utils
{
    public static class ArrayUtils
    {
        public static T[] ConvertToTArray<T,T2>(this T2 value)
        {
            if (value is Array array)
            {
                int length = array.Length;
                T[] converted = new T[length];

                for (int i = 0; i < length; i++)
                {
                    converted[i] = (T)array.GetValue(i);
                }

                return converted;
            }

            throw new ArgumentException(
                $"Value {value} with type {value.GetType()} is not an array as expected.");
        }
    }
}