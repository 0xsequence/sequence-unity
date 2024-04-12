using System.Collections.Generic;

namespace Sequence
{
    public static class ChainDictionaries
    {
        public static Dictionary<Chain, string> NameOf = new Dictionary<Chain, string>()
        {
            { Chain.Ethereum, "Ethereum" },
            { Chain.Polygon, "Polygon" },
            { Chain.PolygonZkEvm, "Polygon zkEvm"},
            { Chain.BNBSmartChain, "BNB Smart Chain" },
            { Chain.ArbitrumOne, "Arbitrum One" },
            { Chain.ArbitrumNova, "Arbitrum Nova" },
            { Chain.Optimism, "Optimism" },
            { Chain.Avalanche, "Avalanche" },
            { Chain.Gnosis, "Gnosis" },
            { Chain.Base, "Base" },
            { Chain.OasysHomeverse, "Oasys Homeverse" },
            { Chain.Xai, "Xai" },
            { Chain.AstarZKEvm, "Astar zkEVM" },
            
            { Chain.TestnetSepolia, "Sepolia" },
            { Chain.TestnetArbitrumSepolia, "Arbitrum Sepolia" },
            { Chain.TestnetBNBSmartChain, "BNB Smart Chain Testnet" },
            { Chain.TestnetBaseSepolia, "Base Sepolia" },
            { Chain.TestnetOasysHomeverse, "Testnet Oasys Homeverse"},
            { Chain.TestnetAvalanche, "Testnet Avalanche"},
            { Chain.TestnetOptimisticSepolia, "Optimistic Sepolia"},
            { Chain.TestnetPolygonAmoy, "Polygon Amoy" },
            { Chain.TestnetAstarZKyoto, "Astar zKyoto Testnet"},
            { Chain.TestnetXrSepolia, "XR Sepolia" }
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
            { Chain.AstarZKEvm, "ETH" },
            { Chain.Xai, "XAI" },

            { Chain.TestnetSepolia, "ETH" },
            { Chain.TestnetArbitrumSepolia, "AETH" },
            { Chain.TestnetBNBSmartChain, "BNB" },
            { Chain.TestnetBaseSepolia, "ETH" },
            { Chain.TestnetOasysHomeverse, "OAS"},
            { Chain.TestnetAvalanche, "AVAX"},
            { Chain.TestnetOptimisticSepolia, "OP"},
            { Chain.TestnetPolygonAmoy, "MATIC" },
            { Chain.TestnetAstarZKyoto, "ETH"},
            { Chain.TestnetXrSepolia, "tXR" }
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
            { Chain.AstarZKEvm, "https://astar-zkevm.explorer.startale.com/" },
            { Chain.Xai, "" }, // None found

            { Chain.TestnetSepolia, "https://sepolia.etherscan.io/" },
            { Chain.TestnetArbitrumSepolia, "https://sepolia.arbiscan.io/" },
            { Chain.TestnetBNBSmartChain, "https://testnet.bscscan.com/" },
            { Chain.TestnetBaseSepolia, "https://sepolia.basescan.org/" },
            { Chain.TestnetOasysHomeverse, "https://explorer.testnet.oasys.games/" },
            { Chain.TestnetAvalanche, "https://testnet.snowtrace.io/" },
            { Chain.TestnetOptimisticSepolia, "https://sepolia-optimism.etherscan.io/" },
            { Chain.TestnetPolygonAmoy, "https://amoy.polygonscan.com/" }, // Not active, but this will likely be it
            { Chain.TestnetAstarZKyoto, "https://astar-zkyoto.blockscout.com/" }, 
            { Chain.TestnetXrSepolia, "https://xr-sepolia-testnet.explorer.caldera.xyz/" }
        };
    }
}