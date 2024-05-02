using System;

namespace SequenceSDK.WaaS
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