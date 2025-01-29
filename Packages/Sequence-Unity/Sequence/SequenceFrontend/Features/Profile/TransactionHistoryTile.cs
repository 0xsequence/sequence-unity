using Sequence;
using TMPro;
using UnityEngine;

namespace SequenceSDK.Samples
{
    public class TransactionHistoryTile : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _dateText;
        
        public void Show(TransactionHistory transaction)
        {
            _titleText.text = transaction.txnHash;
            Debug.Log($"{transaction.timestamp}");
            //_dateText.text = new Datte(transaction.timestamp).ToString();
        }

        public void ShowEmpty()
        {
            _titleText.text = "No transactions yet.";
            _dateText.text = string.Empty;
        }
    }
}
