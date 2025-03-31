using System;
using UnityEngine.Scripting;

namespace Sequence.Pay.Sardine
{
    [Serializable]
    internal class EnabledTokenResponse
    {
        public SardineEnabledToken[] tokens;
        
        [Preserve]
        public EnabledTokenResponse(SardineEnabledToken[] tokens)
        {
            this.tokens = tokens;
        }
    }
}