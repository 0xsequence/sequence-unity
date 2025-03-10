using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class IntentResponseGetSession
    {
        public string sessionId;
        public string wallet;
        public bool validated;
        
        [UnityEngine.Scripting.Preserve]
        public IntentResponseGetSession(string sessionId, string wallet, bool validated)
        {
            this.sessionId = sessionId;
            this.wallet = wallet;
            this.validated = validated;
        }
    }
}