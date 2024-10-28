using System;

namespace Sequence.EmbeddedWallet
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