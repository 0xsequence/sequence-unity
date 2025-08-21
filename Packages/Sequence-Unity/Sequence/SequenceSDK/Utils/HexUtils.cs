using System.Linq;

namespace Sequence.Utils
{
    public static class HexUtils
    {
        public static string Concat(params string[] values)
        {
            return "0x" + string.Concat(values.Select(v => v.StartsWith("0x") ? v.Substring(2) : v));
        }
    }
}