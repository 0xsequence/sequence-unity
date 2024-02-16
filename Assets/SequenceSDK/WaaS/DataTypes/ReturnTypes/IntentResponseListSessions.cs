using System;
using Sequence.WaaS.Authentication;

namespace SequenceSDK.WaaS
{
    [Serializable]
    public class IntentResponseListSessions
    {
        public string[] sessions { get; private set; }

        public IntentResponseListSessions(string[] sessions)
        {
            this.sessions = sessions;
        }
    }
}