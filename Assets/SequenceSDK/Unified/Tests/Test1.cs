using System.Threading.Tasks;
using Sequence.Unified;

namespace Sequence.Pay.Tests.Transak
{
    public class Test1
    {
        private static readonly Address TokenAddress = new ("");
        private static readonly Address CurrencyAddress = new ("");
        private static readonly Address SellCurrencyAddress = new ("");
        private static readonly Address RecipientAddress = new ("");
        
        public async Task SwapToken()
        {
            var wallet = new UnifiedWallet(Chain.TestnetArbitrumSepolia);
            await wallet.TryRecoverWalletFromStorage();
            await wallet.SwapToken(SellCurrencyAddress, CurrencyAddress, 1000);
        }
    }
}