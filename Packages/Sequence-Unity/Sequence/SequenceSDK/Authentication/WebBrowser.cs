using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sequence.Utils;
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
            LoginMethod method = _authenticator.GetMethodFromState(_state);
            Dictionary<string, string> queryParams = url.ExtractQueryAndHashParameters();
            if (queryParams.TryGetValue("client_id", out string clientId))
            {
                UnityEngine.GameObject receiver = new UnityEngine.GameObject("WebBrowserMessageReceiver");
                receiver.AddComponent<WebBrowserMessageReceiver>().SetWebBrowser(this);
                ISocialSignIn socialSignIn = WebSocialSignInFactory.Create(method);
                socialSignIn.SignIn(clientId, "");
            }
            else
            {
                _authenticator.InvokeSignInFailed("Social sign in failed: missing client_id, nonce, or state", method);
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
            _authenticator.InvokeSignedIn(new OpenIdAuthenticationResult(idToken, LoginMethod.Google));
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
    
#if UNITY_2017_1_OR_NEWER
    public class WebBrowserMessageReceiver : UnityEngine.MonoBehaviour
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
#endif
}