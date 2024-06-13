using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Object = System.Object;

namespace Sequence.Authentication
{
    public class WebBrowser : IBrowser
    {
        private OpenIdAuthenticator _authenticator;
        private string _state;

        public WebBrowser(OpenIdAuthenticator authenticator)
        {
            _authenticator = authenticator;
        }

#if UNITY_WEBGL 
        [DllImport("__Internal")]
        private static extern void GoogleSignIn(string googleClientId, string nonce);

        public void SetState(string state)
        {
            _state = state;
        }
        
        public void Authenticate(string url, string redirectUrl = "")
        {
            Dictionary<string, string> queryParams = url.ExtractQueryAndHashParameters();
            if (queryParams.TryGetValue("client_id", out string clientId) && queryParams.TryGetValue("nonce", out string nonce) && queryParams.TryGetValue("state", out string state))
            {
                GameObject receiver = new GameObject("WebBrowserMessageReceiver");
                receiver.AddComponent<WebBrowserMessageReceiver>().SetWebBrowser(this);
                ISocialSignIn socialSignIn = WebSocialSignInFactory.Create(_authenticator.GetMethodFromState(state));
                socialSignIn.SignIn(clientId, nonce);
            }
            else
            {
                _authenticator.OnSignInFailed?.Invoke("Social sign in failed: missing client_id, nonce, or state");
            }
        }
#else
        private static void GoogleSignIn(string clientId, string nonce)
        {
            throw new NotImplementedException("Google sign in is only supported on Web platforms.");
        }
        
        public void Authenticate(string url, string redirectUrl = "")
        {
            throw new NotImplementedException("Web browser is only supported on Web platforms.");
        }

        public void SetState(string state)
        {
            
        }
#endif

        public void OnGoogleSignIn(string idToken)
        {
            _authenticator.SignedIn?.Invoke(new OpenIdAuthenticationResult(idToken, LoginMethod.Google));
        }

        private static class WebSocialSignInFactory
        {
            public static ISocialSignIn Create(LoginMethod method)
            {
                switch (method)
                {
                    case LoginMethod.Google:
                        return new WebGoogleSignIn();
                    default:
                        throw new NotImplementedException("Social sign in method is not implemented on web platforms for the specified LoginMethod " + method);
                }
            }
        }
        
        private interface ISocialSignIn
        {
            public void SignIn(string clientId, string nonce);
        }
        
        private class WebGoogleSignIn : ISocialSignIn
        {
            public void SignIn(string clientId, string nonce)
            {
                GoogleSignIn(clientId, nonce);
            }
        }
    }
    
    public class WebBrowserMessageReceiver : MonoBehaviour
    {
        private WebBrowser _webBrowser;

        public void SetWebBrowser(WebBrowser webBrowser)
        {
            _webBrowser = webBrowser;
        }

        public void OnGoogleSignIn(string idToken)
        {
            _webBrowser.OnGoogleSignIn(idToken);
            Destroy(gameObject);
        }
    }
}