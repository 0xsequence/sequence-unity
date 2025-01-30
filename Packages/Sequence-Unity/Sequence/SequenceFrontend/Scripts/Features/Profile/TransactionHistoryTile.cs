using Sequence;
using Sequence.Demo.Utils;
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
            _dateText.text = TimeUtils.FormatRemainingTime(
                TimeUtils.GetTimestampSecondsNow() - TimeUtils.ConvertDateTimeToSeconds(transaction.timestamp));
        }

        public void ShowEmpty()
        {
            _titleText.text = "No transactions yet.";
            _dateText.text = string.Empty;
        }
    }
}
