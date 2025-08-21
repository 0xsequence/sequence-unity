using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.EcosystemWallet.Primitives.Passkeys;
using UnityEngine;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class PasskeysTests
    {
        public Task<string> EncodeSignature(Dictionary<string, object> parameters)
        {
            var encoded = PasskeysHelper.EncodeSignature(new PasskeysArgs
            {
                x = (string)parameters["x"],
                y = (string)parameters["y"],
                r = (string)parameters["r"],
                s = (string)parameters["s"],
                requireUserVerification = (bool)parameters["requireUserVerification"],
                embedMetadata = (bool)parameters["embedMetadata"],
                credentialId = parameters.TryGetValue("credentialId", out var credentialIdValue) ? (string)credentialIdValue : null,
                metadataHash = parameters.TryGetValue("metadataHash", out var metadataHashValue) ? (string)metadataHashValue : null,
                authenticatorData = (string)parameters["authenticatorData"],
                clientDataJson = parameters["clientDataJson"].ToString(),
            });
            
            return Task.FromResult(encoded);
        }
        
        public Task<string> DecodeSignature(Dictionary<string, object> parameters)
        {
            throw new System.NotImplementedException();
        }
        
        public Task<string> ComputeRoot(Dictionary<string, object> parameters)
        {
            var x = (string)parameters["x"];
            var y = (string)parameters["y"];
            var requireUserVerification = (bool)parameters["requireUserVerification"];
            var credentialId = parameters.TryGetValue("credentialId", out var credentialIdValue) ? (string)credentialIdValue : null;
            var metadataHash = parameters.TryGetValue("metadataHash", out var metadataHashValue) ? (string)metadataHashValue : null;

            var result = PasskeysHelper.ComputeRoot(new PasskeysArgs
            {
                x = x,
                y = y,
                requireUserVerification = requireUserVerification,
                credentialId = credentialId,
                metadataHash = metadataHash
            });
            
            return Task.FromResult(result);
        }
        
        public Task<string> ValidateSignature(Dictionary<string, object> parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}