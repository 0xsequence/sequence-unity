using System;
using System.Numerics;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class Currency
    {
        public uint id;
        public Chain chain;
        public string contractAddress;

        public Currency(uint id, Chain chain, string contractAddress)
        {
            this.id = id;
            this.chain = chain;
            this.contractAddress = contractAddress;
        }
        
        [Preserve]
        [JsonConstructor]
        public Currency(uint id, BigInteger chainId, string contractAddress)
        {
            this.id = id;
            this.chain = ChainDictionaries.ChainById[chainId.ToString()];
            this.contractAddress = contractAddress;
        }
    }
}