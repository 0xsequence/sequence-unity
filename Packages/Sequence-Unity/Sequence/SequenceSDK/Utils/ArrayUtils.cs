using System;
using System.Collections;
using System.Collections.Generic;
using Array = System.Array;

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
            else if (value is List<object> list)
            {
                int length = list.Count;
                T[] converted = new T[length];
                
                for (int i = 0; i < length; i++)
                {
                    converted[i] = (T)list[i];
                }

                return converted;
            }
            else if (value is IEnumerator enumerator)
            {
                List<T> temp = new List<T>();
                while (enumerator.MoveNext())
                {
                    temp.Add((T)enumerator.Current);
                }

                return temp.ToArray();
            }

            throw new ArgumentException(
                $"Value {value} with type {value.GetType()} is not an {typeof(Array)}, {typeof(List<object>)}, or {typeof(IEnumerator)} as expected.");
        }

        public static T[] BuildArrayWithRepeatedValue<T>(T value, int repetitions)
        {
            T[] array = new T[repetitions];
            for (int i = 0; i < repetitions; i++)
            {
                array[i] = value;
            }

            return array;
        }

        public static bool IsIn<T>(this T value, T[] arr)
        {
            int length = arr.Length;
            for (int i = 0; i < length; i++)
            {
                if (arr[i].Equals(value))
                {
                    return true;
                }
            }

            return false;
        }

        public static List<T> ConvertToList<T>(this T[] arr)
        {
            List<T> newList = new List<T>();
            int length = arr.Length;
            for (int i = 0; i < length; i++)
            {
                newList.Add(arr[i]);
            }

            return newList;
        }
    }
}