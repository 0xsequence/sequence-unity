using System.Collections.Generic;

namespace Sequence
{
    [System.Serializable]
    public class Block
    {
        public string number;
        public string hash;
        public string timestamp;
        public string nonce;
        public string gasUsed;
        public string gasLimit;
        public string parentHash;
        public string mixHash;
        public string size;
        public string extraData;
        public string logsBloom;
        public string miner;
        public string receiptsRoot;
        public string sha3Uncles;
        public string stateRoot;
        public string totalDifficulty;
        public List<Transaction> transactions;
        public string transactionsRoot;
        public List<object> uncles;

        public override string ToString()
        {
            return $"Block: number {number} | hash: {hash} | timestamp: {timestamp} | nonce: {nonce} | gas used: {gasUsed}";
        }
    }
}