using System;

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
    }
}