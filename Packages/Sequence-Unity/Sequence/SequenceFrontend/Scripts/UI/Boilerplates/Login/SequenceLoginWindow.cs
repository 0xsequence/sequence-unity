using System;
using System.Collections;
using System.Collections.Generic;
using Sequence.Authentication;
using Sequence.EmbeddedWallet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Boilerplates.Login
{
    public class SequenceLoginWindow : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _emailLoginButton;
        [SerializeField] private Button _emailContinueButton;
        [SerializeField] private Button _guestLoginButton;
        [SerializeField] private TMP_InputField _emailInput;
        [SerializeField] private TMP_InputField _emailCodeInput;
        [SerializeField] private TMP_Text _emailCodeErrorText;
        [SerializeField] private Transform _socialButtonsParent;
        [SerializeField] private GameObject _loginState;
        [SerializeField] private GameObject _emailCodeState;
        [SerializeField] private GameObject _loadingOverlay;
        [SerializeField] private MessagePopup _messagePopup;
        [SerializeField] private GameObject[] _socialTexts;

        private Action _onClose;
        private SequenceLogin _loginHandler;
        private string _curEmail;
        
        private void Start()
        {
            _emailInput.onValueChanged.AddListener(VerifyEmailInput);
        }

        private void OnEnable()
        {
            SequenceWallet.OnAccountFederated += AccountFederated;
            SequenceWallet.OnAccountFederationFailed += AccountFederationFailed;
        }

        private void OnDisable()
        {
            SequenceWallet.OnAccountFederated -= AccountFederated;
            SequenceWallet.OnAccountFederationFailed -= AccountFederationFailed;
        }

        private void OnDestroy()
        {
            _loginHandler.OnLoginSuccess -= LoginHandlerOnOnLoginSuccess;
            _loginHandler.OnLoginFailed -= LoginHandlerOnOnLoginFailed;
            _loginHandler.OnMFAEmailSent -= LoginHandlerOnOnMFAEmailSent;
        }

        /// <summary>
        /// This function is called when the user clicks the close button.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            _onClose?.Invoke();
        }
        
        /// <summary>
        /// Required function to configure this Boilerplate.
        /// </summary>
        public void Show(Action onClose = null)
        {
            _onClose = onClose;
            if (_loginHandler == null)
            {
                _loginHandler = SequenceLogin.GetInstance();
                _loginHandler.OnLoginSuccess += LoginHandlerOnOnLoginSuccess;
                _loginHandler.OnLoginFailed += LoginHandlerOnOnLoginFailed;
                _loginHandler.OnMFAEmailSent += LoginHandlerOnOnMFAEmailSent;
            }

            var isFederating = _loginHandler.HasConnectedWalletAddress();
            _closeButton.gameObject.SetActive(isFederating);
            _guestLoginButton.gameObject.SetActive(!isFederating);
            
            gameObject.SetActive(true);
            _messagePopup.gameObject.SetActive(false);
            _loginState.SetActive(true);
            _emailCodeState.SetActive(false);
            _emailInput.text = string.Empty;
            _emailCodeErrorText.text = string.Empty;
            EnableEmailButton(true);
            VerifyEmailInput(string.Empty);
            HandleSocialIconState();
            SetLoading(false);
        }

        public void LoginWithEmail()
        {
            SetLoading(true);
            _loginHandler.Login(_curEmail);
        }

        public void VerifyEmailCode()
        {
            SetLoading(true);
            _loginHandler.Login(_curEmail, _emailCodeInput.text);
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

        public void EnableEmailButton(bool enable)
        {
            _emailLoginButton.gameObject.SetActive(enable);
            _emailInput.gameObject.SetActive(!enable);
        }
        
        private void VerifyEmailInput(string input)
        {
            _curEmail = input;
            var parts = _curEmail.Split("@");
            var validEmail = _curEmail.Contains(".") && 
                             parts.Length == 2 && 
                             parts[0].Length > 1 && 
                             parts[1].Length > 1;
            
            _emailContinueButton.interactable = validEmail;
        }

        private void HandleSocialIconState()
        {
            var activeChildCount = 0;
            foreach (Transform children in _socialButtonsParent)
                if (children.gameObject.activeSelf)
                    activeChildCount++;
            
            var enableText = activeChildCount == 1;
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
            Debug.LogError($"Error during login: {error}");
            SetLoading(false);
            _emailCodeInput.text = string.Empty;

            if (method == LoginMethod.Email)
                _emailCodeErrorText.text = "Invalid code.";
            else
                _messagePopup.Show(error, true);
        }
        
        private void AccountFederationFailed(string error)
        {
            Debug.LogError($"Failed to federate account with error: {error}");
            _messagePopup.Show(error, true);
            SetLoading(false);
        }

        private void AccountFederated(Account account)
        {
            Debug.Log($"Account federated, email: {account.email}");
            Hide();
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
#if !UNITY_IOS
            if (hasFocus)
            {
                StartCoroutine(DisableLoadingScreenIfNotLoggingIn());
            }
#endif
        }

        private IEnumerator DisableLoadingScreenIfNotLoggingIn()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            if (!_loginHandler.IsLoggingIn())
                SetLoading(false);
        }
    }
}
