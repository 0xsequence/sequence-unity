using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sequence.Relayer
{
    public class PremiumIAPMinter : PermissionedMinter
    {
        private string _iapReceipt;
        
        public PremiumIAPMinter(MintingRequestProver mintingRequestProver, string mintEndpoint, string contractAddress, string iapReceipt) : base(mintingRequestProver, mintEndpoint, contractAddress)
        {
            _iapReceipt = iapReceipt;
        }

        public override async Task<string> BuildMintTokenRequestJson(string tokenId, uint amount = 1)
        {
            MintingRequestProof requestProof =
                await _mintingRequestProver.GenerateProof(new PremiumIAPMintRequestPayload(_contractAddress, tokenId, amount, _iapReceipt));

            MintTokenRequest mintTokenRequest = new MintTokenRequest(requestProof);

            string mintTokenRequestJson = JsonConvert.SerializeObject(mintTokenRequest);
            return mintTokenRequestJson;
        }
    }
    
    [Serializable]
    public class PremiumIAPMintRequestPayload : MintRequestPayload
    {
        public string contractAddress;
        public string tokenId;
        public uint amount;
        public string iapReceipt;

        public PremiumIAPMintRequestPayload(string contractAddress, string tokenId, uint amount, string iapReceipt)
        {
            this.contractAddress = contractAddress;
            this.tokenId = tokenId;
            this.amount = amount;
            this.iapReceipt = iapReceipt;
        }
    }
}