using System;

namespace Sequence.WaaS
{
    [Serializable]
    public class IntentResponseValidationRequired
    {
        public string sessionId;

        public IntentResponseValidationRequired(string sessionId)
        {
            this.sessionId = sessionId;
        }
    }
}