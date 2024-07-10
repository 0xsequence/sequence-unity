using System;
using System.Text;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using UnityEngine;

namespace Sequence.Authentication
{
    public class SignInWithApple : MonoBehaviour
    {
        IAppleAuthManager m_AppleAuthManager;
        public string Token { get; private set; }
        public string Error { get; private set; }

        private void Initialize()
        {
            var deserializer = new PayloadDeserializer();
            m_AppleAuthManager = new AppleAuthManager(deserializer);
        }

        private void Update()
        {
            if (m_AppleAuthManager != null) 
            {
                m_AppleAuthManager.Update();
            }
        }

        public void LoginToApple(IAuthenticator authenticator, string nonce, string state)
        {
            if (m_AppleAuthManager == null)
            {
                Initialize();
            }

            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName,
                nonce, state);

            m_AppleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                    var appleIDCredential = credential as IAppleIDCredential;
                    if (appleIDCredential != null)
                    {
                        var idToken = Encoding.UTF8.GetString(
                            appleIDCredential.IdentityToken,
                            0,
                            appleIDCredential.IdentityToken.Length);
                        Token = idToken;

                        string receivedState = appleIDCredential.State;
                        if (receivedState != state)
                        {
                            Error =
                                "Sign-in with Apple error. Message: state token received doesn't match what was given";
                            authenticator.InvokeSignInFailed(Error);
                        }
                        
                        authenticator.InvokeSignedIn(new OpenIdAuthenticationResult(Token, LoginMethod.Apple));
                    }
                    else
                    {
                        Error = "Sign-in with Apple error. Message: appleIDCredential is null";
                        authenticator.InvokeSignInFailed(Error);
                    }
                },
                error =>
                {
                    Error = "Sign-in with Apple error. Message: " + error;
                    authenticator.InvokeSignInFailed(Error);
                }
            );
        }
    }
}