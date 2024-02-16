using System;

namespace Sequence.WaaS
{
    [Serializable]
    public class IntentResponseValidationRequired
    {
        public string sessionId { get; private set; }

        public IntentResponseValidationRequired(string sessionId)
        {
            this.sessionId = sessionId;
        }
    }
}