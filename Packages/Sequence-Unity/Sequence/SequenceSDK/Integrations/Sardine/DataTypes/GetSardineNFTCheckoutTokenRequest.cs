using System;
using System.Numerics;
using Newtonsoft.Json;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    public class GetSardineNFTCheckoutTokenRequest
    {
        public string referenceId;
        public uint expiresIn;
        public PaymentMethodTypeConfig paymentMethodTypeConfig;
        public string name;
        public string imageUrl;
        public string network;
        public Address recipientAddress;
        public Address contractAddress;
        public string platform;
        public string executionType;
        public string blockchainNftId;
        public BigInteger quantity;
        public BigInteger decimals;
        public string tokenAmount;
        public Address tokenAddress;
        public string tokenSymbol;
        public BigInteger tokenDecimals;
        public string callData;

        public GetSardineNFTCheckoutTokenRequest(PaymentMethodTypeConfig paymentMethodTypeConfig, string imageUrl, Chain network, Address recipientAddress, Address contractAddress, string blockchainNftId, BigInteger quantity, BigInteger decimals, string tokenAmount, Address tokenAddress, string tokenSymbol, BigInteger tokenDecimals, string callData, string name = "whitelist-check", string platform = "calldata_execution", string executionType = "smart_contract", uint expiresIn = 3600)
        {
            this.paymentMethodTypeConfig = paymentMethodTypeConfig;
            this.name = name;
            this.imageUrl = imageUrl;
            this.network = ChainDictionaries.PathOf[network];
            this.recipientAddress = recipientAddress;
            this.contractAddress = contractAddress;
            this.blockchainNftId = blockchainNftId;
            this.quantity = quantity;
            this.decimals = decimals;
            this.tokenAmount = tokenAmount;
            this.tokenAddress = tokenAddress;
            this.tokenSymbol = tokenSymbol;
            this.tokenDecimals = tokenDecimals;
            this.callData = callData;
            this.platform = platform;
            this.executionType = executionType;
            this.expiresIn = expiresIn;
        }

        [JsonConstructor]
        public GetSardineNFTCheckoutTokenRequest(string referenceId, uint expiresIn, PaymentMethodTypeConfig paymentMethodTypeConfig, string name, string imageUrl, string network, string recipientAddress, string contractAddress, string platform, string executionType, string blockchainNftId, BigInteger quantity, BigInteger decimals, string tokenAmount, string tokenAddress, string tokenSymbol, BigInteger tokenDecimals, string callData)
        {
            this.referenceId = referenceId;
            this.expiresIn = expiresIn;
            this.paymentMethodTypeConfig = paymentMethodTypeConfig;
            this.name = name;
            this.imageUrl = imageUrl;
            this.network = network;
            this.recipientAddress = new Address(recipientAddress);
            this.contractAddress = new Address(contractAddress);
            this.platform = platform;
            this.executionType = executionType;
            this.blockchainNftId = blockchainNftId;
            this.quantity = quantity;
            this.decimals = decimals;
            this.tokenAmount = tokenAmount;
            this.tokenAddress = new Address(tokenAddress);
            this.tokenSymbol = tokenSymbol;
            this.tokenDecimals = tokenDecimals;
            this.callData = callData;
        }
    }
}