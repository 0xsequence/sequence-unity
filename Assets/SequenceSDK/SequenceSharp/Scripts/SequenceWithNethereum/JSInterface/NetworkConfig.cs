using System.Numerics;

namespace Sequence
{
    public class NetworkConfig
    {
#nullable enable
        public string? title;
        public string name;
        public BigInteger chainId;
        public string? ensAddress;
        public bool? testnet;
        public BlockExplorerConfig? blockExplorer;
        public string? rpcUrl;
        // public JsonRpcProvider? provider;
        public string? indexerUrl;
        // public Indexer? indexer;
        // public Relayer | RpcRelayerOptions? relayer?;
        public bool? isDefaultChain;
        public bool? isAuthChain;

        public NetworkConfig(string name)
        {
            this.name = name;
        }
#nullable disable
    }
}