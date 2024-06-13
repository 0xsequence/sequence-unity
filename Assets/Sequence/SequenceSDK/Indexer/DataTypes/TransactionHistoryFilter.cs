using System;
using Newtonsoft.Json;

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
        
        [Obsolete]
        [JsonIgnore]
        public int fromBlock;
        
        [Obsolete]
        [JsonIgnore]
        public int toBlock;
    }
}