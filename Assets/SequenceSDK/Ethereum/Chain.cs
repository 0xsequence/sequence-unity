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
        
        // Testnets
        TestnetGoerli = 5,
        TestnetSepolia = 11155111,
        TestnetPolygonMumbai = 80001,
        TestnetArbitrumGoerli = 421613,
        TestnetBNBSmartChain = 97,
        TestnetBaseGoerli = 84531,
        TestnetAvalanche = 43113,
        TestnetOasysHomeverse = 40875,
        
        // Null
        None = 0
    }
}