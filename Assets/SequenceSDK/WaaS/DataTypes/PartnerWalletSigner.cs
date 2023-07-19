namespace Sequence.WaaS
{
    [System.Serializable]
    public class PartnerWalletSigner
    {
        public uint number;
        public uint partnerId;
        public string address;
        public string url;
        public bool passThroughCredentials;
        public bool isSequenceGuard;
        public string authToken;

        public PartnerWalletSigner(uint number, uint partnerId, string address, string url, bool passThroughCredentials, bool isSequenceGuard, string authToken)
        {
            this.number = number;
            this.partnerId = partnerId;
            this.address = address;
            this.url = url;
            this.passThroughCredentials = passThroughCredentials;
            this.isSequenceGuard = isSequenceGuard;
            this.authToken = authToken;
        }
    }
}