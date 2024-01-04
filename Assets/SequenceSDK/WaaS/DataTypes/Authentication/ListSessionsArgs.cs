using System;

namespace SequenceSDK.WaaS.Authentication
{
    [Serializable]
    public class ListSessionsArgs
    {
        public string sessionId { get; private set; }

        public ListSessionsArgs(string sessionId)
        {
            this.sessionId = sessionId;
        }
    }
}