using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Sequence.Authentication
{
    public class Validator : IValidator
    {
        public bool ValidateEmail(string email)
        {
            try
            {
                MailAddress mailAddress = new MailAddress(email);
                bool isValid = mailAddress.Address == email;
                return isValid;
            }
            catch
            {
                return false;
            }
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