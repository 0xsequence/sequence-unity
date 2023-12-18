namespace Sequence.WaaS
{
    [System.Serializable]
    public class UpdatePartnerArgs
    {
        public uint partnerId { get; private set; }
        public string name { get; private set; }
        public string jwtAlg { get; private set; }
        private string jwtSecret;
        public string jwtPublic { get; private set; }
        
        public UpdatePartnerArgs(uint partnerId, string name, string jwtAlg, string jwtSecret = null, string jwtPublic = null)
        {
            this.name = name;
            this.partnerId = partnerId;
            this.jwtAlg = jwtAlg;
            this.jwtSecret = jwtSecret;
            this.jwtPublic = jwtPublic;
        }
    }
}