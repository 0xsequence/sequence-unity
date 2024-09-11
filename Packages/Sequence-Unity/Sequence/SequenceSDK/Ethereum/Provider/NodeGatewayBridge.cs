using System;
using System.Collections.Generic;
using Sequence;
using Sequence.Config;

namespace Sequence.Provider
{
    public static class NodeGatewayBridge
    {
        private static SequenceConfig _config = SequenceConfig.GetConfig();
        
        private const string _baseUrl = "https://nodes.sequence.app/";
        
        public static string GetNodeGatewayUrl(Chain chain)
        {
            if (!ChainDictionaries.PathOf.ContainsKey(chain))
            {
                throw new Exception(
                    "Network is not supported. Please contact Sequence support and use your own RPC url in the meantime");
            }

            string builderApiKey = _config.BuilderAPIKey;
            if (string.IsNullOrWhiteSpace(builderApiKey))
            {
                throw SequenceConfig.MissingConfigError("Builder API Key");
            }
            
            string url = _baseUrl + ChainDictionaries.PathOf[chain] + "/" + builderApiKey; 
            return url;
        }
    }
}