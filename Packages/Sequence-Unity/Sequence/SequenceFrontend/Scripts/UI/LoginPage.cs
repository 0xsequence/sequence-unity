using System.Collections.Generic;
using Sequence.Authentication;
using Sequence.EmbeddedWallet;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class LoginPage : UIPage
    {
        [SerializeField] private TextMeshProUGUI _errorText;
        [SerializeField] private GameObject _infoPopupPanelPrefab;
        [SerializeField] private GameObject _loadingScreen;

        private TMP_InputField _inputField;
        private LoginMethod _loginMethod;
        private string _loginEmail;
        private LoginButtonHighlighter _loginButtonHighlighter;

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

            SetEmailInputInitialText();
        }

        protected virtual void SetEmailInputInitialText()
        {
            if (_inputField == null)
            {
                _inputField = GetComponentInChildren<TMP_InputField>();
                if (_inputField == null)
                {
                    return;
                }
            }

            _inputField.text = _loginEmail;
        }

        public override void Close()
        {
            base.Close();
            _errorText.text = "";
            EnableLoadingScreen(false);

            LoginHandler.OnMFAEmailFailedToSend -= OnMFAEmailFailedToSendHandler;
            LoginHandler.OnLoginFailed -= OnLoginFailedHandler;
            SequenceWallet.OnAccountFederationFailed -= OnAccountFederationFailedHandler;
        }

        public void SetupLogin(ILogin loginHandler)
        {
            LoginHandler = loginHandler;
            LoginHandler.OnMFAEmailFailedToSend += OnMFAEmailFailedToSendHandler;
            LoginHandler.OnLoginFailed += OnLoginFailedHandler;
            SequenceWallet.OnAccountFederationFailed += OnAccountFederationFailedHandler;
            EnableLoadingScreen(false);
        }

        public void Login()
        {
            string email = _inputField.text;
            Debug.Log($"Signing in with email: {email}");
            _errorText.text = "";
            EnableLoadingScreen(true);
            LoginHandler.Login(email);
        }

        public void GoogleLogin()
        {
            Debug.Log("Google Login");
            EnableLoadingScreen(true);
            LoginHandler.GoogleLogin();
        }

        public void DiscordLogin()
        {
            Debug.Log("Discord Login");
            EnableLoadingScreen(true);
            LoginHandler.DiscordLogin();
        }

        public void FacebookLogin()
        {
            Debug.Log("Facebook Login");
            EnableLoadingScreen(true);
            LoginHandler.FacebookLogin();
        }

        public void AppleLogin()
        {
            Debug.Log("Apple Login");
            EnableLoadingScreen(true);
            LoginHandler.AppleLogin();
        }

        private void OnMFAEmailFailedToSendHandler(string email, string error)
        {
            Debug.LogError($"Failed to send MFA email to {email} with error: {error}");
            SetError(error);
        }

        private void OnLoginFailedHandler(string error, LoginMethod method, string email,
            List<LoginMethod> loginMethods)
        {
            Debug.LogError($"Failed to sign in to WaaS API with error: {error}");
            SetError(error);
        }

        private void OnAccountFederationFailedHandler(string error)
        {
            Debug.LogError($"Failed to federate account with Sequence API: {error}");
            SetError(error);
        }

        private LoginMethod GetLoginMethod()
        {
            if (PlayerPrefs.HasKey(SequenceLogin.WaaSLoginMethod))
            {
                return (LoginMethod) PlayerPrefs.GetInt(SequenceLogin.WaaSLoginMethod);
            }

            return LoginMethod.None;
        }

        private string GetLoginEmail()
        {
            if (PlayerPrefs.HasKey(OpenIdAuthenticator.LoginEmail))
            {
                return PlayerPrefs.GetString(OpenIdAuthenticator.LoginEmail);
            }

            return "";
        }

        private void SetError(string error)
        {
            _errorText.text = error;
            EnableLoadingScreen(false);
        }

        private void EnableLoadingScreen(bool enable)
        {
            _loadingScreen.SetActive(enable);
        }
    }
}