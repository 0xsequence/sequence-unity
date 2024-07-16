using System;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class SendIntentPayload
    {
        public IntentPayload intent {get; private set;}
        
        public SendIntentPayload(IntentPayload intent)
        {
            this.intent = intent;
        }
    }
}