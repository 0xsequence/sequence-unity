using System;
using Sequence.EcosystemWallet.Authentication;
using Sequence.EcosystemWallet.Primitives;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Boilerplates
{
    public class SequenceEcosystemWalletWindow : MonoBehaviour
    {
        private enum SessionType
        {
            Implicit,
            ExplicitOpen,
            ExplicitRestrictive
        }
        
        [SerializeField] private Button _emailLoginButton;
        [SerializeField] private Button _emailContinueButton;
        [SerializeField] private Button _signMessageButton;
        [SerializeField] private TMP_InputField _emailInput;
        [SerializeField] private TMP_InputField _messageInput;
        [SerializeField] private TMP_Text _walletText;
        [SerializeField] private TMP_Text _signatureText;
        [SerializeField] private GameObject _loginState;
        [SerializeField] private GameObject _walletState;
        [SerializeField] private GameObject _loadingOverlay;
        [SerializeField] private MessagePopup _messagePopup;
        
        private SequenceEcosystemWalletLogin _login;
        private SequenceEcosystemWallet _wallet;
        private SessionType _sessionType;
        private string _curEmail;
        private string _curSignature;
        
        private void Start()
        {
            _login = new(Chain.TestnetArbitrumSepolia);
            _emailInput.onValueChanged.AddListener(VerifyEmailInput);
            _messagePopup.gameObject.SetActive(false);
            _loadingOverlay.SetActive(false);

            OnSessionTypeChanged(0);
            EnableWalletState(false);
            EnableEmailButton(true);
            ShowSignature(string.Empty);

            RecoverWalletFromStorage();
        }
        
        public async void SignInWithEmail()
        {
            SetLoading(true);
            
            try
            {
                var wallet = await _login.SignInWithEmail(_curEmail, GetPermissionsFromSessionType());
                ShowWallet(wallet, false);
            }
            catch (Exception e)
            {
                ShowError(e.Message);
            }
        }
        
        public async void SignInWithGoogle()
        {
            SetLoading(true);
            
            try
            {
                var wallet = await _login.SignInWithGoogle(GetPermissionsFromSessionType());
                ShowWallet(wallet, false);
            }
            catch (Exception e)
            {
                ShowError(e.Message);
            }
        }
        
        public async void SignInWithApple()
        {
            SetLoading(true);

            try
            {
                var wallet = await _login.SignInWithApple(GetPermissionsFromSessionType());
                ShowWallet(wallet, false);
            }
            catch (Exception e)
            {
                ShowError(e.Message);
            }
        }
        
        public async void SignInWithPasskey()
        {
            SetLoading(true);

            try
            {
                var wallet = await _login.SignInWithPasskey(GetPermissionsFromSessionType());
                ShowWallet(wallet, false);
            }
            catch (Exception e)
            {
                ShowError(e.Message);
            }
        }
        
        public async void SignInWithMnemonic()
        {
            SetLoading(true);

            try
            {
                var wallet = await _login.SignInWithMnemonic(GetPermissionsFromSessionType());
                ShowWallet(wallet, false);
            }
            catch (Exception e)
            {
                ShowError(e.Message);
            }
        }

        public async void SignMessage()
        {
            var message = _messageInput.text;
            SetLoading(true);

            try
            {
                var signature = await _wallet.SignMessage(Chain.TestnetArbitrumSepolia, message);
                ShowSignature(signature.signature);
                SetLoading(false);
            }
            catch (Exception e)
            {
                ShowError(e.Message);
            }
        }

        public void CopyWalletAddress()
        {
            CopyText(_wallet.Address.Value);
        }

        public void CopySignature()
        {
            CopyText(_curSignature);
        }
        
        private void CopyText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                _messagePopup.Show("Empty text", true);
                return;
            }
            
            GUIUtility.systemCopyBuffer = text;
            _messagePopup.Show("Copied");
        }

        public void SignOut()
        {
            _login.SignOut();
            EnableWalletState(false);
        }

        public void OnSessionTypeChanged(int index)
        {
            _sessionType = (SessionType)index;
        }

        private void ShowError(string error)
        {
            Debug.LogError(error);
            _messagePopup.Show(error, true);
            SetLoading(false);
        }

        private void RecoverWalletFromStorage()
        {
            var wallet = _login.RecoverSessionFromStorage();
            ShowWallet(wallet, true);
        }

        private void ShowSignature(string signature)
        {
            _curSignature = signature;
            _signatureText.text = signature;
            _signMessageButton.interactable = !string.IsNullOrEmpty(_curSignature);
        }

        private void ShowWallet(SequenceEcosystemWallet wallet, bool recovered)
        {
            _wallet = wallet;
            _walletText.text = wallet.Address;
            
            EnableWalletState(true);
            SetLoading(false);
            
            if (!recovered)
                _messagePopup.Show("Session Created.");
        }

        public void EnableWalletState(bool enable)
        {
            _loginState.SetActive(!enable);
            _walletState.SetActive(enable);
        }
        
        public void EnableEmailButton(bool enable)
        {
            _emailLoginButton.gameObject.SetActive(enable);
            _emailInput.gameObject.SetActive(!enable);
        }

        private void SetLoading(bool value)
        {
            _loadingOverlay.SetActive(value);
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
        
        private SessionPermissions GetPermissionsFromSessionType()
        {
            var templates = new SessionTemplates(Chain.TestnetArbitrumSepolia);
            return _sessionType switch
            {
                SessionType.Implicit => null,
                SessionType.ExplicitOpen => templates.BuildUnrestrictivePermissions(),
                SessionType.ExplicitRestrictive => templates.BuildBasicRestrictivePermissions(),
                _ => throw new Exception("Unsupported session type")
            };
        }
    }
}
