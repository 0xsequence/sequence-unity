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
        private string _sessionId;
        
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
            _sessionId = IntentDataOpenSession.CreateSessionId(_sessionWallet.GetAddress());
        }

        public async Task<T> SendIntent<T, T2>(T2 args, IntentType type, uint timeBeforeExpiryInSeconds = 30)
        {
            string payload = AssemblePayloadJson(args);
            object intentPayload = await AssembleIntentPayload(payload, type, timeBeforeExpiryInSeconds);
            string path = "SendIntent";
            if (type == IntentType.OpenSession)
            {
                path = "RegisterSession";
                RegisterSessionIntent intent = intentPayload as RegisterSessionIntent;
                string intentPayloadJson = JsonConvert.SerializeObject(intent, serializerSettings);
                RegisterSessionResponse registerSessionResponse = await PostIntent<RegisterSessionResponse>(intentPayloadJson, path);
                return (T)(object)(registerSessionResponse.response.data);
            }
            SendIntentPayload sendIntentPayload = new SendIntentPayload(intentPayload as IntentPayload);
            string sendIntentPayloadJson = JsonConvert.SerializeObject(sendIntentPayload, serializerSettings);
            IntentResponse<T> result = await PostIntent<IntentResponse<T>>(sendIntentPayloadJson, path);
            return result.response.data;
        }

        private async Task<IntentResponse<TransactionReturn>> SendTransactionIntent(string intent,
            Dictionary<string, string> headers)
        {
            IntentResponse<JObject> result = await _httpClient.SendRequest<string, IntentResponse<JObject>>("SendIntent", intent, headers);
            if (result.response.code == SuccessfulTransactionReturn.IdentifyingCode)
            {
                SuccessfulTransactionReturn successfulTransactionReturn = JsonConvert.DeserializeObject<SuccessfulTransactionReturn>(result.response.data.ToString());
                return new IntentResponse<TransactionReturn>(new Response<TransactionReturn>(result.response.code, successfulTransactionReturn));
            }
            else if (result.response.code == FailedTransactionReturn.IdentifyingCode)
            {
                FailedTransactionReturn failedTransactionReturn = JsonConvert.DeserializeObject<FailedTransactionReturn>(result.response.data.ToString());
                return new IntentResponse<TransactionReturn>(new Response<TransactionReturn>(result.response.code, failedTransactionReturn));
            }
            else
            {
                throw new Exception($"Unexpected result code: {result.response.code}");
            }
        }

        private string AssemblePayloadJson<T>(T args)
        {
            return JsonConvert.SerializeObject(args, serializerSettings);
        }

        private async Task<object> AssembleIntentPayload(string payload, IntentType type, uint timeToLiveInSeconds)
        {
            JObject packet = JsonConvert.DeserializeObject<JObject>(payload);
            IntentPayload toSign = new IntentPayload(_waasVersion, type, packet, null, timeToLiveInSeconds);
            string toSignJson = JsonConvert.SerializeObject(toSign, serializerSettings);
            string signedPayload = await _sessionWallet.SignMessage(SequenceCoder.KeccakHash(toSignJson.ToByteArray()));
            IntentPayload intentPayload = new IntentPayload(_waasVersion, type, toSign.expiresAt, toSign.issuedAt, packet,
                new Signature[] {new Signature(_sessionId, signedPayload)});
            if (type == IntentType.OpenSession)
            {
                RegisterSessionIntent registerSessionIntent = new RegisterSessionIntent(Guid.NewGuid().ToString(), intentPayload);
                return registerSessionIntent;
            }

            return intentPayload;
        }

        public async Task<bool> DropSession(string dropSessionId)
        {
            IntentResponseSessionClosed result = await SendIntent<IntentResponseSessionClosed, IntentDataCloseSession>(
                new IntentDataCloseSession(dropSessionId),
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

        public async Task<WaaSSession[]> ListSessions()
        {
            WaaSSession[] sessions = await SendIntent<WaaSSession[], IntentDataListSessions>(
                new IntentDataListSessions(_sessionWallet.GetAddress()), IntentType.ListSessions);
            return sessions;
        }
    }
}