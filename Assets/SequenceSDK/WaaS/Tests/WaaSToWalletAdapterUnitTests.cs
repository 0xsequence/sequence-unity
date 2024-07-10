using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Mocks;
using Sequence.Provider;
using Sequence.Transactions;

namespace Sequence.WaaS.Tests
{
    public class WaaSToWalletAdapterUnitTests
    {
        private IEthClient _client =
            new SequenceEthClient("https://nodes.sequence.app/polygon/YfeuczOMRyP7fpr1v7h8SvrCAAAAAAAAA");
        private Address address = new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249");
        
        [Test]
        public async Task TestSendTransactionFailed()
        {
            IIntentSender intentSender = new MockIntentSender(new FailedTransactionReturn("something happened",null,null));
            WaaSToWalletAdapter adapter = new WaaSToWalletAdapter(new WaaSWallet(address, "", intentSender));

            try
            {
                string transactionHash = await adapter.SendTransaction(_client,
                    new EthTransaction(0, 1, 1, address, 0, "", Chain.Polygon.AsHexString()));
                Assert.Fail("Expected exception, but none was thrown");
            }
            catch (System.Exception e)
            {
                Assert.AreEqual("something happened", e.Message);
            }
        }
        
        [Test]
        public async Task TestSendTransactionException()
        {
            IIntentSender intentSender = new MockIntentSender(new Exception("some bad stuff happened"));
            WaaSToWalletAdapter adapter = new WaaSToWalletAdapter(new WaaSWallet(address, "", intentSender));

            try
            {
                string transactionHash = await adapter.SendTransaction(_client,
                    new EthTransaction(0, 1, 1, address, 0, "", Chain.Polygon.AsHexString()));
                Assert.Fail("Expected exception, but none was thrown");
            }
            catch (System.Exception e)
            {
                Assert.AreEqual("some bad stuff happened", e.Message);
            }
        }
        
        [Test]
        public async Task TestSendTransactionSuccess()
        {
            IIntentSender intentSender = new MockIntentSender(new SuccessfulTransactionReturn("a cool hash","",null,null));
            WaaSToWalletAdapter adapter = new WaaSToWalletAdapter(new WaaSWallet(address, "", intentSender));

            string transactionHash = await adapter.SendTransaction(_client,
                new EthTransaction(0, 1, 1, address, 0, "", Chain.Polygon.AsHexString()));
            Assert.AreEqual("a cool hash", transactionHash);
        }
        
        [Test]
        public async Task TestSendTransactionBatchFailed()
        {
            IIntentSender intentSender = new MockIntentSender(new FailedTransactionReturn("something happened",null,null));
            WaaSToWalletAdapter adapter = new WaaSToWalletAdapter(new WaaSWallet(address, "", intentSender));

            try
            {
                string[] transactionHash = await adapter.SendTransactionBatch(_client,
                    new EthTransaction[]
                    {
                        new EthTransaction(0, 1, 1, address, 0, "", Chain.Polygon.AsHexString()),
                        new EthTransaction(0, 1, 1, address, 0, "", Chain.Polygon.AsHexString())
                    });
                Assert.Fail("Expected exception, but none was thrown");
            }
            catch (System.Exception e)
            {
                Assert.AreEqual("something happened", e.Message);
            }
        }
        
        [Test]
        public async Task TestSendTransactionBatchSuccess()
        {
            IIntentSender intentSender = new MockIntentSender(new SuccessfulTransactionReturn("a cool hash","",null,null));
            WaaSToWalletAdapter adapter = new WaaSToWalletAdapter(new WaaSWallet(address, "", intentSender));

            
            string[] transactionHash = await adapter.SendTransactionBatch(_client,
                new EthTransaction[]
                {
                    new EthTransaction(0, 1, 1, address, 0, "", Chain.Polygon.AsHexString()),
                    new EthTransaction(0, 1, 1, address, 0, "", Chain.Polygon.AsHexString())
                });
            Assert.AreEqual(1, transactionHash.Length);
            Assert.AreEqual("a cool hash", transactionHash[0]);
        }
        
        [Test]
        public async Task TestSendTransactionBatchEmptyBatch()
        {
            IIntentSender intentSender = new MockIntentSender(new FailedTransactionReturn("something happened",null,null));
            WaaSToWalletAdapter adapter = new WaaSToWalletAdapter(new WaaSWallet(address, "", intentSender));

            try
            {
                string[] transactionHash = await adapter.SendTransactionBatch(_client,
                    new EthTransaction[]{});
                Assert.Fail("Expected exception, but none was thrown");
            }
            catch (System.Exception e)
            {
                Assert.AreEqual("Cannot send empty transaction batch", e.Message);
            }
        }

        [Test]
        public async Task TestDeployContractFailed()
        {
            IIntentSender intentSender = new MockIntentSender(new FailedTransactionReturn("something happened",null,null));
            WaaSToWalletAdapter adapter = new WaaSToWalletAdapter(new WaaSWallet(address, "", intentSender));

            try
            {
                TransactionReceipt receipt = await adapter.DeployContract(_client, "");
                Assert.Fail("Expected exception, but none was thrown");
            }
            catch (System.Exception e)
            {
                Assert.AreEqual("Failed to deploy contract: something happened", e.Message);
            }
        }

        [Test]
        public async Task TestDeployContractTransactionException()
        {
            IIntentSender intentSender = new MockIntentSender(new Exception("something happened"));
            WaaSToWalletAdapter adapter = new WaaSToWalletAdapter(new WaaSWallet(address, "", intentSender));

            try
            {
                TransactionReceipt receipt = await adapter.DeployContract(_client, "");
                Assert.Fail("Expected exception, but none was thrown");
            }
            catch (System.Exception e)
            {
                Assert.AreEqual("Failed to deploy contract: something happened", e.Message);
            }
        }

        [Test]
        public async Task TestDeployContractSuccess()
        {
            SuccessfulTransactionReturn response = new SuccessfulTransactionReturn("0xaeaeaf3bac46dfb11ca18ab318dbc36362b1033a3d637e1b1c49496bab9581a3", "", null, new MetaTxnReceipt("",
                "", 0, null, new MetaTxnReceipt[]
                {
                    new MetaTxnReceipt("", "", 0, new MetaTxnReceiptLog[]
                    {
                        new MetaTxnReceiptLog("", new string[]
                        {
                            "",
                            "something",
                            "something else",
                        }, "", 0, "", 0, "", 0, false),
                        new MetaTxnReceiptLog(address.Value, new string[]
                        {
                            "",
                            "0xa506ad4e7f05eceba62a023c3219e5bd98a615f4fa87e2afb08a2da5cf62bf0c", // contract deployment topic
                            "something else",
                        }, "0x000000000000000000000000bc70b804b842686d68242eb329ea7e385fa58ee7", 0, "", 0, "", 0, false),
                    }, null, "")
                }, ""));
            IIntentSender intentSender = new MockIntentSender(response);
            WaaSToWalletAdapter adapter = new WaaSToWalletAdapter(new WaaSWallet(address, "", intentSender));
            IEthClient client = new MockEthClient(Chain.Polygon.AsHexString(), new TransactionReceipt());
            
            TransactionReceipt receipt = await adapter.DeployContract(client, "");
            
            Assert.IsNotNull(receipt);
            Assert.IsNotNull(receipt.contractAddress);
            Assert.AreEqual("0xbc70b804b842686d68242eb329ea7e385fa58ee7", receipt.contractAddress);
        }
    }
}