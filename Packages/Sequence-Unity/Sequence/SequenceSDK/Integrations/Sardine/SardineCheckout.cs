using System;
using System.Threading.Tasks;
using Sequence.Config;
using Sequence.Utils;

namespace Sequence.Integrations.Sardine
{
    public class SardineCheckout
    {
        private Chain _chain;
        private string _apiKey;
        private HttpClient _client;
        
        private const string _baseUrl = "https://api.sequence.app/rpc/API";

        public SardineCheckout(Chain chain)
        {
            _chain = chain;
            SequenceConfig config = SequenceConfig.GetConfig();
            _apiKey = config.BuilderAPIKey;
            _client = new HttpClient(_apiKey);
        }

        public async Task<bool> CheckSardineWhitelistStatus(Address marketplaceAddress)
        {
            string url = _baseUrl.AppendTrailingSlashIfNeeded() + "GetSardineNFTCheckoutToken";
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

        public async Task<SardineRegion[]> GetSardineSupportedRegions()
        {
            string url = _baseUrl.AppendTrailingSlashIfNeeded() + "GetSardineSupportedRegions";
            try {
                return await _client.SendRequest<SardineRegion[]>(url);
            } catch (Exception e) {
                throw new Exception("Error fetching Sardine supported regions: " + e.Message);
            }
        }

        public async Task<string> GetSardineClientToken()
        {
            string url = _baseUrl.AppendTrailingSlashIfNeeded() + "GetSardineClientToken";
            try {
                SardineTokenResponse response = await _client.SendRequest<SardineTokenResponse>(url);
                return response.token;
            } catch (Exception e) {
                throw new Exception("Error fetching Sardine client token: " + e.Message);
            }
        }
    }
}