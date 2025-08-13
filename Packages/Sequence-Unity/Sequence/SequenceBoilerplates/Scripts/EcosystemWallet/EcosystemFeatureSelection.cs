using Sequence.EcosystemWallet;
using UnityEngine;

namespace Sequence.Boilerplates
{
    public class EcosystemFeatureSelection : MonoBehaviour
    {
        [SerializeField] private SequenceEcosystemWalletWindow _login;
        [SerializeField] private GameObject _featureSelection;
        [SerializeField] private EcosystemWalletProfile _profile;
        [SerializeField] private EcosystemWalletTransactions _transactions;

        private IWallet _wallet;
        
        private void Start()
        {
            SequenceWallet.Disconnected += OpenLogin;
            SequenceWallet.WalletCreated += wallet =>
            {
                _wallet = wallet;
                OpenFeatureSelection();
            };

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
    }
}
