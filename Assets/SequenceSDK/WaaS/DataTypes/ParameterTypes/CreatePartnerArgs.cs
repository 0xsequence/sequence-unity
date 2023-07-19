

namespace Sequence.WaaS
{
    [System.Serializable]
    public class CreatePartnerArgs
    {
        public string name { get; private set; }
        public string jwtAlg { get; private set; }
        private string jwtSecret;
        public string jwtPublic { get; private set; }
        
        
        public CreatePartnerArgs(string name, string jwtAlg, string jwtSecret = null, string jwtPublic = null)
        {
            this.jwtSecret = jwtSecret;
            this.name = name;
            this.jwtAlg = jwtAlg;
            this.jwtPublic = jwtPublic;
        }
    }
}