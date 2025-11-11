using System;
using Sequence.EmbeddedWallet;
using UnityEngine;
using IWallet = Sequence.EcosystemWallet.IWallet;

namespace Sequence.Boilerplates
{
    public class EcosystemWalletHome : MonoBehaviour
    {
        private EcosystemWalletProfile _profile;
        private EcosystemWalletTransactions _transactions;

        private IWallet _wallet;
        
        public void Show(IWallet wallet)
        {
            _wallet = wallet;
        }
        
        public void OpenProfile()
        {
            gameObject.SetActive(false);
            GetProfile().Show(_wallet, () => gameObject.SetActive(true));
        }

        public void OpenTransactions()
        {
            gameObject.SetActive(false);
            GetTransactions().Show(_wallet, () => gameObject.SetActive(true));
        }
        
        private EcosystemWalletProfile GetProfile()
        {
            if (!_profile)
                _profile = BoilerplateFactory.OpenEcosystemWalletProfile(transform.parent, _wallet);

            return _profile;
        }

        private EcosystemWalletTransactions GetTransactions()
        {
            if (!_transactions)
                _transactions = BoilerplateFactory.OpenEcosystemWalletTransactions(transform.parent, _wallet);

            return _transactions;
        }
    }
}
