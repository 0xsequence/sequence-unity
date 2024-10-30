using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class SendIntentPayload
    {
        public IntentPayload intent;
        
        public SendIntentPayload(IntentPayload intent)
        {
            this.intent = intent;
        }
    }
}