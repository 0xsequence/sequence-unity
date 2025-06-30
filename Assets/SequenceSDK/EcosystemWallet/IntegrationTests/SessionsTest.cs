using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class SessionsTest
    {
        public Task<string> SessionEmpty(Dictionary<string, object> parameters)
        {
            var identitySigner = parameters["identitySigner"].ToString();

            return Task.FromResult(identitySigner);
        }

        public Task<string> SessionEncodeTopology(Dictionary<string, object> parameters)
        {
            var topologyJson = parameters["sessionTopology"].ToString();
            var topology = SessionsTopology.FromJson(topologyJson);
            var encoded = topology.Encode().ByteArrayToHexStringWithPrefix();
            
            return Task.FromResult(encoded);
        }
        
        public Task<string> SessionEncodeCallSignatures(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }
        
        public Task<string> SessionImageHash(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }
        
        public Task<string> SessionExplicitAdd(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }
        
        public Task<string> SessionExplicitRemove(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }
        
        public Task<string> SessionImplicitAddBlacklistAddress(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }
        
        public Task<string> SessionImplicitRemoveBlacklistAddress(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }
    }
}