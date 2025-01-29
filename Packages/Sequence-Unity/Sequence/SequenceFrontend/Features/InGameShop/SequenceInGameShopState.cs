using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.Provider;
using UnityEngine;
using UnityEngine.Assertions;

namespace Sequence.Demo
{
    public class SequenceInGameShopState
    {
        public Address PaymentToken { get; private set; }
        public string PaymentTokenSymbol { get; private set; }
        public BigInteger PaymentTokenDecimals { get; private set; }
        public BigInteger UserPaymentBalance { get; private set; }
        public BigInteger Cost { get; private set; }
        public BigInteger SupplyCap { get; private set; }
        public int StartTime { get; private set; }
        public int EndTime { get; private set; }
        public int TotalMinted { get; private set; }
        public Dictionary<BigInteger, TokenSupply> TokenSupplies { get; private set; }

        private Address _tokenContractAddress;
        private ERC1155Sale _saleContract;
        private IEthClient _client;
        private IWallet _wallet;
        private Chain _chain;
        private int[] _itemsForSale;

        public async Task Construct(Address saleContractAddress, Address tokenContractAddress, 
            IWallet wallet, Chain chain, int[] itemsForSale)
        {
            _tokenContractAddress = tokenContractAddress;
            _saleContract = new ERC1155Sale(saleContractAddress);
            _client = new SequenceEthClient(chain);
            _wallet = wallet;
            _chain = chain;
            _itemsForSale = itemsForSale;

            await Task.WhenAll(
                UpdateSaleDetailsAsync(),
                UpdatePaymentTokenAsync(),
                UpdateTokenSuppliesAsync());
        }

        public async Task<bool> PurchaseAsync(BigInteger tokenId, int amount)
        {
            if (UserPaymentBalance < Cost * amount)
            {
                Debug.Log($"User is missing {Cost * amount - UserPaymentBalance} {PaymentTokenSymbol} " +
                          $"to purchase tokenId {tokenId}");
                return false;
            }

            var to = _wallet.GetWalletAddress();
            var defaultProof = Array.Empty<byte>();

            var fn = _saleContract.Mint(to, new[] {tokenId},
                new[] {new BigInteger(amount)}, null, PaymentToken, new BigInteger(1), defaultProof);

            Assert.IsNotNull(fn, "Failed to create mint function in ERC1155Sale.cs");

            var transactions = new Transaction[] { new RawTransaction(fn) };
            var result = await _wallet.SendTransaction(_chain, transactions);
            
            if (result is FailedTransactionReturn failed)
            {
                Debug.Log($"Error purchasing item: {failed.error}");
                return false;
            }

            AddAmountToTokenId(tokenId, amount);
            return true;
        }

        private async Task UpdateSaleDetailsAsync()
        {
            var globalSaleDetails = await _saleContract.GetGlobalSaleDetailsAsync(_client);
            Cost = globalSaleDetails.Cost;
            SupplyCap = globalSaleDetails.SupplyCap;
            StartTime = globalSaleDetails.StartTime;
            EndTime = globalSaleDetails.EndTime;
        }

        private async Task UpdatePaymentTokenAsync()
        {
            PaymentToken = await _saleContract.GetPaymentTokenAsync(_client);

            var contract = new ERC20(PaymentToken);
            PaymentTokenSymbol = await contract.Symbol(_client);
            PaymentTokenDecimals = await contract.Decimals(_client);
            
            await UserPaymentTokenBalanceAsync();
        }

        private async Task UserPaymentTokenBalanceAsync()
        {
            var balancesArgs = new GetTokenBalancesArgs(_wallet.GetWalletAddress(), PaymentToken);
            var result = await Indexer.GetTokenBalances(_chain.GetChainId(), balancesArgs);

            var balances = result.balances;
            UserPaymentBalance = balances.Length > 0 ? balances[0].balance : 0;
        }

        private async Task UpdateTokenSuppliesAsync()
        {
            TokenSupplies = new Dictionary<BigInteger, TokenSupply>();
            var supplyArgs = new GetTokenSuppliesArgs(_tokenContractAddress, true);
            var suppliesReturn = await Indexer.GetTokenSupplies(_chain.GetChainId(), supplyArgs);

            TotalMinted = 0;
            foreach (var tokenId in _itemsForSale)
            {
                var supply = Array.Find(suppliesReturn.tokenIDs, t => t.tokenID == tokenId);
                if (supply == null)
                    continue;
                
                TokenSupplies.Add(supply.tokenID, supply);
                TotalMinted += int.TryParse(supply.supply, out var value) ? value : 0;
            }
        }
        
        private void AddAmountToTokenId(BigInteger tokenId, int amount)
        {
            TotalMinted += amount;
            var curSupplyStr = TokenSupplies[tokenId].supply;
            var curSupply = int.TryParse(curSupplyStr, out var value) ? value : 0;
            TokenSupplies[tokenId].supply = (curSupply + amount).ToString();
        }
    }
}