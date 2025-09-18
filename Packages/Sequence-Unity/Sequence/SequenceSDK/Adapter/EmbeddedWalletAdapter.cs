using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Authentication;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Adapter
{
    public class EmbeddedWalletAdapter : IEmbeddedWalletAdapter, IDisposable
    {
        private static EmbeddedWalletAdapter _instance;
        
        public IWallet Wallet { get; private set; }

        public Address WalletAddress { get; private set; }

        private SequenceLogin _loginHandler;
        private SequenceLogin LoginHandler
        {
            get
            {
                if (_loginHandler != null) 
                    return _loginHandler;
                
                _loginHandler = SequenceLogin.GetInstance();
                _loginHandler.OnLoginSuccess += OnOnLoginSuccess;
                _loginHandler.OnMFAEmailSent += OnOnMFAEmailSent;
                _loginHandler.OnLoginFailed += OnLoginFailed;
                _loginHandler.OnMFAEmailFailedToSend += OnOnMFAEmailFailedToSend;

                return _loginHandler;
            }
        }

        private Chain _chain;
        public Chain Chain
        {
            get => _chain;
            set
            {
                _chain = value;
                _indexer = new ChainIndexer(_chain);
                _swap = new CurrencySwap(_chain);
                _marketplace = new MarketplaceReader(_chain);
            }
        }
        
        private ChainIndexer _indexer;
        private CurrencySwap _swap;
        private MarketplaceReader _marketplace;
        private Checkout _checkout;
        private bool _isLoggingIn;
        private bool _isError;

        public static EmbeddedWalletAdapter GetInstance()
        {
            if (_instance != null) 
                return _instance;
            
            _instance = new EmbeddedWalletAdapter();
            _instance.Chain = Chain.TestnetArbitrumSepolia;

            return _instance;
        }

        public EmbeddedWalletAdapter()
        {
            SequenceWallet.OnWalletCreated += UpdateWallet;
            SequenceWallet.OnAccountFederated += AccountFederated;
            SequenceWallet.OnAccountFederationFailed += AccountFederationFailed;
        }
        
        public void Dispose()
        {
            SequenceWallet.OnWalletCreated -= UpdateWallet;
            SequenceWallet.OnAccountFederated -= AccountFederated;
            SequenceWallet.OnAccountFederationFailed -= AccountFederationFailed;
        }

        public async Task<bool> TryRecoverWalletFromStorage()
        {
            var result = await LoginHandler.TryToRestoreSessionAsync();
            if (!result.StorageEnabled || result.Wallet == null)
                return false;
            
            Wallet = result.Wallet;
            return true;
        }

        public async Task<bool> EmailLogin(string email)
        {
            SetLoginResult(true, false);
            await LoginHandler.Login(email);
            return !_isError;
        }

        public async Task<bool> ConfirmEmailCode(string email, string code)
        {
            SetLoginResult(true, false);
            await LoginHandler.Login(email, code);
            return !_isError;
        }

        public async Task<bool> GuestLogin()
        {
            SetLoginResult(true, false);
            await LoginHandler.GuestLogin();
            return !_isError;
        }
        
        public async Task<bool> GoogleLogin()
        {
            SetLoginResult(true, false);
            LoginHandler.GoogleLogin();
            await WaitForLoginProcess();
            return !_isError;
        }
        
        public async Task<bool> AppleLogin()
        {
            SetLoginResult(true, false);
            LoginHandler.AppleLogin();
            await WaitForLoginProcess();
            return !_isError;
        }

        public async Task<bool> SignOut()
        {
            EnsureWalletReferenceExists();
            var result = await Wallet.DropThisSession();
            if (result)
                LoginHandler.RemoveConnectedWalletAddress();
            
            return result;
        }
        
        public async Task<string> GetIdToken()
        {
            EnsureWalletReferenceExists();
            var response = await Wallet.GetIdToken();
            return response.IdToken;
        }

        public Task<string> SignMessage(string message)
        {
            EnsureWalletReferenceExists();
            return Wallet.SignMessage(_chain, message);
        }
        
        public async Task<BigInteger> GetMyNativeTokenBalance()
        {
            EnsureWalletReferenceExists();
            var result = await _indexer.GetNativeTokenBalance(Wallet.GetWalletAddress());
            return result.balanceWei;
        }

        public async Task<TokenBalance[]> GetMyTokenBalances(string tokenAddress)
        {
            var args = new GetTokenBalancesArgs(Wallet.GetWalletAddress(), tokenAddress, true);
            var result = await _indexer.GetTokenBalances(args);
            return result.balances;
        }

        public async Task<TokenSupply[]> GetTokenSupplies(string tokenAddress)
        {
            var args = new GetTokenSuppliesArgs(tokenAddress, true);
            var result = await _indexer.GetTokenSupplies(args);
            return result.tokenIDs;
        }
        
        public async Task<string> SendToken(string recipientAddress, BigInteger amount, string tokenAddress = null, 
            string tokenId = null, Func<FeeOption[], Task<FeeOption>> selectFee = null)
        {
            Debug.Log($"sending to {recipientAddress} amount: {amount} token: {tokenAddress} tokenId: {tokenId}");
            EnsureWalletReferenceExists();

            Transaction transaction = null;
            if (tokenAddress == null || (tokenAddress.IsAddress() && tokenAddress.IsZeroAddress()))
            {
                transaction = new RawTransaction(recipientAddress, amount.ToString());
            }
            else
            {
                var suppliesArgs = new GetTokenSuppliesArgs(tokenAddress, true);
                var supplies = await _indexer.GetTokenSupplies(suppliesArgs);
                
                transaction = supplies.contractType switch
                {
                    ContractType.ERC20 => new SendERC20(tokenAddress, recipientAddress, amount.ToString()),
                    ContractType.ERC1155 => new SendERC1155(tokenAddress, recipientAddress,
                        new SendERC1155Values[] { new(tokenId, amount.ToString()) }),
                    ContractType.ERC721 => new SendERC721(tokenAddress, recipientAddress, tokenId),
                    _ => throw new Exception("Unknown contract type")
                };
            }

            return await SendTransaction(new[] { transaction }, selectFee);
        }
        
        public async Task<string> SwapToken(string sellToken, string buyToken, BigInteger buyAmount
            , Func<FeeOption[], Task<FeeOption>> selectFee = null)
        {
            var walletAddress = Wallet.GetWalletAddress();
            var quote = await _swap.GetSwapQuote(walletAddress, new Address(buyToken),
                new Address(sellToken), buyAmount.ToString(), true);
            
            return await SendTransaction(new Transaction[]
                { 
                    new RawTransaction(sellToken, string.Empty, quote.approveData),
                    new RawTransaction(quote.to, quote.transactionValue, quote.transactionData),
                }, selectFee
            );
        }

        public async Task<CollectibleOrder[]> GetAllListingsFromMarketplace(string collectionAddress)
        {
            return await _marketplace.ListAllCollectibleListingsWithLowestPricedListingsFirst(collectionAddress);
        }
        
        public async Task<string> CreateListingOnMarketplace(string contractAddress, string currencyAddress, 
            string tokenId, BigInteger amount, BigInteger pricePerToken, DateTime expiry, Func<FeeOption[], Task<FeeOption>> selectFee = null)
        {
            EnsureWalletReferenceExists();
            var steps = await _checkout.GenerateListingTransaction(new Address(contractAddress), tokenId, amount, 
                Marketplace.ContractType.ERC20, new Address(currencyAddress), pricePerToken, expiry);
            
            var transactions = steps.AsTransactionArray();
            return await SendTransaction(transactions, selectFee);
        }
        
        public async Task<string> PurchaseOrderFromMarketplace(Order order, BigInteger amount
            , Func<FeeOption[], Task<FeeOption>> selectFee = null)
        {
            EnsureWalletReferenceExists();
            var steps = await _checkout.GenerateBuyTransaction(order, amount);
            var transactions = steps.AsTransactionArray();
            return await SendTransaction(transactions, selectFee);
        }

        private async Task<string> SendTransaction(Transaction[] transactions, Func<FeeOption[], Task<FeeOption>> selectFee)
        {
            TransactionReturn transactionReturn = null;
            
            if (selectFee != null)
            {
                var feeOptionsResponse = await Wallet.GetFeeOptions(Chain, transactions);
                var feeOptions = feeOptionsResponse.FeeOptions
                    .Where(f => f.InWallet)
                    .Select(f => f.FeeOption)
                    .ToArray();
                
                var feeOption = await selectFee(feeOptions);
                transactionReturn = await Wallet.SendTransactionWithFeeOptions(_chain, transactions, feeOption, feeOptionsResponse.FeeQuote);
            }
            else
            {
                transactionReturn = await Wallet.SendTransaction(_chain, transactions);
            }
            
            return transactionReturn switch
            {
                SuccessfulTransactionReturn success => success.receipt.txnReceipt,
                FailedTransactionReturn failed => throw new Exception($"Failed transaction {failed.error}"),
                _ => throw new Exception("Unknown error while sending transaction.")
            };
        }

        private async Task WaitForLoginProcess()
        {
            while (_isLoggingIn)
                await Task.Yield();
        }
        
        private void OnLoginFailed(string error, LoginMethod method, string email, List<LoginMethod> loginMethods)
        {
            SetLoginResult(false, true);
        }
        
        private void OnOnMFAEmailFailedToSend(string email, string error)
        {
            SetLoginResult(false, true);
            
        }

        private void OnOnMFAEmailSent(string email)
        {
            SetLoginResult(false, false);
        }

        private void OnOnLoginSuccess(string sessionId, string walletAddress)
        {
            SetLoginResult(false, false);
        }
        
        private void AccountFederated(Account obj)
        {
            SetLoginResult(false, false);
        }
        
        private void AccountFederationFailed(string obj)
        {
            SetLoginResult(false, true);
        }
        
        private void SetLoginResult(bool isLoggingIn, bool isError)
        {
            _isError = isError;
            _isLoggingIn = isLoggingIn;
        }

        private void UpdateWallet(IWallet wallet)
        {
            Wallet = wallet;
            WalletAddress = wallet.GetWalletAddress();

            Wallet.OnDropSessionComplete += sessionId =>
            {
                Wallet = null;
            };
            
            _checkout = new Checkout(wallet, _chain);
            LoginHandler.SetConnectedWalletAddress(wallet.GetWalletAddress());
        }
        
        private void EnsureWalletReferenceExists()
        {
            Assert.IsNotNull(Wallet, "Please sign in first. For example, call 'new SequenceQuickstart().GuestLogin();'");
        }
    }
}