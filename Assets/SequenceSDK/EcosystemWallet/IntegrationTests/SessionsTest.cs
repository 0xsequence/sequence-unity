using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Primitives;
using Sequence.EcosystemWallet.UnitTests;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class SessionsTest
    {
        public Task<string> SessionEmpty(Dictionary<string, object> parameters)
        {
            var identitySigner = parameters["identitySigner"].ToString();
            var topology = SessionsUtils.CreateSessionsTopologyWithSingleIdentity(identitySigner);

            return Task.FromResult(topology.JsonSerialize());
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
            var explicitSessionJson = parameters["explicitSession"].ToString();
            var sessionTopologyJson = parameters["sessionTopology"].ToString();

            var explicitSession = SessionPermissions.FromJson(explicitSessionJson);
            var sessionTopology = SessionsTopology.FromJson(sessionTopologyJson);

            var existingPermission = sessionTopology.FindLeaf<PermissionLeaf>(leaf => 
                leaf.permissions.signer.Equals(explicitSession.signer));
            
            if (existingPermission != null)
                throw new Exception("Session already exists.");

            var newTopology = sessionTopology.AddExplicitSession(explicitSession);
            return Task.FromResult(newTopology.JsonSerialize());
        }
        
        public Task<string> SessionExplicitRemove(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }
        
        public Task<string> SessionImplicitAddBlacklistAddress(Dictionary<string, object> parameters)
        {
            var blacklistAddress = new Address((string)parameters["blacklistAddress"]);
            var sessionTopologyJson = parameters["sessionTopology"].ToString();

            var sessionsTopology = SessionsTopology.FromJson(sessionTopologyJson);
            sessionsTopology.AddToImplicitBlacklist(blacklistAddress);
            
            return Task.FromResult(sessionsTopology.JsonSerialize());
        }
        
        public Task<string> SessionImplicitRemoveBlacklistAddress(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }
    }
}