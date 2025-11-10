using System.Collections.Generic;

namespace Sequence
{
    public static class ChainDictionaries
    {
        private struct ChainConfig
        {
            public readonly Chain Chain;
            public readonly string ChainId;
            public readonly string Name;
            public readonly string Path;
            public readonly string BlockExplorerUrl;
            public readonly string GasCurrency;
            public readonly string NativeTokenAddress;

            public ChainConfig(Chain chain, string chainId, string name, string path, string blockExplorerUrl, 
                string gasCurrency, string nativeTokenAddress)
            {
                Chain = chain;
                ChainId = chainId;
                Name = name;
                Path = path;
                BlockExplorerUrl = blockExplorerUrl;
                GasCurrency = gasCurrency;
                NativeTokenAddress = nativeTokenAddress;
            }
        }

        private static ChainConfig[] ChainConfigs = new ChainConfig[]
        {
            new (Chain.Ethereum, "1", "Ethereum", "mainnet", "https://etherscan.io/", "ETH", "0xC02aaA39b223FE8D0A0E5C4F27eAD9083C756Cc2"),
            new (Chain.Polygon, "137", "Polygon", "polygon", "https://polygonscan.com/", "POL", "0x0000000000000000000000000000000000001010"),
            new (Chain.PolygonZkEvm, "1101", "Polygon zkEvm", "polygon-zkevm", "https://zkevm.polygonscan.com/", "POL", "0xa2036f0538221a77A3937F1379699f44945018d0"),
            new (Chain.BNBSmartChain, "56", "BNB Smart Chain", "bsc", "https://bscscan.com/", "BNB", "0xbb4CdB9CBd36B01bD1cBaEBF2De08d9173bc095c"),
            new (Chain.ArbitrumOne, "42161", "Arbitrum One", "arbitrum", "https://arbiscan.io/", "AETH", "0x912CE59144191C1204E64559FE8253a0e49E6548"),
            new (Chain.ArbitrumNova, "42170", "Arbitrum Nova", "arbitrum-nova", "https://nova.arbiscan.io/", "AETH", "0xf823c3cd3cebe0a1fa952ba88dc9eef8e0bf46ad"),
            new (Chain.Optimism, "10", "Optimism", "optimism", "https://optimistic.etherscan.io/", "OP", "0x4200000000000000000000000000000000000042"),
            new (Chain.Avalanche, "43114", "Avalanche", "avalanche", "https://subnets.avax.network/c-chain/", "AVAX", "0xB31f66AA3C1e785363F0875A1B74E27b85FD66c7"),
            new (Chain.Gnosis, "100", "Gnosis", "gnosis", "https://gnosisscan.io/", "xDai", "0x9C58BAcC331c9aa871AFD802DB6379a98e80CEdb"),
            new (Chain.Base, "8453", "Base", "base", "https://basescan.org/", "ETH", "0x4200000000000000000000000000000000000006"),
            new (Chain.OasysHomeverse, "19011", "Oasys Homeverse", "homeverse", "https://explorer.oasys.homeverse.games/", "OAS", "0xd07df0da6e67b31db33cde4a6893e06bd87f8a08"),
            new (Chain.AstarZKEvm, "3776", "Astar zkEVM", "astar-zkevm", "https://astar-zkevm.explorer.startale.com/", "ETH", "0x0000000000000000000000000000000000000000"),
            new (Chain.Xai, "660279", "Xai", "xai", "https://explorer.xai-chain.net/", "XAI", "0x0000000000000000000000000000000000000000"),
            new (Chain.Blast, "81457", "Blast", "blast", "https://blastscan.io/", "ETH", "0x0000000000000000000000000000000000000000"),
            new (Chain.B3, "8333", "B3", "b3", "https://explorer.b3.fun/", "ETH", "0x0000000000000000000000000000000000000000"),
            new (Chain.APEChain, "33139", "APE Chain", "apechain", "https://apescan.io/", "APE", "0x0000000000000000000000000000000000000000"),
            new (Chain.ImmutableZkEvm, "13371", "Immutable zkEVM", "immutable-zkevm", "https://explorer.immutable.com/", "IMX", "0x0000000000000000000000000000000000000000"),
            new (Chain.SkaleNebula, "1482601649", "Skale Nebula", "skale-nebula", "https://green-giddy-denebola.explorer.mainnet.skalenodes.com/", "sFUEL", "0x0000000000000000000000000000000000000000"),
            new (Chain.Soneium, "1868", "Soneium", "soneium", "https://vk9a3tgpne6qmub8.blockscout.com/", "ETH", "0x0000000000000000000000000000000000000000"),
            new (Chain.Telos, "40", "Telos", "telos", "https://www.teloscan.io/", "TLOS", "0x0000000000000000000000000000000000000000"),
            new (Chain.Moonbeam, "1284", "Moonbeam", "moonbeam", "https://moonscan.io/", "GLMR", "0x0000000000000000000000000000000000000000"),
            new (Chain.Etherlink, "42793", "Etherlink", "etherlink", "https://explorer.etherlink.com/", "XTZ", "0x0000000000000000000000000000000000000000"),
            new (Chain.XR1, "273", "XR1", "xr1", "", "XR1", "0x0000000000000000000000000000000000000000"),
            new (Chain.Somnia, "5031", "Somnia", "somnia", "https://mainnet.somnia.w3us.site/", "STT", "0x0000000000000000000000000000000000000000"),

            // --- TESTNETS ---
            new (Chain.TestnetSepolia, "11155111", "Sepolia", "sepolia", "https://sepolia.etherscan.io/", "ETH", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetArbitrumSepolia, "421614", "Arbitrum Sepolia", "arbitrum-sepolia", "https://sepolia.arbiscan.io/", "AETH", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetBNBSmartChain, "97", "BNB Smart Chain Testnet", "bsc-testnet", "https://testnet.bscscan.com/", "BNB", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetBaseSepolia, "84532", "Base Sepolia", "base-sepolia", "https://sepolia.basescan.org/", "ETH", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetOasysHomeverse, "40875", "Testnet Oasys Homeverse", "homeverse-testnet", "https://explorer.testnet.oasys.games/", "OAS", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetAvalanche, "43113", "Testnet Avalanche", "avalanche-testnet", "https://testnet.snowtrace.io/", "AVAX", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetOptimisticSepolia, "11155420", "Optimistic Sepolia", "optimism-sepolia", "https://sepolia-optimism.etherscan.io/", "OP", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetPolygonAmoy, "80002", "Polygon Amoy", "amoy", "https://amoy.polygonscan.com/", "POL", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetAstarZKyoto, "6038361", "Astar zKyoto Testnet", "astar-zkyoto", "https://astar-zkyoto.blockscout.com/", "ETH", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetXrSepolia, "2730", "XR Sepolia", "xr-sepolia", "https://xr-sepolia-testnet.explorer.caldera.xyz/", "tXR", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetXaiSepolia, "37714555429", "Xai Sepolia", "xai-sepolia", "https://testnet-explorer-v2.xai-chain.net/", "sXAI", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetB3Sepolia, "1993", "B3 Sepolia", "b3-sepolia", "https://sepolia.explorer.b3.fun/", "ETH", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetAPEChain, "33111", "APE Chain Testnet", "apechain-testnet", "https://curtis.explorer.caldera.xyz/", "APE", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetBlastSepolia, "168587773", "Blast Sepolia", "blast-sepolia", "https://testnet.blastscan.io/", "ETH", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetBorne, "94984", "Borne Testnet", "borne-testnet", "https://subnets-test.avax.network/bornegfdn", "BORNE", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetSkaleNebulaGamingHub, "37084624", "Skale Nebula Gaming Hub Testnet", "skale-nebula-testnet", "https://green-giddy-denebola.explorer.mainnet.skalenodes.com/", "sFUEL", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetSoneiumMinato, "1946", "Soneium Minato Testnet", "soneium-minato", "https://explorer-testnet.soneium.org/", "ETH", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetToy, "21000000", "TOY Testnet", "toy-testnet", "https://toy-chain-testnet.explorer.caldera.xyz/", "TOY", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetImmutableZkEvm, "13473", "Immutable zkEVM Testnet", "immutable-zkevm-testnet", "https://explorer.testnet.immutable.com/", "IMX", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetTelos, "41", "Telos Testnet", "telos-testnet", "https://testnet.teloscan.io/", "TLOS", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetMoonbaseAlpha, "1287", "Moonbase Alpha", "moonbase-alpha", "https://moonbase.moonscan.io/", "DEV", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetEtherlink, "128123", "Etherlink Testnet", "etherlink-testnet", "https://testnet.explorer.etherlink.com/", "XTZ", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetMonad, "10143", "Monad Testnet", "monad-testnet", "https://testnet.monadexplorer.com/", "MON", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetSomnia, "50312", "Somnia Testnet", "somnia-testnet", "https://somnia-testnet.socialscan.io/", "STT", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetIncentivV2, "28802", "Incentiv Testnet v2", "incentiv-testnet-v2", "https://explorer.testnet.incentiv.net/", "TCENT", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetSandbox, "6252", "Sandbox Testnet", "sandbox-testnet", "https://sandbox-testnet.explorer.caldera.xyz/", "SAND", "0x0000000000000000000000000000000000000000"),
            new (Chain.TestnetArc, "5042002", "Arc Testnet", "arc-testnet", "https://arc-testnet-explorer.stg.blockchain.circle.com/", "USDC", "0x0000000000000000000000000000000000000000"),
        };

        public static Dictionary<Chain, string> NameOf;
        public static Dictionary<Chain, string> GasCurrencyOf;
        public static Dictionary<Chain, string> NativeTokenAddressOf;
        public static Dictionary<Chain, string> BlockExplorerOf;
        public static Dictionary<Chain, string> ChainIdOf;
        public static Dictionary<string, Chain> ChainById;
        public static Dictionary<Chain, string> PathOf;

        /// <summary>
        /// Initialize the dictionaries once the domain reloads, ensuring they always reflect the latest chains from
        /// the ChainConfig array.
        /// </summary>
        static ChainDictionaries() 
            => InitializeDictionaries();

        private static void InitializeDictionaries()
        {
            NameOf = new Dictionary<Chain, string>();
            GasCurrencyOf = new Dictionary<Chain, string>();
            NativeTokenAddressOf = new Dictionary<Chain, string>();
            BlockExplorerOf = new Dictionary<Chain, string>();
            ChainIdOf = new Dictionary<Chain, string>();
            ChainById = new Dictionary<string, Chain>();
            PathOf = new Dictionary<Chain, string>();

            foreach (var config in ChainConfigs)
            {
                NameOf.Add(config.Chain, config.Name);
                GasCurrencyOf.Add(config.Chain, config.GasCurrency);
                NativeTokenAddressOf.Add(config.Chain, config.NativeTokenAddress);
                BlockExplorerOf.Add(config.Chain, config.BlockExplorerUrl);
                ChainIdOf.Add(config.Chain, config.ChainId);
                ChainById.Add(config.ChainId, config.Chain);
                PathOf.Add(config.Chain, config.Path);
            }
        }
    }
}