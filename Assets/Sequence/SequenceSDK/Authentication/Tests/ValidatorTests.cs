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

        [TestCase("123456", true)]
        [TestCase("1234567", false)]
        [TestCase("12345", false)]
        [TestCase("12345a", false)]
        [TestCase("123 45", false)]
        [TestCase("123 456", false)]
        public void TestCodeValidation(string code, bool expectedIsValid)
        {
            IValidator validator = new Validator();
            bool isValid = validator.ValidateCode(code);
            Assert.AreEqual(expectedIsValid, isValid);
        }
    }
}