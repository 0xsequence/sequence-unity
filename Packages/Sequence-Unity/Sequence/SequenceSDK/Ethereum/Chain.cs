using System;
using System.Numerics;

namespace Sequence
{
    public enum Chain
    {
        // Mainnets
        Ethereum = 1,
        Polygon = 137,
        PolygonZkEvm = 1101,
        BNBSmartChain = 56,
        ArbitrumOne = 42161,
        ArbitrumNova = 42170,
        Optimism = 10,
        Avalanche = 43114,
        Gnosis = 100,
        Base = 8453,
        OasysHomeverse = 19011,
        [Obsolete("Chain is no longer active; use TestnetSoneiumMinato instead")]
        AstarZKEvm = 3776,
        Xai = 660279,
        Blast = 81457,
        B3 = 8333,
        APEChain = 33139,
        ImmutableZkEvm = 13371,
        SkaleNebula = 1482601649,
        Root = 7668,
        LAOS = 6283,
        Soneium = 1868,
        Telos = 40,
        Moonbeam = 1284,
        Etherlink = 42793,
        [Obsolete("Chain is not currently supported")]
        XR1 = 273,
        Somnia = 5031,

        // Testnets
        TestnetSepolia = 11155111,
        TestnetPolygonAmoy = 80002,
        TestnetArbitrumSepolia = 421614,
        TestnetBNBSmartChain = 97,
        TestnetBaseSepolia = 84532,
        TestnetAvalanche = 43113,
        TestnetOasysHomeverse = 40875,
        TestnetOptimisticSepolia = 11155420,
        [Obsolete("Chain is no longer active; use TestnetSoneiumMinato instead")]
        TestnetAstarZKyoto = 6038361,
        [Obsolete("Chain is not currently supported")]
        TestnetXrSepolia = 2730,
        TestnetB3Sepolia = 1993,
        TestnetAPEChain = 33111,
        TestnetBlastSepolia = 168587773,
        [Obsolete("Chain is not currently supported")]
        TestnetBorne = 94984,
        TestnetSkaleNebulaGamingHub = 37084624,
        TestnetSoneiumMinato = 1946,
        TestnetToy = 21000000,
        TestnetImmutableZkEvm = 13473,
        TestnetRootPorcini = 7672,
        TestnetLAOSSigma = 62850,
        TestnetTelos = 41,
        TestnetMoonbaseAlpha = 1287,
        TestnetEtherlink = 128123,
        TestnetMonad = 10143,
        TestnetSomnia = 50312,
        TestnetFrequency = 53716,
        TestnetIncentiv = 11690,
        TestnetIncentivV2 = 28802,
        TestnetSandbox = 6252,
        
        LocalChain = 31337, // A chain running locally on your system

        TestnetXaiSepolia = -1, // Xai Sepolia's testnet's chain ID is too large to fit inside an int
        
        // Null
        None = 0
    }
}