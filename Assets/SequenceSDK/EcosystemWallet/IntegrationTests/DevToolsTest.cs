using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class DevToolsTest
    {
        public Task<string> DevToolsRandomConfig(Dictionary<string, object> parameters)
        {
            var maxDepth = int.Parse(parameters["maxDepth"].ToString());
            var seed = (string)parameters["seed"];
            var minThresholdOnNested = int.Parse(parameters["minThresholdOnNested"].ToString());
            var checkpointer = parameters.TryGetValue("checkpointer", out var checkpointerObj) ? checkpointerObj.ToString() : "no";
            var skewed = parameters.TryGetValue("skewed", out var skewedObj) ? skewedObj.ToString() : "none";

            var options = new DevTools.RandomOptions
            {
                seededRandom = string.IsNullOrEmpty(seed) ? null : DevTools.CreateSeededRandom(seed),
                checkpointer = checkpointer,
                minThresholdOnNested = minThresholdOnNested,
                skewed = skewed
            };
            
            return Task.FromResult(DevTools.CreateRandomConfig(maxDepth, options).ToJson());
        }
        
        public Task<string> DevToolsRandomSessionTopology(Dictionary<string, object> parameters)
        {
            throw new System.NotImplementedException("This test is not yet used during the tests with forge.");
        }
    }
}