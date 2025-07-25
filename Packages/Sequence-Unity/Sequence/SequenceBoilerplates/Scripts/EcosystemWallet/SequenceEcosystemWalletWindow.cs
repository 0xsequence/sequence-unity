using System;
using System.Linq;
using Sequence.EcosystemWallet;
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
        [SerializeField] private TMP_Dropdown _chainDropdown;
        [SerializeField] private TMP_InputField _emailInput;
        [SerializeField] private TMP_InputField _messageInput;
        [SerializeField] private TMP_Text _walletText;
        [SerializeField] private TMP_Text _signatureText;
        [SerializeField] private GameObject _loginState;
        [SerializeField] private GameObject _walletState;
        [SerializeField] private GameObject _loadingOverlay;
        [SerializeField] private MessagePopup _messagePopup;
        [SerializeField] private GenericObjectPool<SessionWalletTile> _sessionPool;
        
        private SequenceConnect _login;
        private SequenceWallet _wallet;
        private ImplicitSessionType _implicitPermissions;
        private ExplicitSessionType _explicitPermissions;
        private int _selectedWallet;
        private string _curEmail;
        private string _curSignature;
        
        private Chain[] _chains =
        {
            Chain.TestnetArbitrumSepolia,
            Chain.ArbitrumOne
        };
        
        private void Start()
        {
            _login = new(Chain.TestnetArbitrumSepolia, EcosystemType.Sequence);
            
            _emailInput.onValueChanged.AddListener(VerifyEmailInput);
            _messagePopup.gameObject.SetActive(false);
            _loadingOverlay.SetActive(false);

            OnImplicitSessionTypeChanged(0);
            OnExplicitSessionTypeChanged(0);
            EnableWalletState(false);
            EnableEmailButton(true);
            ShowSignature(string.Empty);
            
            if (_login.GetAllSessionWallets().Length > 0)
                ShowWallet(true);
            
            _chainDropdown.ClearOptions();
            _chainDropdown.AddOptions(_chains.Select(c => c.ToString()).ToList());
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
                var signature = await _wallet.SignMessage(_login.Chain, message);
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
        
        public void OnChainChanged(int index)
        {
            _login.SetChain(_chains[index]);
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

        private void ShowSignature(string signature)
        {
            _curSignature = signature;
            _signatureText.text = signature;
            _signMessageButton.interactable = !string.IsNullOrEmpty(_curSignature);
        }

        private void ShowWallet(bool recovered)
        {
            _wallet = _login.GetWallet();
            _walletText.text = _wallet.Address.Value;

            LoadSessions();
            EnableWalletState(true);
            SetLoading(false);
            
            if (!recovered)
                _messagePopup.Show("Session Created.");
        }

        private void RemoveSession(Address address)
        {
            _login.RemoveSession(address);
            LoadSessions();
        }

        private void LoadSessions()
        {
            _sessionPool.Cleanup();
            foreach (var wallet in _wallet.SessionWallets)
                _sessionPool.GetObject().Apply(wallet, RemoveSession);
        }

        public void EnableWalletState(bool enable)
        {
            _loginState.SetActive(!enable);
            _walletState.SetActive(enable);
            _signOutButton.gameObject.SetActive(enable);

            var rect = transform as RectTransform;
            var size = rect.sizeDelta;
            size.y = enable ? 315 : 290;
            rect.sizeDelta = size;
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
            var templates = new SessionTemplates(_login.Chain);
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
