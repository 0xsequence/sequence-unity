using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class SignatureTests
    {
        public async Task<string> SignatureEncode(Dictionary<string, object> parameters)
        {
            var input = parameters["input"].ToString();
            var signatures = parameters["signatures"].ToString().Split(' ');
            var chainId = !parameters.TryGetValue("chainId", out var chainIdValue) || (bool)chainIdValue;
            
            return await SignatureUtils.DoEncode(input, signatures, !chainId);
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