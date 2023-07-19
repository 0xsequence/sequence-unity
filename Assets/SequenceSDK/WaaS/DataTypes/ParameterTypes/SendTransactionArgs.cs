namespace Sequence.WaaS
{
    [System.Serializable]
    public class SendTransactionArgs
    {
        public Transaction tx;

        public SendTransactionArgs(Transaction tx)
        {
            this.tx = tx;
        }
    }
}