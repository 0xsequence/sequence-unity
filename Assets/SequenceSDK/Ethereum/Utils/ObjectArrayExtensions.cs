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
    }
}