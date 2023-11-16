using System;

namespace Sequence.WaaS.Authentication
{
    [Serializable]
    public class WaaSLoginPayload
    {
        public int projectId;
        public string idToken;
        public string sessionAddress;
        public string friendlyName;
        public string intentJSON;

        public WaaSLoginPayload(int projectId, string idToken, string sessionAddress, string friendlyName, string intentJson)
        {
            this.projectId = projectId;
            this.idToken = idToken;
            this.sessionAddress = sessionAddress;
            this.friendlyName = friendlyName;
            intentJSON = intentJson;
        }
    }
}