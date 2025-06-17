using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class SignatureTests
    {
        public Task<string> SignatureEncode(Dictionary<string, object> parameters)
        {
            var signatures = (string)parameters["signatures"];
            var chainId = (bool)parameters["chainId"];
            
            var input = parameters.GetNestedObjects("input");
            Debug.Log($"## Parameters {JsonConvert.SerializeObject(parameters)}");
            Debug.Log($"## Input {JsonConvert.SerializeObject(input)}");
            
            var threshold = (string)input["threshold"];
            var checkpoint = (string)input["checkpoint"];
            var checkpointer = input.GetAddress("checkpointer");
            
            return Task.FromResult("");
        }

        public Task<string> SignatureDecode(Dictionary<string, object> parameters)
        {
            throw new System.NotImplementedException();
        }
        
        public Task<string> SignatureConcat(Dictionary<string, object> parameters)
        {
            return Task.FromResult("");
        }
    }
}