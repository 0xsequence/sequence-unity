using System.Collections.Generic;
using System.Numerics;

namespace Sequence
{
    [System.Serializable]
    public class TransactionReceipt
    {
        public string transactionHash;
        public string transactionIndex;
        public string blockHash;
        public string blockNumber;
        public string from;
        public string to;
        public string cumulativeGasUsed;
        public string effectiveGasPrice;
        public string gasUsed;
        public string contractAddress;
        public List<string> logs;
        public string logsBloom;
        public string type;
        public string root;
        public string status;
    }
}
