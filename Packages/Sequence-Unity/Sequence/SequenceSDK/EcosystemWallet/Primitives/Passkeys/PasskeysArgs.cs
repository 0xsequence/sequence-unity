namespace Sequence.EcosystemWallet.Primitives.Passkeys
{
    public class PasskeysArgs
    {
        public string x;
        public string y;
        public string r;
        public string s;
        public string challenge;
        public bool requireUserVerification;
        public string credentialId;
        public string metadataHash;
        public string authenticatorData;
        public string clientDataJson;
        public bool embedMetadata;
    }
}