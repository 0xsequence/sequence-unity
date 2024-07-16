using System;
using NUnit.Framework;
using Sequence.Authentication;

namespace Sequence.WaaS.Tests
{
    public class InitiateEmailSignInUnitTests
    {
        private string _email = "email@domain.com";

        [Test]
        public void TestLogin_Success()
        {
            SequenceLogin waasLogin = SequenceLogin.GetInstance(null, null, new MockWaaSConnector("success"));
            
            waasLogin.OnMFAEmailFailedToSend += (email, error) => Assert.Fail($"Unexpected error: {error}");
            waasLogin.OnMFAEmailSent += (email) => Assert.Pass();
            
            waasLogin.Login(_email);
        }

        [Test]
        public void TestLogin_MissingChallenge()
        {
            SequenceLogin waasLogin = SequenceLogin.GetInstance(null, null, new MockWaaSConnector(""));
            
            waasLogin.OnMFAEmailFailedToSend += (email, error) => Assert.AreEqual("Email challenge is missing from response from WaaS API", error);
            waasLogin.OnMFAEmailSent += (email) => Assert.Fail($"Unexpected success");
            
            waasLogin.Login(_email);
        }

        [Test]
        public void TestLogin_UnexpectedError()
        {
            SequenceLogin waasLogin = SequenceLogin.GetInstance(null, null, new MockWaaSConnector("Error: something happened"));
            
            waasLogin.OnMFAEmailFailedToSend += (email, error) => Assert.AreEqual("Error: something happened", error);
            waasLogin.OnMFAEmailSent += (email) => Assert.Fail($"Unexpected success");
            
            waasLogin.Login(_email);
        }

        [Test]
        public void TestLogin_ExceptionThrown()
        {
            SequenceLogin waasLogin = SequenceLogin.GetInstance(null, null, new MockWaaSConnector(new Exception("Error: something happened")));
            
            waasLogin.OnMFAEmailFailedToSend += (email, error) => Assert.AreEqual("Error: something happened", error);
            waasLogin.OnMFAEmailSent += (email) => Assert.Fail($"Unexpected success");
            
            waasLogin.Login(_email);
        }
    }
}