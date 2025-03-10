using System;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class IntentResponseGetSession
    {
        public string sessionId;
        public string wallet;
        public bool validated;
        
        [Preserve]
        public IntentResponseGetSession(string sessionId, string wallet, bool validated)
        {
            this.sessionId = sessionId;
            this.wallet = wallet;
            this.validated = validated;
        }
    }
}