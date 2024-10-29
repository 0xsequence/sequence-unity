using System;

namespace Sequence.EmbeddedWallet
{
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