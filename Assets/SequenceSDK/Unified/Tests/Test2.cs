using System;
using System.Threading.Tasks;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;

namespace Sequence.Pay.Tests.Transak
{
    public class Test2
    {
        private static readonly Address TokenAddress = new ("");
        private static readonly Address CurrencyAddress = new ("");
        private static readonly Address SellCurrencyAddress = new ("");
        private static readonly Address RecipientAddress = new ("");
        
        public async Task SwapToken()
        {
            var loginHandler = SequenceLogin.GetInstance();
            var result = await loginHandler.TryToRestoreSessionAsync();
            var wallet = result.Wallet;
            
            var walletAddress = wallet.GetWalletAddress();

            var swap = new CurrencySwap(Chain.TestnetArbitrumSepolia);
            var quote = await swap.GetSwapQuote(walletAddress, CurrencyAddress, 
                SellCurrencyAddress, "1000", true);

            var transactions = new Transaction[]
            {
                new RawTransaction(SellCurrencyAddress, string.Empty, quote.approveData),
                new RawTransaction(quote.to, quote.transactionValue, quote.transactionData),
            };
            
            var transactionResult = await wallet.SendTransaction(Chain.TestnetArbitrumSepolia, transactions);
            var receipt = transactionResult switch
            {
                SuccessfulTransactionReturn success => success.receipt.txnReceipt,
                FailedTransactionReturn failed => throw new Exception($"Failed transaction {failed.error}"),
                _ => throw new Exception("Unknown error while sending transaction.")
            };
        }
    }
}