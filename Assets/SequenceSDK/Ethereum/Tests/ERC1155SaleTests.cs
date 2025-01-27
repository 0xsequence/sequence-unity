using System;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence;
using Sequence.Contracts;
using Sequence.Ethereum.Tests;
using Sequence.Provider;
using Sequence.Wallet;

namespace Sequence.Ethereum
{
    public class ERC1155SaleTests
    {
        // private EOAWallet _wallet1 = new EOAWallet("0xabc0000000000000000000000000000000000000000000000000000000000001");
        // private EOAWallet _wallet2 = new EOAWallet("0xabc0000000000000000000000000000000000000000000000000000000000002");
        // private SequenceEthClient _client = new SequenceEthClient("http://localhost:8545/");
        // private string _collectionAddress;
        // private string _saleContractAddress;
        //
        // [OneTimeSetUp]
        // public async Task DeployContracts()
        // {
        //     try
        //     {
        //         ContractDeploymentResult result = await ContractDeployer.Deploy(_client, _wallet1, ERC1155Tests.bytecode);
        //         TransactionReceipt receipt = result.Receipt;
        //         _collectionAddress = receipt.contractAddress;
        //         Assert.IsNotEmpty(_collectionAddress);
        //         Assert.AreEqual(_collectionAddress, result.DeployedContractAddress.Value);
        //     }
        //     catch (Exception ex)
        //     {
        //         Assert.Fail("Expected no exception when deploying ERC155 contract, but got: " + ex.Message);
        //     }
        //     
        //     try
        //     {
        //         ContractDeploymentResult result = await ContractDeployer.Deploy(_client, _wallet1, bytecode);
        //         TransactionReceipt receipt = result.Receipt;
        //         _saleContractAddress = receipt.contractAddress;
        //         Assert.IsNotEmpty(_saleContractAddress);
        //         Assert.AreEqual(_saleContractAddress, result.DeployedContractAddress.Value);
        //     }
        //     catch (Exception ex)
        //     {
        //         Assert.Fail("Expected no exception when deploying ERC155 Sale contract, but got: " + ex.Message);
        //     }
        // }

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
            BigInteger valid = await _sale.CheckMerkleProofAsync(_client, new byte(), new byte[]{}, _sale.Contract.GetAddress(), new byte());
            
            Assert.AreEqual(0, valid);
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