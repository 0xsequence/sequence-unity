using System.Text.RegularExpressions;

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
#if UNITY_2017_1_OR_NEWER
            UnityEngine.Application.OpenURL(authUrl);
#endif
            PollForSuccess();
        }

        private void PollForSuccess()
        {
#if UNITY_2017_1_OR_NEWER
            UnityEngine.GameObject successPoller = UnityEngine.Object.Instantiate(new UnityEngine.GameObject("SuccessPoller"));
            WebAuthSuccessPoller poller = successPoller.AddComponent<WebAuthSuccessPoller>();
            poller.Setup(_authenticator, _authenticator.GetRedirectUrl(), _state);
            poller.PollForAuthSuccess();
#endif
        }
    }
}