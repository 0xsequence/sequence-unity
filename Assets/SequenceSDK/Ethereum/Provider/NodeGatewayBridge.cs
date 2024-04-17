using System;
using System.Collections.Generic;
using Sequence;
using Sequence.Config;

namespace Sequence.Provider
{
    public static class NodeGatewayBridge
    {
        private static SequenceConfig _config = SequenceConfig.GetConfig();
        
        private static Dictionary<Chain, string> _pathAt = new Dictionary<Chain, string>()
        {
            { Chain.Ethereum, "mainnet" },
            { Chain.Polygon, "polygon" },
            { Chain.PolygonZkEvm, "polygon-zkevm"},
            { Chain.BNBSmartChain, "bsc" },
            { Chain.ArbitrumOne, "arbitrum" },
            { Chain.ArbitrumNova, "arbitrum-nova" },
            { Chain.Optimism, "optimism" },
            { Chain.Avalanche, "avalanche" },
            { Chain.Gnosis, "gnosis" },
            { Chain.Base, "base" },
            { Chain.OasysHomeverse, "homeverse" },
            
            { Chain.TestnetSepolia, "sepolia" },
            { Chain.TestnetPolygonMumbai, "mumbai" },
            { Chain.TestnetAvalanche, "avalanche-testnet" },
            { Chain.TestnetBNBSmartChain, "bsc-testnet" },
            { Chain.TestnetOasysHomeverse, "homeverse-testnet" },
            { Chain.TestnetArbitrumSepolia, "arbitrum-sepolia"},
            { Chain.TestnetOptimisticSepolia, "optimism-sepolia"},
            { Chain.TestnetBaseSepolia, "base-sepolia"},
        };
        
        private const string _baseUrl = "https://nodes.sequence.app/";
        
        public static string GetNodeGatewayUrl(Chain chain)
        {
            if (!_pathAt.ContainsKey(chain))
            {
                throw new Exception(
                    "Network is not supported. Please contact Sequence support and use your own RPC url in the meantime");
            }

            string builderApiKey = _config.BuilderAPIKey;
            if (string.IsNullOrWhiteSpace(builderApiKey))
            {
                throw SequenceConfig.MissingConfigError("Builder API Key");
            }
            
            string url = _baseUrl + _pathAt[chain] + "/" + builderApiKey; 
            return url;
        }
    }
}