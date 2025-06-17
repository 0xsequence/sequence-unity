using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class DevToolsTest
    {
        public async Task<string> DevToolsRandomConfig(Dictionary<string, object> parameters)
        {
            var maxDepth = (int)parameters["maxDepth"];
            var seed = (string)parameters["seed"];
            var minThresholdOnNested = (int)parameters["minThresholdOnNested"];
            var checkpointer = (string)parameters["checkpointer"];
            
            throw new NotImplementedException("Not implemented");
        }
        
        public async Task<string> DevToolsRandomSessionTopology(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException("Not implemented");
        }
    }
}