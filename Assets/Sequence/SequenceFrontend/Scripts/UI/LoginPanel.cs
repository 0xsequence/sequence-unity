using System;
using System.Net.Sockets;
using Sequence.Authentication;
using Sequence.WaaS;
using TMPro;
using UnityEngine;
using Sequence.Config;
using Sequence.Utils;
using Sequence.Utils.SecureStorage;

namespace Sequence.Demo
{
    public class LoginPanel : UIPanel
    {
        [SerializeField] private GameObject _waaSSessionManagerPrefab;
        private bool _storeSessionInfoAndSkipLoginWhenPossible = false;
        
        private TransitionPanel _transitionPanel;
        private LoginPage _loginPage;
        private MultifactorAuthenticationPage _mfaPage;
        private LoginSuccessPage _loginSuccessPage;
        private WaaSDemoPage _waasDemoPage;
        private static string _urlScheme;
        internal ILogin LoginHandler { get; private set; }
        
        private bool _alreadyAttemptedToRestoreSession = false;
        
        protected override void Awake()
        {
            base.Awake();
            _transitionPanel = FindObjectOfType<TransitionPanel>();
            
            _loginPage = GetComponentInChildren<LoginPage>();
            _mfaPage = GetComponentInChildren<MultifactorAuthenticationPage>();

            _waasDemoPage = FindObjectOfType<WaaSDemoPage>();

            SequenceConfig config = SequenceConfig.GetConfig();
            
            WaaSWallet.OnFailedToLoginWithStoredSessionWallet += OnFailedToLoginWithStoredSessionWalletHandler;

            _storeSessionInfoAndSkipLoginWhenPossible = config.StoreSessionPrivateKeyInSecureStorage;
            
            ILogin loginHandler = WaaSLogin.GetInstance();
            SetupLoginHandler(loginHandler);

            _loginSuccessPage = GetComponentInChildren<LoginSuccessPage>();
            
            if (_urlScheme == null) 
            {
                _urlScheme = config.UrlScheme;
            }
        }

        private void Start()
        {
            LoginHandler.TryToRestoreSession();
        }

        /// <summary>
        /// Open LoginPanel, include bool: true if you wish to open the login page after attempting to restore a session
        /// </summary>
        /// <param name="args"></param>
        public override void Open(params object[] args)
        {
            if (!_alreadyAttemptedToRestoreSession)
            {
                _alreadyAttemptedToRestoreSession = args.GetObjectOfTypeIfExists<bool>();
            }

            if (SecureStorageFactory.IsSupportedPlatform())
            {
                if (_storeSessionInfoAndSkipLoginWhenPossible && !_alreadyAttemptedToRestoreSession)
                {
                    return;
                }
            }

            base.Open(args);
        }

        private void OnDestroy()
        {
            WaaSWallet.OnFailedToLoginWithStoredSessionWallet -= OnFailedToLoginWithStoredSessionWalletHandler;
        }

        public void SetupLoginHandler(ILogin loginHandler)
        {
            LoginHandler = loginHandler;

            _loginPage.SetupLogin(loginHandler);
            loginHandler.OnMFAEmailSent += OnMFAEmailSentHandler;
            
            _mfaPage.SetupLogin(loginHandler);
            loginHandler.OnLoginSuccess += OnLoginSuccessHandler;
            
            WaaSWallet.OnWaaSWalletCreated += OnWaaSWalletCreatedHandler;

            Instantiate(_waaSSessionManagerPrefab);
        } 
        
        private void OnFailedToLoginWithStoredSessionWalletHandler(string error)
        {
            Debug.LogWarning($"Failed to connect to Sequence API with stored session wallet: {error}");
            Open(true);
        }

        public void OpenTransitionPanel()
        {
            if (_transitionPanel != null)
            {
                _transitionPanel.OpenWithDelay(_closeAnimationDurationInSeconds);
            }
        }

        private void OnLoginSuccessHandler(string sessionId, string walletAddress)
        {
            Debug.Log($"Successful login with session Id: {sessionId} | wallet address: {walletAddress}");
            StartCoroutine(SetUIPage(_loginSuccessPage));
        }

        private void OnMFAEmailSentHandler(string email)
        {
            Debug.Log($"Successfully sent MFA email to {email}");
            StartCoroutine(SetUIPage(_mfaPage, email));
        }
        
        // On Windows standalone, deep link will open a second instance of the game.
        // We need to catch this, and send our deep link URL to the already-running instance (through a TCP server)
#if UNITY_STANDALONE_WIN
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void PassDeepLinkViaLocalServer()
        {
            var args = System.Environment.GetCommandLineArgs();
            if (_urlScheme == null) 
            {
                _urlScheme = SequenceConfig.GetConfig().UrlScheme;
            }
            if (args.Length > 1 && args[1].StartsWith(_urlScheme))
            {
                var socketConnection = new TcpClient("localhost", OpenIdAuthenticator.WINDOWS_IPC_PORT);
                var bytes = System.Text.Encoding.ASCII.GetBytes("@@@@" + args[1] + "$$$$");
                socketConnection.GetStream().Write(bytes, 0, bytes.Length);
                socketConnection.Close();
                Application.Quit();
            }
        }
#endif

        private void OnWaaSWalletCreatedHandler(WaaSWallet wallet)
        {
            if (_waasDemoPage != null)
            {
                _waasDemoPage.Open(wallet);
            }
            else if (_storeSessionInfoAndSkipLoginWhenPossible)
            {
                _gameObject.SetActive(true);
                _animator.AnimateIn( _openAnimationDurationInSeconds);
                _isOpen = true;
                
                StartCoroutine(SetUIPage(_loginSuccessPage));
            }
        }
    }
}