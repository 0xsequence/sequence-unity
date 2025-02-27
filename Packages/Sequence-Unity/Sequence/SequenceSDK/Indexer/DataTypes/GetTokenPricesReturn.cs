using System;
using UnityEngine.Scripting;

namespace Sequence
{
    [Serializable]
    internal class GetTokenPricesReturn
    {
        public TokenPrice[] tokenPrices;

        [Preserve]
        public GetTokenPricesReturn(TokenPrice[] tokenPrices)
        {
            this.tokenPrices = tokenPrices;
        }
    }
}