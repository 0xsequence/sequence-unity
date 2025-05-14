using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    internal class Config
    {
        public BigInteger threshold;
        public BigInteger checkpoint;
        public Topology topology;
        public Address checkpointer;
    }
}