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
        internal ILogin LoginHandler { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            _inputField = GetComponentInChildren<TMP_InputField>();
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
            string email = _inputField.text;
            Debug.Log($"Signing in with email: {email}");
            _errorText.text = "";
            LoginHandler.Login(email);
        }

        public void GoogleLogin()
        {
            Debug.Log("Google Login");
            LoginHandler.GoogleLogin();
        }

        public void DiscordLogin()
        {
            Debug.Log("Discord Login");
            LoginHandler.DiscordLogin();
        }

        public void FacebookLogin()
        {
            Debug.Log("Facebook Login");
            LoginHandler.FacebookLogin();
        }

        public void AppleLogin()
        {
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
            WaaSLogin.OnWaaSWalletCreated += OnWaaSWalletCreatedHandler;
        }
    }
}