using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Quickstart;

namespace Sequence.Pay.Tests.Transak
{
    public class QuickstartTests
    {
        private const string TokenId = "0";
        
        private static readonly Address TokenAddress = new ("");
        private static readonly Address CurrencyAddress = new ("");
        private static readonly Address SellCurrencyAddress = new ("");
        private static readonly Address RecipientAddress = new ("");

        [Test]
        public async Task QuickstartEndToEndTest()
        {
            var quickstart = new SequenceQuickstart(Chain.TestnetArbitrumSepolia);
            
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
            await quickstart.BuyTokenFromMarketplace(listings[0].order, 1);
        }
    }
}