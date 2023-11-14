using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.Authentication
{
    public class OpenIdAuthenticator
    {
        public Action<OpenIdAuthenticationResult> SignedIn;
        
        private static readonly string ClientId = "1041613285238-6hrgdboqrjglsj583njhhseh4b1nh16n.apps.googleusercontent.com";
        private static readonly string RedirectUrl = "https://3d41-142-115-54-118.ngrok-free.app/";
        
        private readonly string _stateToken = Guid.NewGuid().ToString();
        private readonly string _nonce = Guid.NewGuid().ToString();

        public void GoogleSignIn()
        {
            try
            {
                string googleSignInUrl = 
                    $"https://accounts.google.com/o/oauth2/auth?response_type=id_token&client_id={ClientId}&redirect_uri={RedirectUrl.AppendTrailingSlashIfNeeded()}&scope=openid+profile+email&state={_stateToken}&nonce={_nonce}/";
                Application.OpenURL(googleSignInUrl);
            }
            catch (Exception e)
            {
                Debug.LogError($"Google sign in error: {e.Message}");
            }
        }

        public void PlatformSpecificSetup()
        {

        }

        public void HandleDeepLink(string link)
        {
            link = link.RemoveTrailingSlash();
            
            Dictionary<string, string> queryParams = link.ExtractQueryParameters();
            if (queryParams.TryGetValue("state", out string state))
            {
                if (state != _stateToken)
                {
                    Debug.LogError("State token mismatch");
                    return;
                }
            }
            else
            {
                Debug.LogError("State token missing");
                return;
            }
            
            if (queryParams.TryGetValue("id_token", out string idToken))
            {
                SignedIn?.Invoke(new OpenIdAuthenticationResult(idToken));
            }
            else
            {
                Debug.LogError($"Unexpected deep link: {link}");
            }
        }
    }

    public class OpenIdAuthenticationResult
    {
        public string IdToken { get; private set; }

        public OpenIdAuthenticationResult(string idToken)
        {
            IdToken = idToken;
        }
    }
}