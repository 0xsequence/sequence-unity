using System;

namespace Sequence.EmbeddedWallet
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