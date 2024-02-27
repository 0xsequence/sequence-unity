using System;

namespace SequenceSDK.WaaS
{
    [Serializable]
    public class Identity
    {
        public string type { get; private set; }
        public string iss { get; private set; }
        public string sub { get; private set; }
        public string email { get; private set; }

        public Identity(string type, string iss, string sub, string email)
        {
            this.type = type;
            this.iss = iss;
            this.sub = sub;
            this.email = email;
        }
    }
}