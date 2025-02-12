using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
using Sequence.Pay.Sardine;
using Sequence.Pay.Transak;
using Sequence.Provider;

namespace Sequence.Pay
{
    public class SequencePay : IFiatPay
    {
        private Chain _chain;
        private IWallet _wallet;
        private ICheckout _checkout;
        private IFiatPayFactory _factory; // We use the factory pattern to create the correct IFiatPay implementation based on which Pay method is available to the user based on their location, chain, etc. - this makes it easy to add new Fiat Payment providers

        public SequencePay(IWallet wallet, Chain chain, IFiatPayFactory factory = null, IEthClient client = null, ICheckout checkout = null,
            IMarketplaceReader reader = null)
        {
            if (client == null)
            {
                client = new SequenceEthClient(chain);
            }

            if (checkout == null)
            {
                checkout = new Checkout(wallet, chain);
            }
            
            if (reader == null)
            {
                reader = new MarketplaceReader(chain);
            }

            if (factory == null)
            {
                factory = new FiatPayFactory(chain, wallet, client, reader, checkout);
            }
            
            _wallet = wallet;
            _chain = chain;
            _checkout = checkout;
            _factory = factory;
        }
        
        public bool IsOnRampAvailable()
        {
            IFiatPay onRamp = _factory.OnRamp();
            return onRamp is not UnsupportedPay;
        }

        public async Task<bool> NftCheckoutEnabled(CollectibleOrder[] orders)
        {
            IFiatPay nftCheckout = await _factory.NftCheckout(orders);
            return nftCheckout is not UnsupportedPay;
        }

        public async Task<bool> NftCheckoutEnabled(ERC1155Sale saleContract, Address collection,
            Dictionary<string, BigInteger> amountsByTokenId)
        {
            IFiatPay nftCheckout = await _factory.NftCheckout(saleContract, collection, amountsByTokenId);
            return nftCheckout is not UnsupportedPay;
        }
        
        public async Task<bool> NftCheckoutEnabled(ERC721Sale saleContract, Address collection, string tokenId, BigInteger amount)
        {
            IFiatPay nftCheckout = await _factory.NftCheckout(saleContract, collection, tokenId, amount);
            return nftCheckout is not UnsupportedPay;
        }

        public Task OnRamp()
        {
            return _factory.OnRamp().OnRamp();
        }

        public Task<string> GetOnRampLink()
        {
            return _factory.OnRamp().GetOnRampLink();
        }

        public async Task<string> GetNftCheckoutLink(CollectibleOrder order, ulong amount, NFTType nftType = NFTType.ERC721,
            Address recipient = null, AdditionalFee additionalFee = null,
            string marketplaceContractAddress = ISardineCheckout.SequenceMarketplaceV2Contract)
        {
            IFiatPay pay = await _factory.NftCheckout(new[] { order });
            return await pay.GetNftCheckoutLink(order, amount, nftType, recipient, additionalFee, marketplaceContractAddress);
        }

        public void ConfigureSaleContractId(TransakContractId contractId)
        {
            if (contractId.Chain != _chain)
            {
                throw new ArgumentException(
                    $"Provided chain in {nameof(contractId)}, {contractId.Chain}, does not match the chain of this {nameof(SequencePay)} instance, {_chain}");
            }
            _factory.ConfigureSaleContractId(contractId);
        }

        public async Task<string> GetNftCheckoutLink(ERC1155Sale saleContract, Address collection, BigInteger tokenId, BigInteger amount,
            Address recipient = null, byte[] data = null, FixedByte[] proof = null)
        {
            IFiatPay pay = await _factory.NftCheckout(saleContract, collection, new Dictionary<string, BigInteger>()
            {
                { tokenId.ToString(), amount }
            });
            return await pay.GetNftCheckoutLink(saleContract, collection, tokenId, amount, recipient, data, proof);
        }

        public async Task<string> GetNftCheckoutLink(ERC721Sale saleContract, Address collection, BigInteger tokenId, BigInteger amount,
            Address recipient = null, FixedByte[] proof = null)
        {
            IFiatPay pay = await _factory.NftCheckout(saleContract, collection, tokenId.ToString(), amount);
            return await pay.GetNftCheckoutLink(saleContract, collection, tokenId, amount, recipient, proof);
        }

        public async Task<string> GetNftCheckoutLink(CollectibleOrder[] orders, BigInteger quantity, NFTType nftType = NFTType.ERC721,
            string marketplaceContractAddress = ISardineCheckout.SequenceMarketplaceV2Contract, Address recipient = null,
            AdditionalFee[] additionalFee = null)
        {
            IFiatPay pay = await _factory.NftCheckout(orders);
            return await pay.GetNftCheckoutLink(orders, quantity, nftType, marketplaceContractAddress, recipient, additionalFee);
        }
    }
}