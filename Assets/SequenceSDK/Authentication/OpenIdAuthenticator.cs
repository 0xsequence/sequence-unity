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
        
        private static readonly string GoogleClientId = "1041613285238-6hrgdboqrjglsj583njhhseh4b1nh16n.apps.googleusercontent.com";
        private static readonly string DiscordClientId = ""; // Todo replace
        private static readonly string FacebookClientId = ""; // Todo replace
        private static readonly string AppleClientId = ""; // Todo replace
        private static readonly string RedirectUrl = "https://3d41-142-115-54-118.ngrok-free.app/";
        
        private readonly string _stateToken = Guid.NewGuid().ToString();
        private readonly string _nonce = Guid.NewGuid().ToString();

        public void GoogleSignIn()
        {
            try
            {
                string googleSignInUrl = GenerateSignInUrl("https://accounts.google.com/o/oauth2/auth", GoogleClientId);
                Application.OpenURL(googleSignInUrl);
            }
            catch (Exception e)
            {
                Debug.LogError($"Google sign in error: {e.Message}");
            }
        }
        
        public void DiscordSignIn()
        {
            try
            {
                string discordSignInUrl =
                    GenerateSignInUrl("https://discord.com/api/oauth2/authorize", DiscordClientId);
                Application.OpenURL(discordSignInUrl);
            }
            catch (Exception e)
            {
                Debug.LogError($"Discord sign in error: {e.Message}");
            }
        }

        public void FacebookSignIn()
        {
            try
            {
                string facebookSignInUrl =
                    GenerateSignInUrl("https://www.facebook.com/v18.0/dialog/oauth", FacebookClientId);
                Application.OpenURL(facebookSignInUrl);
            }
            catch (Exception e)
            {
                Debug.LogError($"Facebook sign in error: {e.Message}");
            }
        }
        
        public void AppleSignIn()
        {
            try
            {
                string appleSignInUrl =
                    GenerateSignInUrl("https://appleid.apple.com/auth/authorize", AppleClientId);
                Application.OpenURL(appleSignInUrl);
            }
            catch (Exception e)
            {
                Debug.LogError($"Apple sign in error: {e.Message}");
            }
        }

        private string GenerateSignInUrl(string baseUrl, string clientId)
        {
            return
                $"{baseUrl}?response_type=id_token&client_id={clientId}&redirect_uri={RedirectUrl.AppendTrailingSlashIfNeeded()}&scope=openid+profile+email&state={_stateToken}&nonce={_nonce}/";
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