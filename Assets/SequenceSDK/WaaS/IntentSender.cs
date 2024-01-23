using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.ABI;
using Sequence.Authentication;
using Sequence.Extensions;
using Sequence.Utils;
using Sequence.WaaS.Authentication;
using SequenceSDK.WaaS;
using SequenceSDK.WaaS.Authentication;
using UnityEngine;

namespace Sequence.WaaS
{
    public class IntentSender : IIntentSender
    {
        public string SessionId { get; private set; }
        
        private HttpClient _httpClient;
        private DataKey _dataKey;
        private Wallet.IWallet _sessionWallet;
        private int _waasProjectId;
        private string _waasVersion;

        public IntentSender(HttpClient httpClient, DataKey dataKey, Wallet.IWallet sessionWallet, string sessionId, int waasProjectId, string waasVersion)
        {
            _httpClient = httpClient;
            _dataKey = dataKey;
            _sessionWallet = sessionWallet;
            SessionId = sessionId;
            _waasProjectId = waasProjectId;
            _waasVersion = waasVersion;
        }

        public async Task<T> SendIntent<T, T2>(T2 args)
        {
            string payload = AssemblePayloadJson(args);
            string intentPayload = await AssembleIntentPayload(payload);
            string sendIntentPayload = AssembleSendIntentPayload(intentPayload);
            IntentReturn<T> result = await PostIntent<IntentReturn<T>>(sendIntentPayload, "SendIntent");
            return result.data;
        }

        private async Task<IntentReturn<TransactionReturn>> SendTransactionIntent(WaaSPayload intent,
            Dictionary<string, string> headers)
        {
            IntentReturn<JObject> result = await _httpClient.SendRequest<WaaSPayload, IntentReturn<JObject>>("SendIntent", intent, headers);
            if (result.code == SuccessfulTransactionReturn.IdentifyingCode)
            {
                SuccessfulTransactionReturn successfulTransactionReturn = JsonConvert.DeserializeObject<SuccessfulTransactionReturn>(result.data.ToString());
                return new IntentReturn<TransactionReturn>(result.code, successfulTransactionReturn);
            }
            else if (result.code == FailedTransactionReturn.IdentifyingCode)
            {
                FailedTransactionReturn failedTransactionReturn = JsonConvert.DeserializeObject<FailedTransactionReturn>(result.data.ToString());
                return new IntentReturn<TransactionReturn>(result.code, failedTransactionReturn);
            }
            else
            {
                throw new Exception($"Unexpected result code: {result.code}");
            }
        }

        private string AssemblePayloadJson<T>(T args)
        {
            return JsonConvert.SerializeObject(args);
        }

        private async Task<string> AssembleIntentPayload(string payload)
        {
            JObject packet = JsonConvert.DeserializeObject<JObject>(payload);
            string signedPayload = await _sessionWallet.SignMessage(SequenceCoder.KeccakHash(payload.ToByteArray()));
            IntentPayload intentPayload = new IntentPayload(_waasVersion, packet, (SessionId, signedPayload));
            return JsonConvert.SerializeObject(intentPayload);
        }
        
        private async Task<string> PrepareEncryptedPayload(DataKey dataKey, string payload)
        {
            byte[] encryptedPayload = Encryptor.AES256CBCEncryption(dataKey.Plaintext, payload);
            return encryptedPayload.ByteArrayToHexStringWithPrefix();
        }

        private string AssembleSendIntentPayload(string intentPayload)
        {
            SendIntentPayload sendIntentPayload = new SendIntentPayload(SessionId, intentPayload);
            string payload = JsonConvert.SerializeObject(sendIntentPayload);
            return payload;
        }

        public async Task<bool> DropSession(string dropSessionId)
        {
            DropSessionArgs args = new DropSessionArgs(SessionId, dropSessionId);
            string payload = JsonConvert.SerializeObject(args);
            var result = await PostIntent<DropSessionReturn>(payload, "DropSession");
            return result.ok;
        }

        public async Task<T> PostIntent<T>(string payload, string path)
        {
            Debug.Log($"Sending intent: {path} | with payload: {payload}");
            string payloadCiphertext = await PrepareEncryptedPayload(_dataKey, payload);
            string signedPayload = await _sessionWallet.SignMessage(payload);
            WaaSPayload intent = new WaaSPayload(_dataKey.Ciphertext.ByteArrayToHexStringWithPrefix(), payloadCiphertext, signedPayload);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Sequence-Tenant", _waasProjectId.ToString());
            if (typeof(T) == typeof(IntentReturn<TransactionReturn>))
            {
                var transactionReturn = await SendTransactionIntent(intent, headers);
                return (T)(object)transactionReturn;
            }
            T result = await _httpClient.SendRequest<WaaSPayload, T>(path, intent, headers);
            return result;
        }

        public async Task<WaaSSession[]> ListSessions()
        {
            ListSessionsArgs args = new ListSessionsArgs(SessionId);
            string payload = JsonConvert.SerializeObject(args);
            var result = await PostIntent<ListSessionsReturn>(payload, "ListSessions");
            return result.sessions;
        }
    }
}