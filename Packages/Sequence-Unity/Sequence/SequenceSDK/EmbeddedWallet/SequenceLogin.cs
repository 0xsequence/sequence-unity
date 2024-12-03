using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.Authentication;
using Sequence.Config;
using Sequence.Utils;
using Sequence.Utils.SecureStorage;
using Sequence.Wallet;
using UnityEngine;

namespace Sequence.EmbeddedWallet
{
    public class SequenceLogin : ILogin, IWaaSConnector
    {
        public static string WaaSWithAuthUrl { get; private set; }
        public const string WaaSLoginMethod = "WaaSLoginMethod";
        public const string EmailInUseError = "EmailAlreadyInUse";

        private int _waasProjectId;
        private string _waasVersion;
        private IAuthenticator _authenticator;
        private IValidator _validator;
        private EOAWallet _sessionWallet;
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

        private Address _connectedWalletAddress;

        private static SequenceLogin _instance;

        private string _verifierToReject; // Since email auth is two separate requests, an invalid signature error may go unnoticed. So, we cache the verifier used in an initiateAuth attempt that had an invalid signature and reject the corresponding openSession attempt if it uses the same verifier.

        public static SequenceLogin GetInstance(IValidator validator = null, IAuthenticator authenticator = null,
            IWaaSConnector connector = null, bool automaticallyFederateAccountsWhenPossible = true,
            Address connectedWalletAddress = null)
        {
            if (_instance == null)
            {
                _instance = new SequenceLogin(validator, authenticator, connector, automaticallyFederateAccountsWhenPossible, connectedWalletAddress);
            }
            if (connectedWalletAddress != null)
            {
                _instance.SetConnectedWalletAddress(connectedWalletAddress);
            }
            return _instance;
        }

        public static SequenceLogin GetInstanceToFederateAuth(Address connectedWalletAddress, IValidator validator = null,
            IAuthenticator authenticator = null,
            IWaaSConnector connector = null, bool automaticallyFederateAccountsWhenPossible = true)
        {
            if (_instance == null)
            {
                _instance = new SequenceLogin(validator, authenticator, connector, automaticallyFederateAccountsWhenPossible, connectedWalletAddress);
            }
            _instance.SetConnectedWalletAddress(connectedWalletAddress);
            return _instance;
        }
        
        public void SetConnectedWalletAddress(Address connectedWalletAddress)
        {
            _connectedWalletAddress = connectedWalletAddress;
        }

        [Obsolete("Use GetInstance() instead.")]
        public SequenceLogin(IValidator validator = null, IAuthenticator authenticator = null, IWaaSConnector connector = null, bool automaticallyFederateAccountsWhenPossible = true, Address connectedWalletAddress = null)
        {
            if (connector == null)
            {
                connector = this;
            }
            _connector = connector;
            
            _automaticallyFederateAccountsWhenPossible = automaticallyFederateAccountsWhenPossible;
            SetConnectedWalletAddress(connectedWalletAddress);
            
            bool storeSessionWallet = SequenceConfig.GetConfig().StoreSessionKey() && SecureStorageFactory.IsSupportedPlatform() && connectedWalletAddress == null;
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

        public void ResetSessionId()
        {
            _sessionWallet = new EOAWallet();
            _sessionId = IntentDataOpenSession.CreateSessionId(_sessionWallet.GetAddress());
            _intentSender = new IntentSender(new HttpClient(WaaSWithAuthUrl), _sessionWallet, _sessionId, _waasProjectId, _waasVersion);
            _emailConnector = new EmailConnector(_sessionId, _sessionWallet, _connector, _validator);
        }

        /// <summary>
        /// Use this to reset the authenticator, validator, and other dependancies to new instances. Useful for when you're testing and using mock implementations
        /// </summary>
        public void ResetLoginAfterTest()
        {
            _connector = this;
            SetupAuthenticator();
        }

        public void SetupAuthenticator(IValidator validator = null, IAuthenticator authenticator = null)
        {
            ConfigJwt configJwt = SequenceConfig.GetConfigJwt();
            if (_connectedWalletAddress == null || _sessionWallet == null)
            {
                _sessionWallet = new EOAWallet();
            }
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

            if (_connectedWalletAddress != null)
            {
                FailedLoginWithStoredSessionWallet("Cannot restore session when connected wallet address is set");
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
            
            int projectId = configJwt.projectId;
            if (string.IsNullOrWhiteSpace(projectId.ToString()))
            {
                throw SequenceConfig.MissingConfigError("Project ID");
            }
            _waasProjectId = projectId;
        }

        private void TryToLoginWithStoredSessionWallet()
        {
            (EOAWallet, string) walletInfo = (null, "");
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
            
            SequenceWallet wallet = new SequenceWallet(new Address(walletInfo.Item2), _sessionId, new IntentSender(new HttpClient(WaaSWithAuthUrl), walletInfo.Item1, _sessionId, _waasProjectId, _waasVersion));

            EnsureSessionIsValid(wallet);
        }

        private void FailedLoginWithStoredSessionWallet(string error)
        {
            CreateWallet();
            SequenceWallet.OnFailedToRecoverSession?.Invoke(error);
        }

        private async Task EnsureSessionIsValid(SequenceWallet wallet)
        {
            Session[] activeSessions = await wallet.ListSessions();
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
                    SequenceWallet.OnWalletCreated?.Invoke(wallet);
                    return;
                }
            }
            
            FailedLoginWithStoredSessionWallet("Stored session wallet is not active");
        }

        private (EOAWallet, string) AttemptToCreateWalletFromSecureStorage()
        {
            ISecureStorage secureStorage = SecureStorageFactory.CreateSecureStorage();
            string walletInfo = secureStorage.RetrieveString(Application.companyName + "-" + Application.productName + "-" + _walletKey);
            if (string.IsNullOrEmpty(walletInfo))
            {
                return (null, "");
            }
            string[] walletInfoSplit = walletInfo.Split('-');
            string privateKey = walletInfoSplit[0];
            string walletAddress = walletInfoSplit[1];
            EOAWallet wallet = new EOAWallet(privateKey);
            return (wallet, walletAddress);
        }
        
        public event ILogin.OnLoginSuccessHandler OnLoginSuccess;
        public event ILogin.OnLoginFailedHandler OnLoginFailed;
        public event ILogin.OnMFAEmailSentHandler OnMFAEmailSent;
        public event ILogin.OnMFAEmailFailedToSendHandler OnMFAEmailFailedToSend;

        public async Task Login(string email)
        {
            ResetSessionId();
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
            if (_connectedWalletAddress != null)
            {
                await FederateEmail(email, code, _connectedWalletAddress);
            }
            else
            {
                await _emailConnector.ConnectToWaaSViaEmail(email, code);
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
            ResetSessionId();
            if (_connectedWalletAddress != null)
            {
                FederateAccountSocial(result.IdToken, result.Method, _connectedWalletAddress);
            }
            else
            {
                ConnectToWaaSViaSocialLogin(result.IdToken, result.Method);
            }
        }

        private void OnSocialSignInFailed(string error, LoginMethod method)
        {
            OnLoginFailed?.Invoke($"Connecting to WaaS API failed due to error with {method} sign in: {error}", method);
        }

        public async Task ConnectToWaaS(IntentDataOpenSession loginIntent, LoginMethod method, string email = "")
        {
            string walletAddress = "";
            if (_verifierToReject == loginIntent.verifier)
            {
                OnLoginFailed?.Invoke("The initiateAuth request associated with this login attempt received a response with an invalid signature. For security reasons, this login request will not be sent to the API as your network traffic may be being monitored. Please try initiating auth again.", method);
                _isLoggingIn = false;
                return;
            }
            try
            {
                IntentResponseSessionOpened registerSessionResponse = await _intentSender.SendIntent<IntentResponseSessionOpened, IntentDataOpenSession>(loginIntent, IntentType.OpenSession);
                string sessionId = registerSessionResponse.sessionId;
                walletAddress = registerSessionResponse.wallet;
                OnLoginSuccess?.Invoke(sessionId, walletAddress);
                SequenceWallet wallet = new SequenceWallet(new Address(walletAddress), sessionId, new IntentSender(new HttpClient(SequenceLogin.WaaSWithAuthUrl), _sessionWallet, sessionId, _waasProjectId, _waasVersion));
                PlayerPrefs.SetInt(WaaSLoginMethod, (int)method);
                PlayerPrefs.SetString(OpenIdAuthenticator.LoginEmail, email);
                PlayerPrefs.Save();
                _isLoggingIn = false;
                wallet.OnDropSessionComplete += session =>
                {
                    if (session == sessionId)
                    {
                        _connectedWalletAddress = null;
                    }
                };
                SequenceWallet.OnWalletCreated?.Invoke(wallet);
            }
            catch (Exception e)
            {
                if (e.Message.Contains(EmailInUseError))
                {
                    List<LoginMethod> associatedLoginMethods = ParseLoginMethods(e.Message);
                    OnLoginFailed?.Invoke("Error registering session: " + e.Message, method, email, associatedLoginMethods);
                }
                else
                {
                    OnLoginFailed?.Invoke("Error registering session: " + e.Message, method, email);
                }
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

            try
            {
                if (_storeSessionWallet && SecureStorageFactory.IsSupportedPlatform())
                {
                    StoreWalletSecurely(walletAddress);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error storing session wallet securely: " + e.Message);
            }
        }

        private List<LoginMethod> ParseLoginMethods(string errorMessage)
        {
            if (!errorMessage.Contains(EmailInUseError))
            {
                throw new ArgumentException($"Error message must contain {EmailInUseError}. Given: {errorMessage}");
            }
            
            string[] errorComponents = errorMessage.Split('{');
            string errorLeft = "{" + errorComponents[1];
            errorComponents = errorLeft.Split('}');
            string error = errorComponents[0] + "}";

            ErrorResponse response = JsonConvert.DeserializeObject<ErrorResponse>(error);
            string cause = response.cause;
            string[] methodStrings = cause.Split(',');
            List<LoginMethod> methods = new List<LoginMethod>();
            int count = methodStrings.Length;
            for (int i = 0; i < count; i++)
            {
                string[] components = methodStrings[i].Trim().Split('|');
                IdentityType identityType = (IdentityType)Enum.Parse(typeof(IdentityType), components[0]);
                switch (identityType)
                {
                    case IdentityType.OIDC:
                        if (components.Length < 3)
                        {
                            Debug.LogError(
                                "Invalid response from WaaS server, expected at least 3 components in OIDC login method string");
                        }

                        if (components[2].Contains("google"))
                        {
                            methods.Add(LoginMethod.Google);
                        }
                        else if (components[2].Contains("apple"))
                        {
                            methods.Add(LoginMethod.Apple);
                        }
                        else if (components[2].Contains("discord"))
                        {
                            methods.Add(LoginMethod.Discord);
                        }
                        else if (components[2].Contains("facebook"))
                        {
                            methods.Add(LoginMethod.Facebook);
                        }
                        else
                        {
                            Debug.LogError("Unexpected OIDC login method string: " + components[2]);
                        }
                        break;
                    case IdentityType.Email:
                        methods.Add(LoginMethod.Email);
                        break;
                    case IdentityType.Guest:
                        methods.Add(LoginMethod.Guest);
                        break;
                    case IdentityType.PlayFab:
                        methods.Add(LoginMethod.PlayFab);
                        break;
                    default:
                        Debug.LogError("Unexpected identity type " + identityType);
                        break;
                }
            }

            return methods;
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
                string error = "Error initiating auth: " + e.Message;
                if (error.Contains("Error validating response"))
                {
                    _verifierToReject = initiateAuthIntent.verifier;
                }
                OnLoginFailed?.Invoke(error, method);
                _isLoggingIn = false;
                throw new Exception(error);
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
            ResetSessionId();
            if (_connectedWalletAddress != null)
            {
                FederateAccountPlayFab(titleId, sessionTicket, email, _connectedWalletAddress);
            }
            else
            {
                ConnectToWaaSViaPlayFab(titleId, sessionTicket, email);
            }
        }

        public void ForceCreateAccount()
        {
            ForceCreateWaaSAccount();
        }

        private async Task ForceCreateWaaSAccount()
        {
            _failedLoginIntent.forceCreateAccount = true;
            
            await ConnectToWaaS(_failedLoginIntent, _failedLoginMethod, _failedLoginEmail);
            
            PlayerPrefs.SetInt(WaaSLoginMethod, (int)_failedLoginMethod);
            PlayerPrefs.SetString(OpenIdAuthenticator.LoginEmail, _failedLoginEmail);
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
            ResetSessionId();
            _isLoggingIn = true;
            GuestConnector connector = new GuestConnector(_sessionId, _sessionWallet, _connector);
            await connector.ConnectToWaaSViaGuest();
        }

        private void StoreWalletSecurely(string waasWalletAddress)
        {
            ISecureStorage secureStorage = SecureStorageFactory.CreateSecureStorage();
            byte[] privateKeyBytes = new byte[32];
            _sessionWallet.privKey.WriteToSpan(privateKeyBytes);
            string privateKey = privateKeyBytes.ByteArrayToHexString();
            secureStorage.StoreString(Application.companyName + "-" + Application.productName + "-" + _walletKey, privateKey + "-" + waasWalletAddress);
        }

        public async Task FederateAccount(IntentDataFederateAccount federateAccount, LoginMethod method, string email)
        {
            try
            {
                IntentResponseAccountFederated federateAccountResponse = await _intentSender.SendIntent<IntentResponseAccountFederated, IntentDataFederateAccount>(federateAccount, IntentType.FederateAccount);
                Account account = federateAccountResponse.account;
                string responseEmail = account.email;
                if (responseEmail != email.ToLower())
                {
                    throw new Exception($"Email received from WaaS server doesn't match, received {responseEmail}, sent {email}");
                }
                
                PlayerPrefs.SetInt(WaaSLoginMethod, (int)method);
                PlayerPrefs.SetString(OpenIdAuthenticator.LoginEmail, email);
                PlayerPrefs.Save();
                _failedLoginEmail = "";
                _failedLoginIntent = null;
                _failedLoginMethod = LoginMethod.None;
                SequenceWallet.OnAccountFederated?.Invoke(account);
            }
            catch (Exception e)
            {
                SequenceWallet.OnAccountFederationFailed?.Invoke("Error federating account: " + e.Message);
            }
        }
        
        public async Task FederateAccountPlayFab(string titleId, string sessionTicket, string email, string walletAddress)
        {
            PlayFabConnector playFabConnector = new PlayFabConnector(titleId, sessionTicket, _sessionId, _sessionWallet, _connector);
            await playFabConnector.FederateAccount(email, walletAddress);
        }
        
        public async Task FederateAccountSocial(string idToken, LoginMethod method, string walletAddress)
        {
            OIDCConnector oidcConnector = new OIDCConnector(idToken, _sessionId, _sessionWallet, _connector);
            await oidcConnector.FederateAccount(method, walletAddress);
        }
        
        public async Task FederateEmail(string email, string code, string walletAddress)
        {
            await _emailConnector.FederateAccount(email, code, walletAddress);
        }
    }
}