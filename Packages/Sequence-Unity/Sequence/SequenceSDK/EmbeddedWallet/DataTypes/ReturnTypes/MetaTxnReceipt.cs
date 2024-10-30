using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [System.Serializable]
    public class MetaTxnReceipt
    {
        public string id;
        public string status;
        public string revertReason;
        public int index;
        public MetaTxnReceiptLog[] logs;
        public MetaTxnReceipt[] receipts;
        public string txnReceipt;

        public MetaTxnReceipt(string id, string status, int index, MetaTxnReceiptLog[] logs, MetaTxnReceipt[] receipts, string txnReceipt, string revertReason = null)
        {
            this.id = id;
            this.status = status;
            this.index = index;
            this.logs = logs;
            this.receipts = receipts;
            this.txnReceipt = txnReceipt;
            this.revertReason = revertReason;
        }
    }
}