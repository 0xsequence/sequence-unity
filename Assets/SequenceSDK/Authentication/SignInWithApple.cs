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

        public void LoginToApple(OpenIdAuthenticator authenticator, string nonce, string state)
        {
            // Initialize the Apple Auth Manager
            if (m_AppleAuthManager == null)
            {
                Initialize();
            }

            // Set the login arguments
            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName,
                nonce, state);

            // Perform the login
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
                            Debug.LogError(Error);
                        }
                        
                        authenticator.SignedIn?.Invoke(new OpenIdAuthenticationResult(Token, LoginMethod.Apple));
                    }
                    else
                    {
                        Error = "Sign-in with Apple error. Message: appleIDCredential is null";
                        Debug.LogError(Error);
                    }
                },
                error =>
                {
                    Error = "Sign-in with Apple error. Message: " + error;
                    Debug.LogError(Error);
                }
            );
        }
    }
}