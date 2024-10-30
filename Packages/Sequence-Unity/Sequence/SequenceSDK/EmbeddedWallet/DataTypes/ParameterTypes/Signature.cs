using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    public class Signature
    {
        public string sessionId;
        public string signature;
        
        public Signature(string sessionId, string signature)
        {
            this.sessionId = sessionId;
            this.signature = signature;
        }
    }
}