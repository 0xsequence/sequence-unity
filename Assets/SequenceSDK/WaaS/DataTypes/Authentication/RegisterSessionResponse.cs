using System;

namespace Sequence.WaaS.Authentication
{
    [Serializable]
    public class RegisterSessionResponse
    {
        public WaaSSession session;
        public WaaSSessionData data;

        [Serializable]
        public class WaaSSession
        {
            public string id;
            public string address;
            public string userId;
            public int projectId;
            public string issuer;
            public string subject;
            public string friendlyName;
            public DateTime createdAt;
            public DateTime refreshedAt;
            public DateTime expiresAt;
            
        }
        
        [Serializable]
        public class WaaSSessionData
        {
            public string sessionId;
            public string wallet;
        }
    }
}