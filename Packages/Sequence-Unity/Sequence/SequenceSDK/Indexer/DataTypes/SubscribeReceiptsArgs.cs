namespace Sequence
{
    [System.Serializable]
    public class SubscribeReceiptsArgs
    {
        public TransactionFilter filter;

        public SubscribeReceiptsArgs(TransactionFilter filter)
        {
            this.filter = filter;
        }
    }
}