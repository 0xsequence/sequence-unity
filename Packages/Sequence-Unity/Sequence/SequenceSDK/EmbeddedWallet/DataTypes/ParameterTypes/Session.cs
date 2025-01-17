using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
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

        [Preserve]
        public Session(string id, string address, string userId, int projectId, Identity identity, string friendlyName, DateTime createdAt, DateTime refreshedAt, DateTime expiresAt)
        {
            this.id = id;
            this.address = address;
            this.userId = userId;
            this.projectId = projectId;
            this.identity = identity;
            this.friendlyName = friendlyName;
            this.createdAt = createdAt;
            this.refreshedAt = refreshedAt;
            this.expiresAt = expiresAt;
        }
    }
}