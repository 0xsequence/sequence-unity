using NUnit.Framework;
using Sequence.Authentication;

namespace Sequence.Authenticator.Tests
{
    public class ValidatorTests
    {
        [TestCase("test@example.com", true)]
        [TestCase("user123@gmail.com", true)]
        [TestCase("invalid_email", false)]
        [TestCase("user@domain", false)]
        [TestCase("name@.com", false)]
        [TestCase("user@domain.", false)]
        [TestCase("", false)]
        [TestCase("test@.com", false)]
        public void TestEmailValidation(string email, bool expectedIsValid)
        {
            IValidator validator = new Validator();
            bool isValid = validator.ValidateEmail(email);
            Assert.AreEqual(expectedIsValid, isValid);
        }
    }
}