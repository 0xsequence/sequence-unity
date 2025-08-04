using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet
{
    public struct ConnectArgs
    {
        public Address sessionAddress;
        public string preferredLoginMethod;
        public string email;
        public string origin;
        public SessionPermissions permissions;
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