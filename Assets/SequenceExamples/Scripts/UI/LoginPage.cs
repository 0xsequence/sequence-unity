using System;
using Sequence.Authentication;
using Sequence.Demo.Tweening;
using Sequence.WaaS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class LoginPage : UIPage
    {
        [SerializeField] private TextMeshProUGUI _errorText;
        
        private TMP_InputField _inputField;
        private LoginMethod _loginMethod;
        private LoginButtonHighlighter _loginButtonHighlighter;
        private InfoPopupPanel _infoPopupPanel;
        private bool _hasShownInfoPopupForDifferentLoginMethod = false;
        internal ILogin LoginHandler { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            _inputField = GetComponentInChildren<TMP_InputField>();
            _loginButtonHighlighter = GetComponent<LoginButtonHighlighter>();
            _infoPopupPanel = FindObjectOfType<InfoPopupPanel>();
        }

        private void Start()
        {
            _loginMethod = GetLoginMethod();
            if (_loginButtonHighlighter != null)
            {
                _loginButtonHighlighter.HighlightAppropriateButton(_loginMethod);
            }
        }

        public override void Close()
        {
            base.Close();
            _errorText.text = "";
            LoginHandler.OnMFAEmailFailedToSend -= OnMFAEmailFailedToSendHandler;
        }

        public void SetupLogin(ILogin loginHandler)
        {
            LoginHandler = loginHandler;
            LoginHandler.OnMFAEmailFailedToSend += OnMFAEmailFailedToSendHandler;
        }

        public void Login()
        {
            if (_loginMethod != LoginMethod.Email && !_hasShownInfoPopupForDifferentLoginMethod)
            {
                NotifyUserTheyAreLoggingInOnADifferentAccount();
                return;
            }
            string email = _inputField.text;
            Debug.Log($"Signing in with email: {email}");
            _errorText.text = "";
            LoginHandler.Login(email);
        }

        public void GoogleLogin()
        {
            if (_loginMethod != LoginMethod.Google && !_hasShownInfoPopupForDifferentLoginMethod)
            {
                NotifyUserTheyAreLoggingInOnADifferentAccount();
                return;
            }
            Debug.Log("Google Login");
            LoginHandler.GoogleLogin();
        }

        public void DiscordLogin()
        {
            if (_loginMethod != LoginMethod.Discord && !_hasShownInfoPopupForDifferentLoginMethod)
            {
                NotifyUserTheyAreLoggingInOnADifferentAccount();
                return;
            }
            Debug.Log("Discord Login");
            LoginHandler.DiscordLogin();
        }

        public void FacebookLogin()
        {
            if (_loginMethod != LoginMethod.Facebook && !_hasShownInfoPopupForDifferentLoginMethod)
            {
                NotifyUserTheyAreLoggingInOnADifferentAccount();
                return;
            }
            Debug.Log("Facebook Login");
            LoginHandler.FacebookLogin();
        }

        public void AppleLogin()
        {
            if (_loginMethod != LoginMethod.Apple && !_hasShownInfoPopupForDifferentLoginMethod)
            {
                NotifyUserTheyAreLoggingInOnADifferentAccount();
                return;
            }
            Debug.Log("Apple Login");
            LoginHandler.AppleLogin();
        }
        
        private void OnMFAEmailFailedToSendHandler(string email, string error)
        {
            Debug.LogError($"Failed to send MFA email to {email} with error: {error}");
            _errorText.text = error;
        }

        public void SubscribeToWaaSWalletCreatedEvent(Action<WaaSWallet> OnWaaSWalletCreatedHandler)
        {
            if (LoginHandler is WaaSLogin login)
            {
                login.OnWaaSWalletCreated += OnWaaSWalletCreatedHandler;
            }
        }
        
        private LoginMethod GetLoginMethod()
        {
            if (PlayerPrefs.HasKey(WaaSLogin.WaaSLoginMethod))
            {
                return (LoginMethod) PlayerPrefs.GetInt(WaaSLogin.WaaSLoginMethod);
            }
            return LoginMethod.None;
        }
        
        private void NotifyUserTheyAreLoggingInOnADifferentAccount()
        {
            string message = $"Last time, you logged in with <b>{_loginMethod}</b>. Logging in with a different method will use a different account.";
            if (_infoPopupPanel != null)
            {
                _infoPopupPanel.Open(message);
                _hasShownInfoPopupForDifferentLoginMethod = true;
            }
        }
    }
}