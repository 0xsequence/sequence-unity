using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class SendIntentPayload
    {
        public IntentPayload intent;
        
        [UnityEngine.Scripting.Preserve]
        public SendIntentPayload(IntentPayload intent)
        {
            this.intent = intent;
        }
    }
}