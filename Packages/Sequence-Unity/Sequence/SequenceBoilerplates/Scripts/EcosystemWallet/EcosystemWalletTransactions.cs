using Sequence.EcosystemWallet;
using UnityEngine;

namespace Sequence.Boilerplates
{
    public class EcosystemWalletTransactions : MonoBehaviour
    {
        [SerializeField] private TransactionButton[] _transactionButtons;
        
        public void Load(IWallet wallet)
        {
            gameObject.SetActive(true);
            
            foreach (var button in _transactionButtons)
                button.Load(wallet);
        }
    }
}
