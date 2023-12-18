using System;
using System.Net.Sockets;
using Sequence.Authentication;
using Sequence.WaaS;
using TMPro;
using UnityEngine;
using Sequence.Config;

namespace Sequence.Demo
{
    public class LoginPanel : UIPanel
    {
        private TransitionPanel _transitionPanel;
        private LoginPage _loginPage;
        private MultifactorAuthenticationPage _mfaPage;
        private LoginSuccessPage _loginSuccessPage;
        private WaaSDemoPage _waasDemoPage;
        private static string _urlScheme;
        
        protected override void Awake()
        {
            base.Awake();
            _transitionPanel = FindObjectOfType<TransitionPanel>();
            
            _loginPage = GetComponentInChildren<LoginPage>();
            _mfaPage = GetComponentInChildren<MultifactorAuthenticationPage>();

            _waasDemoPage = FindObjectOfType<WaaSDemoPage>();

            SequenceConfig config = SequenceConfig.GetConfig();
            
            ILogin loginHandler = new WaaSLogin(new AWSConfig(
                config.Region, 
                config.IdentityPoolId, 
                config.KMSEncryptionKeyId,
                config.CognitoClientId),
                config.WaaSProjectId, 
                config.WaaSVersion);
            SetupLoginHandler(loginHandler);

            _loginSuccessPage = GetComponentInChildren<LoginSuccessPage>();
            
            if (_urlScheme == null) 
            {
                _urlScheme = SequenceConfig.GetConfig().UrlScheme;
            }
        }

        public void SetupLoginHandler(ILogin loginHandler)
        {
            _loginPage.SetupLogin(loginHandler);
            loginHandler.OnMFAEmailSent += OnMFAEmailSentHandler;
            
            _mfaPage.SetupLogin(loginHandler);
            loginHandler.OnLoginSuccess += OnLoginSuccessHandler;
            
            WaaSWallet.OnWaaSWalletCreated += OnWaaSWalletCreatedHandler;
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
        
        // On Windows standalone, deep link will open a second instance of tghe game.
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
        }
    }
}