using System;

namespace Sequence.Config
{
    [Serializable]
    public class ConfigJwt
    {
        public int projectId;
        public string identityPoolId;
        public string idpRegion;
        public string rpcServer;
        public string emailClientId;
        public string emailRegion;
    }
}