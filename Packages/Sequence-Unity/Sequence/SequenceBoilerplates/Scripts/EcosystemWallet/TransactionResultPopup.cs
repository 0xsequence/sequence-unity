using TMPro;
using UnityEngine;

namespace Sequence.Boilerplates
{
    [RequireComponent(typeof(ITween))]
    public class TransactionResultPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text _messageText;
        
        private ITween _tweenAnimation;
        private Chain _chain;
        private string _txnHash;
        
        public void Show(Chain chain, string txnHash)
        {
            _chain = chain;
            _txnHash = txnHash;
            _tweenAnimation ??= GetComponent<ITween>();
            _tweenAnimation.AnimateIn(0.3f);
            _messageText.text = txnHash;
        }

        public void Hide()
        {
            _tweenAnimation.AnimateOut(0.3f);
        }

        public void OpenExplorer()
        {
            var explorer = ChainDictionaries.BlockExplorerOf[_chain];
            Application.OpenURL($"{explorer}txn/{_txnHash}");
        }
    }
}