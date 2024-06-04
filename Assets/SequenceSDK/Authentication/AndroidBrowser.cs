using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sequence.Authentication
{
    public class AndroidBrowser : IBrowser
    {
        private OpenIdAuthenticator _authenticator;
        
        public AndroidBrowser(OpenIdAuthenticator authenticator)
        {
            _authenticator = authenticator;
        }
        
        public void Authenticate(string url, string redirectUrl = "")
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            (string, string) clientIdAndNonceFromUrl = ExtractClientIdAndNonceFromUrl(url);
            string clientId = clientIdAndNonceFromUrl.Item1;
            string nonce = clientIdAndNonceFromUrl.Item2;

            AndroidJavaClass googleSignIn = new AndroidJavaClass("xyz.sequence.unitysdk.GoogleSignIn");
            AndroidJavaObject activity = GetCurrentActivity();
            SignInCallback callback = new SignInCallback(_authenticator);
            googleSignIn.CallStatic("signIn", activity, clientId, nonce, callback);
        }

        private (string, string) ExtractClientIdAndNonceFromUrl(string url)
        {
            Dictionary<string, string> queryParams = url.ExtractQueryAndHashParameters();
            string clientId = queryParams["client_id"];
            string nonce = queryParams["nonce"];
            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(nonce))
            {
                throw new ArgumentException("Invalid URL: missing client_id or nonce. Given: " + url);
            }
            return (clientId, nonce);
        }
 
        private AndroidJavaObject GetCurrentActivity()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        private class SignInCallback : AndroidJavaProxy
        {
            private OpenIdAuthenticator _authenticator;

            public SignInCallback(OpenIdAuthenticator authenticator) : base("xyz.sequence.unitysdk.SignInCallback")
            {
                _authenticator = authenticator;
            }

            public void onSuccess(string idToken)
            {
                _authenticator.SignedIn?.Invoke(new OpenIdAuthenticationResult(idToken, LoginMethod.Google));
            }
            
            public void onError(string error)
            {
                Debug.LogError("Google sign in failure: " + error);
            }
        }
    }
}