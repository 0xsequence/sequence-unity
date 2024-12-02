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
        TestnetXrSepolia = 2730,
        TestnetB3Sepolia = 1993,
        TestnetAPEChain = 33111,
        TestnetBlastSepolia = 168587773,
        TestnetBorne = 94984,
        TestnetSkaleNebulaGamingHub = 37084624,
        TestnetSoneiumMinato = 1946,
        TestnetToy = 21000000,
        TestnetImmutableZkEvm = 13473,
        TestnetRootPorcini = 7672,
        TestnetLAOSSigma = 62850,
        
        TestnetXaiSepolia = -1, // Xai Sepolia's testnet's chain ID is too large to fit inside an int
        
        // Null
        None = 0
    }
}