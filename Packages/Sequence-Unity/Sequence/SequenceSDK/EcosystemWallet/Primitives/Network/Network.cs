using System.Numerics;
using Sequence.Provider;

namespace Sequence.EcosystemWallet.Primitives
{
    internal class Network
    {
        public string name;
        public string rpc;
        public BigInteger chainId;
        public string explorer;
        public NativeCurrency nativeCurrency;

        public class NativeCurrency
        {
            public string name;
            public string symbol;
            public BigInteger decimals;
        }

        public Network(Chain chain, ulong nativeCurrencyDecimals = 18)
        {
            name = ChainDictionaries.NameOf[chain];
            rpc = NodeGatewayBridge.GetNodeGatewayUrl(chain);
            chainId = BigInteger.Parse(ChainDictionaries.ChainIdOf[chain]);
            explorer = ChainDictionaries.BlockExplorerOf[chain];
            nativeCurrency = new NativeCurrency()
            {
                name = ChainDictionaries.GasCurrencyOf[chain],
                symbol = ChainDictionaries.GasCurrencyOf[chain],
                decimals = (BigInteger)nativeCurrencyDecimals
            };
        }
    }
}