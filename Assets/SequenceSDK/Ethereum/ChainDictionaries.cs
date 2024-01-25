using System.Collections.Generic;

namespace Sequence
{
    public static class ChainDictionaries
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
            { Chain.TestnetArbitrumSepolia, "ArbitrumSepolia" },
            { Chain.TestnetBNBSmartChain, "BNBSmartChain" },
            { Chain.TestnetBaseSepolia, "BaseSepolia" },
            { Chain.TestnetOasysHomeverse, "TestnetOasysHomeverse"},
            { Chain.TestnetAvalanche, "TestnetAvalanche"},
            { Chain.TestnetOptimisticSepolia, "OptimisticSepolia"},
        };

        public static Dictionary<Chain, string> GasCurrencyOf = new Dictionary<Chain, string>()
        {
            { Chain.Ethereum, "ETH" },
            { Chain.Polygon, "MATIC" },
            { Chain.PolygonZkEvm, "MATIC" },
            { Chain.BNBSmartChain, "BNB" },
            { Chain.ArbitrumOne, "AETH" },
            { Chain.ArbitrumNova, "AETH" },
            { Chain.Optimism, "OP" },
            { Chain.Avalanche, "AVAX" },
            { Chain.Gnosis, "xDai" },
            { Chain.Base, "ETH" },
            { Chain.OasysHomeverse, "OAS"},

            { Chain.TestnetGoerli, "ETH" },
            { Chain.TestnetSepolia, "ETH" },
            { Chain.TestnetPolygonMumbai, "MATIC" },
            { Chain.TestnetArbitrumSepolia, "AETH" },
            { Chain.TestnetBNBSmartChain, "BNB" },
            { Chain.TestnetBaseSepolia, "ETH" },
            { Chain.TestnetOasysHomeverse, "OAS"},
            { Chain.TestnetAvalanche, "AVAX"},
            { Chain.TestnetOptimisticSepolia, "OP"},
        };

        public static Dictionary<Chain, string> BlockExplorerOf = new Dictionary<Chain, string>()
        {
            { Chain.Ethereum, "https://etherscan.io/" },
            { Chain.Polygon, "https://polygonscan.com/" },
            { Chain.PolygonZkEvm, "https://zkevm.polygonscan.com/" },
            { Chain.BNBSmartChain, "https://bscscan.com/" },
            { Chain.ArbitrumOne, "https://arbiscan.io/" },
            { Chain.ArbitrumNova, "https://nova.arbiscan.io/" },
            { Chain.Optimism, "https://optimistic.etherscan.io/" },
            { Chain.Avalanche, "https://subnets.avax.network/c-chain/" },
            { Chain.Gnosis, "https://gnosisscan.io/" },
            { Chain.Base, "https://basescan.org/" },
            { Chain.OasysHomeverse, "https://explorer.oasys.homeverse.games/" },

            { Chain.TestnetGoerli, "https://goerli.etherscan.io/" },
            { Chain.TestnetSepolia, "https://sepolia.etherscan.io/" },
            { Chain.TestnetPolygonMumbai, "https://mumbai.polygonscan.com/" },
            { Chain.TestnetArbitrumSepolia, "https://sepolia.arbiscan.io/" },
            { Chain.TestnetBNBSmartChain, "https://testnet.bscscan.com/" },
            { Chain.TestnetBaseSepolia, "https://sepolia.basescan.org/" },
            { Chain.TestnetOasysHomeverse, "https://explorer.testnet.oasys.games/" },
            { Chain.TestnetAvalanche, "https://testnet.snowtrace.io/" },
            { Chain.TestnetOptimisticSepolia, "https://sepolia-optimism.etherscan.io/" },
        };
    }
}