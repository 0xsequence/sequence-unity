using System;
using System.Collections.Generic;
using Sequence;
using Sequence.Config;

namespace Sequence.Provider
{
    public static class NodeGatewayBridge
    {
        private static SequenceConfigBase _configBase = SequenceConfig.GetConfig(SequenceService.NodeGateway);
        
#if SEQUENCE_DEV_NODEGATEWAY || SEQUENCE_DEV
        private const string _baseUrl = "https://dev-nodes.sequence.app/";
#else
        private const string _baseUrl = "https://nodes.sequence.app/";
#endif        
        
        public static string GetNodeGatewayUrl(Chain chain)
        {
            if (!ChainDictionaries.PathOf.ContainsKey(chain))
            {
                throw new Exception(
                    "Network is not supported. Please contact Sequence support and use your own RPC url in the meantime");
            }

            string builderApiKey = _configBase.BuilderAPIKey;
            if (string.IsNullOrWhiteSpace(builderApiKey))
            {
                throw SequenceConfig.MissingConfigError("Builder API Key");
            }
            
            string url = _baseUrl + ChainDictionaries.PathOf[chain] + "/" + builderApiKey; 
            return url;
        }
    }
}