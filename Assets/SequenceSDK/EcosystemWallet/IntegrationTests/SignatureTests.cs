using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.EcosystemWallet.Primitives;
using Sequence.EcosystemWallet.Utils;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class SignatureTests
    {
        public Task<string> SignatureEncode(Dictionary<string, object> parameters)
        {
            var input = parameters["input"].ToString();
            var signatures = parameters["signatures"].ToString();
            var noChainId = !parameters.TryGetValue("chainId", out var chainIdValue) || !(bool)chainIdValue;
            var checkpointerData = parameters.TryGetValue("checkpointerData", out var checkpointerDataValue) ? 
                checkpointerDataValue.ToString().HexStringToByteArray() : null;

            return Task.FromResult(SignatureUtils.EncodeSignatureFromInput(input, signatures, noChainId, checkpointerData));
        }

        public Task<string> SignatureDecode(Dictionary<string, object> parameters)
        {
            var encodedSignature = parameters["signature"].ToString().HexStringToByteArray();
            var signature = RawSignature.Decode(encodedSignature);
            
            return Task.FromResult(signature.ToJson());
        }
        
        public Task<string> SignatureConcat(Dictionary<string, object> parameters)
        {
            var signatures = parameters.GetArray<string>("signatures");
            var decoded = signatures.Select(signature => 
                    RawSignature.Decode(signature.HexStringToByteArray())).ToArray();

            var parentSignature = decoded[0];
            parentSignature.suffix = decoded.Slice(1);
            var encoded = parentSignature.Encode();
            
            return Task.FromResult(encoded.ByteArrayToHexStringWithPrefix());
        }
    }
}