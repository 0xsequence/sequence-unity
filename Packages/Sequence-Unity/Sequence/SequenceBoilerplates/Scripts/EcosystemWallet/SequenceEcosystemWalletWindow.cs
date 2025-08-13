using System;
using System.Linq;
using System.Numerics;
using NBitcoin;
using Sequence.EcosystemWallet;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Transaction = Sequence.EcosystemWallet.Transaction;

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
        
        [Header("Config")]
        [SerializeField] private EcosystemType _ecosystem;
        [SerializeField] private Chain _chain;
        
        [Header("Components")]
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
        
        private IWallet _wallet;
        private IConnect _connect;
        private ImplicitSessionType _implicitPermissions;
        private ExplicitSessionType _explicitPermissions;
        private int _selectedWallet;
        private string _curEmail;
        private string _curSignature;
        
        private readonly Chain[] _chains =
        {
            Chain.TestnetArbitrumSepolia,
            Chain.ArbitrumOne,
            Chain.Optimism,
        };
        
        private void Start()
        {
            _connect = new SequenceConnect(_ecosystem);
            _emailInput.onValueChanged.AddListener(VerifyEmailInput);
            _messagePopup.gameObject.SetActive(false);
            _loadingOverlay.SetActive(false);

            OnImplicitSessionTypeChanged(0);
            OnExplicitSessionTypeChanged(0);
            EnableWalletState(false);
            EnableEmailButton(true);
            ShowSignature(string.Empty);
            
            _chainDropdown.ClearOptions();
            _chainDropdown.AddOptions(_chains.Select(c => c.ToString()).ToList());

            _wallet = SequenceWallet.RecoverFromStorage();
            if (_wallet != null)
                ShowWallet(true);
        }
        
        public async void SignInWithEmail()
        {
            SetLoading(true);
            
            try
            {
                _wallet = await _connect.SignInWithEmail(_curEmail);
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
                _wallet = await _connect.SignInWithGoogle(GetImplicitPermissions());
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
                _wallet = await _connect.SignInWithApple(GetImplicitPermissions());
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
                _wallet = await _connect.SignInWithPasskey(GetImplicitPermissions());
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
                _wallet = await _connect.SignInWithMnemonic(GetImplicitPermissions());
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
                var signature = await _wallet.SignMessage(_chain, message);
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
                await _wallet.AddSession(GetExplicitPermissions());
                SetLoading(false);
                LoadSessions();
            }
            catch (Exception e)
            {
                ShowError(e.Message);
            }
        }

        public void SignOut()
        {
            _wallet.Disconnect();
            EnableWalletState(false);
        }
        
        public void OnChainChanged(int index)
        {
            _chain = _chains[index];
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
            _walletText.text = _wallet.Address.Value;

            LoadSessions();
            EnableWalletState(true);
            SetLoading(false);
            
            if (!recovered)
                _messagePopup.Show("Session Created.");
        }

        private void RemoveSession(Address address)
        {
            LoadSessions();
        }

        private void LoadSessions()
        {
            _sessionPool.Cleanup();
            foreach (var wallet in _wallet.GetAllSigners())
                _sessionPool.GetObject().Apply(wallet, RemoveSession);
        }

        public void EnableWalletState(bool enable)
        {
            _loginState.SetActive(!enable);
            _walletState.SetActive(enable);
            _signOutButton.gameObject.SetActive(enable);

            var rect = transform as RectTransform;
            var size = rect.sizeDelta;
            size.y = enable ? 420 : 290;
            rect.sizeDelta = size;
        }
        
        public void EnableEmailButton(bool enable)
        {
            _emailLoginButton.gameObject.SetActive(enable);
            _emailInput.gameObject.SetActive(!enable);
        }

        public async void SendImplicitTransaction()
        {
            await _wallet.SendTransaction(Chain.TestnetArbitrumSepolia, new ITransaction[]
            {
                new Transaction(new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA"), 0, "implicitEmit()")
            });
        }
        
        public async void SendExplicitTransaction()
        {
            await _wallet.SendTransaction(Chain.TestnetArbitrumSepolia, new ITransaction[]
            {
                new Transaction(new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA"), 0, "explicitEmit()")
            });
        }
        
        public async void SendExplicitTransactionWithUsdc()
        {
            var txn = new ITransaction[]
            {
                new Transaction(new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA"), 0, "explicitEmit()")
            };
            
            var feeOptions = await _wallet.GetFeeOption(Chain.Optimism, txn);
            var feeOption = feeOptions.First(o => o.token.symbol == "USDC");
            if (feeOption == null)
                throw new Exception($"Fee option 'USDC' not available");
            
            await _wallet.SendTransaction(Chain.Optimism, txn, feeOption);
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

        private IPermissions GetImplicitPermissions()
        {
            return GetPermissionsFromSessionType((int)_implicitPermissions);
        }
        
        private IPermissions GetExplicitPermissions()
        {
            return GetPermissionsFromSessionType((int)_explicitPermissions + 1);
        }
        
        private IPermissions GetPermissionsFromSessionType(int type)
        {
            if (type == 0)
                return null;
            
            var deadline = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000 + 60 * 60 * 24 * 1000;
            
            var permissions = new Permissions(Chain.Optimism,
                new ContractPermission(new Address("0x7F5c764cBc14f9669B88837ca1490cCa17c31607"), deadline, 0),
                new ContractPermission(new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA"), deadline, 0));

            return permissions;
        }
    }
}
