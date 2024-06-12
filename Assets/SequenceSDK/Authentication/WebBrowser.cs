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
        private string _authenticationUrl;
        private string _state;

        public WebBrowser(OpenIdAuthenticator authenticator, string authenticationUrl)
        {
            _authenticator = authenticator;
            _authenticationUrl = authenticationUrl;
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
            if (queryParams.TryGetValue("client_id", out string clientId) && queryParams.TryGetValue("nonce", out string nonce))
            {
                GameObject receiver = new GameObject("WebBrowserMessageReceiver");
                receiver.AddComponent<WebBrowserMessageReceiver>().SetWebBrowser(this);
                GoogleSignIn(clientId, nonce); // Todo replace this should figure out which provider to use
            }
            else
            {
                _authenticator.OnSignInFailed?.Invoke("Social sign in failed: missing client_id or nonce");
            }
        }
#else
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