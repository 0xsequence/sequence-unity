using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Unified;

namespace Sequence.Pay.Tests.Transak
{
    public class UnifiedTests
    {
        private const string TokenId = "0";
        
        private static readonly Address TokenAddress = new ("");
        private static readonly Address CurrencyAddress = new ("");
        private static readonly Address SellCurrencyAddress = new ("");
        private static readonly Address RecipientAddress = new ("");

        [Test]
        public async Task UnifiedEndToEndTest()
        {
            var sequenceUnified = new SequenceUnified(Chain.TestnetArbitrumSepolia);
            await sequenceUnified.TryRecoverWalletFromStorage();
            
            var recovered = await sequenceUnified.TryRecoverWalletFromStorage();
            if (!recovered)
                await sequenceUnified.GuestLogin();
            
            await sequenceUnified.GetIdToken();

            var nativeTokenBalance= await sequenceUnified.GetMyNativeTokenBalance();
            var tokenBalance = await sequenceUnified.GetMyTokenBalance(TokenAddress);

            await sequenceUnified.SendToken(TokenAddress, RecipientAddress, TokenId, 1);
            await sequenceUnified.SwapToken(SellCurrencyAddress, CurrencyAddress, 1000);

            await sequenceUnified.CreateListingOnMarketplace(TokenAddress, CurrencyAddress, TokenId, 
                1, 1000, DateTime.MaxValue);
            
            var listings = await sequenceUnified.GetAllListingsFromMarketplace(TokenAddress);
            await sequenceUnified.PurchaseOrderFromMarketplace(listings[0].order, 1);
        }
    }
}