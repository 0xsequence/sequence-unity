using System;
using System.Numerics;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GetLifiTokensRequest
    {
        public BigInteger[] chainIds;

        [JsonConstructor]
        [Preserve]
        public GetLifiTokensRequest(BigInteger[] chainIds)
        {
            this.chainIds = chainIds;
        }
        
        public GetLifiTokensRequest(Chain[] chains)
        {
            int length = chains.Length;
            chainIds = new BigInteger[length];
            for (int i = 0; i < length; i++)
            {
                chainIds[i] = BigInteger.Parse(ChainDictionaries.ChainIdOf[chains[i]]);
            }
        }
    }
}