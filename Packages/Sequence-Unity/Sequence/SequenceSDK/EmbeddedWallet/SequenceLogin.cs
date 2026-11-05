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

        // Empty for the default (singleton) instance. Isolated instances persist their session under their own key
        // and don't raise the static SequenceWallet events, so they can run alongside the default instance.
        private readonly string _storageNamespace = "";
        private LoginMethod _lastLoginMethod = LoginMethod.None;

        /// <summary>
        /// True for instances created with <see cref="CreateIsolatedInstance"/>.
        /// </summary>
        public bool IsIsolated => !string.IsNullOrEmpty(_storageNamespace);

        /// <summary>
        /// Raised when this instance creates or restores a wallet session. Raised for both default and isolated
        /// instances. The default instance also raises <see cref="SequenceWallet.OnWalletCreated"/>.
        /// </summary>
        public event Action<SequenceWallet> OnSessionWalletCreated;

        /// <summary>
        /// Raised when this instance fails to restore a stored session. The default instance also raises
        /// <see cref="SequenceWallet.OnFailedToRecoverSession"/>.
        /// </summary>
        public event Action<string> OnSessionRecoveryFailed;

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
            
            _instance.SetupAuthenticator();
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
            if (connectedWalletAddress == null)
            {
                SequenceLog.Error($"The connected wallet address cannot be null or empty.");
                throw new ArgumentNullException(nameof(connectedWalletAddress));
            }
            
            _connectedWalletAddress = connectedWalletAddress;
        }
        
        public void RemoveConnectedWalletAddress()
        {
            _connectedWalletAddress = null;
        }

        /// <summary>
        /// Create a login instance that is independent of <see cref="GetInstance"/>. Use it to hold a second wallet
        /// session (for example, an existing email account) while the default instance keeps the current guest session.
        /// The instance stores its session under its own namespace, does not raise the static
        /// <see cref="SequenceWallet.OnWalletCreated"/> / <see cref="SequenceWallet.OnFailedToRecoverSession"/> events
        /// (use <see cref="OnSessionWalletCreated"/> / <see cref="OnSessionRecoveryFailed"/>), does not federate
        /// accounts automatically and does not touch the PlayerPrefs used for the last login method.
        /// Call <see cref="PromoteToDefaultSession"/> to make its wallet the app's active session.
        /// </summary>
        /// <param name="storageNamespace">Non-empty name unique to this instance, e.g. "user".</param>
        public static SequenceLogin CreateIsolatedInstance(string storageNamespace, IValidator validator = null,
            IAuthenticator authenticator = null)
        {
            if (string.IsNullOrWhiteSpace(storageNamespace))
            {
                throw new ArgumentException("An isolated instance needs a non-empty storage namespace.", nameof(storageNamespace));
            }

            var login = new SequenceLogin(validator, authenticator, null, false, null, storageNamespace);
            if (login._storeSessionWallet)
            {
                // The constructor only configures endpoints when session storage is enabled; GetInstance() completes
                // the setup for the default instance the same way.
                login.SetupAuthenticator(validator, authenticator);
            }
            return login;
        }

        [Obsolete("Use GetInstance() instead.")]
        public SequenceLogin(IValidator validator = null, IAuthenticator authenticator = null, IWaaSConnector connector = null, bool automaticallyFederateAccountsWhenPossible = true, Address connectedWalletAddress = null)
            : this(validator, authenticator, connector, automaticallyFederateAccountsWhenPossible, connectedWalletAddress, "")
        {
        }

        private SequenceLogin(IValidator validator, IAuthenticator authenticator, IWaaSConnector connector,
            bool automaticallyFederateAccountsWhenPossible, Address connectedWalletAddress, string storageNamespace)
        {
            _storageNamespace = storageNamespace ?? "";

            if (connector == null)
            {
                connector = this;
            }
            _connector = connector;
            
            _automaticallyFederateAccountsWhenPossible = automaticallyFederateAccountsWhenPossible;

            if (connectedWalletAddress != null)
            {
                SetConnectedWalletAddress(connectedWalletAddress);
            }
            
            bool storeSessionWallet = SequenceConfig.GetConfig(SequenceService.WaaS).StoreSessionKey() && SecureStorageFactory.IsSupportedPlatform() && connectedWalletAddress == null;
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
            if (_connectedWalletAddress != null) {
                return;
            }
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
            RemoveConnectedWalletAddress();
            SetupAuthenticator();
        }

        public void SetupAuthenticator(IValidator validator = null, IAuthenticator authenticator = null)
        {
            ConfigJwt configJwt = SequenceConfig.GetConfigJwt(SequenceConfig.GetConfig(SequenceService.WaaS));
            if (_sessionWallet == null)
            {
                _sessionWallet = new EOAWallet();
            }
            
            _sessionId = IntentDataOpenSession.CreateSessionId(_sessionWallet.GetAddress());
            _intentSender = new IntentSender(new HttpClient(WaaSWithAuthUrl, true), _sessionWallet, _sessionId, _waasProjectId, _waasVersion);

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

        /// <summary>
        /// Recover the current session asynchronously and get the associated wallet.
        /// </summary>
        /// <returns>
        /// Returns StorageEnabled bool indicating if the SDK is configured to store sessions.
        /// Returns Instance of IWallet if the session was recovered. Returns null if no session was found.
        /// </returns>
        public async Task<(bool StorageEnabled, IWallet Wallet)> TryToRestoreSessionAsync()
        {
            var config = SequenceConfig.GetConfig();
            var storeSessionInfoAndSkipLoginWhenPossible = config.StoreSessionKey();
            if (!SecureStorageFactory.IsSupportedPlatform() || !storeSessionInfoAndSkipLoginWhenPossible)
                return (false, null);
            
            var done = false;
            SequenceWallet wallet = null;
            OnSessionRecoveryFailed += HandleFailedToRecover;
            OnSessionWalletCreated += HandleRecoveredWallet;

            TryToRestoreSession();
            SetupAuthenticator();

            while (!done)
                await Task.Yield();

            return (true, wallet);

            void HandleRecoveredWallet(SequenceWallet newWallet)
            {
                wallet = newWallet;
                done = true;

                OnSessionRecoveryFailed -= HandleFailedToRecover;
                OnSessionWalletCreated -= HandleRecoveredWallet;
            }

            void HandleFailedToRecover(string error)
            {
                HandleRecoveredWallet(null);
            }
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

        public async Task GuestLogin()
        {
            await ConnectToWaaSAsGuest();
        }

        private void SetupAuthenticatorAndListeners()
        {
            try {
                _authenticator.PlatformSpecificSetup();
            }
            catch (Exception e) {
                SequenceLog.Error($"Error encountered during PlatformSpecificSetup: {e.Message}\nSocial sign in will not work.");
            }
            Application.deepLinkActivated += _authenticator.HandleDeepLink;
            _authenticator.SignedIn += OnSocialLogin;
            _authenticator.OnSignInFailed += OnSocialSignInFailed;
        }

        private void Configure()
        {
            SequenceConfig config = SequenceConfig.GetConfig(SequenceService.WaaS);
            string waasVersion = config.WaaSVersion;
            if (string.IsNullOrWhiteSpace(waasVersion))
            {
                throw SequenceConfig.MissingConfigError("WaaS Version");
            }
            _waasVersion = waasVersion;

            ConfigJwt configJwt = SequenceConfig.GetConfigJwt(config);

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
            (EOAWallet, string, string) walletInfo = (null, "", "");
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
            
            SequenceWallet wallet = new SequenceWallet(new Address(walletInfo.Item2), _sessionId, new IntentSender(new HttpClient(WaaSWithAuthUrl), walletInfo.Item1, _sessionId, _waasProjectId, _waasVersion), walletInfo.Item3);
            wallet.AttachSession(this, walletInfo.Item1);

            EnsureSessionIsValid(wallet);
        }

        private void FailedLoginWithStoredSessionWallet(string error)
        {
            CreateWallet();
            NotifyRecoveryFailed(error);
        }

        private void NotifyWalletCreated(SequenceWallet wallet)
        {
            OnSessionWalletCreated?.Invoke(wallet);
            if (!IsIsolated)
            {
                SequenceWallet.OnWalletCreated?.Invoke(wallet);
            }
        }

        private void NotifyRecoveryFailed(string error)
        {
            OnSessionRecoveryFailed?.Invoke(error);
            if (!IsIsolated)
            {
                SequenceWallet.OnFailedToRecoverSession?.Invoke(error);
            }
        }

        private void PersistLastLoginMethod(LoginMethod method, string email)
        {
            _lastLoginMethod = method;
            if (IsIsolated)
            {
                return;
            }
            PlayerPrefs.SetInt(WaaSLoginMethod, (int)method);
            PlayerPrefs.SetString(OpenIdAuthenticator.LoginEmail, email);
            PlayerPrefs.Save();
        }

        private static string StorageKeyFor(string storageNamespace)
        {
            string prefix = Application.companyName + "-" + Application.productName + "-";
            return string.IsNullOrEmpty(storageNamespace) ? prefix + _walletKey : prefix + storageNamespace + "-" + _walletKey;
        }

        private string SessionStorageKey => StorageKeyFor(_storageNamespace);

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
                    NotifyWalletCreated(wallet);
                    return;
                }
            }
            
            FailedLoginWithStoredSessionWallet("Stored session wallet is not active");
        }

        private (EOAWallet, string, string) AttemptToCreateWalletFromSecureStorage()
        {
            ISecureStorage secureStorage = SecureStorageFactory.CreateSecureStorage();
            string walletInfo = secureStorage.RetrieveString(SessionStorageKey);
            if (string.IsNullOrEmpty(walletInfo))
            {
                return (null, "", "");
            }
            string[] walletInfoSplit = walletInfo.Split('-');
            string privateKey = walletInfoSplit[0];
            string walletAddress = walletInfoSplit[1];
            string email = "";
            if (walletInfoSplit.Length == 3)
            {
                email = walletInfoSplit[2];
            }
            EOAWallet wallet = new EOAWallet(privateKey);
            return (wallet, walletAddress, email);
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
                SequenceWallet wallet = new SequenceWallet(new Address(walletAddress), sessionId, new IntentSender(new HttpClient(SequenceLogin.WaaSWithAuthUrl), _sessionWallet, sessionId, _waasProjectId, _waasVersion), email);
                wallet.AttachSession(this, _sessionWallet);
                PersistLastLoginMethod(method, email);
                _isLoggingIn = false;
                wallet.OnDropSessionComplete += session =>
                {
                    if (session == sessionId)
                    {
                        _connectedWalletAddress = null;
                    }
                };
                NotifyWalletCreated(wallet);
            }
            catch (Exception e)
            {
                var emailInUse = e.Message.Contains(EmailInUseError);
                if (emailInUse)
                {
                    List<LoginMethod> associatedLoginMethods = ParseLoginMethods(e.Message);
                    OnLoginFailed?.Invoke("Error registering session: " + e.Message, method, email, associatedLoginMethods);
                    _failedLoginIntent = loginIntent;
                    _failedLoginMethod = method;
                    _failedLoginEmail = email;
                }
                else
                {
                    OnLoginFailed?.Invoke("Error registering session: " + e.Message, method, email);
                }
                _isLoggingIn = false;
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
                    StoreWalletSecurely(walletAddress, email);
                }
            }
            catch (Exception e)
            {
                SequenceLog.Error("Error storing session wallet securely: " + e.Message);
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
                            SequenceLog.Error(
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
                            SequenceLog.Error("Unexpected OIDC login method string: " + components[2]);
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
                        SequenceLog.Error("Unexpected identity type " + identityType);
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
            if (string.IsNullOrEmpty(_failedLoginEmail))
                throw new Exception("Failed to force create account.");
            
            _failedLoginIntent.forceCreateAccount = true;
            
            await ConnectToWaaS(_failedLoginIntent, _failedLoginMethod, _failedLoginEmail);

            PersistLastLoginMethod(_failedLoginMethod, _failedLoginEmail);
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

        internal void StoreWalletSecurely(string waasWalletAddress, string email)
        {
            StoreSessionSecurely(_sessionWallet, waasWalletAddress, email);
        }

        /// <summary>
        /// Persist the given session key under this instance's storage namespace.
        /// </summary>
        internal void StoreSessionSecurely(EOAWallet sessionKey, string waasWalletAddress, string email)
        {
            if (!_storeSessionWallet || !SecureStorageFactory.IsSupportedPlatform() || sessionKey == null) return;
            WriteSession(SessionStorageKey, sessionKey, waasWalletAddress, email);
        }

        private static void WriteSession(string storageKey, EOAWallet sessionKey, string waasWalletAddress, string email)
        {
            ISecureStorage secureStorage = SecureStorageFactory.CreateSecureStorage();
            byte[] privateKeyBytes = new byte[32];
            sessionKey.privKey.WriteToSpan(privateKeyBytes);
            string privateKey = privateKeyBytes.ByteArrayToHexString();
            secureStorage.StoreString(storageKey, privateKey + "-" + waasWalletAddress + "-" + email);
        }

        /// <summary>
        /// Remove the session stored under this instance's namespace. The backend session is not closed; use
        /// <see cref="SequenceWallet.DropThisSession"/> for that.
        /// </summary>
        public void ClearStoredSession()
        {
            if (!SecureStorageFactory.IsSupportedPlatform()) return;
            SecureStorageFactory.CreateSecureStorage().StoreString(SessionStorageKey, "");
        }

        /// <summary>
        /// Make a wallet created by this isolated instance the app's active session: it is stored under the default
        /// session key (replacing whatever is stored there), removed from this instance's namespace, and announced via
        /// <see cref="SequenceWallet.OnWalletCreated"/> so <c>EmbeddedWalletAdapter</c> and
        /// <see cref="SessionManager"/> switch to it. Close the previous default session (e.g. the guest) yourself,
        /// after any transfers from it have been confirmed.
        /// </summary>
        public void PromoteToDefaultSession(SequenceWallet wallet)
        {
            if (!IsIsolated)
            {
                throw new InvalidOperationException("Only isolated instances can promote a session.");
            }
            if (wallet == null)
            {
                throw new ArgumentNullException(nameof(wallet));
            }
            if (!ReferenceEquals(wallet.Owner, this) || wallet.SessionKey == null)
            {
                throw new ArgumentException("The wallet was not created by this login instance.", nameof(wallet));
            }

            SequenceLogin defaultLogin = _instance ?? GetInstance();
            if (_storeSessionWallet && SecureStorageFactory.IsSupportedPlatform())
            {
                WriteSession(StorageKeyFor(""), wallet.SessionKey, wallet.GetWalletAddress(), wallet.Email);
                ClearStoredSession();
            }

            defaultLogin.RemoveConnectedWalletAddress();
            defaultLogin.PersistLastLoginMethod(_lastLoginMethod, wallet.Email);
            wallet.AttachSession(defaultLogin, wallet.SessionKey);
            SequenceWallet.OnWalletCreated?.Invoke(wallet);
        }

        public async Task FederateAccount(IntentDataFederateAccount federateAccount, LoginMethod method, string email)
        {
            try
            {
                IntentResponseAccountFederated federateAccountResponse = await _intentSender.SendIntent<IntentResponseAccountFederated, IntentDataFederateAccount>(federateAccount, IntentType.FederateAccount);
                Account account = federateAccountResponse.account;
                account.wallet = new Address(federateAccount.wallet);
                string responseEmail = account.email;
                if (responseEmail != email.ToLower())
                {
                    throw new Exception($"Email received from WaaS server doesn't match, received {responseEmail}, sent {email}");
                }
                
                PersistLastLoginMethod(method, email);
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