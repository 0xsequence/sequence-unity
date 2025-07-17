using System;
using UnityEngine.Scripting;

namespace Sequence
{
    [Serializable]
    internal class GetTokenPricesArgs
    {
        public Token[] tokens;
        
        public GetTokenPricesArgs(Token[] tokens)
        {
            this.tokens = tokens;
        }
    }
}