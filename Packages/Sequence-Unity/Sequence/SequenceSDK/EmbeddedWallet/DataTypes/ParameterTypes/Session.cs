using System;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class Session
    {
        public string id;
        public string address;
        public string userId;
        public int projectId;
        public Identity identity;
        public string friendlyName;
        public DateTime createdAt;
        public DateTime refreshedAt;
        public DateTime expiresAt;
            
    }
}