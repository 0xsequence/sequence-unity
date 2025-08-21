using System;
using Sequence.EcosystemWallet;
using Sequence.Relayer;
using UnityEngine;

namespace Sequence.Boilerplates
{
    public class EcosystemFeatureSelection : MonoBehaviour
    {
        public static EcosystemFeatureSelection Instance;
        
        [SerializeField] private SequenceEcosystemWalletWindow _login;
        [SerializeField] private GameObject _featureSelection;
        [SerializeField] private EcosystemWalletProfile _profile;
        [SerializeField] private EcosystemWalletTransactions _transactions;
        [SerializeField] private FeeOptionWindow _feeOptionWindow;

        private IWallet _wallet;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SequenceWallet.Disconnected += OpenLogin;
            SequenceWallet.WalletCreated += UpdateWallet;

            _login.gameObject.SetActive(false);
            _featureSelection.SetActive(false);
            _profile.gameObject.SetActive(false);
            _transactions.gameObject.SetActive(false);

            _wallet = SequenceWallet.RecoverFromStorage();
            if (_wallet == null)
                OpenLogin();
            else
                OpenFeatureSelection();
        }

        private void OnDestroy()
        {
            SequenceWallet.Disconnected -= OpenLogin;
            SequenceWallet.WalletCreated -= UpdateWallet;
        }

        private void UpdateWallet(IWallet wallet)
        {
            _wallet = wallet;
            OpenFeatureSelection();
        }

        public void OpenLogin()
        {
            _login.Open();
        }
        
        public void OpenFeatureSelection()
        {
            _login.gameObject.SetActive(false);
            _featureSelection.SetActive(true);
        }
        
        public void OpenProfile()
        {
            _featureSelection.SetActive(false);
            _profile.Load(_wallet, OpenFeatureSelection);
        }

        public void OpenTransactions()
        {
            _featureSelection.SetActive(false);
            _transactions.Load(_wallet, OpenFeatureSelection);
        }

        public void OpenFeeOptionWindow(FeeOption[] feeOptions, Action<FeeOption> onSelected)
        {
            _feeOptionWindow.WaitForSelection(feeOptions, onSelected);
        }
    }
}
