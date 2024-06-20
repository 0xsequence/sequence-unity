using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.CognitoIdentity.Model;
using Newtonsoft.Json;
using Sequence.ABI;
using Sequence.Authentication;
using Sequence.Config;
using Sequence.Utils;
using Sequence.Utils.SecureStorage;
using Sequence.WaaS.Authentication;
using Sequence.Wallet;
using SequenceSDK.WaaS;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sequence.WaaS
{
    public class WaaSLogin : ILogin 
    {
        public static string WaaSWithAuthUrl { get; private set; }
        public const string WaaSLoginMethod = "WaaSLoginMethod";

        private int _waasProjectId;
        private string _waasVersion;
        private OpenIdAuthenticator _authenticator;
        private IValidator _validator;
        private IEmailSignIn _emailSignIn;
        private string _challengeSession;
        private int retries = 0;
        private EthWallet _sessionWallet;
        private string _sessionId;

        private bool _isLoggingIn = false;
        
        private bool _storeSessionWallet = false;
        private const string _walletKey = "SessionWallet";
        private const string _waasWalletAddressKey = "WaaSWalletAddress";

        public WaaSLogin(IValidator validator = null)
        {
            bool storeSessionWallet = SequenceConfig.GetConfig().StoreSessionPrivateKeyInSecureStorage;
            if (storeSessionWallet)
            {
                _storeSessionWallet = true;
                Configure();
            }
            else
            {
                CreateWallet(validator);
            }
        }

        private void CreateWallet(IValidator validator = null)
        {
            Configure();

            SetupAuthenticator(validator);
        }

        public void SetupAuthenticator(IValidator validator = null)
        {
            ConfigJwt configJwt = SequenceConfig.GetConfigJwt();
            _sessionWallet = new EthWallet();
            _sessionId = IntentDataOpenSession.CreateSessionId(_sessionWallet.GetAddress());

            string nonce = SequenceCoder.KeccakHashASCII(_sessionId).EnsureHexPrefix();
            if (OpenIdAuthenticator.Instance == null)
            {
                SetAuthenticatorAndSetupListeners(nonce);
            }
            else
            {
                SetAuthenticator(nonce);
            }

            if (validator == null)
            {
                validator = new Validator();
            }
            _validator = validator;

            try
            {
                _emailSignIn = new AWSEmailSignIn(configJwt.emailRegion, configJwt.emailClientId, nonce);
            }
            catch (Exception e)
            {
                Debug.LogWarning("AWS config not found in config key. Email sign in will not work. Please contact Sequence support for more information.");
            }
        }

        public void TryToRestoreSession()
        {
            if (!_storeSessionWallet)
            {
                return;
            }
            TryToLoginWithStoredSessionWallet();
        }

        private void SetAuthenticator(string nonce)
        {
            _authenticator = OpenIdAuthenticator.GetInstance(nonce);
        }

        private void SetAuthenticatorAndSetupListeners(string nonce)
        {
            SetAuthenticator(nonce);
            try {
                _authenticator.PlatformSpecificSetup();
            }
            catch (Exception e) {
                Debug.LogError($"Error encountered during PlatformSpecificSetup: {e.Message}\nSocial sign in will not work.");
            }
            Application.deepLinkActivated += _authenticator.HandleDeepLink;
            _authenticator.SignedIn += OnSocialLogin;
            _authenticator.OnSignInFailed += OnSocialSignInFailed;
        }

        private void Configure()
        {
            SequenceConfig config = SequenceConfig.GetConfig();
            string waasVersion = config.WaaSVersion;
            if (string.IsNullOrWhiteSpace(waasVersion))
            {
                throw SequenceConfig.MissingConfigError("WaaS Version");
            }
            _waasVersion = waasVersion;

            ConfigJwt configJwt = SequenceConfig.GetConfigJwt();

            string rpcUrl = configJwt.rpcServer;
            if (string.IsNullOrWhiteSpace(rpcUrl))
            {
                throw SequenceConfig.MissingConfigError("RPC Server");
            }
            WaaSWithAuthUrl = $"{rpcUrl.AppendTrailingSlashIfNeeded()}rpc/WaasAuthenticator";
            
            int projectId = configJwt.projectId;
            if (string.IsNullOrWhiteSpace(projectId.ToString()))
            {
                throw SequenceConfig.MissingConfigError("Project ID");
            }
            _waasProjectId = projectId;
        }

        private void TryToLoginWithStoredSessionWallet()
        {
            (EthWallet, string) walletInfo = (null, "");
            try
            {
                walletInfo = AttemptToCreateWalletFromSecureStorage();
            }
            catch (Exception e)
            {
                FailedLoginWithStoredSessionWallet(e.Message);
                return;
            }
            if (walletInfo.Item1 == null || string.IsNullOrWhiteSpace(walletInfo.Item2))
            {
                FailedLoginWithStoredSessionWallet("No stored wallet info found");
                return;
            }
            
            _sessionWallet = walletInfo.Item1;
            
            _sessionId = IntentDataOpenSession.CreateSessionId(_sessionWallet.GetAddress());
            
            WaaSWallet wallet = new WaaSWallet(new Address(walletInfo.Item2), _sessionId, new IntentSender(new HttpClient(WaaSWithAuthUrl), walletInfo.Item1, _sessionId, _waasProjectId, _waasVersion));

            EnsureSessionIsValid(wallet);
        }

        private void FailedLoginWithStoredSessionWallet(string error)
        {
            CreateWallet();
            WaaSWallet.OnFailedToLoginWithStoredSessionWallet?.Invoke(error);
        }

        private async Task EnsureSessionIsValid(WaaSWallet wallet)
        {
            WaaSSession[] activeSessions = await wallet.ListSessions();
            if (activeSessions == null || activeSessions.Length == 0)
            {
                FailedLoginWithStoredSessionWallet("No active sessions found");
                return;
            }

            int sessions = activeSessions.Length;
            string expectedSessionId = wallet.SessionId;
            for (int i = 0; i < sessions; i++)
            {
                if (activeSessions[i].id == expectedSessionId)
                {
                    WaaSWallet.OnWaaSWalletCreated?.Invoke(wallet);
                    return;
                }
            }
            
            FailedLoginWithStoredSessionWallet("Stored session wallet is not active");
        }

        private (EthWallet, string) AttemptToCreateWalletFromSecureStorage()
        {
            ISecureStorage secureStorage = SecureStorageFactory.CreateSecureStorage();
            string privateKey = secureStorage.RetrieveString(_walletKey);
            if (string.IsNullOrEmpty(privateKey))
            {
                return (null, "");
            }
            string waasWalletAddress = secureStorage.RetrieveString(_waasWalletAddressKey);
            EthWallet wallet = new EthWallet(privateKey);
            return (wallet, waasWalletAddress);
        }
        
        public void InjectEmailSignIn(IEmailSignIn emailSignIn)
        {
            _emailSignIn = emailSignIn;
        }
        
        public event ILogin.OnLoginSuccessHandler OnLoginSuccess;
        public event ILogin.OnLoginFailedHandler OnLoginFailed;
        public event ILogin.OnMFAEmailSentHandler OnMFAEmailSent;
        public event ILogin.OnMFAEmailFailedToSendHandler OnMFAEmailFailedToSend;

        public async Task Login(string email)
        {
            if (!_validator.ValidateEmail(email))
            {
                OnMFAEmailFailedToSend?.Invoke(email, $"Invalid email: {email}");
                return;
            }
            
            try
            {
                if (_emailSignIn == null)
                {
                    OnMFAEmailFailedToSend?.Invoke(email, "Email sign in not available. Please check for logged warnings; there is most likely a configuration issue.");
                    return;
                }
                _challengeSession = await _emailSignIn.SignIn(email);
                if (string.IsNullOrEmpty(_challengeSession))
                {
                    OnMFAEmailFailedToSend?.Invoke(email, "Unknown error establishing AWS session");
                    return;
                }

                if (_challengeSession.Contains("user not found"))
                {
                    if (retries > 1)
                    {
                        OnMFAEmailFailedToSend?.Invoke(email, _challengeSession);
                        return;
                    }
                    await SignUpThenRetryLogin(email);
                    return;
                }
                
                if (_challengeSession.StartsWith("Error"))
                {
                    OnMFAEmailFailedToSend?.Invoke(email, _challengeSession);
                    return;
                }
                
                OnMFAEmailSent?.Invoke(email);
            }
            catch (Exception e)
            {
                OnMFAEmailFailedToSend?.Invoke(email, e.Message);
            }
        }

        public async Task SignUpThenRetryLogin(string email)
        {
            try
            {
                await _emailSignIn.SignUp(email);
                retries++;
                await Login(email);
            }
            catch (Exception e)
            {
                OnMFAEmailFailedToSend?.Invoke(email, e.Message);
            }
        }

        public async Task Login(string email, string code)
        {
            if (!_validator.ValidateCode(code))
            {
                OnLoginFailed?.Invoke($"Invalid code: {code}");
                return;
            }
            
            try
            {
                string idToken = await _emailSignIn.Login(_challengeSession, email, code, _sessionWallet.GetAddress());
                if (string.IsNullOrEmpty(idToken))
                {
                    OnLoginFailed?.Invoke("Unknown error establishing AWS session");
                    return;
                }

                if (idToken.StartsWith("Error"))
                {
                    OnLoginFailed?.Invoke(idToken);
                    return;
                }
                
                await ConnectToWaaS(idToken, LoginMethod.Email);
            }
            catch (Exception e)
            {
                OnLoginFailed?.Invoke(e.Message);
            }
        }

        public void GoogleLogin()
        {
            _authenticator.GoogleSignIn();
        }

        public void DiscordLogin()
        {
            _authenticator.DiscordSignIn();
        }

        public void FacebookLogin()
        {
            _authenticator.FacebookSignIn();
        }
        
        public void AppleLogin()
        {
            _authenticator.AppleSignIn();
        }

        public bool IsLoggingIn()
        {
            return _isLoggingIn;
        }

        private void OnSocialLogin(OpenIdAuthenticationResult result)
        {
            ConnectToWaaS(result.IdToken, result.Method);
        }

        private void OnSocialSignInFailed(string error)
        {
            OnLoginFailed?.Invoke("Connecting to WaaS API failed due to error with social sign in: " + error);
        }

        public async Task ConnectToWaaS(string idToken, LoginMethod method)
        {
            _isLoggingIn = true;
            IntentSender sender = new IntentSender(
                new HttpClient(WaaSWithAuthUrl),
                _sessionWallet,
                _sessionId,
                _waasProjectId,
                _waasVersion);
            IntentDataOpenSession loginIntent = AssembleLoginIntent(idToken, _sessionWallet);

            try
            {
                IntentResponseSessionOpened registerSessionResponse = await sender.SendIntent<IntentResponseSessionOpened, IntentDataOpenSession>(loginIntent, IntentType.OpenSession);
                string sessionId = registerSessionResponse.sessionId;
                string walletAddress = registerSessionResponse.wallet;
                OnLoginSuccess?.Invoke(sessionId, walletAddress);
                WaaSWallet wallet = new WaaSWallet(new Address(walletAddress), sessionId, new IntentSender(new HttpClient(WaaSLogin.WaaSWithAuthUrl), _sessionWallet, sessionId, _waasProjectId, _waasVersion));
                string email = Sequence.Authentication.JwtHelper.GetIdTokenJwtPayload(idToken).email;
                PlayerPrefs.SetInt(WaaSLoginMethod, (int)method);
                PlayerPrefs.SetString(OpenIdAuthenticator.LoginEmail, email);
                PlayerPrefs.Save();
                if (_storeSessionWallet && SecureStorageFactory.IsSupportedPlatform())
                {
                    StoreWalletSecurely(walletAddress);
                }
                _isLoggingIn = false;
                WaaSWallet.OnWaaSWalletCreated?.Invoke(wallet);
            }
            catch (Exception e)
            {
                OnLoginFailed?.Invoke("Error registering waaSSession: " + e.Message);
                _isLoggingIn = false;
                return;
            }
        }

        private IntentDataOpenSession AssembleLoginIntent(string idToken, Wallet.IWallet sessionWallet)
        {
            IntentDataOpenSession intent = new IntentDataOpenSession(sessionWallet.GetAddress(), null, idToken);
            return intent;
        }
        
        private void StoreWalletSecurely(string waasWalletAddress)
        {
            ISecureStorage secureStorage = SecureStorageFactory.CreateSecureStorage();
            byte[] privateKeyBytes = new byte[32];
            _sessionWallet.privKey.WriteToSpan(privateKeyBytes);
            string privateKey = privateKeyBytes.ByteArrayToHexString();
            secureStorage.StoreString(_walletKey, privateKey);
            secureStorage.StoreString(_waasWalletAddressKey, waasWalletAddress);
        }
    }
}