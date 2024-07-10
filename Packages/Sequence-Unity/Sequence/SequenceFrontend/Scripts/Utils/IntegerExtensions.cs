namespace Sequence.Demo.Utils
{
    public static class NumberExtensions
    {
        public static string AppendSignIfNeeded(this float n)
        {
            if (n > 0)
            {
                return "+";
            }
            return "";
        }
        
        public static string AppendSignIfNeeded(this uint n)
        {
            if (n > 0)
            {
                return "+";
            }
            return "";
        }
    }
}