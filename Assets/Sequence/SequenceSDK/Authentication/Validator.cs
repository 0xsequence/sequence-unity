using System.Text.RegularExpressions;

namespace Sequence.Authentication
{
    public class Validator : IValidator
    {
        public bool ValidateEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        public bool ValidateCode(string code)
        {
            int length = code.Length;
            if (length != 6)
            {
                return false;
            }
            for (int i = 0; i < length; i++)
            {
                if (!char.IsDigit(code[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}