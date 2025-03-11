using System;
using Sequence.Utils;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class SendIntentPayload
    {
        public IntentPayload intent;
        
        [Preserve]
        public SendIntentPayload(IntentPayload intent)
        {
            this.intent = intent;
        }
    }
}