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
using UnityEditor;
using UnityEngine;

namespace Sequence.WaaS
{
    public class IntentSender : IIntentSender
    {
        public string SessionId { get; private set; }
        
        private HttpClient _httpClient;
        private Wallet.IWallet _sessionWallet;
        private int _waasProjectId;
        private string _waasVersion;
        
        private JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public IntentSender(HttpClient httpClient, Wallet.IWallet sessionWallet, string sessionId, int waasProjectId, string waasVersion)
        {
            _httpClient = httpClient;
            _sessionWallet = sessionWallet;
            SessionId = sessionId;
            _waasProjectId = waasProjectId;
            _waasVersion = waasVersion;
        }

        public async Task<T> SendIntent<T, T2>(T2 args, IntentType type, uint timeBeforeExpiryInSeconds = 30)
        {
            string payload = AssemblePayloadJson(args);
            string intentPayload = await AssembleIntentPayload(payload, type, timeBeforeExpiryInSeconds);
            string path = "SendIntent";
            if (type == IntentType.OpenSession)
            {
                path = "RegisterSession";
                RegisterSessionResponse registerSessionResponse = await PostIntent<RegisterSessionResponse>(intentPayload, path);
                return (T)(object)(registerSessionResponse.response.data);
            }
            IntentResponse<T> result = await PostIntent<IntentResponse<T>>(intentPayload, path);
            return result.data;
        }

        private async Task<IntentResponse<TransactionReturn>> SendTransactionIntent(string intent,
            Dictionary<string, string> headers)
        {
            IntentResponse<JObject> result = await _httpClient.SendRequest<string, IntentResponse<JObject>>("SendIntent", intent, headers);
            if (result.code == SuccessfulTransactionReturn.IdentifyingCode)
            {
                SuccessfulTransactionReturn successfulTransactionReturn = JsonConvert.DeserializeObject<SuccessfulTransactionReturn>(result.data.ToString());
                return new IntentResponse<TransactionReturn>(result.code, successfulTransactionReturn);
            }
            else if (result.code == FailedTransactionReturn.IdentifyingCode)
            {
                FailedTransactionReturn failedTransactionReturn = JsonConvert.DeserializeObject<FailedTransactionReturn>(result.data.ToString());
                return new IntentResponse<TransactionReturn>(result.code, failedTransactionReturn);
            }
            else
            {
                throw new Exception($"Unexpected result code: {result.code}");
            }
        }

        private string AssemblePayloadJson<T>(T args)
        {
            return JsonConvert.SerializeObject(args, serializerSettings);
        }

        private async Task<string> AssembleIntentPayload(string payload, IntentType type, uint timeToLiveInSeconds)
        {
            JObject packet = JsonConvert.DeserializeObject<JObject>(payload);
            IntentPayload toSign = new IntentPayload(_waasVersion, type, packet, null, timeToLiveInSeconds);
            string toSignJson = JsonConvert.SerializeObject(toSign, serializerSettings);
            string signedPayload = await _sessionWallet.SignMessage(SequenceCoder.KeccakHash(toSignJson.ToByteArray()));
            IntentPayload intentPayload = new IntentPayload(_waasVersion, type, toSign.expiresAt, toSign.issuedAt, packet,
                new Signature[] {new Signature(IntentDataOpenSession.CreateSessionId(_sessionWallet.GetAddress()), signedPayload)});
            if (type == IntentType.OpenSession)
            {
                RegisterSessionIntent registerSessionIntent = new RegisterSessionIntent(Guid.NewGuid().ToString(), intentPayload);
                return JsonConvert.SerializeObject(registerSessionIntent);
            }
            return JsonConvert.SerializeObject(intentPayload);
        }
        
        private async Task<string> PrepareEncryptedPayload(DataKey dataKey, string payload)
        {
            byte[] encryptedPayload = Encryptor.AES256CBCEncryption(dataKey.Plaintext, payload);
            return encryptedPayload.ByteArrayToHexStringWithPrefix();
        }

        public async Task<bool> DropSession(string dropSessionId)
        {
            IntentResponseSessionClosed result = await SendIntent<IntentResponseSessionClosed, IntentDataCloseSession>(
                new IntentDataCloseSession(IntentDataOpenSession.CreateSessionId(_sessionWallet.GetAddress())),
                IntentType.CloseSession);
            return result != null;
        }

        public async Task<T> PostIntent<T>(string payload, string path)
        {
            Debug.Log($"Sending intent: {path} | with payload: {payload}");
            Dictionary<string, string> headers = new Dictionary<string, string>();
            if (typeof(T) == typeof(IntentResponse<TransactionReturn>))
            {
                var transactionReturn = await SendTransactionIntent(payload, headers);
                return (T)(object)transactionReturn;
            }
            T result = await _httpClient.SendRequest<string, T>(path, payload, headers);
            return result;
        }

        public async Task<string[]> ListSessions()
        {
            IntentResponseListSessions sessions = await SendIntent<IntentResponseListSessions, IntentDataListSessions>(
                new IntentDataListSessions(_sessionWallet.GetAddress()), IntentType.ListSessions);
            return sessions.sessions;
        }
    }
}