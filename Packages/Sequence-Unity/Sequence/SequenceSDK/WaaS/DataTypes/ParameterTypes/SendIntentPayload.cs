using System;

namespace Sequence.WaaS
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