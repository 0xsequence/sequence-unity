using System;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class GetLifiTokensResponse
    {
        public Token[] tokens;

        [Preserve]
        public GetLifiTokensResponse(Token[] tokens)
        {
            this.tokens = tokens;
        }
    }
}