using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Sequence.Wallet;
using SequenceSDK.WaaS;

namespace Sequence.WaaS.Tests
{
    public class IntentSenderTests
    {
        [Test]
        public async Task SendTransactionIntentTest_FailedTransaction()
        {
            IntentSender intentSender = new IntentSender(new MockHttpClientReturnsFailedTransaction(), new EthWallet(), "", 0, "");
            var result =
                await intentSender.SendIntent<TransactionReturn, IntentDataSendTransaction>(
                    new IntentDataSendTransaction("", "", null), IntentType.SendTransaction);
            if (result is FailedTransactionReturn failedTransactionReturn)
            {
                Assert.AreEqual(failedTransactionReturn.error, MockHttpClientReturnsFailedTransaction.error);
            }
            else
            {
                Assert.Fail($"Did not receive {nameof(FailedTransactionReturn)}");
            }
        }

        [Test]
        public async Task SendTransactionIntentTest_UnknownResponseCode()
        {
            IntentSender intentSender = new IntentSender(new MockHttpClientReturnsUnknownCode(), new EthWallet(), "", 0, "");
            try
            {
                var result =
                    await intentSender.SendIntent<TransactionReturn, IntentDataSendTransaction>(
                        new IntentDataSendTransaction("", "", null), IntentType.SendTransaction);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, $"Unexpected result code: {MockHttpClientReturnsUnknownCode.code}");
                return;
            }
            Assert.Fail("Expected exception but none was thrown");
        }

        [Test]
        public async Task SendTransactionIntentTest_SuccessfulTransactionResponse()
        {
            IntentSender intentSender = new IntentSender(new MockHttpClientReturnsSuccessfulTransaction(), new EthWallet(), "", 0, "");
            var result =
                await intentSender.SendIntent<TransactionReturn, IntentDataSendTransaction>(
                    new IntentDataSendTransaction("", "", null), IntentType.SendTransaction);
            if (result is SuccessfulTransactionReturn successfulTransactionReturn)
            {
                Assert.AreEqual(successfulTransactionReturn.txHash, MockHttpClientReturnsSuccessfulTransaction.txHash);
            }
            else
            {
                Assert.Fail($"Did not receive {nameof(SuccessfulTransactionReturn)}");
            }
        }

        [Test]
        public async Task TestGetTransactionReceipt()
        {
            EthWallet wallet = new EthWallet();
            IntentDataSendTransaction intentDataSendTransaction =
                new IntentDataSendTransaction(wallet.GetAddress(), Chain.None, null);
            string intentJson = JsonConvert.SerializeObject(intentDataSendTransaction);
            IntentSender intentSender = new IntentSender(new MockHttpClientReturnsSuccessfulTransaction(), wallet, "", 0, "");
            
            var result = await intentSender.GetTransactionReceipt(new SuccessfulTransactionReturn("", "",
                new IntentPayload(
                    "1.0.0", IntentType.GetTransactionReceipt, JsonConvert.DeserializeObject<JObject>(intentJson), null), null));
            
            Assert.NotNull(result);
            Assert.AreEqual(result.txHash, MockHttpClientReturnsSuccessfulTransaction.txHash);
        }

        [Test]
        public async Task TestGetTransactionReceipt_noNetwork()
        {
            EthWallet wallet = new EthWallet();
            IsValidMessageSignatureArgs intentData =
                new IsValidMessageSignatureArgs(Chain.None, "", "", "");
            string intentJson = JsonConvert.SerializeObject(intentData);
            IntentSender intentSender = new IntentSender(new MockHttpClientReturnsSuccessfulTransaction(), wallet, "", 0, "");

            try
            {
                var result = await intentSender.GetTransactionReceipt(new SuccessfulTransactionReturn("", "",
                    new IntentPayload(
                        "1.0.0", IntentType.GetTransactionReceipt, JsonConvert.DeserializeObject<JObject>(intentJson),
                        null), null));
                Assert.Fail("Expected exception but none was thrown");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Network not found in response", e.Message);
            }
        }

        [Test]
        public async Task TestGetTransactionReceipt_noWallet()
        {
            EthWallet wallet = new EthWallet();
            JObjectWithoutWallet intentData =
                new JObjectWithoutWallet(Chain.None.ToString());
            string intentJson = JsonConvert.SerializeObject(intentData);
            IntentSender intentSender = new IntentSender(new MockHttpClientReturnsSuccessfulTransaction(), wallet, "", 0, "");

            try
            {
                var result = await intentSender.GetTransactionReceipt(new SuccessfulTransactionReturn("", "",
                    new IntentPayload(
                        "1.0.0", IntentType.GetTransactionReceipt, JsonConvert.DeserializeObject<JObject>(intentJson),
                        null), null));
                Assert.Fail("Expected exception but none was thrown");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Wallet address not found in response", e.Message);
            }
        }

        private class MockHttpClientReturnsFailedTransaction : IHttpClient
        {
            public static string error = "Mock failed transaction";
            public async Task<T2> SendRequest<T, T2>(string path, T args, Dictionary<string, string> headers = null, string overrideUrl = null)
            {
                var response = new IntentResponse<FailedTransactionReturn>(new Response<FailedTransactionReturn>(
                    FailedTransactionReturn.IdentifyingCode,
                    new FailedTransactionReturn(error, null, null)));
                string responseJson = JsonConvert.SerializeObject(response);
                return JsonConvert.DeserializeObject<T2>(responseJson);
            }
        }

        private class MockHttpClientReturnsUnknownCode : IHttpClient
        {
            public static string code = "some unrecognized code";
            public async Task<T2> SendRequest<T, T2>(string path, T args, Dictionary<string, string> headers = null, string overrideUrl = null)
            {
                var response = new IntentResponse<FailedTransactionReturn>(new Response<FailedTransactionReturn>(
                    code,
                    new FailedTransactionReturn("", null, null)));
                string responseJson = JsonConvert.SerializeObject(response);
                return JsonConvert.DeserializeObject<T2>(responseJson);
            }
        }

        private class MockHttpClientReturnsSuccessfulTransaction : IHttpClient
        {
            public static string txHash = "txhash";
            public async Task<T2> SendRequest<T, T2>(string path, T args, Dictionary<string, string> headers = null, string overrideUrl = null)
            {
                var response = new IntentResponse<SuccessfulTransactionReturn>(new Response<SuccessfulTransactionReturn>(
                    SuccessfulTransactionReturn.IdentifyingCode,
                    new SuccessfulTransactionReturn(txHash, "", null, null)));
                string responseJson = JsonConvert.SerializeObject(response);
                return JsonConvert.DeserializeObject<T2>(responseJson);
            }
        }

        [Serializable]
        private class JObjectWithoutWallet
        {
            public string network;

            public JObjectWithoutWallet(string network)
            {
                this.network = network;
            }
        }
    }
}