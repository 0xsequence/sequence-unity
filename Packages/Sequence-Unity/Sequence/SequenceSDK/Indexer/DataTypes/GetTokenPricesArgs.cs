using System;
using UnityEngine.Scripting;

namespace Sequence
{
    [Serializable]
    internal class GetTokenPricesArgs
    {
        public PriceFeed.Token[] tokens;
        
        public GetTokenPricesArgs(PriceFeed.Token[] tokens)
        {
            this.tokens = tokens;
        }
    }
}