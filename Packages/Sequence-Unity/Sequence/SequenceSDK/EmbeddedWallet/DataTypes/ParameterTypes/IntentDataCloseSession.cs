using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class IntentDataCloseSession
    {
        public string sessionId;

        [UnityEngine.Scripting.Preserve]
        public IntentDataCloseSession(string sessionId)
        {
            this.sessionId = sessionId;
        }
    }
}