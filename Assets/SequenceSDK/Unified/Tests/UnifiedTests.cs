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
        public async Task QuickstartEndToEndTest()
        {
            UnifiedWallet quickstart = new UnifiedWallet(Chain.TestnetArbitrumSepolia);
            quickstart.TryRecoverWalletFromStorage();
            
            var recovered = await quickstart.TryRecoverWalletFromStorage();
            if (!recovered)
                await quickstart.GuestLogin();
            
            await quickstart.GetIdToken();

            var nativeTokenBalance= await quickstart.GetMyNativeTokenBalance();
            var tokenBalance = await quickstart.GetMyTokenBalance(TokenAddress);

            await quickstart.SendToken(TokenAddress, RecipientAddress, TokenId, 1);
            await quickstart.SwapToken(SellCurrencyAddress, CurrencyAddress, 1000);

            await quickstart.CreateListingOnMarketplace(TokenAddress, CurrencyAddress, TokenId, 
                1, 1000, DateTime.MaxValue);
            
            var listings = await quickstart.GetAllListingsFromMarketplace(TokenAddress);
            await quickstart.PurchaseOrderFromMarketplace(listings[0].order, 1);
        }
    }
}