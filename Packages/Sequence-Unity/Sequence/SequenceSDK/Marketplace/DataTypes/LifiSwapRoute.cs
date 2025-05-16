using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class LifiSwapRoute
    {
        public ulong fromChainId;
        public ulong toChainId;
        public Token[] fromTokens;
        public Token[] toTokens;

        [Preserve]
        public LifiSwapRoute(ulong fromChainId, ulong toChainId, Token[] fromTokens, Token[] toTokens)
        {
            this.fromChainId = fromChainId;
            this.toChainId = toChainId;
            this.fromTokens = fromTokens;
            this.toTokens = toTokens;
        }
    }
}