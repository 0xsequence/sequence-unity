using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class RecoveryTests
    {
        public Task<string> HashFromLeaves(Dictionary<string, object> parameters)
        {
            var leavesInput = (string)parameters["leaves"];
            var leaves = leavesInput.Split(' ').Select(RecoveryLeaf.FromInput).ToArray();
            var topology = RecoveryTopology.FromLeaves(leaves);

            return Task.FromResult(topology.Hash(true));
        }
        
        public Task<string> Encode(Dictionary<string, object> parameters)
        {
            var leavesInput = (string)parameters["leaves"];
            var leaves = leavesInput.Split(' ').Select(RecoveryLeaf.FromInput).ToArray();
            var topology = RecoveryTopology.FromLeaves(leaves);
            
            return Task.FromResult(topology.Encode().ByteArrayToHexStringWithPrefix());
        }
        
        public Task<string> Trim(Dictionary<string, object> parameters)
        {
            var leavesInput = (string)parameters["leaves"];
            var signer = new Address((string)parameters["signer"]);
            
            var leaves = leavesInput.Split(' ').Select(RecoveryLeaf.FromInput).ToArray();
            var topology = RecoveryTopology.FromLeaves(leaves);
            var trimmed = topology.Trim(signer);
            
            return Task.FromResult(trimmed.Encode().ByteArrayToHexStringWithPrefix());
        }
        
        public Task<string> HashEncoded(Dictionary<string, object> parameters)
        {
            var encodedStr = (string)parameters["encoded"];
            var encoded = encodedStr.HexStringToByteArray();
            var decoded = RecoveryTopology.Decode(encoded);
            
            return Task.FromResult(decoded.Hash(true));
        }
    }
}