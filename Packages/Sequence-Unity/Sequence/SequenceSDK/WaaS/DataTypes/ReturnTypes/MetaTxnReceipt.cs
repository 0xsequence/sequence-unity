namespace Sequence.WaaS
{
    [System.Serializable]
    public class MetaTxnReceipt
    {
        public string id { get; private set; }
        public string status { get; private set; }
        public string revertReason { get; private set; }
        public int index { get; private set; }
        public MetaTxnReceiptLog[] logs { get; private set; }
        public MetaTxnReceipt[] receipts { get; private set; }
        public string txnReceipt { get; private set; }

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