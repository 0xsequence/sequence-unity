using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.Provider;

namespace Sequence.Demo
{
    public class PrimarySaleStateERC1155 : IPrimarySaleState
    {
        public string PaymentToken { get; private set; }
        public string PaymentTokenSymbol { get; private set; }
        public BigInteger UserPaymentBalance { get; private set; }
        public BigInteger Cost { get; private set; }
        public BigInteger SupplyCap { get; private set; }
        public int StartTime { get; private set; }
        public int EndTime { get; private set; }
        public int TotalMinted { get; private set; }
        public Dictionary<BigInteger, TokenSupply> TokenSupplies { get; private set; }

        private string _tokenContractAddress;
        private ERC1155Sale _saleContract;
        private IEthClient _client;
        private IWallet _wallet;
        private Chain _chain;

        public async Task Construct(string saleContractAddress, string tokenContractAddress, IWallet wallet, Chain chain)
        {
            _tokenContractAddress = tokenContractAddress;
            _saleContract = new ERC1155Sale(saleContractAddress);
            _client = new SequenceEthClient(chain);
            _wallet = wallet;
            _chain = chain;

            await Task.WhenAll(
                UpdateSaleDetailsAsync(),
                UpdatePaymentTokenAsync());
        }

        public async Task<bool> Purchase(BigInteger tokenId, int amount)
        {
            if (UserPaymentBalance < Cost * amount)
                return false;
            
            var to = _wallet.GetWalletAddress();
            var defaultProof = Array.Empty<byte>();

            var fn = _saleContract.Mint(to, new[] {tokenId}, 
                new[] {new BigInteger(amount)}, null, PaymentToken, new BigInteger(1), defaultProof);
            
            Assert.IsNotNull(fn);

            var result = await _wallet.SendTransaction(_chain, new Transaction[] {new RawTransaction(fn)});
            return result is SuccessfulTransactionReturn;
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
            var paymentToken = await _saleContract.GetPaymentTokenAsync(_client);
            PaymentToken = paymentToken;

            TokenSupplies = new Dictionary<BigInteger, TokenSupply>();
            var supplyArgs = new GetTokenSuppliesArgs(_tokenContractAddress, true);
            var suppliesReturn = await Indexer.GetTokenSupplies((int)_chain, supplyArgs);

            TotalMinted = 0;
            foreach (var supply in suppliesReturn.tokenIDs)
            {
                TokenSupplies.Add(supply.tokenID, supply);
                TotalMinted += int.TryParse(supply.supply, out var value) ? value : 0;
            }

            var balancesArgs = new GetTokenBalancesArgs(_wallet.GetWalletAddress(), paymentToken);
            var result = await Indexer.GetTokenBalances((int) _chain, balancesArgs);

            var balances = result.balances;
            
            UserPaymentBalance = balances.Length > 0 ? balances[0].balance : 0;
            PaymentTokenSymbol = await new ERC20(paymentToken).Symbol(_client);
        }
    }
}