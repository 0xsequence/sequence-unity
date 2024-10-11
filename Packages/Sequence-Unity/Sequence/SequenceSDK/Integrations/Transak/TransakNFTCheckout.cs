using System;
using System.Threading.Tasks;
using Sequence.Contracts;
using System.Numerics;
using Newtonsoft.Json;
using Sequence.Marketplace;
using Sequence.Provider;
using Sequence.Transactions;
using Sequence.Utils;
using Sequence.EmbeddedWallet;
using UnityEngine;

namespace Sequence.Integrations.Transak
{
    public class TransakNFTCheckout
    {
        private IWallet _wallet;
        private Chain _chain;
        private IEthClient _client;
        private Checkout _checkout;
        private Address _walletAddress;

        public TransakNFTCheckout(IWallet wallet, Chain chain, IEthClient client = null)
        {
            _wallet = wallet;
            _chain = chain;
            if (client == null)
            {
                client = new SequenceEthClient(chain);
            }

            _client = client;

            _walletAddress = _wallet.GetWalletAddress();

            _checkout = new Checkout(_wallet, chain);
        }

        public static Task<SupportedCountry[]> GetSupportedCountries()
        {
            return TransakOnRamp.GetSupportedCountries();
        }

        /// <summary>
        /// Get a link that, when opened, allows the user to buy the specified token with their credit card using Transak
        ///
        /// The provided Contract, buyFunctionName, and buyFunctionArgs are used to encode the calldata for the buy function that is executed
        /// on the token contract when purchasing the token. For more info, please see the Transak documentation:
        /// https://docs.transak.com/docs/query-params-for-marketplaces
        /// </summary>
        /// <param name="item" the token the user wants to purchase></param>
        /// <param name="contract" the token contract></param>
        /// <param name="buyFunctionName" the name of buy function that needs to be executed on the token contract></param>
        /// <param name="buyFunctionArgs" the arguments needed for the buy function that needs to be executed on the token contract></param>
        /// <returns></returns>
        public async Task<string> GetNFTCheckoutLink(TransakNftData item, Contract contract, string buyFunctionName, params object[] buyFunctionArgs)
        {
            string callData = contract.AssembleCallData(buyFunctionName, buyFunctionArgs);

            return await GetNFTCheckoutLink(item, callData, contract.GetAddress());
        }

        public async Task OpenNFTCheckoutLink(TransakNftData item, Contract contract, string buyFunctionName,
            params object[] buyFunctionArgs)
        {
            string link = await GetNFTCheckoutLink(item, contract, buyFunctionName, buyFunctionArgs);
            Application.OpenURL(link);
        }
        
        internal async Task<string> GetNFTCheckoutLink(TransakNftData item, string callData, Address contractAddress)
        {
            string transakCallData = Uri.EscapeDataString(CompressionUtility.DeflateAndEncodeToBase64(callData));
            
            string baseUrl = "https://global.transak.com";
            string transakContractId = "6675a6d0f597abb8f3e2e9c2";
            
            string transakCryptocurrencyCode = ChainDictionaries.GasCurrencyOf[_chain];
            transakCryptocurrencyCode = "POL"; // Todo remove this line. Currently POL seems to be the only currency code Transak will accept
            
            string itemJson = JsonConvert.SerializeObject(new [] { item });
            string itemJsonBase64 = itemJson.StringToBase64();
            string transakNftDataEncoded = Uri.EscapeDataString(itemJsonBase64);
            
            GasLimitEstimator gasLimitEstimator = new GasLimitEstimator(_client, _walletAddress);
            BigInteger estimatedGasLimit =
                await gasLimitEstimator.EstimateGasLimit(contractAddress, callData, BigInteger.Zero);
            string partnerOrderId = $"{_walletAddress}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

            string transakLink = $"{baseUrl}?apiKey={OnOffRampQueryParameters.apiKey}" +
                                 $"&isNFT=true" +
                                 $"&calldata={transakCallData}" +
                                 $"&contractId={transakContractId}" +
                                 $"&cryptoCurrencyCode={transakCryptocurrencyCode}" +
                                 $"&estimatedGasLimit={estimatedGasLimit}" +
                                 $"&nftData={transakNftDataEncoded}" +
                                 $"&walletAddress={_walletAddress}" +
                                 $"&disableWalletAddressForm=true" +
                                 $"&partnerOrderId={partnerOrderId}";

            return transakLink;
        }
        
        internal async Task OpenNFTCheckoutLink(TransakNftData item, string callData, Address contractAddress)
        {
            string link = await GetNFTCheckoutLink(item, callData, contractAddress);
            Application.OpenURL(link);
        }

        public async Task<string> GetNFTCheckoutLink(Order order, TokenMetadata metadata, uint quantity, NFTType nftType = NFTType.ERC721)
        {
            Step[] steps = await _checkout.GenerateBuyTransaction(order);
            string callData = steps[0].data;

            TransakNftData nftData = new TransakNftData(metadata.image, metadata.name,
                new Address(order.collectionContractAddress),
                new string[] { order.tokenId }, new ulong[] { ulong.Parse(order.priceAmount) }, quantity, nftType);
            
            return await GetNFTCheckoutLink(nftData, callData, new Address(order.collectionContractAddress));
        }
        
        public async Task OpenNFTCheckoutLink(Order order, TokenMetadata metadata, uint quantity, NFTType nftType = NFTType.ERC721)
        {
            string link = await GetNFTCheckoutLink(order, metadata, quantity, nftType);
            Application.OpenURL(link);
        }
    }
}