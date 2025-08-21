using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class AddressTests
    {
        public Task<string> Calculate(Dictionary<string, object> parameters)
        {
            var imageHash = (string)parameters["imageHash"];
            var factory = (string)parameters["factory"];
            var module = (string)parameters["module"];
            
            var address = AddressFactory.Create(imageHash.HexStringToByteArray(), factory, module);
            Debug.Log($"Sequence Address: {address}");
            
            return Task.FromResult(address.Value);
        }
    }
}