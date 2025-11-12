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
            _login.gameObject.SetActive(false);
            _featureSelection.SetActive(false);
            _profile.gameObject.SetActive(false);
            _transactions.gameObject.SetActive(false);
        }
        
        public void OpenFeatureSelection()
        {
            _login.gameObject.SetActive(false);
            _featureSelection.SetActive(true);
        }
        
        public void OpenProfile()
        {
            _featureSelection.SetActive(false);
            _profile.Show(_wallet, OpenFeatureSelection);
        }

        public void OpenTransactions()
        {
            _featureSelection.SetActive(false);
            _transactions.Show(_wallet, OpenFeatureSelection);
        }
    }
}
