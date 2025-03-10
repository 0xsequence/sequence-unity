using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class Signature
    {
        public string sessionId;
        public string signature;
        
        [UnityEngine.Scripting.Preserve]
        public Signature(string sessionId, string signature)
        {
            this.sessionId = sessionId;
            this.signature = signature;
        }
    }
}