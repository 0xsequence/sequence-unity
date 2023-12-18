namespace Sequence.WaaS
{
    [System.Serializable]
    public class SendTransactionBatchArgs
    {
        public Transaction[] txs;

        public SendTransactionBatchArgs(params Transaction[] txs)
        {
            this.txs = txs;
        }
    }
}