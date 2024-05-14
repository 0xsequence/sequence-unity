using System;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using Sequence.Config;
using Sequence.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sequence.Authentication
{
    public class OpenIdAuthenticator
    {
        public const string LoginEmail = "LoginEmail";
        public Action<OpenIdAuthenticationResult> SignedIn;
        public Action<string> OnSignInFailed;
        private string _urlScheme;
        
        public static readonly int WINDOWS_IPC_PORT = 52836;
        
        private string GoogleClientId;
        private string DiscordClientId;
        private string FacebookClientId;
        private string AppleClientId;
        
        private static string RedirectUrl = "https://api.sequence.app/oauth/callback";

        private string _stateToken = Guid.NewGuid().ToString();

        private string _sessionId; // Session Id is expected to be hex(keccak256(sessionWalletAddress))

        private IBrowser _browser;
        
        private static bool _windowsSetup = false;

        /// <summary>
        /// Use this if you'd prefer to redirect to your own URL for Oauth
        /// Your server will need to redirect the URL retrieved during the social sign in process to the custom URL scheme you've set in SequenceConfig
        /// </summary>
        /// <param name="redirectUrl"></param>
        public static void InJectRedirectUrl(string redirectUrl)
        {
            RedirectUrl = redirectUrl;
        }

        public OpenIdAuthenticator(string sessionId)
        {
            SequenceConfig config = SequenceConfig.GetConfig();

            _urlScheme = config.UrlScheme;
            SetClientIds(config);
            
            _sessionId = sessionId;
            
            _browser = CreateBrowser();
        }

        private void SetClientIds(SequenceConfig config)
        {
#if UNITY_IOS
            GoogleClientId = config.GoogleClientIdIOS;
            DiscordClientId = config.DiscordClientIdIOS;
            FacebookClientId = config.FacebookClientIdIOS;
            AppleClientId = config.AppleClientIdIOS;
#elif UNITY_ANDROID
            GoogleClientId = config.GoogleClientIdAndroid;
            DiscordClientId = config.DiscordClientIdAndroid;    
            FacebookClientId = config.FacebookClientIAndroid;
            AppleClientId = config.AppleClientIdAndroid;
#else
            GoogleClientId = config.GoogleClientId;
            DiscordClientId = config.DiscordClientId;
            FacebookClientId = config.FacebookClientId;
            AppleClientId = config.AppleClientId;
#endif
        }

        private IBrowser CreateBrowser()
        {
#if UNITY_WEBGL
            return new WebGLBrowser();
#elif UNITY_EDITOR
            return new EditorBrowser();
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            return new StandaloneBrowser();
#elif UNITY_IOS
            return IosBrowser.Setup(this);
#elif UNITY_ANDROID
            return new StandaloneBrowser();    // Todo switch to AndroidBrowser
#else
            throw new NotImplementedException("No social sign in implementation for this platform");
#endif
        }

        public void GoogleSignIn()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(GoogleClientId))
                {
                    throw SequenceConfig.MissingConfigError("Google Client Id");
                }
                string googleSignInUrl = GenerateSignInUrl("https://accounts.google.com/o/oauth2/auth", GoogleClientId, nameof(LoginMethod.Google));
                _browser.Authenticate(googleSignInUrl, ReverseClientId(GoogleClientId));
            }
            catch (Exception e)
            {
                OnSignInFailed?.Invoke($"Google sign in error: {e.Message}");
            }
        }

        private string ReverseClientId(string clientId)
        {
            string[] parts = clientId.Split('.');
            Array.Reverse(parts);
            return string.Join(".", parts);
        }
        
        public void DiscordSignIn()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(DiscordClientId))
                {
                    throw SequenceConfig.MissingConfigError("Discord Client Id");
                }
                string discordSignInUrl =
                    GenerateSignInUrl("https://discord.com/api/oauth2/authorize", DiscordClientId, nameof(LoginMethod.Discord));
                _browser.Authenticate(discordSignInUrl);
            }
            catch (Exception e)
            {
                OnSignInFailed?.Invoke($"Discord sign in error: {e.Message}");
            }
        }

        public void FacebookSignIn()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FacebookClientId))
                {
                    throw SequenceConfig.MissingConfigError("Facebook Client Id");
                }
                string facebookSignInUrl =
                    GenerateSignInUrl("https://www.facebook.com/v18.0/dialog/oauth", FacebookClientId, nameof(LoginMethod.Facebook));
                _browser.Authenticate(facebookSignInUrl);
            }
            catch (Exception e)
            {
                OnSignInFailed?.Invoke($"Facebook sign in error: {e.Message}");
            }
        }
        
        public void AppleSignIn()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(AppleClientId))
                {
                    throw SequenceConfig.MissingConfigError("Apple Client Id");
                }
                string appleSignInUrl =
                    GenerateSignInUrl("https://appleid.apple.com/auth/authorize", AppleClientId, nameof(LoginMethod.Apple));
                appleSignInUrl = appleSignInUrl.RemoveTrailingSlash() + "&response_mode=form_post";
#if UNITY_IOS
                GameObject appleSignInObject = Object.Instantiate(new GameObject());
                SignInWithApple appleSignIn = appleSignInObject.AddComponent<SignInWithApple>();
                appleSignIn.LoginToApple(this, _sessionId, _stateToken);
#else
                _browser.Authenticate(appleSignInUrl);
#endif
            }
            catch (Exception e)
            {
                OnSignInFailed?.Invoke($"Apple sign in error: {e.Message}");
            }
        }

        private string GenerateSignInUrl(string baseUrl, string clientId, string method)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw SequenceConfig.MissingConfigError("WaaS Project Id");
            }

            if (string.IsNullOrWhiteSpace(_urlScheme))
            {
                throw SequenceConfig.MissingConfigError("Url Scheme");
            }
            
#if UNITY_IOS
            RedirectUrl = $"{ReverseClientId(clientId)}://";
#endif
            
            _stateToken = Guid.NewGuid().ToString();
            
            string url =
                $"{baseUrl}?response_type=code+id_token&client_id={clientId}&redirect_uri={RedirectUrl}&nonce={_sessionId}&scope=openid+email&state={_urlScheme + "---" + _stateToken + method}/";
            if (PlayerPrefs.HasKey(LoginEmail))
            {
                url = url.RemoveTrailingSlash() + $"&login_hint={PlayerPrefs.GetString(LoginEmail)}".AppendTrailingSlashIfNeeded();
            }

            return url;
        }

        public void PlatformSpecificSetup()
        {
#if UNITY_STANDALONE_WIN
            if (_windowsSetup)
            {
                return;
            }

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
            _windowsSetup = true;
        }
#endif

        public void HandleDeepLink(string link)
        {
            LoginMethod method = LoginMethod.None;
            link = link.RemoveTrailingSlash();
            
            Dictionary<string, string> queryParams = link.ExtractQueryAndHashParameters();
            if (queryParams == null)
            {
                OnSignInFailed?.Invoke($"Unexpected deep link: {link}");
                return;
            }
            if (queryParams.TryGetValue("state", out string state))
            {
                if (!state.Contains(_stateToken))
                {
                    OnSignInFailed?.Invoke("State token mismatch");
                    return;
                }
                method = GetMethodFromState(state);
            }
            else
            {
                OnSignInFailed?.Invoke("State token missing");
                return;
            }
            
            if (queryParams.TryGetValue("id_token", out string idToken))
            {
                SignedIn?.Invoke(new OpenIdAuthenticationResult(idToken, method));
            }
            else
            {
                OnSignInFailed?.Invoke($"Unexpected deep link: {link}");
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