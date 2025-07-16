using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet.Authentication
{
    public struct ConnectArgs
    {
        public Address sessionAddress;
        public string preferredLoginMethod;
        public string email;
        public string implicitSessionRedirectUrl;
        public object permissions;
    }
    
    public struct ConnectResponse
    {
        public Address walletAddress;
        public string email;
        public string loginMethod;
        public Attestation attestation;
        public RSY signature;
    }
}