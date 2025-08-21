using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.EcosystemWallet.Primitives;
using Sequence.EcosystemWallet.UnitTests;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class SessionsTest
    {
        public Task<string> SessionEmpty(Dictionary<string, object> parameters)
        {
            var identitySigner = (string)parameters["identitySigner"];
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
            var sessionTopologyJson = parameters["sessionTopology"].ToString();
            
            var signatures = JsonConvert.DeserializeObject<object[]>(parameters["callSignatures"]
                .ToString()).Select(s => SessionCallSignature.FromJson(s.ToString())).ToArray();
            
            var explicitSigners = JsonConvert.DeserializeObject<string[]>(parameters["explicitSigners"]
                .ToString()).Select(v => new Address(v)).ToArray();
            
            var implicitSigners = JsonConvert.DeserializeObject<string[]>(parameters["implicitSigners"]
                .ToString()).Select(v => new Address(v)).ToArray();
            
            var sessionsTopology = SessionsTopology.FromJson(sessionTopologyJson);
            var encodedSignatures = SessionCallSignature.EncodeSignatures(signatures, sessionsTopology, explicitSigners, implicitSigners);
            
            return Task.FromResult(encodedSignatures.ByteArrayToHexStringWithPrefix());
        }
        
        public Task<string> SessionImageHash(Dictionary<string, object> parameters)
        {
            var sessionTopologyJson = parameters["sessionTopology"].ToString();
            var topology = SessionsTopology.FromJson(sessionTopologyJson);

            return Task.FromResult(topology.ImageHash());
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
        
        /// <summary>
        /// This test is not yet used during the tests with forge.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<string> SessionExplicitRemove(Dictionary<string, object> parameters)
        {
            var explicitSessionAddress = new Address((string)parameters["explicitSession"]);
            var sessionTopology = SessionsTopology.FromJson(parameters["sessionTopology"].ToString());
            var newTopology = sessionTopology.RemoveExplicitSession(explicitSessionAddress);
            
            return Task.FromResult(newTopology.JsonSerialize());
        }
        
        public Task<string> SessionImplicitAddBlacklistAddress(Dictionary<string, object> parameters)
        {
            var blacklistAddress = new Address((string)parameters["blacklistAddress"]);
            var sessionTopologyJson = parameters["sessionTopology"].ToString();

            var sessionsTopology = SessionsTopology.FromJson(sessionTopologyJson);
            sessionsTopology.AddToImplicitBlacklist(blacklistAddress);
            
            return Task.FromResult(sessionsTopology.JsonSerialize());
        }
        
        /// <summary>
        /// This test is not yet used during the tests with forge.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<string> SessionImplicitRemoveBlacklistAddress(Dictionary<string, object> parameters)
        {
            var address = new Address((string)parameters["address"]);
            var sessionTopology = SessionsTopology.FromJson(parameters["sessionTopology"].ToString());
            sessionTopology.RemoveFromImplicitBlacklist(address);
            
            return Task.FromResult(sessionTopology.JsonSerialize());
        }
    }
}