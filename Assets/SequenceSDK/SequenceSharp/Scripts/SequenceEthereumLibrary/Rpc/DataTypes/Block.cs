using System.Collections.Generic;

namespace SequenceSharp
{
    [System.Serializable]
    public class Block
    {
        public string extraData;
        public string gasLimit;
        public string gasUsed;
        public string hash;
        public string logsBloom;
        public string miner;
        public string mixHash;
        public string nonce;
        public string number;
        public string parentHash;
        public string receiptsRoot;
        public string sha3Uncles;
        public string size;
        public string stateRoot;
        public string timestamp;
        public string totalDifficulty;
        public List<string> transactions;
        public string transactionsRoot;
        public List<object> uncles;
        
    }
}