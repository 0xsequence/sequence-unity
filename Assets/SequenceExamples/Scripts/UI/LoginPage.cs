using System;
using Sequence.Authentication;
using Sequence.Demo.Tweening;
using Sequence.WaaS;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class LoginPage : UIPage
    {
        public bool NotifyUserIfTheyAreLoggingInWithADifferentAccountFromLastTime = true;
        [SerializeField] private TextMeshProUGUI _errorText;
        [SerializeField] private GameObject _infoPopupPanelPrefab;
        [SerializeField] private GameObject _loadingScreenPrefab;
        
        private TMP_InputField _inputField;
        private LoginMethod _loginMethod;
        private string _loginEmail;
        private LoginButtonHighlighter _loginButtonHighlighter;
        private bool _hasShownInfoPopupForDifferentLoginMethod = false;
        internal ILogin LoginHandler { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            _inputField = GetComponentInChildren<TMP_InputField>();
            _loginButtonHighlighter = GetComponent<LoginButtonHighlighter>();
        }

        private void Start()
        {
            _loginMethod = GetLoginMethod();
            
            if (_loginButtonHighlighter != null)
            {
                _loginButtonHighlighter.HighlightAppropriateButton(_loginMethod);
            }

            _loginEmail = GetLoginEmail();

            if (_loginMethod == LoginMethod.Email)
            {
                _inputField.text = _loginEmail;
            }
        }

        public override void Close()
        {
            base.Close();
            _errorText.text = "";
            LoginHandler.OnMFAEmailFailedToSend -= OnMFAEmailFailedToSendHandler;
            LoginHandler.OnLoginFailed -= OnLoginFailedHandler;
        }

        public void SetupLogin(ILogin loginHandler)
        {
            LoginHandler = loginHandler;
            LoginHandler.OnMFAEmailFailedToSend += OnMFAEmailFailedToSendHandler;
            LoginHandler.OnLoginFailed += OnLoginFailedHandler;
        }

        public void Login()
        {
            string email = _inputField.text;
            if (NotifyUserIfTheyAreLoggingInWithADifferentAccountFromLastTime)
            {
                if ((_loginMethod != LoginMethod.Email || (_loginMethod == LoginMethod.Email && _loginEmail != email))  && !_hasShownInfoPopupForDifferentLoginMethod)
                {
                    NotifyUserTheyAreLoggingInOnADifferentAccount();
                    return;
                }
            }
            Debug.Log($"Signing in with email: {email}");
            _errorText.text = "";
            LoginHandler.Login(email);
            InstantiateLoadingScreen();
        }

        public void GoogleLogin()
        {
            if (NotifyUserIfTheyAreLoggingInWithADifferentAccountFromLastTime)
            {
                if (_loginMethod != LoginMethod.Google && !_hasShownInfoPopupForDifferentLoginMethod)
                {
                    NotifyUserTheyAreLoggingInOnADifferentAccount();
                    return;
                }
            }
            Debug.Log("Google Login");
            LoginHandler.GoogleLogin();
            InstantiateLoadingScreen();
        }

        public void DiscordLogin()
        {
            if (NotifyUserIfTheyAreLoggingInWithADifferentAccountFromLastTime)
            {
                if (_loginMethod != LoginMethod.Discord && !_hasShownInfoPopupForDifferentLoginMethod)
                {
                    NotifyUserTheyAreLoggingInOnADifferentAccount();
                    return;
                }
            }
            Debug.Log("Discord Login");
            LoginHandler.DiscordLogin();
            InstantiateLoadingScreen();
        }

        public void FacebookLogin()
        {
            if (NotifyUserIfTheyAreLoggingInWithADifferentAccountFromLastTime)
            {
                if (_loginMethod != LoginMethod.Facebook && !_hasShownInfoPopupForDifferentLoginMethod)
                {
                    NotifyUserTheyAreLoggingInOnADifferentAccount();
                    return;
                }
            }
            Debug.Log("Facebook Login");
            LoginHandler.FacebookLogin();
            InstantiateLoadingScreen();
        }

        public void AppleLogin()
        {
            if (NotifyUserIfTheyAreLoggingInWithADifferentAccountFromLastTime)
            {
                if (_loginMethod != LoginMethod.Apple && !_hasShownInfoPopupForDifferentLoginMethod)
                {
                    NotifyUserTheyAreLoggingInOnADifferentAccount();
                    return;
                }
            }
            Debug.Log("Apple Login");
            LoginHandler.AppleLogin();
            InstantiateLoadingScreen();
        }
        
        private void OnMFAEmailFailedToSendHandler(string email, string error)
        {
            Debug.LogError($"Failed to send MFA email to {email} with error: {error}");
            _errorText.text = error;
        }
        
        private void OnLoginFailedHandler(string error)
        {
            Debug.LogError($"Failed to sign in to WaaS API with error: {error}");
            _errorText.text = error;
        }
        
        private LoginMethod GetLoginMethod()
        {
            if (PlayerPrefs.HasKey(WaaSLogin.WaaSLoginMethod))
            {
                return (LoginMethod) PlayerPrefs.GetInt(WaaSLogin.WaaSLoginMethod);
            }

            _hasShownInfoPopupForDifferentLoginMethod = true; // Don't show popup if we can't find the previous login method
            return LoginMethod.None;
        }
        
        private void NotifyUserTheyAreLoggingInOnADifferentAccount()
        {
            string message = $"Last time, you logged in with <b>{_loginMethod}</b>. Logging in with a different method will use a different account.";
            if (_loginMethod == LoginMethod.Email)
            {
                message = $"Last time, you logged in with <b>{_loginMethod}</b>: <i>{_loginEmail}</i>. Logging in with a different method or email will use a different account.";
            }
            GameObject infoPopupPanelGameObject = Instantiate(_infoPopupPanelPrefab, transform.parent);
            InfoPopupPanel infoPopupPanel = infoPopupPanelGameObject.GetComponent<InfoPopupPanel>();
            if (infoPopupPanel != null)
            {
                infoPopupPanel.Open(message);
                _hasShownInfoPopupForDifferentLoginMethod = true;
            }
        }

        private string GetLoginEmail()
        {
            if (PlayerPrefs.HasKey(OpenIdAuthenticator.LoginEmail))
            {
                return PlayerPrefs.GetString(OpenIdAuthenticator.LoginEmail);
            }

            return "";
        }

        private void InstantiateLoadingScreen()
        {
            Instantiate(_loadingScreenPrefab, transform.parent);
        }
    }
}