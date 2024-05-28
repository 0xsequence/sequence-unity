using NUnit.Framework;
using Sequence.Authentication;
using UnityEngine;
using UnityEngine.TestTools;

namespace Sequence.Authenticator.Tests
{
    public class DeepLinkHandlerTests
    {
        [Test]
        public void TestNoQueryParams()
        {
            OpenIdAuthenticator authenticator = new OpenIdAuthenticator("");
            string url = "https://sequence.app";
            bool signInFailedEventReceived = false;
            authenticator.OnSignInFailed += s =>
            {
                signInFailedEventReceived = true;
                Assert.AreEqual("Unexpected deep link: https://sequence.app", s);
            };
            authenticator.HandleDeepLink(url);
            Assert.IsTrue(signInFailedEventReceived);
        }
        
        [Test]
        public void TestNoStateToken()
        {
            OpenIdAuthenticator authenticator = new OpenIdAuthenticator("");
            string url = "https://sequence.app?code=123456";
            bool signInFailedEventReceived = false;
            authenticator.OnSignInFailed += s =>
            {
                signInFailedEventReceived = true;
                Assert.AreEqual("State token missing", s);
            };
            authenticator.HandleDeepLink(url);
            Assert.IsTrue(signInFailedEventReceived);
        }

        [Test]
        public void TestStateTokenMismatch()
        {
            OpenIdAuthenticator authenticator = new OpenIdAuthenticator("");
            string url = "https://sequence.app?code=123456&state=123456";
            bool signInFailedEventReceived = false;
            authenticator.OnSignInFailed += s =>
            {
                signInFailedEventReceived = true;
                Assert.AreEqual("State token mismatch", s);
            };
            authenticator.HandleDeepLink(url);
            Assert.IsTrue(signInFailedEventReceived);
        }
        
        [Test]
        public void TestNoIdToken()
        {
            OpenIdAuthenticator authenticator = new OpenIdAuthenticator("");
            string url = "https://sequence.app?state=123456";
            authenticator.InjectStateTokenForTesting("123456");
            bool signInFailedEventReceived = false;
            authenticator.OnSignInFailed += s =>
            {
                signInFailedEventReceived = true;
                Assert.AreEqual("Unexpected deep link: https://sequence.app?state=123456", s);
            };
            authenticator.HandleDeepLink(url);
            Assert.IsTrue(signInFailedEventReceived);
        }

        [Test]
        public void TestValidDeepLink()
        {
            OpenIdAuthenticator authenticator = new OpenIdAuthenticator("");
            string url = "https://sequence.app?state=123456&id_token=654321";
            authenticator.InjectStateTokenForTesting("123456");
            bool eventReceived = false;
            authenticator.SignedIn += (result) =>
            {
                Assert.AreEqual("654321", result.IdToken);
                eventReceived = true;
            };
            authenticator.HandleDeepLink(url);
            Assert.IsTrue(eventReceived);
        }

        [Test]
        public void TestValidDeepLink_withTrailingSlash()
        {
            OpenIdAuthenticator authenticator = new OpenIdAuthenticator("");
            string url = "https://sequence.app?state=123456&id_token=654321/";
            authenticator.InjectStateTokenForTesting("123456");
            bool eventReceived = false;
            authenticator.SignedIn += (result) =>
            {
                Assert.AreEqual("654321", result.IdToken);
                eventReceived = true;
            };
            authenticator.HandleDeepLink(url);
            Assert.IsTrue(eventReceived);
        }
    }
}