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
        
        private static readonly string ClientId = "851462432325-q9r1fl22ubes0nhopllpulocfn2fi57h.apps.googleusercontent.com";
        private static readonly string RedirectUrl = "https://d625-70-27-5-107.ngrok-free.app/";
        
        private readonly string _stateToken = Guid.NewGuid().ToString();
        private readonly string _nonce = Guid.NewGuid().ToString();

        public void GoogleSignIn()
        {
            try
            {
                string googleSignInUrl = 
                    $"https://accounts.google.com/o/oauth2/auth?response_type=code&client_id={ClientId}&redirect_uri={RedirectUrl.AppendTrailingSlashIfNeeded()}&scope=openid+profile+email&state={_stateToken}&nonce={_nonce}/";
                Application.OpenURL(googleSignInUrl);
            }
            catch (Exception e)
            {
                Debug.LogError($"Google sign in error: {e.Message}");
            }
        }

        public void PlatformSpecificSetup()
        {
#if UNITY_STANDALONE_OSX
            // ensure our URL protocol handler is registered - MacOS sometimes doesn't pick it up from Info.plist.
            var appPath = System.IO.Directory.GetParent(Application.dataPath);
            var command = new System.Diagnostics.ProcessStartInfo();
            command.FileName = "/System/Library/Frameworks/CoreServices.framework/Versions/A/Frameworks/LaunchServices.framework/Versions/A/Support/lsregister";
            command.Arguments = " -R -f " + appPath;
            command.UseShellExecute = false;
            command.CreateNoWindow = true;
            System.Diagnostics.Process.Start(command);
#endif
        }

        public void HandleDeepLink(string link)
        {
            Debug.LogError($"Deep link: {link}");
            link = RemoveTrailingSlash(link);
            
            Dictionary<string, string> queryParams = ExtractQueryParameters(link);
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
            
            if (queryParams.TryGetValue("id_token", out string idToken) &&queryParams.TryGetValue("access_token", out string accessToken) && queryParams.TryGetValue("refresh_token", out string refreshToken))
            {
                SignedIn?.Invoke(new OpenIdAuthenticationResult(idToken, accessToken, refreshToken));
            }
            else
            {
                Debug.LogError($"Unexpected deep link: {link}");
            }
        }
        
        private string RemoveTrailingSlash(string link)
        {
            if (link.EndsWith("/"))
            {
                link = link.Remove(link.Length - 1);
            }

            return link;
        }

        private Dictionary<string, string> ExtractQueryParameters(string link)
        {
            string[] urlSegments = link.Split('?');
            string[] parameters = urlSegments[1].Split('&');
            Dictionary<string, string> queryParameters = new Dictionary<string, string>();
            int totalParameters = parameters.Length;
            for (int i = 0; i < totalParameters; i++)
            {
                string[] keyValue = parameters[i].Split('=');
                queryParameters.Add(keyValue[0], keyValue[1]);
            }

            return queryParameters;
        }
    }

    public class OpenIdAuthenticationResult
    {
        public string IdToken { get; private set; }
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }

        public OpenIdAuthenticationResult(string idToken, string accessToken, string refreshToken)
        {
            IdToken = idToken;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}