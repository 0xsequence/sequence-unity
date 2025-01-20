using System;
using UnityEngine.Scripting;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    internal class SupportedTokenResponse
    {
        public SardineSupportedToken[] tokens;

        [Preserve]
        public SupportedTokenResponse(SardineSupportedToken[] tokens)
        {
            this.tokens = tokens;
        }
    }
}