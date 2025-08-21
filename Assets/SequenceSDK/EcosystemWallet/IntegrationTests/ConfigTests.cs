using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class ConfigTests
    {
        public Task<string> ConfigNew(Dictionary<string, object> parameters)
        {
            var threshold = (string)parameters["threshold"];
            var checkpoint = (string)parameters["checkpoint"];
            var content = (string)parameters["content"];
            
            var checkpointer = parameters.TryGetValue("checkpointer", out var checkpointerValue) && 
                               checkpointerValue != null ? new Address(checkpointerValue as string) : null;
            
            var config = new Primitives.Config
            {
                threshold = BigInteger.Parse(threshold),
                checkpoint = BigInteger.Parse(checkpoint),
                topology = Topology.FromLeaves(Topology.ParseContentToLeafs(content)),
                checkpointer = checkpointer
            };

            return Task.FromResult(JsonConvert.SerializeObject(config));
        }
        
        public Task<string> ConfigEncode(Dictionary<string, object> parameters)
        {
            var input = parameters["input"].ToString();
            var config = Primitives.Config.FromJson(input);
            var signature = new RawSignature
            {
                checkpointerData = null,
                configuration = config,
            };

            var encoded = signature.Encode().ByteArrayToHexStringWithPrefix();
            return Task.FromResult(encoded);
        }
        
        public Task<string> ConfigImageHash(Dictionary<string, object> parameters)
        {
            var input = parameters["input"].ToString();
            var config = Primitives.Config.FromJson(input);
            var imageHash = config.HashConfiguration().ByteArrayToHexStringWithPrefix();
            return Task.FromResult(imageHash);
        }
    }
}