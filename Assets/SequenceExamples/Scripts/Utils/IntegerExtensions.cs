namespace Sequence.Demo.Utils
{
    public static class FloatExtensions
    {
        public static string AppendSignIfNeeded(this float n)
        {
            if (n > 0)
            {
                return "+";
            }
            return "";
        }
    }
}