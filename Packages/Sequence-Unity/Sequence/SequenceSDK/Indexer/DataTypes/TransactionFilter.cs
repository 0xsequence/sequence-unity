namespace Sequence
{
    [System.Serializable]
    public class TransactionFilter
    {
        public string txnHash;
        public string from;
        public string to;
        public string contractAddress;
        public string @event;
    }
}