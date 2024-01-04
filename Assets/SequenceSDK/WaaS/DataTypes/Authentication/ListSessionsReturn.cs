using System;
using Sequence.WaaS.Authentication;

namespace SequenceSDK.WaaS.Authentication
{
    [Serializable]
    public class ListSessionsReturn
    {
        public WaaSSession[] sessions { get; private set; }

        public ListSessionsReturn(WaaSSession[] sessions)
        {
            this.sessions = sessions;
        }
    }
}