using System;
using System.Collections.Generic;
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
            List<Chain> result = new List<Chain>();
            for (int i = 0; i < length; i++)
            {
                string chainId = chains[i].ToString();
                if (ChainDictionaries.ChainById.TryGetValue(chainId, out Chain chain)) // There may be chains that aren't supported by Sequence
                {
                    result.Add(chain);
                }
            }
            return result.ToArray();
        }
    }
}