namespace Sequence.Authentication
{
    public interface IValidator
    {
        /// <summary>
        /// Returns true iff an email is in correct form
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool ValidateEmail(string email);
        
        /// <summary>
        /// Returns true if an OTP code is in correct form
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool ValidateCode(string code);
    }
}