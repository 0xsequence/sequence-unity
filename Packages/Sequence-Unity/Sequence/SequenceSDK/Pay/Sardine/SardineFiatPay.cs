using System.Numerics;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Contracts;
using Sequence.Marketplace;
using Sequence.Pay.Transak;

namespace Sequence.Pay.Sardine
{
    internal class SardineFiatPay : IFiatPay
    {
        private ISardineCheckout _checkout;
        
        public SardineFiatPay(ISardineCheckout checkout)
        {
            _checkout = checkout;
        }
        
        public Task OnRamp()
        {
            return _checkout.OnRampAsync();
        }

        public Task<string> GetOnRampLink()
        {
            return _checkout.SardineGetClientToken();
        }

        public async Task<string> GetNftCheckoutLink(CollectibleOrder order, ulong amount, NFTType nftType = NFTType.ERC721,
            Address recipient = null, AdditionalFee additionalFee = null,
            string marketplaceContractAddress = ISardineCheckout.SequenceMarketplaceV2Contract)
        {
            AdditionalFee[] additionalFees = new[] { additionalFee };
            if (additionalFee == null)
            {
                additionalFees = null;
            }
            SardineNFTCheckout nftCheckout = await _checkout.SardineGetNFTCheckoutToken(new [] { order }, amount, recipient, additionalFees, marketplaceContractAddress);
            return _checkout.CheckoutUrl(nftCheckout);
        }

        public void ConfigureSaleContractId(TransakContractId contractId)
        {
            throw new System.NotSupportedException();
        }

        public async Task<string> GetNftCheckoutLink(ERC1155Sale saleContract, Address collection, BigInteger tokenId, BigInteger amount,
            Address recipient = null, byte[] data = null, FixedByte[] proof = null)
        {
            SardineNFTCheckout nftCheckout = await _checkout.SardineGetNFTCheckoutToken(saleContract, collection,
                tokenId, amount, recipient, data, proof);
            return _checkout.CheckoutUrl(nftCheckout);
        }

        public async Task<string> GetNftCheckoutLink(ERC721Sale saleContract, Address collection, BigInteger tokenId, BigInteger amount,
            Address recipient = null, FixedByte[] proof = null)
        {
            SardineNFTCheckout nftCheckout =
                await _checkout.SardineGetNFTCheckoutToken(saleContract, collection, tokenId, amount, recipient, proof);
            return _checkout.CheckoutUrl(nftCheckout);
        }

        public async Task<string> GetNftCheckoutLink(CollectibleOrder[] orders, BigInteger quantity, NFTType nftType = NFTType.ERC721,
            string marketplaceContractAddress = ISardineCheckout.SequenceMarketplaceV2Contract, Address recipient = null,
            AdditionalFee[] additionalFee = null)
        {
            SardineNFTCheckout nftCheckout = await _checkout.SardineGetNFTCheckoutToken(orders, quantity, recipient,
                additionalFee, marketplaceContractAddress);
            return _checkout.CheckoutUrl(nftCheckout);
        }
    }
}