using System;
using Sequence.Utils;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class Signature
    {
        public string sessionId;
        public string signature;
        
        [Preserve]
        public Signature(string sessionId, string signature)
        {
            this.sessionId = sessionId;
            this.signature = signature;
        }
    }
}