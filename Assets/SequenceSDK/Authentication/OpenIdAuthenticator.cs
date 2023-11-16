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
        
        private static readonly string GoogleClientId = "970987756660-35a6tc48hvi8cev9cnknp0iugv9poa23.apps.googleusercontent.com";
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
                SignedIn?.Invoke(new OpenIdAuthenticationResult("eyJhbGciOiJSUzI1NiIsImtpZCI6IjViMzcwNjk2MGUzZTYwMDI0YTI2NTVlNzhjZmE2M2Y4N2M5N2QzMDkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiI5NzA5ODc3NTY2NjAtMzVhNnRjNDhodmk4Y2V2OWNua25wMGl1Z3Y5cG9hMjMuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiI5NzA5ODc3NTY2NjAtMzVhNnRjNDhodmk4Y2V2OWNua25wMGl1Z3Y5cG9hMjMuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMTMzMjMxMDU3OTA0MTE1MjgyMTIiLCJoZCI6Imhvcml6b24uaW8iLCJlbWFpbCI6InFwQGhvcml6b24uaW8iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwibmJmIjoxNzAwMTY2MTkwLCJuYW1lIjoiUXVpbm4gUHVyZHkiLCJnaXZlbl9uYW1lIjoiUXVpbm4iLCJmYW1pbHlfbmFtZSI6IlB1cmR5IiwibG9jYWxlIjoiZW4iLCJpYXQiOjE3MDAxNjY0OTAsImV4cCI6MTcwMDE3MDA5MCwianRpIjoiMzE2ZGVjZjZjMjllN2U1Mzc0YWJmYjk3Njk0YmQ2MDcxNzk5YTFjNSJ9.uiGV-mG4FtGH2Y5I2JBzIHJm3v_boEA4d6iRokERG55uZuOxMjnBRk6Na-G9V-es8RQe8-u5uyN3JmhWZeZZXXtPIORe3b0JIaLdow5cp3DOar8RYWIgnQEbd8V_2YkQyxrWF7snBbQNljrKSE2FJeQTLLICN-jR7nWqAMzQKMuN_MT8IuPL-_XqgN55Q0G5reaVxNjMhmKhrGUvUB0ocDHzqV3kgmLNOyM_bP9C6UmFTwP-m4ev-YLcKTWZ2TJIxReP6n5fPmhwLtoKZjm-Qzf4ZpvYdVOGdn9jSUDW2Hj-IUFuVKk3Z9TcV74zCmyyZvN4MAw-egQ19qXXO74NyA"));
                // string discordSignInUrl =
                //     GenerateSignInUrl("https://discord.com/api/oauth2/authorize", DiscordClientId);
                // Application.OpenURL(discordSignInUrl);
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