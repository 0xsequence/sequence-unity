using System;
using System.Collections.Generic;

namespace Sequence.Utils
{
    public static class EnumExtensions
    {
        private static Random random = new Random();

        public static T GetRandomEnumValue<T>()
        {
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(random.Next(values.Length));
        }

        public static List<T> GetEnumValuesAsList<T>()
        {
            Array values = Enum.GetValues(typeof(T));
            int length = values.Length;
            List<T> newList = new List<T>();
            for (int i = 0; i < length; i++)
            {
                newList.Add((T)values.GetValue(i));
            }

            return newList;
        }
    }
}