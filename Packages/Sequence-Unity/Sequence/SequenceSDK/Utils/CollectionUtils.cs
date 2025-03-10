using System;
using System.Collections;
using System.Collections.Generic;

namespace Sequence.Utils
{
    public static class CollectionUtils
    {
        public static bool IsCollection<T>()
        {
            Type type = typeof(T);
            return IsCollection(type);
        }

        public static bool IsCollection(Type type)
        {
            return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
        }
        
        public static Type GetUnderlyingType<T>()
        {
            Type type = typeof(T);
            if (IsCollection<T>())
            {
                // Check if the type is a generic collection (List<T>, HashSet<T>, etc.)
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return type.GetGenericArguments()[0];
                }

                // Check if the type implements the non-generic IEnumerable interface (e.g., arrays)
                if (!type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type))
                {
                    return type.GetElementType();
                }
            }

            throw new ArgumentException($"T {typeof(T)} is not a collection type.");
        }
        
        public static List<object> CreateListOfType(Type elementType)
        {
            Type listType = typeof(List<>).MakeGenericType(elementType);
            return (List<object>)Activator.CreateInstance(listType);
        }

        public static T GetRandomObjectFromArray<T>(this T[] arr)
        {
            int randomIndex = AppEnvironment.GetRandomNumber(0, arr.Length);
            return arr[randomIndex];
        }
    }
}