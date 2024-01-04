namespace SequenceSDK.WaaS
{
    public class Signature
    {
        public string session { get; private set; }
        public string signature { get; private set; }
        
        public Signature(string session, string signature)
        {
            this.session = session;
            this.signature = signature;
        }
    }
}