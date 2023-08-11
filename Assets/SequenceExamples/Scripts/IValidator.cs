namespace Sequence.Demo
{
    public interface IValidator
    {
        /// <summary>
        /// Returns true iff an email is in correct form
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool ValidateEmail(string email);
    }
}