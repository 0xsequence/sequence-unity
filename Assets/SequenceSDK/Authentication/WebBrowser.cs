using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        private static extern void SocialSignIn(string url);

        public void SetState(string state)
        {
            _state = state;
        }
        
        public void Authenticate(string url, string redirectUrl = "")
        {
            string authUrl = Regex.Replace(url, @"(&|\?)state=([^&]+)---", "&state=unity-web---");
            _state = "unity-web" + _state.Substring(_state.IndexOf("---"));
            _authenticator.BrowserModifyStateToken(_state);
            SocialSignIn(authUrl);
            PollForSuccess();
        }

        private void PollForSuccess()
        {
            GameObject successPoller = UnityEngine.Object.Instantiate(new GameObject("SuccessPoller")) as GameObject;
            WebAuthSuccessPoller poller = successPoller.AddComponent<WebAuthSuccessPoller>();
            poller.Setup(_authenticator, _authenticationUrl, _state);
            poller.PollForAuthSuccess();
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
    }
}