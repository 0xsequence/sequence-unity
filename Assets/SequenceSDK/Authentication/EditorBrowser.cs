using System.Text.RegularExpressions;
using UnityEngine;

namespace Sequence.Authentication
{
    public class EditorBrowser : IBrowser
    {
        private OpenIdAuthenticator _authenticator;
        private string _state;

        public EditorBrowser(OpenIdAuthenticator authenticator)
        {
            _authenticator = authenticator;
        }
        
        public void SetState(string state)
        {
            _state = state;
        }
        
        public void Authenticate(string url, string redirectUrl = "")
        {
            string authUrl = Regex.Replace(url, @"(&|\?)state=([^&]+)---", "&state=unity-editor---");
            int stateIndex = _state.LastIndexOf("---");
            string stateFragment = _state.Substring(stateIndex);
            _state = "unity-editor" + stateFragment;
            _authenticator.BrowserModifyStateToken(_state);
            Application.OpenURL(authUrl);
            PollForSuccess();
        }

        private void PollForSuccess()
        {
            GameObject successPoller = UnityEngine.Object.Instantiate(new GameObject("SuccessPoller")) as GameObject;
            WebAuthSuccessPoller poller = successPoller.AddComponent<WebAuthSuccessPoller>();
            poller.Setup(_authenticator, OpenIdAuthenticator.RedirectUrl, _state);
            poller.PollForAuthSuccess();
        }
    }
}