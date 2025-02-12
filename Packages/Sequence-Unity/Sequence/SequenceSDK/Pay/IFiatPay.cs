using System.Numerics;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Contracts;
using Sequence.Marketplace;
using Sequence.Pay.Sardine;
using Sequence.Pay.Transak;

namespace Sequence.Pay
{
    public interface IFiatPay
    {
        /// <summary>
        /// Open a web-based flow where the user can onboard funds into cryptocurrencies using their credit card
        /// </summary>
        /// <returns></returns>
        public Task OnRamp();
        
        /// <summary>
        /// Retrieve a link to a web-based flow where the user can onboard funds into cryptocurrencies using their credit card
        /// </summary>
        /// <returns></returns>
        public Task<string> GetOnRampLink();

        /// <summary>
        /// Retrieve a link to a web-based flow where the user can fulfill a CollectibleOrder and purchase ERC1155 or ERC721 tokens
        /// </summary>
        /// <param name="order"></param>
        /// <param name="amount"></param>
        /// <param name="nftType"></param>
        /// <param name="recipient"></param>
        /// <param name="additionalFee"></param>
        /// <param name="marketplaceContractAddress"></param>
        /// <returns></returns>
        public Task<string> GetNftCheckoutLink(CollectibleOrder order, ulong amount, NFTType nftType = NFTType.ERC721,
            Address recipient = null, AdditionalFee additionalFee = null,
            string marketplaceContractAddress = ISardineCheckout.SequenceMarketplaceV2Contract);

        /// <summary>
        /// Configure a TransakContractId for a specific sale contract
        /// Must be called before using the ERC1155/721Sale contract for checkout (with Transak)
        /// </summary>
        /// <param name="contractId"></param>
        public void ConfigureSaleContractId(TransakContractId contractId);
        
        /// <summary>
        /// Retrieve a link to a web-based flow where the user can purchase an ERC1155 token via a primary sales contract
        /// Must call ConfigureSaleContractId with the saleContract's address before using (Transak requirement only)
        /// </summary>
        /// <param name="saleContract"></param>
        /// <param name="collection"></param>
        /// <param name="tokenId"></param>
        /// <param name="amount"></param>
        /// <param name="recipient"></param>
        /// <param name="data"></param>
        /// <param name="proof"></param>
        /// <returns></returns>
        public Task<string> GetNftCheckoutLink(ERC1155Sale saleContract, Address collection,
            BigInteger tokenId, BigInteger amount, Address recipient = null, byte[] data = null,
            FixedByte[] proof = null);

        /// <summary>
        /// Retrieve a link to a web-based flow where the user can purchase an ERC721 token via a primary sales contract
        /// Must call ConfigureSaleContractId with the saleContract's address before using (Transak requirement only)
        /// </summary>
        /// <param name="saleContract"></param>
        /// <param name="collection"></param>
        /// <param name="tokenId"></param>
        /// <param name="amount"></param>
        /// <param name="recipient"></param>
        /// <param name="proof"></param>
        /// <returns></returns>
        public Task<string> GetNftCheckoutLink(ERC721Sale saleContract, Address collection,
            BigInteger tokenId, BigInteger amount, Address recipient = null, byte[] proof = null);
        
        /// <summary>
        /// Retrieve a link to a web-based flow where the user can fulfill CollectibleOrders and purchase ERC1155 or ERC721 tokens
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="quantity"></param>
        /// <param name="nftType"></param>
        /// <param name="marketplaceContractAddress"></param>
        /// <param name="recipient"></param>
        /// <param name="additionalFee"></param>
        /// <returns></returns>
        public Task<string> GetNftCheckoutLink(CollectibleOrder[] orders, BigInteger quantity, NFTType nftType = NFTType.ERC721,
            string marketplaceContractAddress = ISardineCheckout.SequenceMarketplaceV2Contract,
            Address recipient = null, AdditionalFee[] additionalFee = null);
    }
}