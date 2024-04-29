using System.Numerics;
using Newtonsoft.Json;

namespace Sequence.WaaS
{
    public class FeeToken
    {
        public uint chainId { get; private set; }
        public string contractAddress { get; private set; }
        public uint decimals { get; private set; }
        public string logoURL { get; private set; }
        public string name { get; private set; }
        public string symbol { get; private set; }
        public string tokenID { get; private set; }
        public FeeTokenType type { get; private set; }

        [JsonConstructor]
        public FeeToken(uint chainId, string contractAddress, uint decimals, string logoURL, string name, string symbol, string tokenID, FeeTokenType type)
        {
            this.chainId = chainId;
            this.contractAddress = contractAddress;
            this.decimals = decimals;
            this.logoURL = logoURL;
            this.name = name;
            this.symbol = symbol;
            this.tokenID = tokenID;
            this.type = type;
        }

        public FeeToken(Chain chain, string tokenName, string tokenSymbol, FeeTokenType feeTokenType,
            string tokenLogoUrl, string contractAddress = "", string tokenId = "", uint decimals = 18)
        {
            this.chainId = (uint)BigInteger.Parse(chain.GetChainId());
            if (!string.IsNullOrWhiteSpace(contractAddress))
            {
                this.contractAddress = contractAddress;
            }
            this.decimals = decimals;
            this.logoURL = tokenLogoUrl;
            this.name = tokenName;
            this.symbol = tokenSymbol;
            if (!string.IsNullOrWhiteSpace(tokenId))
            {
                this.tokenID = tokenId;
            }
            this.type = type;
        }
    }

    public enum FeeTokenType
    {
        unknown,
        erc20Token,
        erc1155Token,
    }
}