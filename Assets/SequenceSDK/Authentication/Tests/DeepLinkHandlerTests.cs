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
            authenticator.HandleDeepLink(url);
            LogAssert.Expect(LogType.Error, "Unexpected deep link: https://sequence.app");
        }
        
        [Test]
        public void TestNoStateToken()
        {
            OpenIdAuthenticator authenticator = new OpenIdAuthenticator("");
            string url = "https://sequence.app?code=123456";
            authenticator.HandleDeepLink(url);
            LogAssert.Expect(LogType.Error, "State token missing");
        }

        [Test]
        public void TestStateTokenMismatch()
        {
            OpenIdAuthenticator authenticator = new OpenIdAuthenticator("");
            string url = "https://sequence.app?code=123456&state=123456";
            authenticator.HandleDeepLink(url);
            LogAssert.Expect(LogType.Error, "State token mismatch");
        }
        
        [Test]
        public void TestNoIdToken()
        {
            OpenIdAuthenticator authenticator = new OpenIdAuthenticator("");
            string url = "https://sequence.app?state=123456";
            authenticator.InjectStateTokenForTesting("123456");
            authenticator.HandleDeepLink(url);
            LogAssert.Expect(LogType.Error, "Unexpected deep link: https://sequence.app?state=123456");
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