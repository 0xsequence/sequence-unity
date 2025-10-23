using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Sequence.Config;
using Sequence.Wallet;
using Sequence.EmbeddedWallet;
using Sequence.Utils;
using Sequence.WaaS.DataTypes;
using UnityEngine;
using UnityEngine.TestTools;

namespace Sequence.EmbeddedWallet.Tests
{
    public class IntentSenderTests
    {
        [Test]
        public async Task SendTransactionIntentTest_FailedTransaction()
        {
            IntentSender intentSender = new IntentSender(new MockHttpClientReturnsFailedTransaction(), new EOAWallet(), "", 0, "");
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
            IntentSender intentSender = new IntentSender(new MockHttpClientReturnsUnknownCode(), new EOAWallet(), "", 0, "");
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
            IntentSender intentSender = new IntentSender(new MockHttpClientReturnsSuccessfulTransaction(), new EOAWallet(), "", 0, "");
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
            EOAWallet wallet = new EOAWallet();
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
            EOAWallet wallet = new EOAWallet();
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
            EOAWallet wallet = new EOAWallet();
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

        [TestCase(-1000)] // intent will have expired
        [TestCase(1000)] // intent will have been issued in the future
        public async Task TestTimeMismatchExceptionResultsInRetry(int timeOffset)
        {
            SequenceConfig config = SequenceConfig.GetConfig(SequenceService.WaaS);
            ConfigJwt configJwt = SequenceConfig.GetConfigJwt(config);
            IntentSender intentSender = new IntentSender(new HttpClient($"{configJwt.rpcServer.AppendTrailingSlashIfNeeded()}rpc/WaasAuthenticator"), new EOAWallet(), "", configJwt.projectId, config.WaaSVersion);
            LogAssert.Expect(LogType.Warning, new Regex("Time mismatch*"));
            try
            {
                var result = await intentSender.SendIntent<TransactionReturn, IntentDataSendTransaction>(
                    new IntentDataSendTransaction("", "", null), IntentType.SendTransaction, 30, (uint)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + timeOffset));
                Assert.Fail("Expected exception but none was thrown");
            }
            catch (Exception e)
            {
                Assert.True(e is not TimeMismatchException);
            }
        }

        private class MockHttpClientReturnsFailedTransaction : IHttpClient
        {
            public static string error = "Mock failed transaction";

            public async Task<TResponse> SendPostRequest<TArgs, TResponse>(string path, TArgs args, Dictionary<string, string> headers = null)
            {
                var response = new IntentResponse<FailedTransactionReturn>(new Response<FailedTransactionReturn>(
                    FailedTransactionReturn.IdentifyingCode, new FailedTransactionReturn(error, null, null)));
                
                string responseJson = JsonConvert.SerializeObject(response);
                return JsonConvert.DeserializeObject<TResponse>(responseJson);
            }

            public Task<TResponse> SendGetRequest<TResponse>(string path, Dictionary<string, string> headers = null)
            {
                throw new NotImplementedException();
            }
        }

        private class MockHttpClientReturnsUnknownCode : IHttpClient
        {
            public static string code = "some unrecognized code";

            public async Task<TResponse> SendPostRequest<TArgs, TResponse>(string path, TArgs args, Dictionary<string, string> headers = null)
            {
                var response = new IntentResponse<FailedTransactionReturn>(new Response<FailedTransactionReturn>(
                    code, new FailedTransactionReturn("", null, null)));
                
                string responseJson = JsonConvert.SerializeObject(response);
                return JsonConvert.DeserializeObject<TResponse>(responseJson);
            }

            public Task<TResponse> SendGetRequest<TResponse>(string path, Dictionary<string, string> headers = null)
            {
                throw new NotImplementedException();
            }
        }

        private class MockHttpClientReturnsSuccessfulTransaction : IHttpClient
        {
            public static string txHash = "txhash";

            public async Task<TResponse> SendPostRequest<TArgs, TResponse>(string path, TArgs args, Dictionary<string, string> headers = null)
            {
                var response = new IntentResponse<SuccessfulTransactionReturn>(new Response<SuccessfulTransactionReturn>(
                    SuccessfulTransactionReturn.IdentifyingCode, new SuccessfulTransactionReturn(txHash, "", null, null)));
                
                string responseJson = JsonConvert.SerializeObject(response);
                return JsonConvert.DeserializeObject<TResponse>(responseJson);
            }

            public Task<TResponse> SendGetRequest<TResponse>(string path, Dictionary<string, string> headers = null)
            {
                throw new NotImplementedException();
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