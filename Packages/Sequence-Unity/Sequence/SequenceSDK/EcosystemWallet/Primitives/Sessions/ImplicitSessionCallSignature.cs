namespace Sequence.EcosystemWallet.Primitives
{
    public class ImplicitSessionCallSignature : SessionCallSignature
    {
        public Attestation attestation;
        public RSY identitySignature;
        public RSY sessionSignature;
    }
}