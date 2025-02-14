using System;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence;
using Sequence.ABI;
using Sequence.Contracts;
using Sequence.Ethereum.Tests;
using Sequence.Provider;
using Sequence.Wallet;

namespace Sequence.Ethereum
{
    public class ERC1155SaleTests
    {
        private ERC1155Sale _sale = new ERC1155Sale("0xf0056139095224f4eec53c578ab4de1e227b9597");
        private IEthClient _client = new SequenceEthClient(Chain.Polygon);
        
        [Test]
        public async Task TestGetSaleToken()
        {
            Address saleCurrency = await _sale.GetPaymentTokenAsync(_client);
            
            Assert.NotNull(saleCurrency);
        }

        [Test]
        public async Task TestMerkleProof()
        {
            bool valid = await _sale.CheckMerkleProofAsync(_client, new FixedByte(32, ""), new FixedByte[]{}, _sale.Contract.GetAddress(), new FixedByte(32, ""));
            
            Assert.IsFalse(valid);
        }

        [Test]
        public async Task TestGetGlobalSaleDetails()
        {
            ERC1155Sale.SaleDetails details = await _sale.GetGlobalSaleDetailsAsync(_client);
            
            Assert.NotNull(details);
        }
        
        [Test]
        public async Task TestGetTokenSaleDetails()
        {
            ERC1155Sale.SaleDetails details = await _sale.TokenSaleDetailsAsync(_client, 1);
            
            Assert.NotNull(details);
            Assert.Greater(details.StartTimeLong, 0);
            Assert.Greater(details.EndTimeLong, 0);
            Assert.Greater(details.SupplyCap, BigInteger.Zero);
            Assert.Greater(details.Cost, BigInteger.Zero);
        }
    }
}