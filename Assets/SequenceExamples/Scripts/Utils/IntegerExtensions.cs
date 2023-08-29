namespace Sequence.Demo.Utils
{
    public static class FloatExtensions
    {
        public static string GetSignAsString(this float n)
        {
            if (n > 0)
            {
                return "+";
            }

            return "-";
        }
    }
}