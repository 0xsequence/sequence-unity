namespace SequenceSDK.WaaS
{
    public class Signature
    {
        public string sessionId { get; private set; }
        public string signature { get; private set; }
        
        public Signature(string sessionId, string signature)
        {
            this.sessionId = sessionId;
            this.signature = signature;
        }
    }
}