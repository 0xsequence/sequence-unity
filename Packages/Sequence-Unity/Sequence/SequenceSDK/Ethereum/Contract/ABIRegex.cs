namespace Sequence.Contracts
{
    public static class ABIRegex
    {
        
        public const string FunctionNameRegex = @"^[A-Z|a-z|_,-|0-9]+$";
        public const string FunctionABIRegex = @"^[A-Z|a-z|_,-|0-9]+\(([A-Za-z0-9\[\]]+(, *[A-Za-z0-9\[\]]+)*)?\)$";
        
        public static bool MatchesFunctionName(string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, FunctionNameRegex);
        }
        
        public static bool MatchesFunctionABI(string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, FunctionABIRegex);
        }
    }
}