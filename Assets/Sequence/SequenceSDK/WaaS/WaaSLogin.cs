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
    public class WaaSLogin : ILogin, IWaaSConnector
    {
        public static string WaaSWithAuthUrl { get; private set; }
        public const string WaaSLoginMethod = "WaaSLoginMethod";

        private int _waasProjectId;
        private string _waasVersion;
        private IAuthenticator _authenticator;
        private IValidator _validator;
        private IEmailSignIn _emailSignIn;
        private EthWallet _sessionWallet;
        private string _sessionId;
        private IntentSender _intentSender;
        private EmailConnector _emailConnector;

        private bool _isLoggingIn = false;
        
        private bool _storeSessionWallet = false;
        private const string _walletKey = "SessionWallet";
        
        private static WaaSLogin _instance;
        
        public static WaaSLogin GetInstance(IValidator validator = null, IAuthenticator authenticator = null)
        {
            if (_instance == null)
            {
                _instance = new WaaSLogin(validator, authenticator);
            }
            return _instance;
        }

        [Obsolete("Use GetInstance() instead.")]
        public WaaSLogin(IValidator validator = null, IAuthenticator authenticator = null)
        {
            bool storeSessionWallet = SequenceConfig.GetConfig().StoreSessionPrivateKeyInSecureStorage;
            if (storeSessionWallet)
            {
                _storeSessionWallet = true;
                Configure();
            }
            else
            {
                CreateWallet(validator, authenticator);
            }
        }

        private void CreateWallet(IValidator validator = null, IAuthenticator authenticator = null)
        {
            Configure();

            SetupAuthenticator(validator, authenticator);
        }

        public void SetupAuthenticator(IValidator validator = null, IAuthenticator authenticator = null)
        {
            ConfigJwt configJwt = SequenceConfig.GetConfigJwt();
            _sessionWallet = new EthWallet();
            _sessionId = IntentDataOpenSession.CreateSessionId(_sessionWallet.GetAddress());
            _intentSender = new IntentSender(new HttpClient(WaaSWithAuthUrl), _sessionWallet, _sessionId, _waasProjectId, _waasVersion);

            string nonce = SequenceCoder.KeccakHashASCII(_sessionId).EnsureHexPrefix();

            if (authenticator != null)
            {
                _authenticator = authenticator;
            }
            else
            {
                _authenticator = new OpenIdAuthenticator();
            }
            SetupAuthenticatorAndListeners(nonce);

            if (validator == null)
            {
                validator = new Validator();
            }
            _validator = validator;

            _emailConnector = new EmailConnector(_sessionId, _sessionWallet, this, _validator);
            
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

        private void SetupAuthenticatorAndListeners(string nonce)
        {
            _authenticator.SetNonce(nonce);
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
            
            // Todo remove
            WaaSWithAuthUrl = "https://dev-waas.sequence.app/rpc/WaasAuthenticator";
            
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
            string walletInfo = secureStorage.RetrieveString(_walletKey);
            if (string.IsNullOrEmpty(walletInfo))
            {
                return (null, "");
            }
            string[] walletInfoSplit = walletInfo.Split('-');
            string privateKey = walletInfoSplit[0];
            string waasWalletAddress = walletInfoSplit[1];
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
            try
            {
                _isLoggingIn = true;
                await _emailConnector.Login(email);
                OnMFAEmailSent?.Invoke(email);
            }
            catch (Exception e)
            {
                OnMFAEmailFailedToSend?.Invoke(email, e.Message);
            }
        }

        public async Task Login(string email, string code)
        {
            _isLoggingIn = true;
            await _emailConnector.ConnectToWaaSViaEmail(email, code);
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
            ConnectToWaaSViaSocialLogin(result.IdToken, result.Method);
        }

        private void OnSocialSignInFailed(string error)
        {
            OnLoginFailed?.Invoke("Connecting to WaaS API failed due to error with social sign in: " + error);
        }

        public async Task ConnectToWaaS(IntentDataOpenSession loginIntent, LoginMethod method, string email = "")
        {
            try
            {
                IntentResponseSessionOpened registerSessionResponse = await _intentSender.SendIntent<IntentResponseSessionOpened, IntentDataOpenSession>(loginIntent, IntentType.OpenSession);
                string sessionId = registerSessionResponse.sessionId;
                string walletAddress = registerSessionResponse.wallet;
                OnLoginSuccess?.Invoke(sessionId, walletAddress);
                WaaSWallet wallet = new WaaSWallet(new Address(walletAddress), sessionId, new IntentSender(new HttpClient(WaaSLogin.WaaSWithAuthUrl), _sessionWallet, sessionId, _waasProjectId, _waasVersion));
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
            }
        }

        public async Task<string> InitiateAuth(IntentDataInitiateAuth initiateAuthIntent)
        {
            string challenge = "";

            try
            {
                IntentResponseAuthInitiated initiateAuthResponse = await _intentSender.SendIntent<IntentResponseAuthInitiated, IntentDataInitiateAuth>(initiateAuthIntent, IntentType.InitiateAuth);
                string sessionId = initiateAuthResponse.sessionId;
                if (sessionId != _sessionId)
                {
                    throw new Exception($"Session Id received from WaaS server doesn't match, received {sessionId}, sent {_sessionId}");
                }

                if (!initiateAuthResponse.ValidateChallenge())
                {
                    throw new Exception("Invalid challenge received from WaaS server, received: " + initiateAuthResponse.challenge);
                }
                
                challenge = initiateAuthResponse.challenge;
            }
            catch (Exception e)
            {
                OnLoginFailed?.Invoke("Error initiating auth: " + e.Message);
                _isLoggingIn = false;
            }

            return challenge;
        }

        public async Task ConnectToWaaSViaSocialLogin(string idToken, LoginMethod method)
        {
            if (!method.IsOIDC())
            {
                OnLoginFailed?.Invoke($"Invalid login method, given: {method}, expected one of {nameof(IdentityType)}: {nameof(IdentityType.OIDC)}");
                _isLoggingIn = false;
                return;
            }
            
            _isLoggingIn = true;
            
            IntentDataInitiateAuth initiateAuthIntent = AssembleOIDCInitiateAuthIntent(idToken, _sessionId);

            string challenge = await InitiateAuth(initiateAuthIntent);
            
            IntentDataOpenSession loginIntent = AssembleOIDCOpenSessionIntent(idToken, _sessionWallet);

            string email = Sequence.Authentication.JwtHelper.GetIdTokenJwtPayload(idToken).email;
            await ConnectToWaaS(loginIntent, method, email);
        }
        
        private IntentDataInitiateAuth AssembleOIDCInitiateAuthIntent(string idToken, string sessionId)
        {
            IdTokenJwtPayload idTokenPayload = Sequence.Authentication.JwtHelper.GetIdTokenJwtPayload(idToken);
            string idTokenHash = SequenceCoder.KeccakHashASCII(idToken).WithoutHexPrefix();
            string verifier = $"{idTokenHash};{idTokenPayload.exp}";
            IntentDataInitiateAuth intent = new IntentDataInitiateAuth(IdentityType.OIDC, sessionId, verifier);
            return intent;
        }

        private IntentDataOpenSession AssembleOIDCOpenSessionIntent(string idToken, Wallet.IWallet sessionWallet)
        {
            IdTokenJwtPayload idTokenPayload = Sequence.Authentication.JwtHelper.GetIdTokenJwtPayload(idToken);
            string idTokenHash = SequenceCoder.KeccakHashASCII(idToken).EnsureHexPrefix();
            string verifier = $"{idTokenHash};{idTokenPayload.exp}";
            IntentDataOpenSession intent =
                new IntentDataOpenSession(sessionWallet.GetAddress(), IdentityType.OIDC, verifier, idToken);
            return intent;
        }
        
        private void StoreWalletSecurely(string waasWalletAddress)
        {
            ISecureStorage secureStorage = SecureStorageFactory.CreateSecureStorage();
            byte[] privateKeyBytes = new byte[32];
            _sessionWallet.privKey.WriteToSpan(privateKeyBytes);
            string privateKey = privateKeyBytes.ByteArrayToHexString();
            secureStorage.StoreString(_walletKey, privateKey + "-" + waasWalletAddress);
        }
    }
}