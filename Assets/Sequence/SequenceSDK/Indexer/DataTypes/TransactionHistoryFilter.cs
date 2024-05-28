namespace Sequence
{
    [System.Serializable]
    public class TransactionHistoryFilter
    {
        public string accountAddress;
        public string contractAddress;
        public string[] accountAddresses;
        public string[] contractAddresses;
        public string[] transactionHashes;
        public string[] metaTransactionIDs;
        public int fromBlock;
        public int toBlock;
    }
}