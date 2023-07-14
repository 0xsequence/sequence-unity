using System;
using System.Numerics;

namespace Sequence.Core
{
    public class NetworkConfig
    {
        public string Name { get; set; }
        public BigInteger ChainID { get; set; }
        public string ENSAddress { get; set; }

        public string RpcURL { get; set; }
        public RPCProvider Provider;

        public string RelayerURL { get; set; } // optional, one of the these should be set
        public Relayer Relayer { get; set; }

        public string IndexerURL { get; set; }

        public bool IsDefaultChain { get; set; }
        public bool IsAuthChain { get; set; }

        public string SequenceAPIURL { get; set; }
    }
}
