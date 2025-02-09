using System;
using System.Collections.Generic;
using Sequence.Authentication;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceSDK.Samples
{
    public class SequenceLoginWindow : MonoBehaviour
    {
        [SerializeField] private Button _emailLoginButton;
        [SerializeField] private Button _emailContinueButton;
        [SerializeField] private TMP_InputField _emailInput;
        [SerializeField] private TMP_InputField _emailCodeInput;
        [SerializeField] private Transform _socialButtonsParent;
        [SerializeField] private GameObject _loginState;
        [SerializeField] private GameObject _emailCodeState;
        [SerializeField] private GameObject _loadingOverlay;
        [SerializeField] private GameObject[] _socialTexts;

        private ILogin _loginHandler;
        
        private void Start()
        {
            _emailInput.onValueChanged.AddListener(VerifyEmailInput);
        }

        private void OnDestroy()
        {
            _loginHandler.OnLoginSuccess -= LoginHandlerOnOnLoginSuccess;
            _loginHandler.OnLoginFailed -= LoginHandlerOnOnLoginFailed;
            _loginHandler.OnMFAEmailSent -= LoginHandlerOnOnMFAEmailSent;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public void Show(ILogin loginHandler)
        {
            if (_loginHandler == null)
            {
                _loginHandler = loginHandler;
                _loginHandler.OnLoginSuccess += LoginHandlerOnOnLoginSuccess;
                _loginHandler.OnLoginFailed += LoginHandlerOnOnLoginFailed;
                _loginHandler.OnMFAEmailSent += LoginHandlerOnOnMFAEmailSent;
            }
            
            gameObject.SetActive(true);
            _emailLoginButton.gameObject.SetActive(true);
            _emailInput.gameObject.SetActive(false);
            _loginState.SetActive(true);
            _emailCodeState.SetActive(false);
            _emailInput.text = string.Empty;
            VerifyEmailInput(string.Empty);
            HandleSocialIconState();
            SetLoading(false);
        }

        public void LoginWithEmail()
        {
            SetLoading(true);
            _loginHandler.Login(_emailInput.text);
        }

        public void VerifyEmailCode()
        {
            SetLoading(true);
            _loginHandler.Login(_emailInput.text, _emailCodeInput.text);
        }

        public void LoginWithGoogle()
        {
            SetLoading(true);
            _loginHandler.GoogleLogin();
        }

        public void LoginWithApple()
        {
            SetLoading(true);
            _loginHandler.AppleLogin();
        }

        public void LoginAsGuest()
        {
            SetLoading(true);
            _loginHandler.GuestLogin();
        }

        private void VerifyEmailInput(string input)
        {
            var parts = input.Split("@");
            var validEmail = parts.Length == 2 && parts[0].Length > 1 && parts[1].Length > 1;
            _emailContinueButton.interactable = validEmail;
        }

        private void HandleSocialIconState()
        {
            var enableText = _socialButtonsParent.childCount == 1;
            foreach (var social in _socialTexts)
                social.SetActive(enableText);
        }

        private void SetLoading(bool loading)
        {
            _loadingOverlay.SetActive(loading);
        }
        
        private void LoginHandlerOnOnMFAEmailSent(string email)
        {
            SetLoading(false);
            _loginState.SetActive(false);
            _emailCodeState.SetActive(true);
        }
        
        private void LoginHandlerOnOnLoginSuccess(string sessionid, string walletaddress)
        {
            SetLoading(false);
        }
        
        private void LoginHandlerOnOnLoginFailed(string error, LoginMethod method, string email, List<LoginMethod> loginmethods)
        {
            Debug.LogError(error);
            SetLoading(false);
            _emailCodeInput.text = string.Empty;
        }
    }
}
