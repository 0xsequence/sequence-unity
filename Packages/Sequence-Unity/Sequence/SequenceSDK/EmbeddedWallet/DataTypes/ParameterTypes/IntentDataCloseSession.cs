using System;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class IntentDataCloseSession
    {
        public string sessionId;

        public IntentDataCloseSession(string sessionId)
        {
            this.sessionId = sessionId;
        }
    }
}