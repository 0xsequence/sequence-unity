using System.Collections.Generic;

namespace Sequence
{
    [System.Serializable]
    public class Filter
    {
        public string fromBlock;
        public string toBlock;
        public string address;
        public List<string> topics;
        public string blockhash;
    }
}