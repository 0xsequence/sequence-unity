using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
using Sequence.Pay.Sardine;
using Sequence.Pay.Transak;
using Sequence.Provider;
using Sequence.Utils;

namespace Sequence.Pay
{
    internal class FiatPayFactory : IFiatPayFactory
    {
        private Chain _chain;
        private IWallet _wallet;
        private IEthClient _client;
        private IMarketplaceReader _reader;
        private ICheckout _checkout;
        private SardineFiatPay _sardine;
        private TransakFiatPay _transak;
        private UnsupportedPay _unsupported;

        public FiatPayFactory(Chain chain, IWallet wallet, IEthClient client, IMarketplaceReader reader, ICheckout checkout)
        {
            _chain = chain;
            _wallet = wallet;
            _client = client;
            _reader = reader;
            _checkout = checkout;
            _sardine = new SardineFiatPay(new SardineCheckout(chain, wallet, checkout, reader, client));
        }

        public IFiatPay OnRamp(TransactionOnRampProvider provider = TransactionOnRampProvider.transak)
        {
            switch (provider)
            {
                case TransactionOnRampProvider.sardine:
                    return _sardine;
                case TransactionOnRampProvider.transak:
                    return _transak;
                default:
                    return _unsupported;
            }
        }

        public Task<IFiatPay> NftCheckout(CollectibleOrder[] orders)
        {
            Order[] orderArray = orders.ToOrderArray();
            return NftCheckout(orderArray);
        }

        public async Task<IFiatPay> NftCheckout(Order[] orders)
        {
            Marketplace.CheckoutOptions options = await _checkout.GetCheckoutOptions(orders);
            return NftCheckout(options);
        }

        public void ConfigureSaleContractId(TransakContractId contractId)
        {
            _transak.ConfigureSaleContractId(contractId);
        }

        private IFiatPay NftCheckout(Marketplace.CheckoutOptions checkoutOptions)
        {
            var options = checkoutOptions.nftCheckout;
            if (options.Contains(TransactionNFTCheckoutProvider.sardine))
            {
                return _sardine;
            }
            else if (options.Contains(TransactionNFTCheckoutProvider.transak))
            {
                return _transak;
            }
            else
            {
                return _unsupported;
            }
        }

        public async Task<IFiatPay> NftCheckout(ERC1155Sale saleContract, Address collection, Dictionary<string, BigInteger> amountsByTokenId)
        {
            Marketplace.CheckoutOptions options =
                await _checkout.GetCheckoutOptions(saleContract, collection, amountsByTokenId);
            return NftCheckout(options);
        }

        public async Task<IFiatPay> NftCheckout(ERC721Sale saleContract, Address collection, string tokenId, BigInteger amount)
        {
            Marketplace.CheckoutOptions options =
                await _checkout.GetCheckoutOptions(saleContract, collection, tokenId, amount);
            return NftCheckout(options);
        }
    }
}