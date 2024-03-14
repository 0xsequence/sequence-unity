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
            
            // CoPilot generated the below code... not sure if I need it yet
            // We may want to try referencing what the unreal team has done
            // AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            // AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            // intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
            // AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            // AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", url);
            // intentObject.Call<AndroidJavaObject>("setData", uriObject);
            // AndroidJavaClass unityPlayer
            //     = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //
            // This is mine
            // AndroidJavaObject activity = GetCurrentActivity();
            // SignInCallback callback = new SignInCallback();
            // callback.SetAuthenticator(_authenticator);
            // activity.CallStatic("signIn", activity, clientId, nonce, callback);
            
            
            AndroidJavaClass credentialManagerClass = new AndroidJavaClass("androidx.credentials.CredentialManager"); // Why can't this get picked up?
            AndroidJavaObject credentialManager = credentialManagerClass.CallStatic<AndroidJavaObject>("getInstance");
            
            
            AndroidJavaObject credentialRequestBuilder = new AndroidJavaObject("androidx.credentials.GetCredentialRequest$Builder");
            credentialRequestBuilder.Call<AndroidJavaObject>("addCredentialOption", GetSignInWithGoogleOption(clientId, nonce));
            AndroidJavaObject credentialRequest = credentialRequestBuilder.Call<AndroidJavaObject>("build");

            SignInCallback callback = new SignInCallback();
            callback.SetAuthenticator(_authenticator);
            
            AndroidJavaObject executor = new AndroidJavaClass("java.util.concurrent.Executors").CallStatic<AndroidJavaObject>("newSingleThreadExecutor");

            credentialManager.Call("getCredentialAsync", 
                new object[] { 
                    credentialRequest, 
                    null, 
                    executor,
                    callback
                });
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
        
        private AndroidJavaObject GetSignInWithGoogleOption(string clientId, string nonce)
        {
            AndroidJavaObject optionBuilder = new AndroidJavaObject("com.google.android.libraries.identity.googleid.GetSignInWithGoogleOption$Builder", clientId);
            optionBuilder.Call<AndroidJavaObject>("setNonce", nonce);
            return optionBuilder.Call<AndroidJavaObject>("build");
        }
 
        private AndroidJavaObject GetCurrentActivity()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        private class SignInCallback : AndroidJavaProxy
        {
            private OpenIdAuthenticator _authenticator;
            
            public SignInCallback() : base("com.sequence.sdk.authentication.GoogleSignIn$SignInCallback") { }
            
            public void SetAuthenticator(OpenIdAuthenticator authenticator)
            {
                _authenticator = authenticator;
            }
            
            public void onResult(AndroidJavaObject credential)
            {
                string idToken = credential.Call<string>("getIdToken");
                if (!string.IsNullOrWhiteSpace(idToken))
                {
                    OnSignInSuccess(idToken);
                }
                else
                {
                    OnSignInFailure("Unable to retrieve idToken");
                }
            }
            
            public void onError(AndroidJavaObject error)
            {
                OnSignInFailure(error.Call<string>("getMessage"));
            }

            private void OnSignInSuccess(string idToken)
            {
                _authenticator.SignedIn?.Invoke(new OpenIdAuthenticationResult(idToken, LoginMethod.Google));
            }
            
            private void OnSignInFailure(string error)
            {
                Debug.LogError("Google sign in failure: " + error);
            }
        }
    }
}