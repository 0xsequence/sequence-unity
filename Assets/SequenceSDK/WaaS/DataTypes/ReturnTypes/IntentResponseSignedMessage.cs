using System;

namespace Sequence.WaaS
{
    [Serializable]
    public class IntentResponseSignedMessage
    {
        public string signature;
        public string message;
        
        public IntentResponseSignedMessage(string message, string signature)
        {
            this.message = message;
            this.signature = signature;
        }
    }
}