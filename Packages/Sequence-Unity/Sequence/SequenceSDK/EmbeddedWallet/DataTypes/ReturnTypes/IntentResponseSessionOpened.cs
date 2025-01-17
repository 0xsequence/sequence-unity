using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class IntentResponseSessionOpened
    {
        public string sessionId;
        public string wallet;

        [Preserve]
        public IntentResponseSessionOpened(string sessionId, string wallet)
        {
            this.sessionId = sessionId;
            this.wallet = wallet;
        }
    }
}