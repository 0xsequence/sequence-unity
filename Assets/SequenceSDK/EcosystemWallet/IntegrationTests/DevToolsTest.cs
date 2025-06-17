using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class DevToolsTest
    {
        public Task<string> DevToolsRandomConfig(Dictionary<string, object> parameters)
        {
            var maxDepth = parameters["maxDepth"].ToString();
            var seed = (string)parameters["seed"];
            var minThresholdOnNested = parameters["minThresholdOnNested"].ToString();
            var checkpointer = (string)parameters["checkpointer"];
            
            return Task.FromResult("");
        }
        
        public Task<string> DevToolsRandomSessionTopology(Dictionary<string, object> parameters)
        {
            return Task.FromResult("");
        }
    }
}