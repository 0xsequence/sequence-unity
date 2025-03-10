using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class IntentResponseSessionOpened
    {
        public string sessionId;
        public string wallet;

        [UnityEngine.Scripting.Preserve]
        public IntentResponseSessionOpened(string sessionId, string wallet)
        {
            this.sessionId = sessionId;
            this.wallet = wallet;
        }
    }
}