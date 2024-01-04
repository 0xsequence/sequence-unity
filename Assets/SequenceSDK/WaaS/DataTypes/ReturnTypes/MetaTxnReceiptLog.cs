namespace Sequence.WaaS
{
    [System.Serializable]
    public class MetaTxnReceiptLog
    {
        public string address { get; private set; }
        public string[] topics { get; private set; }
        public string data { get; private set; }
        public uint blockNumber { get; private set; }
        public string transactionHash { get; private set; }
        public uint transactionIndex { get; private set; }
        public string blockHash { get; private set; }
        public uint logIndex { get; private set; }
        public bool removed { get; private set; }
        
        public MetaTxnReceiptLog(string address, string[] topics, string data, uint blockNumber, string transactionHash, uint transactionIndex, string blockHash, uint logIndex, bool removed)
        {
            this.address = address;
            this.topics = topics;
            this.data = data;
            this.blockNumber = blockNumber;
            this.transactionHash = transactionHash;
            this.transactionIndex = transactionIndex;
            this.blockHash = blockHash;
            this.logIndex = logIndex;
            this.removed = removed;
        }
    }
}