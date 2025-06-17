using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public static class ConfigUtils
    {
        public static Primitives.Config FromJson(string json)
        {
            var input = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            
            var threshold = (string)input["threshold"];
            var checkpoint = (string)input["checkpoint"];
            var checkpointer = input.GetAddress("checkpointer");
            var topology = input["topology"].ToString();
            
            return new Primitives.Config
            {
                threshold = BigInteger.Parse(threshold),
                checkpoint = BigInteger.Parse(checkpoint),
                checkpointer = checkpointer,
                topology = Topology.Decode(topology)
            };
        }
    }
}