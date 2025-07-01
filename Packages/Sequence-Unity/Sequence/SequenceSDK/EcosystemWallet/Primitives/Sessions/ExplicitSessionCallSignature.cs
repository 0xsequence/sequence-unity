using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    public class ExplicitSessionCallSignature : SessionCallSignature
    {
        public BigInteger permissionIndex;
        public RSY sessionSignature;
        
        public override byte[] Encode()
        {
            throw new System.NotImplementedException();
        }
    }
}