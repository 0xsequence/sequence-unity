using System;

namespace SequenceSDK.WaaS
{
    [Serializable]
    public class IntentResponseGetSession
    {
        public string sessionId;
        public string wallet;
        public bool validated;
        
        public IntentResponseGetSession(string sessionId, string wallet, bool validated)
        {
            this.sessionId = sessionId;
            this.wallet = wallet;
            this.validated = validated;
        }
    }
}