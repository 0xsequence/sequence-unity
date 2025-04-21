using System;
using System.Numerics;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class LifiSupportedChainsResponse
    {
        public BigInteger[] chains;

        [Preserve]
        public LifiSupportedChainsResponse(BigInteger[] chains)
        {
            this.chains = chains;
        }

        public Chain[] GetChains()
        {
            int length = chains.Length;
            Chain[] result = new Chain[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ChainDictionaries.ChainById[chains[i].ToString()];
            }
            return result;
        }
    }
}