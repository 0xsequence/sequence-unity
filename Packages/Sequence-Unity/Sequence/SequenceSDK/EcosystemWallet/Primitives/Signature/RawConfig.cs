using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    internal class RawConfig
    {
        public BigInteger threshold;
        public BigInteger checkpoint;
        public RawTopology topology;
        public Address checkpointer;
    }
}