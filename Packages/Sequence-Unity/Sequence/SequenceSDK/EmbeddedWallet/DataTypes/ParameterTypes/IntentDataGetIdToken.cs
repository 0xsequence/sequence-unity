using System;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class IntentDataGetIdToken
    {
        public string sessionId { get; private set; }
        public string wallet { get; private set; }
        public string nonce { get; private set; } 

        public IntentDataGetIdToken(string sessionId, string walletAddress, string nonce = null)
        {
            this.sessionId = sessionId;
            this.wallet = walletAddress;
            this.nonce = nonce; 
        }
    }
}