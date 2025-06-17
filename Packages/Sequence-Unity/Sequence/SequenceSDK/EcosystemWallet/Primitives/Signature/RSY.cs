using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    public abstract class RSY : SignatureOfSignerLeaf
    {
        public abstract string type { get; }
        public BigInteger r;
        public BigInteger s;
        public BigInteger yParity;
    }
}