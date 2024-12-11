using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Config;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
using Sequence.Provider;
using Sequence.Utils;

namespace Sequence.Integrations.Sardine
{
    public class SardineCheckout
    {
        private Chain _chain;
        private string _apiKey;
        private HttpClient _client;
        private IWallet _wallet;
        private Checkout _checkout;
        
        private const string _baseUrl = "https://dev-api.sequence.app/rpc/API";

        public SardineCheckout(Chain chain, IWallet wallet)
        {
            _chain = chain;
            SequenceConfig config = SequenceConfig.GetConfig();
            _apiKey = config.BuilderAPIKey;
            _apiKey = "AQAAAAAAAAOciu6BP4WM_6ftwlZFRT5pays";
            _client = new HttpClient(_apiKey);
            _wallet = wallet;
            _checkout = new Checkout(_wallet, _chain);
        }

        public async Task<bool> CheckSardineWhitelistStatus(Address marketplaceAddress)
        {
            string url = _baseUrl.AppendTrailingSlashIfNeeded() + "SardineGetNFTCheckoutToken";
            string referenceId = "sequence-unity-sardine-whitelist-check";
            string name = "whitelist-check";
            string imageUrl = "https://www.sequence.market/images/placeholder.png";
            string platfrom = "calldata_execution";
            string executionType = "smart_contract";
            string blockchainNftId = "42";
            uint quantity = 1;
            uint decimals = 0;
            string tokenAmount = "1000000";
            Address tokenAddress = new Address("0xa0b86991c6218b36c1d19d4a2e9eb0ce3606eb48");
            string tokenSymbol = "USDC";
            uint tokenDecimals = 6;
            string callData = "0x1";
            GetSardineNFTCheckoutTokenRequest request = new GetSardineNFTCheckoutTokenRequest(
                new PaymentMethodTypeConfig(EnumExtensions.GetEnumValuesAsList<PaymentMethod>().ToArray(),
                    PaymentMethod.us_debit),
                imageUrl, _chain, Address.ZeroAddress, marketplaceAddress, blockchainNftId, quantity, decimals,
                tokenAmount,
                tokenAddress, tokenSymbol, tokenDecimals, callData, name, platfrom, executionType);
            
            try {
                await _client.SendRequest<GetSardineNFTCheckoutTokenRequest, SardineNFTCheckout>(url, request);
                return true;
            } catch (Exception e) {
                if (e.Message.Contains("It must me allow listed") || e.Message.Contains("It must be allow listed"))
                {
                    return false;
                }
                throw new Exception("Error fetching Sardine whitelist status: " + e.Message);
            }
        }

        public async Task<SardineRegion[]> SardineGetSupportedRegions()
        {
            string url = _baseUrl.AppendTrailingSlashIfNeeded() + "SardineGetSupportedRegions";
            try {
                SardineSupportedRegionsResponse response = await _client.SendRequest<SardineSupportedRegionsResponse>(url);
                return response.regions;
            } catch (Exception e) {
                throw new Exception("Error fetching Sardine supported regions: " + e.Message);
            }
        }

        public async Task<string> SardineGetClientToken()
        {
            string url = _baseUrl.AppendTrailingSlashIfNeeded() + "SardineGetClientToken";
            try {
                SardineTokenResponse response = await _client.SendRequest<SardineTokenResponse>(url);
                return response.token;
            } catch (Exception e) {
                throw new Exception("Error fetching Sardine client token: " + e.Message);
            }
        }

        private async Task<SardineNFTCheckout> SardineGetNFTCheckoutToken(CollectibleOrder order, Address recipient, BigInteger quantity, string callData)
        {
            string url = _baseUrl.AppendTrailingSlashIfNeeded() + "SardineGetNFTCheckoutToken";
            string priceSymbol = ChainDictionaries.GasCurrencyOf[_chain];
            string currencyAddress = order.order.priceCurrencyAddress;
            if (!currencyAddress.IsZeroAddress())
            {
                priceSymbol = await new ERC20(order.order.priceCurrencyAddress).Symbol(new SequenceEthClient(_chain));
            }
            GetSardineNFTCheckoutTokenRequest request = new GetSardineNFTCheckoutTokenRequest(
                new PaymentMethodTypeConfig(EnumExtensions.GetEnumValuesAsList<PaymentMethod>().ToArray(),
                    PaymentMethod.us_debit),
                order.metadata.image, _chain, recipient, new Address(order.order.collectionContractAddress), order.order.tokenId, quantity, order.order.quantityDecimals,
                order.order.priceAmount,
                new Address(order.order.priceCurrencyAddress), priceSymbol, order.order.priceDecimals, callData, order.metadata.name, order.order.marketplace.ToString());
            try {
                return await _client.SendRequest<GetSardineNFTCheckoutTokenRequest, SardineNFTCheckout>(url, request);
            } catch (Exception e) {
                throw new Exception("Error fetching Sardine NFT checkout token: " + e.Message);
            }
        }

        public async Task<SardineNFTCheckout> SardineGetNFTCheckoutToken(CollectibleOrder order, BigInteger quantity, Address recipient = null, AdditionalFee additionalFee = null)
        {
            if (recipient == null)
            {
                recipient = _wallet.GetWalletAddress();
            }
            
            Step[] steps = await _checkout.GenerateBuyTransaction(order.order, quantity, additionalFee);
            string callData = steps[0].data;
            
            return await SardineGetNFTCheckoutToken(order, recipient, quantity, callData);
        }
    }
}