using System.Numerics;

namespace Sequence.EcosystemWallet.Primitives
{
    public class RawConfig
    {
        public BigInteger threshold;
        public BigInteger checkpoint;
        public RawTopology topology;
        public Address checkpointer;
    }
}