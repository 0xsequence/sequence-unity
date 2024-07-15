using System;

namespace Sequence.Authentication
{
    public interface IAuthenticator
    {
        public void PlatformSpecificSetup();
        public void HandleDeepLink(string url);
        public event Action<OpenIdAuthenticationResult> SignedIn;
        public event Action<string, LoginMethod> OnSignInFailed;
        public void GoogleSignIn();
        public void DiscordSignIn();
        public void FacebookSignIn();
        public void AppleSignIn();
        public void InvokeSignedIn(OpenIdAuthenticationResult result);
        public void AppleInvokeSignInFailed(string errorMessage);
        public string GetRedirectUrl();
    }

    public class OpenIdAuthenticationResult
    {
        public string IdToken { get; private set; }
        public LoginMethod Method { get; private set; }

        public OpenIdAuthenticationResult(string idToken, LoginMethod method)
        {
            IdToken = idToken;
            Method = method;
        }
    }
}