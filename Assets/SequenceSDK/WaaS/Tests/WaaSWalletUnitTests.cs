using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.TestTools;

namespace Sequence.EmbeddedWallet.Tests
{
    public class WaaSWalletUnitTests
    {
        private Address address = new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249");
        
        [Test]
        public async Task TestSendTransactionSuccessEvent()
        {
            IIntentSender intentSender = new MockIntentSender(new SuccessfulTransactionReturn("0xaeaeaf3bac46dfb11ca18ab318dbc36362b1033a3d637e1b1c49496bab9581a3","",null,null));
            SequenceWallet wallet = new SequenceWallet(address, "", intentSender);
            
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
            
            await wallet.SendTransaction(Chain.None, Array.Empty<Transaction>());
            
            Assert.IsTrue(successEventHit);
            Assert.IsFalse(failedEventHit);
        }
        
        [Test]
        public async Task TestSendTransactionFailedEvent()
        {
            IIntentSender intentSender = new MockIntentSender(new FailedTransactionReturn("",null,null));
            SequenceWallet wallet = new SequenceWallet(address, "", intentSender);
            
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
            
            await wallet.SendTransaction(Chain.None, Array.Empty<Transaction>());
            
            Assert.IsFalse(successEventHit);
            Assert.IsTrue(failedEventHit);
        }

        private static (DelayedEncodeData, bool)[] _invalidDelayedEncodeData = new[]
        {
            (new DelayedEncodeData("functionName", Array.Empty<object>(), "functionName"), false),
            (new DelayedEncodeData("functionName()", Array.Empty<object>(), "functionName"), true),
            (new DelayedEncodeData("functionName()", Array.Empty<object>(), "functionName()"), false),
            (new DelayedEncodeData("functionName(", Array.Empty<object>(), "functionName"), false),
            (new DelayedEncodeData("functionName)", Array.Empty<object>(), "functionName"), false),
            (new DelayedEncodeData("functionName(uint banana)", new object[] {1}, "functionName"), true),
            (new DelayedEncodeData("functionName(uint, uint)", new object[]{1,2}, "functionName"), true),
            (new DelayedEncodeData("functionName(uint,uint)", new object[]{1,2}, "functionName(uint,uint)"), false),
            (new DelayedEncodeData("functionName(uint banana", new object[] {1}, "functionName"), false),
            (new DelayedEncodeData("functionNameuint, uint)", new object[]{1,2}, "functionName"), false),
            (new DelayedEncodeData("functionName(uint,uint", new object[]{1,2}, "functionName"), false)
        };
        
        [TestCaseSource(nameof(_invalidDelayedEncodeData))]
        public async Task TestSendTransactionEvent_delayedEncodeValidation((DelayedEncodeData, bool) args)
        {
            DelayedEncodeData data = args.Item1;
            bool success = args.Item2;
            IIntentSender intentSender = new MockIntentSender(new SuccessfulTransactionReturn());
            SequenceWallet wallet = new SequenceWallet(address, "", intentSender);
            
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
            
            await wallet.SendTransaction(Chain.None, new Transaction[]
            {
                new RawTransaction("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"),
                new DelayedEncode("0xc683a014955b75F5ECF991d4502427c8fa1Aa249", "0", data),
                new RawTransaction("0xc683a014955b75F5ECF991d4502427c8fa1Aa249")
            }, waitForReceipt: false);

            if (!success)
            {
                Assert.IsFalse(successEventHit);
                Assert.IsTrue(failedEventHit);
            }
            else
            {
                Assert.IsTrue(successEventHit);
                Assert.IsFalse(failedEventHit);
            }
        }
        
        private static (AbiData, bool)[] _invalidContractCallData = new[]
        {
            (new AbiData("functionName", Array.Empty<object>()), false),
            (new AbiData("functionName()", Array.Empty<object>()), true),
            (new AbiData("functionName(", Array.Empty<object>()), false),
            (new AbiData("functionName)", Array.Empty<object>()), false),
            (new AbiData("functionName(uint banana)", new object[] {1}), true),
            (new AbiData("functionName(uint, uint)", new object[]{1,2}), true),
            (new AbiData("functionName(uint,uint)", new object[]{1,2}), true),
            (new AbiData("functionName(uint banana", new object[] {1}), false),
            (new AbiData("functionNameuint, uint)", new object[]{1,2}), false),
            (new AbiData("functionName(uint,uint", new object[]{1,2}), false)
        };
        
        [TestCaseSource(nameof(_invalidContractCallData))]
        public async Task TestSendTransactionEvent_contractCallValidation((AbiData, bool) args)
        {
            AbiData data = args.Item1;
            bool success = args.Item2;
            IIntentSender intentSender = new MockIntentSender(new SuccessfulTransactionReturn());
            SequenceWallet wallet = new SequenceWallet(address, "", intentSender);
            
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
            
            await wallet.SendTransaction(Chain.None, new Transaction[]
            {
                new RawTransaction("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"),
                new SequenceContractCall("0xc683a014955b75F5ECF991d4502427c8fa1Aa249", data),
                new RawTransaction("0xc683a014955b75F5ECF991d4502427c8fa1Aa249")
            }, waitForReceipt: false);

            if (!success)
            {
                Assert.IsFalse(successEventHit);
                Assert.IsTrue(failedEventHit);
            }
            else
            {
                Assert.IsTrue(successEventHit);
                Assert.IsFalse(failedEventHit);
            }
        }

        [Test]
        public async Task TestSendTransactionException()
        {
            IIntentSender intentSender = new MockIntentSender(new Exception("Something bad happened"));
            SequenceWallet wallet = new SequenceWallet(address, "", intentSender);
            
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
            
            await wallet.SendTransaction(Chain.None, Array.Empty<Transaction>());
            
            Assert.IsFalse(successEventHit);
            Assert.IsTrue(failedEventHit);
        }
        
        [Test]
        public async Task TestSignMessageSuccessEvent()
        {
            IIntentSender intentSender = new MockIntentSender(new IntentResponseSignedMessage("","0x"));
            SequenceWallet wallet = new SequenceWallet(address, "", intentSender);
            
            bool successEventHit = false;
            wallet.OnSignMessageComplete += (result)=>
            {
                successEventHit = true;
            };
            
            await wallet.SignMessage(Chain.None, "");
            
            Assert.IsTrue(successEventHit);
        }

        [Test]
        public async Task TestDeployContractSuccessEvent()
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
            SequenceWallet wallet = new SequenceWallet(address, "", intentSender);
            
            
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
            SequenceWallet wallet = new SequenceWallet(address, "", intentSender);
            
            
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
            SequenceWallet wallet = new SequenceWallet(address, "", intentSender);
            
            
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
            SequenceWallet wallet = new SequenceWallet(address, "", intentSender);
            
            
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

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(10)]
        public async Task TestWaitForTransactionReceipt(int numberOfFetches)
        {
            SuccessfulTransactionReturn resultWithNoTransactionHash =
                new SuccessfulTransactionReturn("", "", null, null);
            object[] mockReturnObjects = new object[numberOfFetches];
            for (int i = 0; i < numberOfFetches; i++)
            {
                mockReturnObjects[i] = resultWithNoTransactionHash;
            }

            SuccessfulTransactionReturn resultWithTransactionHash =
                new SuccessfulTransactionReturn("0xaeaeaf3bac46dfb11ca18ab318dbc36362b1033a3d637e1b1c49496bab9581a3",
                    "", null, null);

            MockIntentSender intentSender =
                new MockIntentSender(mockReturnObjects.AppendObject(resultWithTransactionHash));
            SequenceWallet wallet = new SequenceWallet(address, "", intentSender);
            
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

            var result = await wallet.SendTransaction(Chain.None, Array.Empty<Transaction>());
            
            Assert.IsTrue(successEventHit);
            Assert.IsFalse(failedEventHit);
            if (result is SuccessfulTransactionReturn successfulTransactionReturn)
            {
                Assert.AreEqual(resultWithTransactionHash, successfulTransactionReturn);
            }
            else
            {
                Assert.Fail($"Expected {nameof(SuccessfulTransactionReturn)} return type");
            }
        }

        [Test]
        public async Task TestWaitForTransactionReceipt_failedToFetch()
        {
            int numberOfFetches = 5;
            
            SuccessfulTransactionReturn resultWithNoTransactionHash =
                new SuccessfulTransactionReturn("", "", null, null);
            object[] mockReturnObjects = new object[numberOfFetches];
            for (int i = 0; i < numberOfFetches; i++)
            {
                mockReturnObjects[i] = resultWithNoTransactionHash;
            }

            SuccessfulTransactionReturn resultWithTransactionHash =
                new SuccessfulTransactionReturn("0xaeaeaf3bac46dfb11ca18ab318dbc36362b1033a3d637e1b1c49496bab9581a3",
                    "", null, null);

            MockIntentSender intentSender =
                new MockIntentSender(mockReturnObjects.AppendObject(resultWithTransactionHash));
            intentSender.InjectException(new Exception("some random error"));
            SequenceWallet wallet = new SequenceWallet(address, "", intentSender);
            
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
            
            LogAssert.Expect(LogType.Error, "Transaction was successful, but we're unable to obtain the transaction hash. Reason: some random error");
            
            var result = await wallet.SendTransaction(Chain.None, Array.Empty<Transaction>());
            
            Assert.IsTrue(successEventHit);
            Assert.IsFalse(failedEventHit);
            if (result is SuccessfulTransactionReturn successfulTransactionReturn)
            {
                Assert.AreEqual(resultWithNoTransactionHash, successfulTransactionReturn);
            }
            else
            {
                Assert.Fail($"Expected {nameof(SuccessfulTransactionReturn)} return type");
            }
        }
        
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(10)]
        public async Task TestDontWaitForTransactionReceipt(int numberOfFetches)
        {
            SuccessfulTransactionReturn resultWithNoTransactionHash =
                new SuccessfulTransactionReturn("", "", null, null);
            object[] mockReturnObjects = new object[numberOfFetches];
            for (int i = 0; i < numberOfFetches; i++)
            {
                mockReturnObjects[i] = resultWithNoTransactionHash;
            }

            SuccessfulTransactionReturn resultWithTransactionHash =
                new SuccessfulTransactionReturn("0xaeaeaf3bac46dfb11ca18ab318dbc36362b1033a3d637e1b1c49496bab9581a3",
                    "", null, null);

            MockIntentSender intentSender =
                new MockIntentSender(mockReturnObjects.AppendObject(resultWithTransactionHash));
            SequenceWallet wallet = new SequenceWallet(address, "", intentSender);
            
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

            var result = await wallet.SendTransaction(Chain.None, Array.Empty<Transaction>(), false);
            
            Assert.IsTrue(successEventHit);
            Assert.IsFalse(failedEventHit);
            if (result is SuccessfulTransactionReturn successfulTransactionReturn)
            {
                Assert.AreEqual(resultWithNoTransactionHash, successfulTransactionReturn);
            }
            else
            {
                Assert.Fail($"Expected {nameof(SuccessfulTransactionReturn)} return type");
            }
        }
    }
}