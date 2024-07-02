using System;

namespace Sequence.Authentication
{
    public interface IAuthenticator
    {
        public void PlatformSpecificSetup();
        public void HandleDeepLink(string url);
        public event Action<OpenIdAuthenticationResult> SignedIn;
        public event Action<string> OnSignInFailed;
        public void GoogleSignIn();
        public void DiscordSignIn();
        public void FacebookSignIn();
        public void AppleSignIn();
        public void InvokeSignedIn(OpenIdAuthenticationResult result);
        public void InvokeSignInFailed(string errorMessage);
        public string GetRedirectUrl();
        public void SetNonce(string nonce);
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