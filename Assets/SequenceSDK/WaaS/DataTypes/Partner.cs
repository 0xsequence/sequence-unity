

namespace Sequence.WaaS
{
    [System.Serializable]
    public class Partner
    {
        public uint id { get; private set; }
        public string name;
        public string jwtAlg { get; private set; }
        private string jwtSecret;
        public string jwtPublic { get; private set; }

        public Partner(uint id, string name, string jwtAlg, string jwtSecret = null, string jwtPublic = null)
        {
            this.id = id;
            this.name = name;
            this.jwtAlg = jwtAlg;
            this.jwtSecret = jwtSecret;
            this.jwtPublic = jwtPublic;
        }
    }
}
