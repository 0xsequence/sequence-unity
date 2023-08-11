namespace Sequence.Demo
{
    public class MockValidator : IValidator
    {
        public bool ValidateEmail(string email)
        {
            return email != "invalidEmail"; 
        }
    }
}