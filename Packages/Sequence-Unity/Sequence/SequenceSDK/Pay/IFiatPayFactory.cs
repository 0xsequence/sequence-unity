using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Contracts;
using Sequence.Marketplace;
using Sequence.Pay.Transak;

namespace Sequence.Pay
{
    public interface IFiatPayFactory
    {
        public IFiatPay OnRamp(TransactionOnRampProvider provider = TransactionOnRampProvider.transak);

        public Task<IFiatPay> NftCheckout(CollectibleOrder[] orders);
        public Task<IFiatPay> NftCheckout(Order[] orders);

        public void ConfigureSaleContractId(TransakContractId contractId);
        public Task<IFiatPay> NftCheckout(ERC1155Sale saleContract, Address collection,
            Dictionary<string, BigInteger> amountsByTokenId);

        public Task<IFiatPay> NftCheckout(ERC721Sale saleContract, Address collection, string tokenId, BigInteger amount);
    }
}