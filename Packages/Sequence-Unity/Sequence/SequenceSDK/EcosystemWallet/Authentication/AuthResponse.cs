using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet.Authentication
{
    public class AuthResponse
    {
        public Address walletAddress;
        public string email;
        public string loginMethod;
        public Attestation attestation;
        public RSY signature;
    }
}