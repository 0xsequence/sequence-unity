using System;
using Sequence.Utils;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class IntentDataGetIdToken
    {
        public string sessionId;
        public string wallet;
        public string nonce;

        [Preserve]
        public IntentDataGetIdToken(string sessionId, string walletAddress, string nonce = null)
        {
            this.sessionId = sessionId;
            this.wallet = walletAddress;
            this.nonce = nonce; 
        }
    }
}