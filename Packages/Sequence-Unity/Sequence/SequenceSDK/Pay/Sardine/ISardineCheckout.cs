using System.Numerics;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Contracts;
using Sequence.Marketplace;

namespace Sequence.Pay.Sardine
{
    public interface ISardineCheckout
    {
        public const string SequenceMarketplaceV2Contract = "0xfdb42A198a932C8D3B506Ffa5e855bC4b348a712";
        
        /// <summary>
        /// This async method checks if the given marketplace address is whitelisted with Sardine
        /// If false, please reach out to Sequence for support and next steps
        /// </summary>
        /// <param name="marketplaceAddress"></param>
        /// <returns></returns>
        public Task<bool> CheckSardineWhitelistStatus(Address marketplaceAddress);

        /// <summary>
        /// Retrieve a quote for amount of token that can be bought (or sold) and associated fees
        /// </summary>
        /// <param name="token">the token the user wishes to buy or sell</param>
        /// <param name="amount">the amount (in the quotedCurrency) the user wishes to buy or sell</param>
        /// <param name="paymentType"></param>
        /// <param name="quotedCurrency"></param>
        /// <param name="quoteType"></param>
        /// <returns></returns>
        public Task<SardineQuote> SardineGetQuote(SardineToken token, ulong amount,
            SardinePaymentType paymentType = SardinePaymentType.credit, SardineFiatCurrency quotedCurrency = null,
            SardineQuoteType quoteType = SardineQuoteType.buy);
        
        /// <summary>
        /// Use this method to request a token from Sardine for on-ramping funds into cryptocurrency via credit/debit card
        /// </summary>
        /// <returns></returns>
        public Task<string> SardineGetClientToken();

        /// <summary>
        /// Use the provided client token (obtained from SardineGetClientToken) to open web-based on-ramp flow
        /// </summary>
        /// <param name="clientToken"></param>
        public void OnRamp(string clientToken);

        /// <summary>
        /// Combines SardineGetClientToken and OnRamp into a single async method
        /// Use this method to open web-based on-ramp flow with Sardine
        /// </summary>
        /// <returns></returns>
        public Task OnRampAsync();
        
        /// <summary>
        /// Request a checkout token for the given CollectibleOrder (fetched with the IMarketplaceReader) and quantity
        /// Requires that the marketplace contract has been whitelisted for the given Chain; will throw an exception if not
        /// You can check if a marketplace contract is whitelisted with CheckSardineWhitelistStatus or by catching the exception
        /// If your marketplace contract is not whitelisted, please reach out to Sequence for support and next steps
        /// </summary>
        /// <param name="orders">the collectible orders to fulfill - must all be the same collectible!</param>
        /// <param name="quantity"></param>
        /// <param name="recipient"></param>
        /// <param name="additionalFee"></param>
        /// <param name="marketplaceContractAddress"></param>
        /// <returns></returns>
        public Task<SardineNFTCheckout> SardineGetNFTCheckoutToken(CollectibleOrder[] orders, BigInteger quantity,
            Address recipient = null, AdditionalFee[] additionalFee = null,
            string marketplaceContractAddress = SequenceMarketplaceV2Contract);
        
        /// <summary>
        /// Request a checkout token for the given ERC1155Sale contract, collection, tokenId, and amount
        /// Requires that the ERC1155Sale contract has been whitelisted for the given Chain; will throw an exception if not
        /// You can check if an ERC1155Sale contract is whitelisted with CheckSardineWhitelistStatus or by catching the exception
        /// If your ERC1155Sale contract is not whitelisted, please reach out to Sequence for support and next steps
        /// </summary>
        /// <param name="saleContract"></param>
        /// <param name="collection"></param>
        /// <param name="tokenId"></param>
        /// <param name="amount"></param>
        /// <param name="recipient"></param>
        /// <param name="data"></param>
        /// <param name="proof"></param>
        /// <returns></returns>
        public Task<SardineNFTCheckout> SardineGetNFTCheckoutToken(ERC1155Sale saleContract, Address collection,
            BigInteger tokenId, BigInteger amount, Address recipient = null, byte[] data = null, FixedByte[] proof = null);

        /// <summary>
        /// Request a checkout token for the given ERC721Sale contract, collection, tokenId, and amount
        /// Requires that the ERC721Sale contract has been whitelisted for the given Chain; will throw an exception if not
        /// You can check if an ERC721Sale contract is whitelisted with CheckSardineWhitelistStatus or by catching the exception
        /// If your ERC721Sale contract is not whitelisted, please reach out to Sequence for support and next steps
        /// </summary>
        /// <param name="saleContract"></param>
        /// <param name="collection"></param>
        /// <param name="amount"></param>
        /// <param name="tokenId"></param>
        /// <param name="recipient"></param>
        /// <param name="proof"></param>
        /// <returns></returns>
        public Task<SardineNFTCheckout> SardineGetNFTCheckoutToken(ERC721Sale saleContract, Address collection,
            BigInteger amount, BigInteger tokenId, Address recipient = null, FixedByte[] proof = null);

        /// <summary>
        /// Check on the status of an NFT checkout order
        /// Useful for polling to see if the user has completed, cancelled, or otherwise changed the status of their order, to be represented in your UI
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Task<SardineOrder> SardineGetNFTCheckoutOrderStatus(string orderId);

        /// <summary>
        /// Fetch the Sardine-supported countries and their associated services
        /// </summary>
        /// <returns></returns>
        public Task<SardineRegion[]> SardineGetSupportedRegions();

        /// <summary>
        /// Fetch the Sardine-supported fiat currencies and any associated limitations
        /// </summary>
        /// <returns></returns>
        public Task<SardineFiatCurrency[]> SardineGetSupportedFiatCurrencies();

        /// <summary>
        /// Fetch the Sardine-supported tokens
        /// By default, will filter the results to only show the tokens supported for the given Chain
        /// If you want all tokens for all chains, provide fullList = true
        /// </summary>
        /// <param name="fullList"></param>
        /// <returns></returns>
        public Task<SardineSupportedToken[]> SardineGetSupportedTokens(bool fullList = false);

        /// <summary>
        /// Fetch the Sardine-enabled tokens
        /// By default, will filter the results to only show the tokens enabled for the given Chain
        /// If you want all tokens for all chains, provide fullList = true
        /// </summary>
        /// <param name="fullList"></param>
        /// <returns></returns>
        public Task<SardineEnabledToken[]> SardineGetEnabledTokens(bool fullList = false);

        /// <summary>
        /// Given a SardineNFTCheckout token, generate the URL for the web-based NFT checkout flow
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string CheckoutUrl(SardineNFTCheckout token);

        /// <summary>
        /// Given a SardineNFTCheckout token, open the web-based NFT checkout flow
        /// </summary>
        /// <param name="token"></param>
        public void Checkout(SardineNFTCheckout token);
    }
}