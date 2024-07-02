namespace Sequence
{
    [System.Serializable]
    public class GetTransactionHistoryArgs
    {
        public TransactionHistoryFilter filter;
        public Page page;
        public bool includeMetadata;

        public GetTransactionHistoryArgs()
        {

        }

        public GetTransactionHistoryArgs(TransactionHistoryFilter filter)
        {
            this.filter = filter;
        }

        public GetTransactionHistoryArgs(TransactionHistoryFilter filter, Page page)
        {
            this.filter = filter;
            this.page = page;
        }

        public GetTransactionHistoryArgs(TransactionHistoryFilter filter, Page page, bool includeMetadata)
        {
            this.filter = filter;
            this.page = page;
            this.includeMetadata = includeMetadata;
        }
    }
}
