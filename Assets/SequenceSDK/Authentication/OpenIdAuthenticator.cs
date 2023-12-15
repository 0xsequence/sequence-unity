using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using Sequence.Config;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.Authentication
{
    public class OpenIdAuthenticator
    {
        public const string LoginEmail = "LoginEmail";
        public Action<OpenIdAuthenticationResult> SignedIn;
        private string _urlScheme;
        
        public static readonly int WINDOWS_IPC_PORT = 52836;
        
        private static readonly string GoogleClientId = "970987756660-35a6tc48hvi8cev9cnknp0iugv9poa23.apps.googleusercontent.com";
        private static readonly string DiscordClientId = ""; // Todo replace
        private static readonly string FacebookClientId = ""; // Todo replace
        private static readonly string AppleClientId = ""; // Todo replace
        private static readonly string RedirectUrl = "https://dev2-api.sequence.app/oauth/callback";
        
        private string _stateToken = Guid.NewGuid().ToString();
        private readonly string _nonce = Guid.NewGuid().ToString();

        public OpenIdAuthenticator()
        {
            SequenceConfig config = SequenceConfig.GetConfig();

            _urlScheme = config.UrlScheme;
        }

        public void GoogleSignIn()
        {
            try
            {
                string googleSignInUrl = GenerateSignInUrl("https://accounts.google.com/o/oauth2/auth", GoogleClientId, nameof(LoginMethod.Google));
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
                    GenerateSignInUrl("https://discord.com/api/oauth2/authorize", DiscordClientId, nameof(LoginMethod.Discord));
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
                    GenerateSignInUrl("https://www.facebook.com/v18.0/dialog/oauth", FacebookClientId, nameof(LoginMethod.Facebook));
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
                    GenerateSignInUrl("https://appleid.apple.com/auth/authorize", AppleClientId, nameof(LoginMethod.Apple));
                Application.OpenURL(appleSignInUrl);
            }
            catch (Exception e)
            {
                Debug.LogError($"Apple sign in error: {e.Message}");
            }
        }

        private string GenerateSignInUrl(string baseUrl, string clientId, string method)
        {
            string url =
                $"{baseUrl}?response_type=id_token&client_id={clientId}&redirect_uri={RedirectUrl}&scope=openid+profile+email&state={_urlScheme + "---" + _stateToken + method}&nonce={_nonce}/";
            if (PlayerPrefs.HasKey(LoginEmail))
            {
                url = url.RemoveTrailingSlash() + $"&login_hint={PlayerPrefs.GetString(LoginEmail)}".AppendTrailingSlashIfNeeded();
            }

            return url;
        }

        public void PlatformSpecificSetup()
        {
#if UNITY_STANDALONE_WIN
            // Register a Windows URL protocol handler in the Windows Registry.
            var appPath = Path.GetFullPath(Application.dataPath.Replace("_Data", ".exe"));
            string[] commands = new string[]{
                $"add HKEY_CURRENT_USER\\Software\\Classes\\{_urlScheme} /t REG_SZ /d \"URL:Sequence Login for {Application.productName}\" /f",
                $"add HKEY_CURRENT_USER\\Software\\Classes\\{_urlScheme} /v \"URL Protocol\" /t REG_SZ /d \"\" /f",
                $"add HKEY_CURRENT_USER\\Software\\Classes\\{_urlScheme}\\shell /f",
                $"add HKEY_CURRENT_USER\\Software\\Classes\\{_urlScheme}\\shell\\open /f",
                $"add HKEY_CURRENT_USER\\Software\\Classes\\{_urlScheme}\\shell\\open\\command /t REG_SZ /d \"\\\"{appPath}\\\" \\\"%1\\\"\" /f",
            };
            foreach(var args in commands) {
                var command = new System.Diagnostics.ProcessStartInfo();
                command.FileName = "C:\\Windows\\System32\\reg.exe";
                command.Arguments = args;
                command.UseShellExecute = false;
                command.CreateNoWindow = true;
                System.Diagnostics.Process.Start(command);
            }
            
            StartWindowsServer();
#elif UNITY_STANDALONE_OSX
            // ensure our URL protocol handler is registered - MacOS doesn't pick it up automatically unless it has already been registered.
            var appPath = System.IO.Directory.GetParent(Application.dataPath);
            var command = new System.Diagnostics.ProcessStartInfo();
            command.FileName = "/System/Library/Frameworks/CoreServices.framework/Versions/A/Frameworks/LaunchServices.framework/Versions/A/Support/lsregister";
            command.Arguments = " -R -f " + appPath;
            command.UseShellExecute = false;
            command.CreateNoWindow = true;
            System.Diagnostics.Process.Start(command);
#endif
        }
        
#if UNITY_STANDALONE_WIN
        private void StartWindowsServer() {
            // Run a TCP server on Windows standalone to get the auth token from the other instance.
            /**
            ***** IMPORTANT NOTE *****
            ***** IMPORTANT NOTE *****
            ***** IMPORTANT NOTE *****
            ***** IMPORTANT NOTE *****
            ***** IMPORTANT NOTE *****
            Do not add code to this TCP server/client without thinking very carefully; it's easy to get driveby exploited since this is a TCP server any application can talk to.
            Do not increase the attack surface without careful thought.
            ***** IMPORTANT NOTE *****
            ***** IMPORTANT NOTE *****
            ***** IMPORTANT NOTE *****
            ***** IMPORTANT NOTE *****
            **/
            var syncContext = SynchronizationContext.Current;
            var ipcListener = new Thread(() =>
            {
                var socketConnection = new TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), WINDOWS_IPC_PORT);
                socketConnection.Start();
                var bytes = new System.Byte[1024];
                var msg = "";
                while (true)
                {
                    using (var connectedTcpClient = socketConnection.AcceptTcpClient())
                    {
                        using (NetworkStream stream = connectedTcpClient.GetStream())
                        {
                            int length;
                            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                var data = new byte[length];
                                System.Array.Copy(bytes, 0, data, 0, length);
                                // Convert byte array to string message. 							
                                string clientMessage = System.Text.Encoding.ASCII.GetString(data);
                                if (clientMessage.StartsWith("@@@@"))
                                {
                                    msg = clientMessage.Replace("@@@@", "").Replace("$$$$", "");
                                }
                                else
                                {
                                    msg += clientMessage.Replace("$$$$", "");
                                }
                                if (msg.Length > 8192)
                                {
                                    // got some weird garbage, toss it to avoid memory leaks.
                                    msg = "";
                                }
                                if (clientMessage.EndsWith("$$$$"))
                                {
                                    syncContext.Post((data) =>
                                    {
                                        HandleDeepLink((string)data);
                                    }, msg);
                                }
                            }
                        }
                    }
                }
            });
            ipcListener.IsBackground = true;
            ipcListener.Start();
        }
#endif

        public void HandleDeepLink(string link)
        {
            LoginMethod method = LoginMethod.None;
            link = link.RemoveTrailingSlash();
            
            Dictionary<string, string> queryParams = link.ExtractQueryParameters();
            if (queryParams == null)
            {
                Debug.LogError($"Unexpected deep link: {link}");
                return;
            }
            if (queryParams.TryGetValue("state", out string state))
            {
                if (!state.Contains(_stateToken))
                {
                    Debug.LogError("State token mismatch");
                    return;
                }
                method = GetMethodFromState(state);
            }
            else
            {
                Debug.LogError("State token missing");
                return;
            }
            
            if (queryParams.TryGetValue("id_token", out string idToken))
            {
                SignedIn?.Invoke(new OpenIdAuthenticationResult(idToken, method));
            }
            else
            {
                Debug.LogError($"Unexpected deep link: {link}");
            }
        }

        private LoginMethod GetMethodFromState(string state)
        {
            if (state.EndsWith(nameof(LoginMethod.Google)))
            {
                return LoginMethod.Google;
            }

            if (state.EndsWith(nameof(LoginMethod.Discord)))
            {
                return LoginMethod.Discord;
            }

            if (state.EndsWith(nameof(LoginMethod.Facebook)))
            {
                return LoginMethod.Facebook;
            }

            if (state.EndsWith(nameof(LoginMethod.Apple)))
            {
                return LoginMethod.Apple;
            }

            return LoginMethod.Custom;
        }

#if UNITY_EDITOR
        public void InjectStateTokenForTesting(string stateToken)
        {
            _stateToken = stateToken;
        }
#endif
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