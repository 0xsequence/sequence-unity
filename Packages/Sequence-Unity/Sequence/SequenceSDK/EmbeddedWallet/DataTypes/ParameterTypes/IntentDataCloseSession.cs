using System;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class IntentDataCloseSession
    {
        public string sessionId;

        [Preserve]
        public IntentDataCloseSession(string sessionId)
        {
            this.sessionId = sessionId;
        }
    }
}