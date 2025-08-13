using Sequence.EcosystemWallet;
using UnityEngine;
using UnityEngine.Events;

namespace Sequence.Boilerplates
{
    public class EcosystemWalletTransactions : MonoBehaviour
    {
        [SerializeField] private TransactionButton[] _transactionButtons;
        
        private IWallet _wallet;
        private UnityAction _onClose;

        public void Close()
        {
            gameObject.SetActive(false);
            _onClose?.Invoke();
        }
        
        public void Load(IWallet wallet, UnityAction onClose)
        {
            _wallet = wallet;
            _onClose = onClose;
            
            gameObject.SetActive(true);
            
            foreach (var button in _transactionButtons)
                button.Load(wallet);
        }
    }
}
