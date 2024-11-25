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
            { Chain.Blast, "Blast" },
            { Chain.B3, "B3" },
            { Chain.APEChain, "APE Chain" },
            { Chain.ImmutableZkEvm, "Immutable zkEVM"},
            { Chain.SkaleNebula, "Skale Nebula" },
            { Chain.Root, "Root" },

            { Chain.TestnetSepolia, "Sepolia" },
            { Chain.TestnetArbitrumSepolia, "Arbitrum Sepolia" },
            { Chain.TestnetBNBSmartChain, "BNB Smart Chain Testnet" },
            { Chain.TestnetBaseSepolia, "Base Sepolia" },
            { Chain.TestnetOasysHomeverse, "Testnet Oasys Homeverse"},
            { Chain.TestnetAvalanche, "Testnet Avalanche"},
            { Chain.TestnetOptimisticSepolia, "Optimistic Sepolia"},
            { Chain.TestnetPolygonAmoy, "Polygon Amoy" },
            { Chain.TestnetAstarZKyoto, "Astar zKyoto Testnet"},
            { Chain.TestnetXrSepolia, "XR Sepolia" },
            { Chain.TestnetXaiSepolia, "Xai Sepolia" },
            { Chain.TestnetB3Sepolia, "B3 Sepolia" },
            { Chain.TestnetAPEChain, "APE Chain Testnet" },
            { Chain.TestnetBlastSepolia, "Blast Sepolia" },
            { Chain.TestnetBorne, "Borne Testnet" },
            { Chain.TestnetSkaleNebulaGamingHub, "Skale Nebula Gaming Hub Testnet" },
            { Chain.TestnetSoneiumMinato, "Soneium Minato Testnet" },
            { Chain.TestnetToy, "TOY Testnet" },
            { Chain.TestnetImmutableZkEvm, "Immutable zkEVM Testnet" },
            { Chain.TestnetRootPorcini, "Root Porcini Testnet" }
        };

        public static Dictionary<Chain, string> GasCurrencyOf = new Dictionary<Chain, string>()
        {
            { Chain.Ethereum, "ETH" },
            { Chain.Polygon, "POL" },
            { Chain.PolygonZkEvm, "POL" },
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
            { Chain.Blast, "ETH" },
            { Chain.B3, "ETH" },
            { Chain.APEChain, "APE" },
            { Chain.ImmutableZkEvm, "IMX" },
            { Chain.SkaleNebula, "sFUEL" },
            { Chain.Root, "XRP" },

            { Chain.TestnetSepolia, "ETH" },
            { Chain.TestnetArbitrumSepolia, "AETH" },
            { Chain.TestnetBNBSmartChain, "BNB" },
            { Chain.TestnetBaseSepolia, "ETH" },
            { Chain.TestnetOasysHomeverse, "OAS"},
            { Chain.TestnetAvalanche, "AVAX"},
            { Chain.TestnetOptimisticSepolia, "OP"},
            { Chain.TestnetPolygonAmoy, "POL" },
            { Chain.TestnetAstarZKyoto, "ETH"},
            { Chain.TestnetXrSepolia, "tXR" },
            { Chain.TestnetXaiSepolia, "sXAI" },
            { Chain.TestnetB3Sepolia, "ETH" },
            { Chain.TestnetAPEChain, "APE" },
            { Chain.TestnetBlastSepolia, "ETH" },
            { Chain.TestnetBorne, "BORNE" },
            { Chain.TestnetSkaleNebulaGamingHub, "sFUEL" },
            { Chain.TestnetSoneiumMinato, "ETH" },
            { Chain.TestnetToy, "TOY" },
            { Chain.TestnetImmutableZkEvm, "IMX" },
            { Chain.TestnetRootPorcini, "XRP" }
        };

        public static Dictionary<Chain, string> NativeTokenAddressOf = new Dictionary<Chain, string>()
        {
            { Chain.Ethereum, "0xC02aaA39b223FE8D0A0E5C4F27eAD9083C756Cc2" }, // WETH
            { Chain.Polygon, "0x0000000000000000000000000000000000001010" }, // MATIC on Polygon
            { Chain.PolygonZkEvm, "0xa2036f0538221a77A3937F1379699f44945018d0" }, // MATIC on zkEvm
            { Chain.BNBSmartChain, "0xbb4CdB9CBd36B01bD1cBaEBF2De08d9173bc095c" }, // WBNB on BNB SmartChain
            { Chain.ArbitrumOne, "0x912CE59144191C1204E64559FE8253a0e49E6548" }, // ARB on Arbitrum
            { Chain.ArbitrumNova, "0xf823c3cd3cebe0a1fa952ba88dc9eef8e0bf46ad" }, // ARB on Arbitrum Nova
            { Chain.Optimism, "0x4200000000000000000000000000000000000042" }, // OP
            { Chain.Avalanche, "0xB31f66AA3C1e785363F0875A1B74E27b85FD66c7" }, // AVAX
            { Chain.Gnosis, "0x9C58BAcC331c9aa871AFD802DB6379a98e80CEdb" }, // xDai 
            { Chain.Base, "0x4200000000000000000000000000000000000006" }, // WETH9 on Base
            { Chain.OasysHomeverse, "0xd07df0da6e67b31db33cde4a6893e06bd87f8a08" }, // Placeholder for OAS token (0xDeadDeAddeAddEAddeadDEaDDEAdDeaDDeAD0000 explorers OAS token page)
            { Chain.AstarZKEvm, "0x0000000000000000000000000000000000000000" }, // Placeholder for Astar token
            { Chain.Xai, "0x0000000000000000000000000000000000000000" }, // Placeholder for XAI token
            { Chain.Blast, "0x0000000000000000000000000000000000000000" }, // Placeholder for Blast token
            
            { Chain.TestnetSepolia, "0x0000000000000000000000000000000000000000" }, // Placeholder for Sepolia ETH
            { Chain.TestnetArbitrumSepolia, "0x0000000000000000000000000000000000000000" }, // Placeholder for Arbitrum Sepolia ETH
            { Chain.TestnetBNBSmartChain, "0x0000000000000000000000000000000000000000" }, // Placeholder for BNB Testnet
            { Chain.TestnetBaseSepolia, "0x0000000000000000000000000000000000000000" }, // Placeholder for Base Sepolia ETH
            { Chain.TestnetOasysHomeverse, "0x0000000000000000000000000000000000000000" }, // Placeholder for Oasys Testnet token
            { Chain.TestnetAvalanche, "0x0000000000000000000000000000000000000000" }, // Placeholder for Avalanche Testnet token
            { Chain.TestnetOptimisticSepolia, "0x0000000000000000000000000000000000000000" }, // Placeholder for Optimism Sepolia ETH
            { Chain.TestnetPolygonAmoy, "0x0000000000000000000000000000000000000000" }, // Placeholder for Polygon Amoy Testnet token
            { Chain.TestnetAstarZKyoto, "0x0000000000000000000000000000000000000000" }, // Placeholder for Astar zKyoto token
            { Chain.TestnetXrSepolia, "0x0000000000000000000000000000000000000000" }, // Placeholder for XR Sepolia token
            { Chain.TestnetXaiSepolia, "0x0000000000000000000000000000000000000000" }, // Placeholder for Xai Sepolia token
            { Chain.TestnetB3Sepolia, "0x0000000000000000000000000000000000000000" }, // Placeholder for B3 Sepolia token
            { Chain.TestnetAPEChain, "0x0000000000000000000000000000000000000000" }, // Placeholder for APE Chain Testnet token
            { Chain.TestnetBlastSepolia, "0x0000000000000000000000000000000000000000" }  // Placeholder for Blast Sepolia token
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
            { Chain.Xai, "https://explorer.xai-chain.net/" },
            { Chain.Blast, "https://blastscan.io/" },
            { Chain.B3, "https://explorer.b3.fun/" },
            { Chain.APEChain, "https://apescan.io/" },
            { Chain.ImmutableZkEvm, "https://explorer.immutable.com/" },
            { Chain.SkaleNebula, "https://green-giddy-denebola.explorer.mainnet.skalenodes.com/" },
            { Chain.Root, "https://rootscan.io/" },

            { Chain.TestnetSepolia, "https://sepolia.etherscan.io/" },
            { Chain.TestnetArbitrumSepolia, "https://sepolia.arbiscan.io/" },
            { Chain.TestnetBNBSmartChain, "https://testnet.bscscan.com/" },
            { Chain.TestnetBaseSepolia, "https://sepolia.basescan.org/" },
            { Chain.TestnetOasysHomeverse, "https://explorer.testnet.oasys.games/" },
            { Chain.TestnetAvalanche, "https://testnet.snowtrace.io/" },
            { Chain.TestnetOptimisticSepolia, "https://sepolia-optimism.etherscan.io/" },
            { Chain.TestnetPolygonAmoy, "https://amoy.polygonscan.com/" },
            { Chain.TestnetAstarZKyoto, "https://astar-zkyoto.blockscout.com/" },
            { Chain.TestnetXrSepolia, "https://xr-sepolia-testnet.explorer.caldera.xyz/" },
            { Chain.TestnetXaiSepolia, "https://testnet-explorer-v2.xai-chain.net/" },
            { Chain.TestnetB3Sepolia, "https://sepolia.explorer.b3.fun/" },
            { Chain.TestnetAPEChain, "https://curtis.explorer.caldera.xyz/" }, 
            { Chain.TestnetBlastSepolia, "https://testnet.blastscan.io/" },
            { Chain.TestnetBorne, "https://subnets-test.avax.network/bornegfdn" },
            { Chain.TestnetSkaleNebulaGamingHub, "https://green-giddy-denebola.explorer.mainnet.skalenodes.com/" },
            { Chain.TestnetSoneiumMinato, "https://explorer-testnet.soneium.org/" },
            { Chain.TestnetToy, "https://toy-chain-testnet.explorer.caldera.xyz/" },
            { Chain.TestnetImmutableZkEvm, "https://explorer.testnet.immutable.com/" },
            { Chain.TestnetRootPorcini, "https://porcini.rootscan.io/" }
        };

        public static Dictionary<Chain, string> ChainIdOf = new Dictionary<Chain, string>()
        {
            { Chain.None, ""},

            { Chain.Ethereum, "1" },
            { Chain.Polygon, "137" },
            { Chain.PolygonZkEvm, "1101" },
            { Chain.BNBSmartChain, "56" },
            { Chain.ArbitrumOne, "42161" },
            { Chain.ArbitrumNova, "42170" },
            { Chain.Optimism, "10" },
            { Chain.Avalanche, "43114" },
            { Chain.Gnosis, "100" },
            { Chain.Base, "8453" },
            { Chain.OasysHomeverse, "19011" },
            { Chain.AstarZKEvm, "3776" },
            { Chain.Xai, "660279" },
            { Chain.Blast, "81457" },
            { Chain.B3, "8333" },
            { Chain.APEChain, "33139" },
            { Chain.ImmutableZkEvm, "13371" },
            { Chain.SkaleNebula, "1482601649" },
            { Chain.Root, "7668" },
            
            { Chain.TestnetSepolia, "11155111" },
            { Chain.TestnetPolygonAmoy, "80002" },
            { Chain.TestnetArbitrumSepolia, "421614" },
            { Chain.TestnetBNBSmartChain, "97" },
            { Chain.TestnetBaseSepolia, "84532" },
            { Chain.TestnetAvalanche, "43113" },
            { Chain.TestnetOasysHomeverse, "40875" },
            { Chain.TestnetOptimisticSepolia, "11155420" },
            { Chain.TestnetAstarZKyoto, "6038361" },
            { Chain.TestnetXrSepolia, "2730" },
            { Chain.TestnetXaiSepolia, "37714555429" },
            { Chain.TestnetB3Sepolia, "1993" },
            { Chain.TestnetAPEChain, "33111" },
            { Chain.TestnetBlastSepolia, "168587773" },
            { Chain.TestnetBorne, "94984" },
            { Chain.TestnetSkaleNebulaGamingHub, "37084624" },
            { Chain.TestnetSoneiumMinato, "1946" },
            { Chain.TestnetToy, "21000000" },
            { Chain.TestnetImmutableZkEvm, "13473" },
            { Chain.TestnetRootPorcini, "7672" }
        };

        public static Dictionary<string, Chain> ChainById = new Dictionary<string, Chain>()
        {
            { "1", Chain.Ethereum },
            { "137", Chain.Polygon },
            { "1101", Chain.PolygonZkEvm },
            { "56", Chain.BNBSmartChain },
            { "42161", Chain.ArbitrumOne },
            { "42170", Chain.ArbitrumNova },
            { "10", Chain.Optimism },
            { "43114", Chain.Avalanche },
            { "100", Chain.Gnosis },
            { "8453", Chain.Base },
            { "19011", Chain.OasysHomeverse },
            { "3776", Chain.AstarZKEvm },
            { "660279", Chain.Xai },
            { "81457", Chain.Blast },
            { "8333", Chain.B3 },
            { "33139", Chain.APEChain },
            { "13371", Chain.ImmutableZkEvm },
            { "1482601649", Chain.SkaleNebula },
            { "7668", Chain.Root },
            
            { "11155111", Chain.TestnetSepolia },
            { "80002", Chain.TestnetPolygonAmoy },
            { "421614", Chain.TestnetArbitrumSepolia },
            { "97", Chain.TestnetBNBSmartChain },
            { "84532", Chain.TestnetBaseSepolia },
            { "43113", Chain.TestnetAvalanche },
            { "40875", Chain.TestnetOasysHomeverse },
            { "11155420", Chain.TestnetOptimisticSepolia },
            { "6038361", Chain.TestnetAstarZKyoto },
            { "2730", Chain.TestnetXrSepolia },
            { "37714555429", Chain.TestnetXaiSepolia },
            { "1993", Chain.TestnetB3Sepolia },
            { "33111", Chain.TestnetAPEChain },
            { "168587773", Chain.TestnetBlastSepolia },
            { "94984", Chain.TestnetBorne },
            { "37084624", Chain.TestnetSkaleNebulaGamingHub },
            { "1946", Chain.TestnetSoneiumMinato },
            { "21000000", Chain.TestnetToy },
            { "13473", Chain.TestnetImmutableZkEvm },
            { "7672", Chain.TestnetRootPorcini }
        };

        public static Dictionary<Chain, string> PathOf = new Dictionary<Chain, string>()
        {
            { Chain.Ethereum, "mainnet" },
            { Chain.Polygon, "polygon" },
            { Chain.BNBSmartChain, "bsc" },
            { Chain.PolygonZkEvm, "polygon-zkevm" },
            { Chain.ArbitrumOne, "arbitrum" },
            { Chain.ArbitrumNova, "arbitrum-nova" },
            { Chain.Optimism, "optimism" },
            { Chain.Avalanche, "avalanche" },
            { Chain.Gnosis, "gnosis" },
            { Chain.Base, "base" },
            { Chain.OasysHomeverse, "homeverse" },
            { Chain.AstarZKEvm, "astar-zkevm" },
            { Chain.Xai, "xai" },
            { Chain.Blast, "blast" },
            { Chain.B3, "b3" },
            { Chain.APEChain, "apechain" },
            { Chain.ImmutableZkEvm, "immutable-zkevm" },
            { Chain.SkaleNebula, "skale-nebula" },
            { Chain.Root, "root" },

            { Chain.TestnetSepolia, "sepolia" },
            { Chain.TestnetArbitrumSepolia, "arbitrum-sepolia" },
            { Chain.TestnetBNBSmartChain, "bsc-testnet" },
            { Chain.TestnetBaseSepolia, "base-sepolia" },
            { Chain.TestnetOasysHomeverse, "homeverse-testnet" },
            { Chain.TestnetAvalanche, "avalanche-testnet" },
            { Chain.TestnetOptimisticSepolia, "optimism-sepolia" },
            { Chain.TestnetPolygonAmoy, "amoy" },
            { Chain.TestnetAstarZKyoto, "astar-zkyoto" },
            { Chain.TestnetXrSepolia, "xr-sepolia" },
            { Chain.TestnetXaiSepolia, "xai-sepolia" },
            { Chain.TestnetB3Sepolia, "b3-sepolia" },
            { Chain.TestnetAPEChain, "apechain-testnet" },
            { Chain.TestnetBlastSepolia, "blast-sepolia" },
            { Chain.TestnetBorne, "borne-testnet" },
            { Chain.TestnetSkaleNebulaGamingHub, "skale-nebula-testnet" },
            { Chain.TestnetSoneiumMinato, "soneium-minato" },
            { Chain.TestnetToy, "toy-testnet" },
            { Chain.TestnetImmutableZkEvm, "immutable-zkevm-testnet" },
            { Chain.TestnetRootPorcini, "root-porcini" }
        };
    }
}