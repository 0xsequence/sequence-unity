using System;

namespace SequenceSDK.WaaS
{
    [Serializable]
    public class IntentDataGetSession
    {
        public string sessionId { get; private set; }
        public string wallet { get; private set; }

        public IntentDataGetSession(string sessionId, string walletAddress)
        {
            this.sessionId = sessionId;
            this.wallet = walletAddress;
        }
    }
}