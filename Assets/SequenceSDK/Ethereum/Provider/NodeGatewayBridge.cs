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

            { Chain.TestnetSepolia, "sepolia" },
            { Chain.TestnetArbitrumSepolia, "arbitrum-sepolia" },
            { Chain.TestnetBNBSmartChain, "bsc-testnet" },
            { Chain.TestnetBaseSepolia, "base-sepolia" },
            { Chain.TestnetOasysHomeverse, "homeverse-testnet" },
            { Chain.TestnetAvalanche, "avalanche-testnet" },
            { Chain.TestnetOptimisticSepolia, "optimism-sepolia" },
            { Chain.TestnetPolygonAmoy, "amoy" }, 
            { Chain.TestnetAstarZKyoto, "astar-zkyoto" }, 
            { Chain.TestnetXrSepolia, "xr-sepolia-testnet" },
            { Chain.TestnetXaiSepolia, "xai-sepolia" }, 
        };
        
        private const string _baseUrl = "https://nodes.sequence.app/";
        
        public static string GetNodeGatewayUrl(Chain chain)
        {
            if (!_pathAt.ContainsKey(chain))
            {
                throw new Exception(
                    "Network is not supported. Please contact Sequence support and use your own RPC url in the meantime");
            }
            string url = _baseUrl + _pathAt[chain] + "/" + "YfeuczOMRyP7fpr1v7h8SvrCAAAAAAAAA"; // Todo: temporary access key while we wait for prod env deployment. Currently, we are using the staging env and we don't have a staging env for indexer that we can hit publicly
            return url;
        }
    }
}