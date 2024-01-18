using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Sequence.WaaS.Tests
{
    public class WaaSWalletUnitTests
    {
        private Address address = new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249");
        
        [Test]
        public async Task TestSendTransactionSuccessEvent()
        {
            IIntentSender intentSender = new MockIntentSender(new SuccessfulTransactionReturn("","",null,null));
            WaaSWallet wallet = new WaaSWallet(address, "", intentSender);
            
            bool successEventHit = false;
            wallet.OnSendTransactionComplete += (result)=>
            {
                successEventHit = true;
            };
            bool failedEventHit = false;
            wallet.OnSendTransactionFailed += (result)=>
            {
                failedEventHit = true;
            };
            
            await wallet.SendTransaction(null);
            
            Assert.IsTrue(successEventHit);
            Assert.IsFalse(failedEventHit);
        }
        
        [Test]
        public async Task TestSendTransactionFailedEvent()
        {
            IIntentSender intentSender = new MockIntentSender(new FailedTransactionReturn("",null,null));
            WaaSWallet wallet = new WaaSWallet(address, "", intentSender);
            
            bool successEventHit = false;
            wallet.OnSendTransactionComplete += (result)=>
            {
                successEventHit = true;
            };
            bool failedEventHit = false;
            wallet.OnSendTransactionFailed += (result)=>
            {
                failedEventHit = true;
            };
            
            await wallet.SendTransaction(null);
            
            Assert.IsFalse(successEventHit);
            Assert.IsTrue(failedEventHit);
        }

        [Test]
        public async Task TestSendTransactionException()
        {
            IIntentSender intentSender = new MockIntentSender(new Exception("Something bad happened"));
            WaaSWallet wallet = new WaaSWallet(address, "", intentSender);
            
            bool successEventHit = false;
            wallet.OnSendTransactionComplete += (result)=>
            {
                successEventHit = true;
            };
            bool failedEventHit = false;
            wallet.OnSendTransactionFailed += (result)=>
            {
                failedEventHit = true;
            };
            
            await wallet.SendTransaction(null);
            
            Assert.IsFalse(successEventHit);
            Assert.IsTrue(failedEventHit);
        }
        
        [Test]
        public async Task TestSignMessageSuccessEvent()
        {
            IIntentSender intentSender = new MockIntentSender(new SignMessageReturn("",""));
            WaaSWallet wallet = new WaaSWallet(address, "", intentSender);
            
            bool successEventHit = false;
            wallet.OnSignMessageComplete += (result)=>
            {
                successEventHit = true;
            };
            
            await wallet.SignMessage(null);
            
            Assert.IsTrue(successEventHit);
        }

        [Test]
        public async Task TestDeployContractSuccessEvent()
        {
            SuccessfulTransactionReturn response = new SuccessfulTransactionReturn("", "", null, new MetaTxnReceipt("",
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
            WaaSWallet wallet = new WaaSWallet(address, "", intentSender);
            
            
            bool successTransactionEventHit = false;
            wallet.OnSendTransactionComplete += (result)=>
            {
                successTransactionEventHit = true;
            };
            bool failedTransactionEventHit = false;
            wallet.OnSendTransactionFailed += (result)=>
            {
                failedTransactionEventHit = true;
            };
            bool successContractDeployEventHit = false;
            Address contractAddress = null;
            SuccessfulContractDeploymentReturn deploymentReturn = null;
            wallet.OnDeployContractComplete += (result)=>
            {
                successContractDeployEventHit = true;
                contractAddress = result.DeployedContractAddress;
                deploymentReturn = result;
            };
            bool failedContractDeployEventHit = false;
            wallet.OnDeployContractFailed += (result)=>
            {
                failedContractDeployEventHit = true;
            };
            
            var result2 = await wallet.DeployContract(Chain.None, "");
            
            Assert.IsTrue(successTransactionEventHit);
            Assert.IsFalse(failedTransactionEventHit);
            Assert.IsTrue(successContractDeployEventHit);
            Assert.IsFalse(failedContractDeployEventHit);
            Assert.IsNotNull(contractAddress);
            Assert.AreEqual("0xbc70b804b842686d68242eb329ea7e385fa58ee7", contractAddress.Value);
            Assert.AreEqual(result2, deploymentReturn);
        }

        [Test]
        public async Task TestDeployContractCantFindDeployTopic()
        {
            
            SuccessfulTransactionReturn response = new SuccessfulTransactionReturn("", "", null, new MetaTxnReceipt("",
                "", 0, null, new MetaTxnReceipt[]
                {
                    new MetaTxnReceipt("", "", 0, new MetaTxnReceiptLog[]
                    {
                        new MetaTxnReceiptLog("", new string[]
                        {
                            "",
                            "something",
                            "something else",
                            "more things",
                        }, "", 0, "", 0, "", 0, false),
                        new MetaTxnReceiptLog("", new string[]
                        {
                            "something", 
                            "something else",
                        }, "", 0, "", 0, "", 0, false),
                    }, null, "")
                }, ""));
            IIntentSender intentSender = new MockIntentSender(response);
            WaaSWallet wallet = new WaaSWallet(address, "", intentSender);
            
            
            bool successTransactionEventHit = false;
            wallet.OnSendTransactionComplete += (result)=>
            {
                successTransactionEventHit = true;
            };
            bool failedTransactionEventHit = false;
            wallet.OnSendTransactionFailed += (result)=>
            {
                failedTransactionEventHit = true;
            };
            bool successContractDeployEventHit = false;
            wallet.OnDeployContractComplete += (result)=>
            {
                successContractDeployEventHit = true;
            };
            bool failedContractDeployEventHit = false;
            FailedContractDeploymentReturn deploymentReturn = null;
            wallet.OnDeployContractFailed += (result)=>
            {
                failedContractDeployEventHit = true;
                deploymentReturn = result;
            };
            
            var result2 = await wallet.DeployContract(Chain.None, "");
            
            Assert.IsTrue(successTransactionEventHit);
            Assert.IsFalse(failedTransactionEventHit);
            Assert.IsFalse(successContractDeployEventHit);
            Assert.IsTrue(failedContractDeployEventHit);
            Assert.IsNotNull(deploymentReturn);
            Assert.IsNull(deploymentReturn.TransactionReturn);
            Assert.AreEqual("Failed to find newly deployed contract address in transaction receipt logs " +
                            response.receipt, deploymentReturn.Error);
            Assert.AreEqual(result2, deploymentReturn);
        }

        [Test]
        public async Task TestDeployContractFailedTransaction()
        {
            FailedTransactionReturn response = new FailedTransactionReturn("some error", null, null);
            IIntentSender intentSender = new MockIntentSender(response);
            WaaSWallet wallet = new WaaSWallet(address, "", intentSender);
            
            
            bool successTransactionEventHit = false;
            wallet.OnSendTransactionComplete += (result)=>
            {
                successTransactionEventHit = true;
            };
            bool failedTransactionEventHit = false;
            wallet.OnSendTransactionFailed += (result)=>
            {
                failedTransactionEventHit = true;
            };
            bool successContractDeployEventHit = false;
            wallet.OnDeployContractComplete += (result)=>
            {
                successContractDeployEventHit = true;
            };
            bool failedContractDeployEventHit = false;
            FailedContractDeploymentReturn deploymentReturn = null;
            wallet.OnDeployContractFailed += (result)=>
            {
                failedContractDeployEventHit = true;
                deploymentReturn = result;
            };
            
            var result2 = await wallet.DeployContract(Chain.None, "");
            
            Assert.IsFalse(successTransactionEventHit);
            Assert.IsTrue(failedTransactionEventHit);
            Assert.IsFalse(successContractDeployEventHit);
            Assert.IsTrue(failedContractDeployEventHit);
            Assert.IsNotNull(deploymentReturn);
            Assert.IsNotNull(deploymentReturn.TransactionReturn);
            Assert.AreEqual(response, deploymentReturn.TransactionReturn);
            Assert.AreEqual("some error", deploymentReturn.Error);
            Assert.AreEqual(result2, deploymentReturn);
        }

        [Test]
        public async Task TestDeployContractTransactionException()
        {
            Exception exception = new Exception("some error");
            IIntentSender intentSender = new MockIntentSender(exception);
            WaaSWallet wallet = new WaaSWallet(address, "", intentSender);
            
            
            bool successTransactionEventHit = false;
            wallet.OnSendTransactionComplete += (result)=>
            {
                successTransactionEventHit = true;
            };
            bool failedTransactionEventHit = false;
            wallet.OnSendTransactionFailed += (result)=>
            {
                failedTransactionEventHit = true;
            };
            bool successContractDeployEventHit = false;
            wallet.OnDeployContractComplete += (result)=>
            {
                successContractDeployEventHit = true;
            };
            bool failedContractDeployEventHit = false;
            FailedContractDeploymentReturn deploymentReturn = null;
            wallet.OnDeployContractFailed += (result)=>
            {
                failedContractDeployEventHit = true;
                deploymentReturn = result;
            };
            
            var result2 = await wallet.DeployContract(Chain.None, "");
            
            Assert.IsFalse(successTransactionEventHit);
            Assert.IsTrue(failedTransactionEventHit);
            Assert.IsFalse(successContractDeployEventHit);
            Assert.IsTrue(failedContractDeployEventHit);
            Assert.IsNotNull(deploymentReturn);
            Assert.IsNotNull(deploymentReturn.TransactionReturn);
            Assert.AreEqual("some error", deploymentReturn.Error);
            Assert.AreEqual(result2, deploymentReturn);
        }
    }
}