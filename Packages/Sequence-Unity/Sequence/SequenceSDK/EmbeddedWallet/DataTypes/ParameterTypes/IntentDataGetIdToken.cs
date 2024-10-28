using System;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class IntentDataGetIdToken
    {
        public string sessionId;
        public string wallet;
        public string nonce;

        public IntentDataGetIdToken(string sessionId, string walletAddress, string nonce = null)
        {
            this.sessionId = sessionId;
            this.wallet = walletAddress;
            this.nonce = nonce; 
        }
    }
}