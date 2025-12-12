namespace Sequence.Utils
{
    public static class IntExtension
    {
        public static string ToHexadecimal(this int value)
        {
            return value.ToString("X");
        }
    }
}