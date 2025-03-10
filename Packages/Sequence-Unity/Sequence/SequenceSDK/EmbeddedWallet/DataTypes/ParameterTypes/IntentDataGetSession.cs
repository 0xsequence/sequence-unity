using System;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class IntentDataGetSession
    {
        public string sessionId;
        public string wallet;

        [Preserve]
        public IntentDataGetSession(string sessionId, string walletAddress)
        {
            this.sessionId = sessionId;
            this.wallet = walletAddress;
        }
    }
}