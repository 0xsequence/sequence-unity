using System.Threading.Tasks;
using Sequence.Authentication;
using UnityEngine;

namespace Sequence.WaaS
{
    public class WaaSLogin : ILogin 
    {
        public event ILogin.OnLoginSuccessHandler OnLoginSuccess;
        public event ILogin.OnLoginFailedHandler OnLoginFailed;
        public event ILogin.OnMFAEmailSentHandler OnMFAEmailSent;
        public event ILogin.OnMFAEmailFailedToSendHandler OnMFAEmailFailedToSend;
        public async Task Login(string email)
        {
            Debug.LogError("Not Implemented... mocking for now");
            await new MockLogin().Login(email);
        }

        public async Task Login(string email, string code)
        {
            Debug.LogError("Not Implemented... mocking for now");
            await new MockLogin().Login(email, code);
        }

        public void GoogleLogin()
        {
            OpenIdAuthenticator authenticator = new OpenIdAuthenticator();
            authenticator.PlatformSpecificSetup();
            Application.deepLinkActivated += authenticator.HandleDeepLink;
            authenticator.SignedIn += OnSocialLogin;
            authenticator.GoogleSignIn();
        }

        private void OnSocialLogin(OpenIdAuthenticationResult result)
        {
            ConnectToWaaS(result.IdToken);
        }

        public async Task ConnectToWaaS(string idToken)
        {
            Debug.LogError($"idToken: {idToken}");
        }
    }
}