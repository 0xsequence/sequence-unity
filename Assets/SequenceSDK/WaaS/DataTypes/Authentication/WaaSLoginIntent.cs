using System;

namespace Sequence.WaaS.Authentication
{
    [Serializable]
    public class WaaSLoginIntent
    {
        public string version;
        public Packet packet;

        [Serializable]
        public class Packet
        {
            public static string OpenSessionCode = "openSession";
            
            public string code;
            public string session;
            public Proof proof;

            [Serializable]
            public class Proof
            {
                public string idToken;
            }
        }

        public WaaSLoginIntent(string version, string code, string session, string idToken)
        {
            this.version = version;
            this.packet = new Packet()
            {
                code = code,
                session = session,
                proof = new Packet.Proof()
                {
                    idToken = idToken
                }
            };
        }
    }
}