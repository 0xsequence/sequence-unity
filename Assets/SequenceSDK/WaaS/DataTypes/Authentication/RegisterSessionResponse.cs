using System;

namespace Sequence.WaaS.Authentication
{
    [Serializable]
    public class RegisterSessionResponse
    {
        public WaaSSession session;
        public WaaSSessionData data;
    }
}