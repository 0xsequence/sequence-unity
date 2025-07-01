using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class RecoveryTests
    {
        public Task<string> HashFromLeaves(Dictionary<string, object> parameters)
        {
            var leavesInput = (string)parameters["leaves"];
            var leaves = leavesInput.Split(' ').Select(RecoveryLeaf.FromInput).ToArray();
            
            throw new System.NotImplementedException();
        }
        
        public Task<string> Encode(Dictionary<string, object> parameters)
        {
            throw new System.NotImplementedException();
        }
        
        public Task<string> Trim(Dictionary<string, object> parameters)
        {
            throw new System.NotImplementedException();
        }
        
        public Task<string> HashEncoded(Dictionary<string, object> parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}