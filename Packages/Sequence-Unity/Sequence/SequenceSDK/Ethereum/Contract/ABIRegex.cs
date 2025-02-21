namespace Sequence.Contracts
{
    public static class ABIRegex
    {
        
        public const string FunctionNameRegex = @"^[A-Z|a-z|_,-|0-9]+$";
        public const string FunctionABIRegex = @"^[A-Za-z_][A-Za-z0-9_-]*\((([A-Za-z0-9]+(?:\[\d*\])*|\((?:[A-Za-z0-9]+(?:\[\d*\])*(?:,\s*)?)*\))(?:,\s*([A-Za-z0-9]+(?:\[\d*\])*|\((?:[A-Za-z0-9]+(?:\[\d*\])*(?:,\s*)?)*\)))*)?\)$";
        
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