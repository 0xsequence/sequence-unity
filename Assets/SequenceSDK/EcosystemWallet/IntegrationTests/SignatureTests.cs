using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.EcosystemWallet.Primitives;
using Sequence.EcosystemWallet.Utils;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class SignatureTests
    {
        public Task<string> SignatureEncode(Dictionary<string, object> parameters)
        {
            var input = parameters["input"].ToString();
            var signatures = parameters["signatures"].ToString();
            var chainId = !parameters.TryGetValue("chainId", out var chainIdValue) || (bool)chainIdValue;

            return Task.FromResult(SignatureUtils.EncodeSignatureFromInput(input, signatures, !chainId));
        }

        public Task<string> SignatureDecode(Dictionary<string, object> parameters)
        {
            var encodedSignature = parameters["signature"].ToString().HexStringToByteArray();
            var signature = RawSignature.Decode(encodedSignature);
            
            return Task.FromResult(JsonConvert.SerializeObject(signature));
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