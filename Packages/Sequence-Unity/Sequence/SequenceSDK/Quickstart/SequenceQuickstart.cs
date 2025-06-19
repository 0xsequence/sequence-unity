using System;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;

namespace Sequence.Quickstart
{
    public class SequenceQuickstart
    {
        private IWallet _wallet;
        public IWallet Wallet
        {
            get
            {
                Assert.IsNotNull(Wallet, "Please sign in first. For example, call 'new SequenceQuickstart().GuestLogin();'");
                return _wallet;
            }
            set => _wallet = value;
        }

        private SequenceLogin _loginHandler;
        public SequenceLogin LoginHandler
        {
            get
            {
                _loginHandler ??= SequenceLogin.GetInstance();
                return _loginHandler;
            }
        }

        private readonly Chain _chain;
        private readonly ChainIndexer _indexer;
        private readonly CurrencySwap _swap;
        private readonly MarketplaceReader _marketplace;
        private Checkout _checkout;

        public SequenceQuickstart(Chain chain)
        {
            _chain = chain;
            _indexer = new ChainIndexer(_chain);
            _swap = new CurrencySwap(_chain);
            _marketplace = new MarketplaceReader(_chain);

            SequenceWallet.OnWalletCreated += wallet =>
            {
                Wallet = wallet;
                _checkout = new Checkout(wallet, _chain);
            };
        }
        
        // ONBOARD

        public async Task<bool> TryRecoverWalletFromStorage()
        {
            var result = await _loginHandler.TryToRestoreSessionAsync();
            if (!result.StorageEnabled || result.Wallet == null)
                return false;
            
            Wallet = result.Wallet;
            return true;
        }

        public async Task EmailLogin(string email)
        {
            await _loginHandler.Login(email);
        }

        public async Task ConfirmEmailCode(string email, string code)
        {
            await _loginHandler.Login(email, code);
        }

        public async Task GuestLogin()
        {
            await LoginHandler.GuestLogin();
        }

        public async Task<string> GetIdToken()
        {
            var response = await Wallet.GetIdToken();
            return response.IdToken;
        }
        
        // POWER
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        public async Task<BigInteger> GetMyNativeTokenBalance()
        {
            var result = await _indexer.GetNativeTokenBalance(Wallet.GetWalletAddress());
            return result.balanceWei;
        }

        public async Task<(BigInteger Balance, TokenMetadata TokenMetadata)> GetMyTokenBalance(Address tokenAddress)
        {
            var args = new GetTokenBalancesArgs(Wallet.GetWalletAddress(), tokenAddress, true);
            var result = await _indexer.GetTokenBalances(args);
            var balance = result.balances[0];
            return (balance.balance, balance.tokenMetadata);
        }

        public async Task<TokenSupply[]> GetTokenSupplies(Address tokenAddress)
        {
            var args = new GetTokenSuppliesArgs(tokenAddress, true);
            var result = await _indexer.GetTokenSupplies(args);
            return result.tokenIDs;
        }
        
        /// <summary>
        /// Send any ERC20, ERC1155 or ERC721 token to another wallet.
        /// </summary>
        /// <param name="recipientAddress">The address of the wallet you want to send the token to.</param>
        /// <param name="tokenAddress">The address of your token. For example one you have deployed through https://sequence.build/</param>
        /// <param name="tokenId">Leave it blank for ERC20 contracts.</param>
        /// <param name="amount">Leave it blank for ERC721 contracts.</param>
        /// <returns>Transaction receipt used to check transaction information onchain.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> SendToken(Address recipientAddress, Address tokenAddress, string tokenId, BigInteger amount)
        {
            var supplies = await _indexer.GetTokenSupplies(new GetTokenSuppliesArgs(tokenAddress, true));

            Transaction transaction = supplies.contractType switch
            {
                ContractType.ERC20 => new SendERC20(tokenAddress, recipientAddress, amount.ToString()),
                ContractType.ERC1155 => new SendERC1155(tokenAddress, recipientAddress,
                    new SendERC1155Values[] { new(tokenId, amount.ToString()) }),
                ContractType.ERC721 => new SendERC721(tokenAddress, recipientAddress, tokenId),
                _ => throw new Exception("Unknown contract type")
            };

            return await SendTransaction(new[] { transaction });
        }
        
        // MONETIZATION

        public async Task SwapToken(Address sellToken, Address buyToken, BigInteger buyAmount)
        {
            var walletAddress = Wallet.GetWalletAddress();
            var quote = await _swap.GetSwapQuote(walletAddress, buyToken, 
                sellToken, buyAmount.ToString(), true);
            
            await SendTransaction(new Transaction[]
                { 
                    new RawTransaction(sellToken, string.Empty, quote.approveData),
                    new RawTransaction(quote.to, quote.transactionValue, quote.transactionData),
                }
            );
        }
        
        public async Task<CollectibleOrder[]> GetAllListingsFromMarketplace(Address contractAddress)
        {
            return await _marketplace.ListAllCollectibleListingsWithLowestPricedListingsFirst(contractAddress);
        }
        
        public async Task<string> CreateListingOnMarketplace(Address contractAddress, Address currencyAddress, 
            string tokenId, BigInteger amount, BigInteger pricePerToken, DateTime expiry)
        {
            var steps = await _checkout.GenerateListingTransaction(contractAddress, tokenId, amount, 
                Marketplace.ContractType.ERC20, currencyAddress, pricePerToken, expiry);
            
            var transactions = steps.AsTransactionArray();
            return await SendTransaction(transactions);
        }
        
        public async Task<string> BuyTokenFromMarketplace(Order order, BigInteger amount)
        {
            var steps = await _checkout.GenerateBuyTransaction(order, amount);
            var transactions = steps.AsTransactionArray();
            return await SendTransaction(transactions);
        }

        private async Task<string> SendTransaction(Transaction[] transactions)
        {
            var transactionResult = await Wallet.SendTransaction(_chain, transactions);
            return transactionResult switch
            {
                SuccessfulTransactionReturn success => success.receipt.txnReceipt,
                FailedTransactionReturn failed => throw new Exception($"Failed transaction {failed.error}"),
                _ => throw new Exception("Unknown error while sending transaction.")
            };
        }
    }
}