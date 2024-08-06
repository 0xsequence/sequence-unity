using System;
using System.Text;

namespace Sequence.Utils
{
    public static class ObjectArrayExtensions
    {
        public static string ExpandToString(this object[] value)
        {
            if (value == null)
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < value.Length; i++)
            {
                sb.Append(value[i].ToString());

                if (i < value.Length - 1)
                    sb.Append(", ");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the first instance of an object of type T in args
        /// Returns default otherwise
        /// </summary>
        /// <param name="args"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetObjectOfTypeIfExists<T>(this object[] args)
        {
            if (args == null)
            {
                return default;
            }
            int length = args.Length;
            for (int i = 0; i < length; i++)
            {
                if (args[i] is T item)
                {
                    return item;
                }
            }

            return default;
        }

        public static object[] AppendObject<T>(this object[] arr, T obj)
        {
            if (arr == null)
            {
                return new object[] { obj };
            }

            int length = arr.Length;
            object[] newArr = new object[length + 1];
            for (int i = 0; i < length; i++)
            {
                newArr[i] = arr[i];
            }

            newArr[length] = obj;

            return newArr;
        }

        public static object[] AppendArray(this object[] arr1, object[] arr2)
        {
            if (arr1 == null)
            {
                if (arr2 == null)
                {
                    return new object[] { };
                }
                return arr2;
            }

            if (arr2 == null)
            {
                return arr1;
            }
            
            int length1 = arr1.Length;
            int length2 = arr2.Length;
            object[] newArr = new object[length1 + length2];
            for (int i = 0; i < length1; i++)
            {
                newArr[i] = arr1[i];
            }

            for (int i = 0; i < length2; i++)
            {
                newArr[i + length1] = arr2[i];
            }

            return newArr;
        }
        
        
    }
}