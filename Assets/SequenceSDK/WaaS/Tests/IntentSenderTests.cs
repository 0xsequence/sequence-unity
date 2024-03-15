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
        
        // Todo write tests where we get blank txHash back
        

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
    }
}