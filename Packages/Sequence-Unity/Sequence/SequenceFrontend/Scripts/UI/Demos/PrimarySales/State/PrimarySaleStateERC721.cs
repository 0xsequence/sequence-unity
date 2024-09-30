using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.Provider;
using UnityEngine;

namespace Sequence.Demo
{
    public class PrimarySaleStateERC721 : IPrimarySaleState
    {
        public string PaymentToken { get; private set; }
        public string PaymentTokenSymbol { get; private set; }
        public BigInteger PaymentTokenDecimals { get; private set; }
        public BigInteger UserPaymentBalance { get; private set; }
        public BigInteger Cost { get; private set; }
        public BigInteger SupplyCap { get; private set; }
        public int StartTime { get; private set; }
        public int EndTime { get; private set; }
        public int TotalMinted { get; private set; }
        public Dictionary<BigInteger, TokenSupply> TokenSupplies { get; private set; }

        private string _tokenContractAddress;
        private ERC721Sale _saleContract;
        private IEthClient _client;
        private IWallet _wallet;
        private Chain _chain;
        
        public async Task Construct(string saleContractAddress, string tokenContractAddress, 
            IWallet wallet, Chain chain, int[] itemsForSale)
        {
            _tokenContractAddress = tokenContractAddress;
            _saleContract = new ERC721Sale(saleContractAddress);
            _client = new SequenceEthClient(chain);
            _wallet = wallet;
            _chain = chain;

            await UpdateSaleDetailsAsync();
            await UpdatePaymentTokenAsync();
            await UpdateTokenSuppliesAsync();
        }

        public async Task<bool> Purchase(BigInteger tokenId, int amount)
        {
            if (UserPaymentBalance < Cost * amount)
                return false;
            
            var to = _wallet.GetWalletAddress();
            var defaultProof = Array.Empty<byte>();

            var fn = _saleContract.Mint(to, new BigInteger(amount), PaymentToken, new BigInteger(1), defaultProof);
            Assert.IsNotNull(fn);
            
            var transactions = new Transaction[] { new RawTransaction(fn) };
            var result = await _wallet.SendTransaction(_chain, transactions);
            if (result is FailedTransactionReturn failed)
            {
                Debug.Log($"Error purchasing item: {failed.error}");
                return false;
            }

            TotalMinted += amount;
            return true;
        }
        
        private async Task UpdateSaleDetailsAsync()
        {
            var saleDetails = await _saleContract.GetSaleDetailsAsync(_client);
            Debug.Log(saleDetails.PaymentToken);
            PaymentToken = saleDetails.PaymentToken;
            Cost = saleDetails.Cost;
            SupplyCap = saleDetails.SupplyCap;
            StartTime = saleDetails.StartTime;
            EndTime = saleDetails.EndTime;
        }

        private async Task UpdatePaymentTokenAsync()
        {
            var balancesArgs = new GetTokenBalancesArgs(_wallet.GetWalletAddress(), PaymentToken);
            var result = await Indexer.GetTokenBalances((int) _chain, balancesArgs);

            var balances = result?.balances;
            UserPaymentBalance = balances != null && balances.Length > 0 ? balances[0].balance : 0;
            
            var contract = new ERC20(PaymentToken);
            PaymentTokenSymbol = await contract.Symbol(_client);
            PaymentTokenDecimals = await contract.Decimals(_client);
        }
        
        private async Task UpdateTokenSuppliesAsync()
        {
            TokenSupplies = new Dictionary<BigInteger, TokenSupply>();
            var supplyArgs = new GetTokenSuppliesArgs(_tokenContractAddress, true);
            var suppliesReturn = await Indexer.GetTokenSupplies((int) _chain, supplyArgs);

            TotalMinted = 0;
            foreach (var supply in suppliesReturn.tokenIDs)
            {
                TokenSupplies.Add(supply.tokenID, supply);
                TotalMinted += int.TryParse(supply.supply, out var value) ? value : 0;
            }
        }
    }
}