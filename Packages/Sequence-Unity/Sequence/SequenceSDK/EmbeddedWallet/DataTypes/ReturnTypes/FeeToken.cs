using System.Numerics;
using Newtonsoft.Json;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    public class FeeToken
    {
        public uint chainId;
        public string contractAddress;
        public uint decimals;
        public string logoURL;
        public string name;
        public string symbol;
        public string tokenID;
        public FeeTokenType type;

        [Preserve]
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

        public override string ToString()
        {
            return
                $"chainId: {chainId}, contractAddress: {contractAddress}, decimals: {decimals}, logoUrl: {logoURL}, name: {name}, symbol: {symbol}, tokenID: {tokenID}, type: {type}";
        }
    }

    public enum FeeTokenType
    {
        unknown,
        erc20Token,
        erc1155Token,
    }
}