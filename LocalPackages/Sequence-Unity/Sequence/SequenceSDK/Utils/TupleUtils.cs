using System;

namespace Sequence.Utils
{
    public static class TupleUtils
    {
        public static bool IsTuple<T>()
        {
            Type type = typeof(T);
            return type.IsValueType && type.IsGenericType && 
                   (type.GetGenericTypeDefinition() == typeof(Tuple<>) || type.GetGenericTypeDefinition() == typeof(ValueTuple<>));
        }
    }
}