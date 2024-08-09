using System;
using System.Net.Sockets;
using Sequence.Authentication;
using Sequence.EmbeddedWallet;
using TMPro;
using UnityEngine;
using Sequence.Config;
using Sequence.Utils;
using Sequence.Utils.SecureStorage;
using UnityEngine.Serialization;

namespace Sequence.Demo
{
    public class LoginPanel : UIPanel
    {
        [FormerlySerializedAs("_waaSSessionManagerPrefab")] [SerializeField] private GameObject _sessionManagerPrefab;
        [SerializeField] private GameObject _federatedAuthPopupPanelPrefab;
        private bool _storeSessionInfoAndSkipLoginWhenPossible = false;
        
        private TransitionPanel _transitionPanel;
        private LoginPage _loginPage;
        private MultifactorAuthenticationPage _mfaPage;
        private LoginSuccessPage _loginSuccessPage;
        private EmbeddedWalletDemoPage _demoPage;
        private static string _urlScheme;
        private FederatedAuthPopupPanel _federatedAuthPopupPanel;
        internal ILogin LoginHandler { get; private set; }
        
        private bool _alreadyAttemptedToRestoreSession = false;
        private GameObject _sessionManager;
        
        protected override void Awake()
        {
            base.Awake();
            _transitionPanel = FindObjectOfType<TransitionPanel>();
            
            _loginPage = GetComponentInChildren<LoginPage>();
            _mfaPage = GetComponentInChildren<MultifactorAuthenticationPage>();

            _demoPage = FindObjectOfType<EmbeddedWalletDemoPage>();

            SequenceConfig config = SequenceConfig.GetConfig();
            
            SequenceWallet.OnFailedToRecoverSession += OnFailedToRecoverSession;

            _storeSessionInfoAndSkipLoginWhenPossible = config.StoreSessionKey();
            
            ILogin loginHandler = SequenceLogin.GetInstance();
            SetupLoginHandler(loginHandler);

            _loginSuccessPage = GetComponentInChildren<LoginSuccessPage>();
            
            if (_urlScheme == null) 
            {
                _urlScheme = config.UrlScheme;
            }
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
                    _alreadyAttemptedToRestoreSession = true;
                    LoginHandler.TryToRestoreSession();
                    return;
                }

                if (_storeSessionInfoAndSkipLoginWhenPossible)
                {
                    LoginHandler.SetupAuthenticator();
                }
            }

            base.Open(args);
        }

        private void OnDestroy()
        {
            SequenceWallet.OnFailedToRecoverSession -= OnFailedToRecoverSession;
            TearDownLoginHandler();
        }

        public void SetupLoginHandler(ILogin loginHandler)
        {
            LoginHandler = loginHandler;

            _loginPage.SetupLogin(loginHandler);
            loginHandler.OnMFAEmailSent += OnMFAEmailSentHandler;
            
            _mfaPage.SetupLogin(loginHandler);
            loginHandler.OnLoginSuccess += OnLoginSuccessHandler;
            
            SequenceWallet.OnWalletCreated += OnWalletCreatedHandler;
            
            GameObject popupPanel = Instantiate(_federatedAuthPopupPanelPrefab, transform.parent);
            _federatedAuthPopupPanel = popupPanel.GetComponent<FederatedAuthPopupPanel>();
            if (_federatedAuthPopupPanel != null)
            {
                _federatedAuthPopupPanel.InjectILogin(loginHandler);
            }

            popupPanel.SetActive(false);

            Instantiate(_sessionManagerPrefab);
        } 
        
        private void TearDownLoginHandler()
        {
            LoginHandler.OnMFAEmailSent -= OnMFAEmailSentHandler;
            LoginHandler.OnLoginSuccess -= OnLoginSuccessHandler;
            SequenceWallet.OnWalletCreated -= OnWalletCreatedHandler;
            Destroy(_sessionManager);
        } 
        
        private void OnFailedToRecoverSession(string error)
        {
            Debug.LogWarning($"Failed to recover session with Sequence API using stored session wallet: {error}");
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

        private void OnWalletCreatedHandler(SequenceWallet wallet)
        {
            if (_demoPage != null)
            {
                _demoPage.Open(wallet);
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