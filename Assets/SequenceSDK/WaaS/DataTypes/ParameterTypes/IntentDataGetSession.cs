using System;

namespace SequenceSDK.WaaS
{
    [Serializable]
    public class IntentDataGetSession
    {
        public string sessionId;
        public string wallet;

        public IntentDataGetSession(string sessionId, string walletAddress)
        {
            this.sessionId = sessionId;
            this.wallet = walletAddress;
        }
    }
}