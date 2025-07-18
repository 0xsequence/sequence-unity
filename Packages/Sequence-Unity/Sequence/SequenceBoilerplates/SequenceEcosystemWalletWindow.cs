using System;
using System.Linq;
using Sequence.EcosystemWallet.Authentication;
using Sequence.EcosystemWallet.Primitives;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Boilerplates
{
    public class SequenceEcosystemWalletWindow : MonoBehaviour
    {
        private enum ImplicitSessionType
        {
            None,
            Unrestrictive,
            BasicRestrictive
        }
        
        private enum ExplicitSessionType
        {
            Unrestrictive,
            BasicRestrictive
        }
        
        [SerializeField] private Button _emailLoginButton;
        [SerializeField] private Button _emailContinueButton;
        [SerializeField] private Button _signOutButton;
        [SerializeField] private Button _signMessageButton;
        [SerializeField] private TMP_Dropdown _walletDropdown;
        [SerializeField] private TMP_InputField _emailInput;
        [SerializeField] private TMP_InputField _messageInput;
        [SerializeField] private TMP_Text _walletText;
        [SerializeField] private TMP_Text _signatureText;
        [SerializeField] private GameObject _loginState;
        [SerializeField] private GameObject _walletState;
        [SerializeField] private GameObject _loadingOverlay;
        [SerializeField] private MessagePopup _messagePopup;
        
        private SequenceEcosystemWalletLogin _login;
        private SequenceEcosystemWallet[] _wallets;
        private ImplicitSessionType _implicitPermissions;
        private ExplicitSessionType _explicitPermissions;
        private int _selectedWallet;
        private string _curEmail;
        private string _curSignature;
        
        private void Start()
        {
            _login = new(Chain.TestnetArbitrumSepolia);
            _emailInput.onValueChanged.AddListener(VerifyEmailInput);
            _messagePopup.gameObject.SetActive(false);
            _loadingOverlay.SetActive(false);

            OnImplicitSessionTypeChanged(0);
            OnExplicitSessionTypeChanged(0);
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
                await _login.SignInWithEmail(_curEmail, GetImplicitPermissions());
                ShowWallet(false);
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
                await _login.SignInWithGoogle(GetImplicitPermissions());
                ShowWallet(false);
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
                await _login.SignInWithApple(GetImplicitPermissions());
                ShowWallet(false);
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
                await _login.SignInWithPasskey(GetImplicitPermissions());
                ShowWallet(false);
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
                await _login.SignInWithMnemonic(GetImplicitPermissions());
                ShowWallet(false);
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
                var signature = await _wallets[_selectedWallet].SignMessage(Chain.TestnetArbitrumSepolia, message);
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
            CopyText(_wallets[_selectedWallet].Address.Value);
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

        public async void AddExplicitSession()
        {
            SetLoading(true);

            try
            {
                await _login.AddSession(GetExplicitPermissions());
                ShowWallet(false);
            }
            catch (Exception e)
            {
                ShowError(e.Message);
            }
        }

        public void SignOut()
        {
            _login.SignOut();
            EnableWalletState(false);
        }
        
        public void OnWalletChanged(int index)
        {
            _selectedWallet = index;
        }

        public void OnImplicitSessionTypeChanged(int index)
        {
            _implicitPermissions = (ImplicitSessionType)index;
        }
        
        public void OnExplicitSessionTypeChanged(int index)
        {
            _explicitPermissions = (ExplicitSessionType)index;
        }

        private void ShowError(string error)
        {
            Debug.LogError(error);
            _messagePopup.Show(error, true);
            SetLoading(false);
        }

        private void RecoverWalletFromStorage()
        {
            ShowWallet(true);
        }

        private void ShowSignature(string signature)
        {
            _curSignature = signature;
            _signatureText.text = signature;
            _signMessageButton.interactable = !string.IsNullOrEmpty(_curSignature);
        }

        private void ShowWallet(bool recovered)
        {
            _wallets = _login.RecoverSessionsFromStorage();
            _walletText.text = _wallets[_selectedWallet].Address.Value;
            
            var addresses = _wallets.Select(w => w.SessionAddress.Value).ToList();
            _walletDropdown.ClearOptions();
            _walletDropdown.AddOptions(addresses);
            _walletDropdown.value = 0;
            _selectedWallet = 0;
            
            EnableWalletState(true);
            SetLoading(false);
            
            if (!recovered)
                _messagePopup.Show("Session Created.");
        }

        public void EnableWalletState(bool enable)
        {
            _loginState.SetActive(!enable);
            _walletState.SetActive(enable);
            _signOutButton.gameObject.SetActive(enable);
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

        private SessionPermissions GetImplicitPermissions()
        {
            return GetPermissionsFromSessionType((int)_implicitPermissions);
        }
        
        private SessionPermissions GetExplicitPermissions()
        {
            return GetPermissionsFromSessionType((int)_explicitPermissions + 1);
        }
        
        private SessionPermissions GetPermissionsFromSessionType(int type)
        {
            var templates = new SessionTemplates(Chain.TestnetArbitrumSepolia);
            return type switch
            {
                0 => null,
                1 => templates.BuildUnrestrictivePermissions(),
                2 => templates.BuildBasicRestrictivePermissions(),
                _ => throw new Exception("Unsupported session type")
            };
        }
    }
}
