using System;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class IntentResponseGetSession
    {
        public string sessionId { get; private set; }
        public string wallet { get; private set; }
        public bool validated { get; private set; }
        
        public IntentResponseGetSession(string sessionId, string wallet, bool validated)
        {
            this.sessionId = sessionId;
            this.wallet = wallet;
            this.validated = validated;
        }
    }
}