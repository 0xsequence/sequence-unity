using System;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    internal class SardineNFTCheckoutParams
    {
        public string name;
        public string imageUrl;
        public string network;
        public string recipientAddress;
        public string blockchainNftId;
        public string contractAddress;
        public int quantity;
        public int decimals;
        public string tokenAmount;
        public string tokenAddress;
        public string tokenSymbol;
        public int tokenDecimals;
        public string callData;
        public string platform;
        public string approvedSpenderAddress;

        public SardineNFTCheckoutParams(string name, string imageUrl, string network, string recipientAddress, string blockchainNftId, string contractAddress, int quantity, int decimals, string tokenAmount, string tokenAddress, string tokenSymbol, int tokenDecimals, string callData, string platform, string approvedSpenderAddress = null)
        {
            this.name = name;
            this.imageUrl = imageUrl;
            this.network = network;
            this.recipientAddress = recipientAddress;
            this.blockchainNftId = blockchainNftId;
            this.contractAddress = contractAddress;
            this.quantity = quantity;
            this.decimals = decimals;
            this.tokenAmount = tokenAmount;
            this.tokenAddress = tokenAddress;
            this.tokenSymbol = tokenSymbol;
            this.tokenDecimals = tokenDecimals;
            this.callData = callData;
            this.platform = platform;
            this.approvedSpenderAddress = approvedSpenderAddress;
        }
    }
}