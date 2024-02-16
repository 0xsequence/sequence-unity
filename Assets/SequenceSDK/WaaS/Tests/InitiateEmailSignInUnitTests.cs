using NUnit.Framework;
using Sequence.Authentication;

namespace Sequence.WaaS.Tests
{
    public class InitiateEmailSignInUnitTests
    {
        private string _email = "email@domain.com";
        
        private WaaSLogin _waasLogin = new WaaSLogin();
        
        [Test]
        public void TestLogin_Success()
        {
            _waasLogin.InjectEmailSignIn(new MockEmailSignIn("success"));
            
            _waasLogin.OnMFAEmailFailedToSend += (email, error) => Assert.Fail($"Unexpected error: {error}");
            _waasLogin.OnMFAEmailSent += (email) => Assert.Pass();
            
            _waasLogin.Login(_email);
        }

        [Test]
        public void TestLogin_UnknownError()
        {
            _waasLogin.InjectEmailSignIn(new MockEmailSignIn(""));
            
            _waasLogin.OnMFAEmailFailedToSend += (email, error) => Assert.AreEqual("Unknown error establishing AWS sessionId", error);
            _waasLogin.OnMFAEmailSent += (email) => Assert.Fail($"Unexpected success");
            
            _waasLogin.Login(_email);
        }

        [Test]
        public void TestLogin_SignUpAndRetrySuccess()
        {
            _waasLogin.InjectEmailSignIn(new MockEmailSignIn("user not found", "success"));
            
            _waasLogin.OnMFAEmailFailedToSend += (email, error) => Assert.Fail($"Unexpected error: {error}");
            _waasLogin.OnMFAEmailSent += (email) => Assert.Pass();
            
            _waasLogin.Login(_email);
        }
        
        [Test]
        public void TestLogin_SignUpAndRetryFail()
        {
            _waasLogin.InjectEmailSignIn(new MockEmailSignIn("user not found", "user not found"));
            
            _waasLogin.OnMFAEmailFailedToSend += (email, error) => Assert.AreEqual("user not found", error);
            _waasLogin.OnMFAEmailSent += (email) => Assert.Fail($"Unexpected success");
            
            _waasLogin.Login(_email);
        }

        [Test]
        public void TestLogin_UnexpectedError()
        {
            _waasLogin.InjectEmailSignIn(new MockEmailSignIn("Error: something happened"));
            
            _waasLogin.OnMFAEmailFailedToSend += (email, error) => Assert.AreEqual("Error: something happened", error);
            _waasLogin.OnMFAEmailSent += (email) => Assert.Fail($"Unexpected success");
            
            _waasLogin.Login(_email);
        }
    }
}