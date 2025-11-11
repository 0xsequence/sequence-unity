using System;
using Sequence.EcosystemWallet;
using UnityEngine;

namespace Sequence.Boilerplates
{
    public class EcosystemWalletTransactions : MonoBehaviour
    {
        [SerializeField] private MessagePopup _messagePopup;
        [SerializeField] private TransactionResultPopup _transactionResult;
        [SerializeField] private TransactionButton[] _transactionButtons;
        
        private IWallet _wallet;
        private Action _onClose;

        public void Close()
        {
            gameObject.SetActive(false);
            _onClose?.Invoke();
        }
        
        public void Show(IWallet wallet, Action onClose)
        {
            _wallet = wallet;
            _onClose = onClose;
            
            gameObject.SetActive(true);
            _messagePopup.gameObject.SetActive(false);
            _transactionResult.gameObject.SetActive(false);
            
            foreach (var button in _transactionButtons)
                button.Load(wallet);
        }
    }
}
