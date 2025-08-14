using System;
using System.Linq;
using Sequence.EcosystemWallet;
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
        
        [Header("Config")]
        [SerializeField] private EcosystemType _ecosystem;
        [SerializeField] private Chain _chain;
        
        [Header("Components")]
        [SerializeField] private Button _emailLoginButton;
        [SerializeField] private Button _emailContinueButton;
        [SerializeField] private TMP_Dropdown _chainDropdown;
        [SerializeField] private TMP_InputField _emailInput;
        [SerializeField] private GameObject _loginState;
        [SerializeField] private GameObject _loadingOverlay;
        [SerializeField] private MessagePopup _messagePopup;
        [SerializeField] private GenericObjectPool<SessionWalletTile> _sessionPool;
        
        private IConnect _connect;
        private ImplicitSessionType _implicitPermissions;
        private ExplicitSessionType _explicitPermissions;
        private int _selectedWallet;
        private string _curEmail;
        
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
            Open();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            _messagePopup.gameObject.SetActive(false);
            _loadingOverlay.SetActive(false);

            OnImplicitSessionTypeChanged(0);
            OnExplicitSessionTypeChanged(0);
            EnableEmailButton(true);
            
            _chainDropdown.ClearOptions();
            _chainDropdown.AddOptions(_chains.Select(c => c.ToString()).ToList());
        }
        
        public async void SignInWithEmail()
        {
            SetLoading(true);
            
            try
            {
                await _connect.SignInWithEmail(_curEmail);
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
                await _connect.SignInWithGoogle(GetImplicitPermissions());
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
                await _connect.SignInWithApple(GetImplicitPermissions());
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
                await _connect.SignInWithPasskey(GetImplicitPermissions());
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
                await _connect.SignInWithMnemonic(GetImplicitPermissions());
            }
            catch (Exception e)
            {
                ShowError(e.Message);
            }
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

        private IPermissions GetImplicitPermissions()
        {
            return GetPermissionsFromSessionType((int)_implicitPermissions);
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
