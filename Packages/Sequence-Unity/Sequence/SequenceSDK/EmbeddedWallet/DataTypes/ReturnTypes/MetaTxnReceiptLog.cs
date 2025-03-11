using Sequence.Utils;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [System.Serializable]
    public class MetaTxnReceiptLog
    {
        public string address;
        public string[] topics;
        public string data;
        public uint blockNumber;
        public string transactionHash;
        public uint transactionIndex;
        public string blockHash;
        public uint logIndex;
        public bool removed;
        
        [Preserve]
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