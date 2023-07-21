using System.Text;

namespace Sequence.Extensions
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
    }
}