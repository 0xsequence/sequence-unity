using System.Collections.Generic;

namespace Sequence
{
    public static class ChainNames
    {
        public static Dictionary<Chain, string> NameOf = new Dictionary<Chain, string>()
        {
            { Chain.Ethereum, "Ethereum" },
            { Chain.Polygon, "Polygon" },
            { Chain.PolygonZkEvm, "PolygonZkEvm"},
            { Chain.BNBSmartChain, "BNBSmartChain" },
            { Chain.ArbitrumOne, "ArbitrumOne" },
            { Chain.ArbitrumNova, "ArbitrumNove" },
            { Chain.Optimism, "Optimism" },
            { Chain.Avalanche, "Avalanche" },
            { Chain.Gnosis, "Gnosis" },
            { Chain.Base, "Base" },
            { Chain.OasysHomeverse, "OasysHomeverse"},
            
            { Chain.TestnetGoerli, "Goerli" },
            { Chain.TestnetSepolia, "Sepolia" },
            { Chain.TestnetPolygonMumbai, "PolygonMumbai" },
            { Chain.TestnetArbitrumGoerli, "ArbitrumGoerli" },
            { Chain.TestnetBNBSmartChain, "BNBSmartChain" },
            { Chain.TestnetBaseGoerli, "BaseGoerli" },
            { Chain.TestnetAvalanche, "AvalancheTestnet"},
            {Chain.TestnetOasysHomeverse, "OasysHomeverseTestnet"}
        };
    }
}