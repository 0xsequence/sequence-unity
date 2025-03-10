using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class IntentDataGetSession
    {
        public string sessionId;
        public string wallet;

        [UnityEngine.Scripting.Preserve]
        public IntentDataGetSession(string sessionId, string walletAddress)
        {
            this.sessionId = sessionId;
            this.wallet = walletAddress;
        }
    }
}