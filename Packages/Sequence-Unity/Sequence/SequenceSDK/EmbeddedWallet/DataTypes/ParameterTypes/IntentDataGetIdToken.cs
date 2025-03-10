using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class IntentDataGetIdToken
    {
        public string sessionId;
        public string wallet;
        public string nonce;

        [UnityEngine.Scripting.Preserve]
        public IntentDataGetIdToken(string sessionId, string walletAddress, string nonce = null)
        {
            this.sessionId = sessionId;
            this.wallet = walletAddress;
            this.nonce = nonce; 
        }
    }
}