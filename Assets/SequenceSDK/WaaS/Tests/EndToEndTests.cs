using System;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Contracts;
using Sequence.Provider;
using Sequence.Transactions;
using UnityEngine;

namespace Sequence.WaaS.Tests
{
    // Note: these tests use a real testnet. If this test fails, double check the RPC is active and that the sending account has funds
    // https://mumbai.polygonscan.com/address/0x660250734f31644681ae32d05bd7e8e29fea29e1
    public class EndToEndTests
    {
        [Test]
        public async Task TestTransferOnTestnet()
        {
            Wallet.IWallet wallet = await WaaSToWalletAdapter.CreateAsync(new WaaSWallet(
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwYXJ0bmVyX2lkIjoyLCJ3YWxsZXQiOiIweDY2MDI1MDczNGYzMTY0NDY4MWFlMzJkMDViZDdlOGUyOWZlYTI5ZTEifQ.FC8WmaC_hW4svdrs4rxyKcvoekfVYFkFFvGwUOXzcHA"));

            IEthClient client = new SequenceEthClient("https://polygon-mumbai-bor.publicnode.com");
            EthTransaction transaction = await TransferEth.CreateTransaction(client, wallet, "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",  1);

            BigInteger startingBalance = await client.BalanceAt("0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f");
            BigInteger startingBalance2 = await client.BalanceAt(wallet.GetAddress().Value);

            TransactionReceipt receipt = await wallet.SendTransactionAndWaitForReceipt(client, transaction);
            
            BigInteger endingBalance = await client.BalanceAt("0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f");
            BigInteger endingBalance2 = await client.BalanceAt(wallet.GetAddress().Value);
            
            Debug.Log($"starting balance {startingBalance} ending balance {endingBalance}");
            Debug.Log($"starting balance 2 {startingBalance2} ending balance 2 {endingBalance2}");
            Assert.Greater(endingBalance, startingBalance);
            Assert.Less(endingBalance2, startingBalance2);
        }

        [Test]
        public async Task TestBatchTransferOnTestnet()
        {
            Wallet.IWallet wallet = await WaaSToWalletAdapter.CreateAsync(new WaaSWallet(
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwYXJ0bmVyX2lkIjoyLCJ3YWxsZXQiOiIweDY2MDI1MDczNGYzMTY0NDY4MWFlMzJkMDViZDdlOGUyOWZlYTI5ZTEifQ.FC8WmaC_hW4svdrs4rxyKcvoekfVYFkFFvGwUOXzcHA"));

            IEthClient client = new SequenceEthClient("https://polygon-mumbai-bor.publicnode.com");
            EthTransaction transaction = await TransferEth.CreateTransaction(client, wallet, "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",  1);
            EthTransaction transaction2 =
                await TransferEth.CreateTransaction(client, wallet, "0xc683a014955b75F5ECF991d4502427c8fa1Aa249", 1);

            BigInteger startingBalance = await client.BalanceAt("0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f");
            BigInteger startingBalance2 = await client.BalanceAt(wallet.GetAddress().Value);
            BigInteger startingBalance3 = await client.BalanceAt("0xc683a014955b75F5ECF991d4502427c8fa1Aa249");

            EthTransaction[] transactionBatch = new EthTransaction[]
            {
                transaction,
                transaction2
            };

            TransactionReceipt[] receipts = await wallet.SendTransactionBatchAndWaitForReceipts(client, transactionBatch);
            
            BigInteger endingBalance = await client.BalanceAt("0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f");
            BigInteger endingBalance2 = await client.BalanceAt(wallet.GetAddress().Value);
            BigInteger endingBalance3 = await client.BalanceAt("0xc683a014955b75F5ECF991d4502427c8fa1Aa249");
            
            Debug.Log($"starting balance {startingBalance} ending balance {endingBalance}");
            Debug.Log($"starting balance 2 {startingBalance2} ending balance 2 {endingBalance2}");
            Debug.Log($"starting balance 3 {startingBalance3} ending balance 3 {endingBalance3}");
            Assert.Greater(endingBalance, startingBalance);
            Assert.Less(endingBalance2, startingBalance2);
            Assert.Greater(endingBalance3, startingBalance3);
        }

        [Test]
        public async Task TestBatchTransferOnTestnet_emptyBatch()
        {
            Wallet.IWallet wallet = await WaaSToWalletAdapter.CreateAsync(new WaaSWallet(
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwYXJ0bmVyX2lkIjoyLCJ3YWxsZXQiOiIweDY2MDI1MDczNGYzMTY0NDY4MWFlMzJkMDViZDdlOGUyOWZlYTI5ZTEifQ.FC8WmaC_hW4svdrs4rxyKcvoekfVYFkFFvGwUOXzcHA"));
            IEthClient client = new SequenceEthClient("https://polygon-mumbai-bor.publicnode.com");
            EthTransaction[] transactionBatch = new EthTransaction[]{};

            try
            {
                TransactionReceipt[] receipts =
                    await wallet.SendTransactionBatchAndWaitForReceipts(client, transactionBatch);
                Assert.Fail("Expected exception but none was thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Error sending request to https://next-api.sequence.app/rpc/Wallet/SendTransactionBatch: HTTP/1.1 500 Internal Server Error", ex.Message);
            }
        }
        
        [Test]
        public async Task TestContractDeploymentAndInteractions()
        {
            Wallet.IWallet wallet = await WaaSToWalletAdapter.CreateAsync(new WaaSWallet(
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwYXJ0bmVyX2lkIjoyLCJ3YWxsZXQiOiIweDY2MDI1MDczNGYzMTY0NDY4MWFlMzJkMDViZDdlOGUyOWZlYTI5ZTEifQ.FC8WmaC_hW4svdrs4rxyKcvoekfVYFkFFvGwUOXzcHA"));
            IEthClient client = new SequenceEthClient("https://polygon-mumbai-bor.publicnode.com");

            BigInteger amount = 100;
            
            try
            {
                ContractDeploymentResult result = await ContractDeployer.Deploy(client, wallet, ERC20Tests.bytecode);
                TransactionReceipt receipt = result.Receipt;
                string contractAddress = result.PreCalculatedContractAddress;
                
                ERC20 token = new ERC20(contractAddress);

                BigInteger balance = await token.BalanceOf(client, wallet.GetAddress());
                Assert.AreEqual(BigInteger.Zero, balance);

                string owner = await token.Owner(client);
                Assert.AreEqual(wallet.GetAddress().Value, owner);
                
                receipt = await token.Mint(wallet.GetAddress(), amount)
                    .SendTransactionMethodAndWaitForReceipt(wallet, client);

                BigInteger supply = await token.TotalSupply(client);
                Assert.AreEqual(amount, supply);
                BigInteger balance1 = await token.BalanceOf(client, wallet.GetAddress());
                Assert.AreEqual(amount, balance1);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
    }
}