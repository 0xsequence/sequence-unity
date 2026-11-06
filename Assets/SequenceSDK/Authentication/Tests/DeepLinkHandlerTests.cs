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
            authenticator.OnSignInFailed += (s, method) =>
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
            authenticator.OnSignInFailed += (s, method) =>
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
            // A state no authenticator issued. Other tests issue "123456", and a state issued by another
            // authenticator is left to that authenticator (see TestStateIssuedByOtherAuthenticator_isIgnored).
            string url = $"https://sequence.app?code=123456&state={System.Guid.NewGuid()}";
            bool signInFailedEventReceived = false;
            authenticator.OnSignInFailed += (s, method) =>
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
            authenticator.OnSignInFailed += (s, method) =>
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

        // Two authenticators exist when there is more than one SequenceLogin (SequenceLogin.CreateIsolatedInstance).
        [Test]
        public void TestStateIssuedByOtherAuthenticator_isIgnored()
        {
            OpenIdAuthenticator starter = new OpenIdAuthenticator("");
            OpenIdAuthenticator other = new OpenIdAuthenticator("");
            string token = System.Guid.NewGuid().ToString();
            starter.InjectStateTokenForTesting(token);
            other.InjectStateTokenForTesting(System.Guid.NewGuid().ToString());
            string url = $"https://sequence.app?state={token}&id_token=654321";
            int starterSignedIn = 0, otherSignedIn = 0, otherFailed = 0;
            starter.SignedIn += _ => starterSignedIn++;
            other.SignedIn += _ => otherSignedIn++;
            other.OnSignInFailed += (s, m) => otherFailed++;

            // Application.deepLinkActivated delivers the link to every authenticator, in either order.
            other.HandleDeepLink(url);
            starter.HandleDeepLink(url);

            Assert.AreEqual(1, starterSignedIn);
            Assert.AreEqual(0, otherSignedIn);
            Assert.AreEqual(0, otherFailed);
        }

        [Test]
        public void TestRouteDeepLink_deliversToIssuer()
        {
            OpenIdAuthenticator receiver = new OpenIdAuthenticator("");
            OpenIdAuthenticator issuer = new OpenIdAuthenticator("");
            string token = System.Guid.NewGuid().ToString();
            receiver.InjectStateTokenForTesting(System.Guid.NewGuid().ToString());
            issuer.InjectStateTokenForTesting(token);
            int issuerSignedIn = 0, receiverSignedIn = 0, receiverFailed = 0;
            issuer.SignedIn += _ => issuerSignedIn++;
            receiver.SignedIn += _ => receiverSignedIn++;
            receiver.OnSignInFailed += (s, m) => receiverFailed++;

            // Windows IPC and the iOS authentication session hand the link to one authenticator only.
            OpenIdAuthenticator.RouteDeepLink($"https://sequence.app?state={token}&id_token=654321", receiver);

            Assert.AreEqual(1, issuerSignedIn);
            Assert.AreEqual(0, receiverSignedIn);
            Assert.AreEqual(0, receiverFailed);
        }

        [Test]
        public void TestRouteDeepLink_unknownState_goesToFallback()
        {
            OpenIdAuthenticator receiver = new OpenIdAuthenticator("");
            string error = null;
            receiver.OnSignInFailed += (s, m) => error = s;

            OpenIdAuthenticator.RouteDeepLink($"https://sequence.app?state={System.Guid.NewGuid()}&id_token=654321", receiver);

            Assert.AreEqual("State token mismatch", error);
        }
    }
}
