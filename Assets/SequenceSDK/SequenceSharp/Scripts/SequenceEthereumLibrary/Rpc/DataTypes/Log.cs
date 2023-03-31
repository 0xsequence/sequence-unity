using System.Collections.Generic;
using System.Numerics;

namespace SequenceSharp
{
    [System.Serializable]
    public class Log
    {
        public bool removed;
        public BigInteger logIndex;
        public BigInteger transactionIndex;
        public string transactionHash;
        public string blockHash;
        public string blockNumber;
        public string address;
        public string data;
        public List<string> topics;
    }
}
