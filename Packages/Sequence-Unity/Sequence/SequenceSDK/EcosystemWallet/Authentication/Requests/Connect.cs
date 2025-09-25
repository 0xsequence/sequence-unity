using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet
{
    public struct ConnectArgs
    {
        public string preferredLoginMethod;
        public string email;
        public string origin;
        public bool includeImplicitSession;
        public SessionPermissions session;
    }
    
    public struct ConnectResponse
    {
        public Address walletAddress;
        public string userEmail;
        public string loginMethod;
        public Attestation attestation;
        public RSY signature;
    }
}