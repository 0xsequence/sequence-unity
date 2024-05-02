using System;

namespace SequenceSDK.WaaS
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