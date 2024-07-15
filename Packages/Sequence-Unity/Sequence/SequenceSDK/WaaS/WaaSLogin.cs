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
        private EoaWallet _sessionWallet;
        private string _sessionId;
        private IntentSender _intentSender;
        private EmailConnector _emailConnector;
        private IWaaSConnector _connector;

        private bool _isLoggingIn = false;
        
        private bool _storeSessionWallet = false;
        private const string _walletKey = "SessionWallet";
        
        private IntentDataOpenSession _failedLoginIntent;
        private LoginMethod _failedLoginMethod;
        private string _failedLoginEmail;
        private bool _automaticallyFederateAccountsWhenPossible;

        private static WaaSLogin _instance;
        
        public static WaaSLogin GetInstance(IValidator validator = null, IAuthenticator authenticator = null, IWaaSConnector connector = null, bool automaticallyFederateAccountsWhenPossible = true)
        {
            if (_instance == null)
            {
                _instance = new WaaSLogin(validator, authenticator, connector);
            }
            return _instance;
        }

        [Obsolete("Use GetInstance() instead.")]
        public WaaSLogin(IValidator validator = null, IAuthenticator authenticator = null, IWaaSConnector connector = null, bool automaticallyFederateAccountsWhenPossible = true)
        {
            if (connector == null)
            {
                connector = this;
            }
            _connector = connector;
            
            _automaticallyFederateAccountsWhenPossible = automaticallyFederateAccountsWhenPossible;
            
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
            _sessionWallet = new EoaWallet();
            _sessionId = IntentDataOpenSession.CreateSessionId(_sessionWallet.GetAddress());
            _intentSender = new IntentSender(new HttpClient(WaaSWithAuthUrl), _sessionWallet, _sessionId, _waasProjectId, _waasVersion);

            if (authenticator != null)
            {
                _authenticator = authenticator;
            }
            else
            {
                _authenticator = new OpenIdAuthenticator();
            }
            SetupAuthenticatorAndListeners();

            if (validator == null)
            {
                validator = new Validator();
            }
            _validator = validator;

            _emailConnector = new EmailConnector(_sessionId, _sessionWallet, _connector, _validator);
        }

        public void TryToRestoreSession()
        {
            if (!_storeSessionWallet)
            {
                return;
            }
            TryToLoginWithStoredSessionWallet();
        }

        public void GuestLogin()
        {
            ConnectToWaaSAsGuest();
        }

        private void SetupAuthenticatorAndListeners()
        {
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
            (EoaWallet, string) walletInfo = (null, "");
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
            
            EmbeddedWallet wallet = new EmbeddedWallet(new Address(walletInfo.Item2), _sessionId, new IntentSender(new HttpClient(WaaSWithAuthUrl), walletInfo.Item1, _sessionId, _waasProjectId, _waasVersion));

            EnsureSessionIsValid(wallet);
        }

        private void FailedLoginWithStoredSessionWallet(string error)
        {
            CreateWallet();
            EmbeddedWallet.OnFailedToLoginWithStoredSessionWallet?.Invoke(error);
        }

        private async Task EnsureSessionIsValid(EmbeddedWallet wallet)
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
                    EmbeddedWallet.OnWaaSWalletCreated?.Invoke(wallet);
                    return;
                }
            }
            
            FailedLoginWithStoredSessionWallet("Stored session wallet is not active");
        }

        private (EoaWallet, string) AttemptToCreateWalletFromSecureStorage()
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
            EoaWallet wallet = new EoaWallet(privateKey);
            return (wallet, waasWalletAddress);
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

        private void OnSocialSignInFailed(string error, LoginMethod method)
        {
            OnLoginFailed?.Invoke($"Connecting to WaaS API failed due to error with {method} sign in: {error}", method);
        }

        public async Task ConnectToWaaS(IntentDataOpenSession loginIntent, LoginMethod method, string email = "")
        {
            string walletAddress = "";
            try
            {
                IntentResponseSessionOpened registerSessionResponse = await _intentSender.SendIntent<IntentResponseSessionOpened, IntentDataOpenSession>(loginIntent, IntentType.OpenSession);
                string sessionId = registerSessionResponse.sessionId;
                walletAddress = registerSessionResponse.wallet;
                OnLoginSuccess?.Invoke(sessionId, walletAddress);
                EmbeddedWallet wallet = new EmbeddedWallet(new Address(walletAddress), sessionId, new IntentSender(new HttpClient(WaaSLogin.WaaSWithAuthUrl), _sessionWallet, sessionId, _waasProjectId, _waasVersion));
                PlayerPrefs.SetInt(WaaSLoginMethod, (int)method);
                PlayerPrefs.SetString(OpenIdAuthenticator.LoginEmail, email);
                PlayerPrefs.SetInt($"{email}-{method}", 1);
                PlayerPrefs.Save();
                if (_storeSessionWallet && SecureStorageFactory.IsSupportedPlatform())
                {
                    StoreWalletSecurely(walletAddress);
                }
                _isLoggingIn = false;
                EmbeddedWallet.OnWaaSWalletCreated?.Invoke(wallet);
            }
            catch (Exception e)
            {
                OnLoginFailed?.Invoke("Error registering waaSSession: " + e.Message, method, email);
                _isLoggingIn = false;
                _failedLoginIntent = loginIntent;
                _failedLoginMethod = method;
                _failedLoginEmail = email;
                return;
            }

            if (_automaticallyFederateAccountsWhenPossible && _failedLoginEmail == email && !loginIntent.forceCreateAccount) // forceCreateAccount should only be true if we are creating another account for the same email address, meaning we don't have a failed login method that needs federating
            {
                await FederateAccount(new IntentDataFederateAccount(_failedLoginIntent, walletAddress), _failedLoginMethod, email);
            }
        }

        public async Task<string> InitiateAuth(IntentDataInitiateAuth initiateAuthIntent, LoginMethod method)
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

                if (!initiateAuthResponse.ValidateChallenge()) {
                    throw new Exception("Invalid challenge received from WaaS server, received: " + initiateAuthResponse.challenge);
                }
                
                challenge = initiateAuthResponse.challenge;
            }
            catch (Exception e)
            {
                OnLoginFailed?.Invoke("Error initiating auth: " + e.Message, method);
                _isLoggingIn = false;
            }

            return challenge;
        }

        public async Task ConnectToWaaSViaSocialLogin(string idToken, LoginMethod method)
        {
            if (!method.IsOIDC())
            {
                OnLoginFailed?.Invoke($"Invalid login method, given: {method}, expected one of {nameof(IdentityType)}: {nameof(IdentityType.OIDC)}", method);
                _isLoggingIn = false;
                return;
            }
            
            _isLoggingIn = true;
            
            OIDCConnector oidcConnector = new OIDCConnector(idToken, _sessionId, _sessionWallet, _connector);
            
            await oidcConnector.ConnectToWaaSViaSocialLogin(method);
        }

        public void PlayFabLogin(string titleId, string sessionTicket, string email)
        {
            ConnectToWaaSViaPlayFab(titleId, sessionTicket, email);
        }

        public void OverrideAccount()
        {
            OverrideWaaSAccount();
        }

        private async Task OverrideWaaSAccount()
        {
            _failedLoginIntent.forceCreateAccount = true;
            
            await ConnectToWaaS(_failedLoginIntent, _failedLoginMethod, _failedLoginEmail);
            
            List<LoginMethod> loginMethods = EnumExtensions.GetEnumValuesAsList<LoginMethod>();
            loginMethods.Remove(LoginMethod.None);
            int loginMethodsCount = loginMethods.Count;
            for (int i = 0; i < loginMethodsCount; i++)
            {
                LoginMethod method = loginMethods[i];
                PlayerPrefs.SetInt($"{_failedLoginEmail}-{method}", 0);
            }
            PlayerPrefs.Save();
        }

        public async Task ConnectToWaaSViaPlayFab(string titleId, string sessionTicket, string email)
        {
            if (string.IsNullOrWhiteSpace(titleId) || string.IsNullOrWhiteSpace(sessionTicket))
            {
                OnLoginFailed?.Invoke($"Invalid titleId: {titleId} or sessionTicket: {sessionTicket}", LoginMethod.PlayFab);
                _isLoggingIn = false;
                return;
            }
            
            _isLoggingIn = true;
            
            PlayFabConnector playFabConnector = new PlayFabConnector(titleId, sessionTicket, _sessionId, _sessionWallet, _connector);
            
            await playFabConnector.ConnectToWaaSViaPlayFab(email);
        }

        public async Task ConnectToWaaSAsGuest()
        {
            _isLoggingIn = true;
            GuestConnector connector = new GuestConnector(_sessionId, _sessionWallet, _connector);
            await connector.ConnectToWaaSViaGuest();
        }

        public List<LoginMethod> GetLoginMethodsAssociatedWithEmail(string email)
        {
            List<LoginMethod> result = new List<LoginMethod>();
            List<LoginMethod> loginMethods = EnumExtensions.GetEnumValuesAsList<LoginMethod>();
            loginMethods.Remove(LoginMethod.None);
            int loginMethodsCount = loginMethods.Count;
            for (int i = 0; i < loginMethodsCount; i++)
            {
                LoginMethod method = loginMethods[i];
                if (PlayerPrefs.GetInt($"{email}-{method}", 0) == 1)
                {
                    result.Add(method);
                }
            }
            return result;
        }

        private void StoreWalletSecurely(string waasWalletAddress)
        {
            ISecureStorage secureStorage = SecureStorageFactory.CreateSecureStorage();
            byte[] privateKeyBytes = new byte[32];
            _sessionWallet.privKey.WriteToSpan(privateKeyBytes);
            string privateKey = privateKeyBytes.ByteArrayToHexString();
            secureStorage.StoreString(_walletKey, privateKey + "-" + waasWalletAddress);
        }

        public async Task FederateAccount(IntentDataFederateAccount federateAccount, LoginMethod method, string email)
        {
            try
            {
                IntentResponseAccountFederated federateAccountResponse = await _intentSender.SendIntent<IntentResponseAccountFederated, IntentDataFederateAccount>(federateAccount, IntentType.FederateAccount);
                Account account = federateAccountResponse.account;
                string responseEmail = account.email;
                if (responseEmail != email)
                {
                    throw new Exception($"Email received from WaaS server doesn't match, received {responseEmail}, sent {email}");
                }
                
                PlayerPrefs.SetInt($"{email}-{method}", 1);
                PlayerPrefs.Save();
                _failedLoginEmail = "";
                _failedLoginIntent = null;
                _failedLoginMethod = LoginMethod.None;
                EmbeddedWallet.OnAccountFederated?.Invoke(account);
            }
            catch (Exception e)
            {
                EmbeddedWallet.OnAccountFederationFailed?.Invoke("Error federating account: " + e.Message);
            }
        }
        
        public async Task FederateAccountPlayFab(string titleId, string sessionTicket, string email)
        {
            PlayFabConnector playFabConnector = new PlayFabConnector(titleId, sessionTicket, _sessionId, _sessionWallet, _connector);
            await playFabConnector.FederateAccount(email);
        }
        
        public async Task FederateAccountGuest()
        {
            GuestConnector connector = new GuestConnector(_sessionId, _sessionWallet, _connector);
            await connector.FederateAccount();
        }
        
        public async Task FederateAccountSocial(string idToken, LoginMethod method)
        {
            OIDCConnector oidcConnector = new OIDCConnector(idToken, _sessionId, _sessionWallet, _connector);
            await oidcConnector.FederateAccount(method);
        }
        
        public async Task FederateEmail(string email, string code)
        {
            await _emailConnector.FederateAccount(email, code);
        }
    }
}