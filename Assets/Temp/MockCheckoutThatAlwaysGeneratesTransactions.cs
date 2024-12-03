using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence;
using Sequence.Marketplace;
using ContractType = Sequence.Marketplace.ContractType;

namespace Temp
{
    public class MockCheckoutThatAlwaysGeneratesTransactions : ICheckout
    {
        public event Action<Sequence.Marketplace.CheckoutOptions> OnCheckoutOptionsReturn;
        public event Action<string> OnCheckoutOptionsError;
        public Task<Sequence.Marketplace.CheckoutOptions> GetCheckoutOptions(CheckoutOptionsMarketplaceOrder[] orders, int additionalFeeBps = 0)
        {
            throw new NotImplementedException();
        }

        public Task<Sequence.Marketplace.CheckoutOptions> GetCheckoutOptions(Order[] orders, int additionalFeeBps = 0)
        {
            throw new NotImplementedException();
        }

        public event Action<Step[]> OnTransactionStepsReturn;
        public event Action<string> OnTransactionStepsError;
        public async Task<Step[]> GenerateBuyTransaction(Order order, BigInteger amount, AdditionalFee additionalFee = null)
        {
            return new Step[]
            {
                new Step(StepType.unknown, "data", "to", "value", null, null)
            };
        }

        public async Task<Step[]> GenerateSellTransaction(Order order, BigInteger amount, AdditionalFee additionalFee = null)
        {
            return new Step[]
            {
                new Step(StepType.unknown, "data", "to", "value", null, null)
            };
        }

        public async Task<Step[]> GenerateListingTransaction(Address collection, string tokenId, BigInteger amount, ContractType contractType,
            Address currencyTokenAddress, BigInteger pricePerToken, DateTime expiry,
            OrderbookKind orderbookKind = OrderbookKind.sequence_marketplace_v2)
        {
            return new Step[]
            {
                new Step(StepType.unknown, "data", "to", "value", null, null)
            };
        }

        public async Task<Step[]> GenerateOfferTransaction(Address collection, string tokenId, BigInteger amount, ContractType contractType,
            Address currencyTokenAddress, BigInteger pricePerToken, DateTime expiry,
            OrderbookKind orderbookKind = OrderbookKind.sequence_marketplace_v2)
        {
            
            return new Step[]
            {
                new Step(StepType.unknown, "data", "to", "value", null, null)
            };
        }

        public async Task<Step[]> GenerateCancelTransaction(Address collection, string orderId,
            MarketplaceKind marketplaceKind = MarketplaceKind.sequence_marketplace_v2)
        {
            
            return new Step[]
            {
                new Step(StepType.unknown, "data", "to", "value", null, null)
            };
        }
    }
}