using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.ABI;
using Sequence.Config;
using Sequence.Utils;
using Sequence.WaaS.DataTypes;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.EmbeddedWallet
{
    public class IntentSender : IIntentSender
    {
        public string SessionId { get; private set; }
        
        private IHttpClient _httpClient;
        private Sequence.Wallet.IWallet _sessionWallet;
        private int _waasProjectId;
        private string _waasVersion;
        private string _sessionId;
        private TimeSpan _timeshift;
        private bool _ready = false;
        
        private JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public IntentSender(IHttpClient httpClient, Sequence.Wallet.IWallet sessionWallet, string sessionId, int waasProjectId, string waasVersion)
        {
            _httpClient = httpClient;
            _sessionWallet = sessionWallet;
            SessionId = sessionId;
            _waasProjectId = waasProjectId;
            _waasVersion = waasVersion;
            _sessionId = IntentDataOpenSession.CreateSessionId(_sessionWallet.GetAddress());
            GetTimeShift().ConfigureAwait(false);
        }
        
        public async Task GetTimeShift()
        {
            var config = SequenceConfig.GetConfig(SequenceService.WaaS);
            var configJwt = SequenceConfig.GetConfigJwt(config);
            var rpcUrl = configJwt.rpcServer;
            
            if (string.IsNullOrWhiteSpace(rpcUrl))
                throw SequenceConfig.MissingConfigError("RPC Server");
            
            UnityWebRequest request = UnityWebRequest.Get(rpcUrl.AppendTrailingSlashIfNeeded() + "status");
            request.method = UnityWebRequest.kHttpVerbGET;

            try
            {
                await request.SendWebRequest();
                DateTime serverTime = DateTime.Parse(request.GetResponseHeader("date")).ToUniversalTime();
                DateTime localTime = DateTime.UtcNow;
                _timeshift = serverTime - localTime;
            }
            catch (Exception e)
            {
                Debug.LogError("Error getting time shift: " + e.Message);
                _timeshift = TimeSpan.Zero;
            }
            finally
            {
                request.Dispose();
            }
        }

        public async Task<T> SendIntent<T, T2>(T2 args, IntentType type, uint timeBeforeExpiryInSeconds = 30, uint currentTime = 0)
        {
            string payload = AssemblePayloadJson(args);
            object intentPayload = await AssembleIntentPayload(payload, type, timeBeforeExpiryInSeconds, currentTime);
            string path = "SendIntent";
            try
            {
                if (type == IntentType.OpenSession)
                {
                    path = "RegisterSession";
                    RegisterSessionIntent intent = intentPayload as RegisterSessionIntent;
                    string intentPayloadJson = JsonConvert.SerializeObject(intent, serializerSettings);
                    RegisterSessionResponse registerSessionResponse =
                        await PostIntent<RegisterSessionResponse>(intentPayloadJson, path);
                    return (T)(object)(registerSessionResponse.response.data);
                }

                SendIntentPayload sendIntentPayload = new SendIntentPayload(intentPayload as IntentPayload);
                string sendIntentPayloadJson = JsonConvert.SerializeObject(sendIntentPayload, serializerSettings);
                IntentResponse<T> result = await PostIntent<IntentResponse<T>>(sendIntentPayloadJson, path);
                return result.response.data;
            }
            catch (TimeMismatchException e)
            {
                long currentTimeAccordingToServer = e.CurrentTime;
                if (currentTimeAccordingToServer == 0)
                {
                    throw;
                }

                long currentTimeAccordingToIntent = 0;
                if (intentPayload is IntentPayload intent)
                {
                    currentTimeAccordingToIntent = (long)intent.issuedAt;
                }
                else if (intentPayload is RegisterSessionIntent registerSessionIntent)
                {
                    currentTimeAccordingToIntent = (long)registerSessionIntent.intent.issuedAt;
                }
                else
                {
                    Debug.LogError("Unexpected intent payload type: " + intentPayload.GetType());
                }
                if (currentTimeAccordingToServer > currentTimeAccordingToIntent + 1 ||
                    currentTimeAccordingToServer < currentTimeAccordingToIntent - 1)
                {
                    Debug.LogWarning("Time mismatch detected. Retrying with server time.");
                    return await SendIntent<T, T2>(args, type, timeBeforeExpiryInSeconds, (uint)currentTimeAccordingToServer);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task<IntentResponse<TransactionReturn>> SendTransactionIntent(string intent,
            Dictionary<string, string> headers)
        {
            IntentResponse<JObject> result = await _httpClient.SendPostRequest<string, IntentResponse<JObject>>("SendIntent", intent, headers);
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

        private async Task<object> AssembleIntentPayload(string payload, IntentType type, uint timeToLiveInSeconds, ulong currentTime)
        {
            while (!_ready)
            {
                await Task.Yield();
            }

            if (currentTime == 0)
            {
                currentTime = (ulong)DateTimeOffset.UtcNow.Add(_timeshift).ToUnixTimeSeconds();
            }
            JObject packet = JsonConvert.DeserializeObject<JObject>(payload);
            IntentPayload toSign = new IntentPayload(_waasVersion, type, packet, null, timeToLiveInSeconds, currentTime);
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
            T result = await _httpClient.SendPostRequest<string, T>(path, payload, headers);
            return result;
        }

        public async Task<Session[]> ListSessions(Address walletAddress = null)
        {
            if (walletAddress == null)
            {
                walletAddress = _sessionWallet.GetAddress();
            }
            Session[] sessions = await SendIntent<Session[], IntentDataListSessions>(
                new IntentDataListSessions(walletAddress), IntentType.ListSessions);
            return sessions;
        }
        
        public async Task<SuccessfulTransactionReturn> GetTransactionReceipt(SuccessfulTransactionReturn response)
        {
            JObject requestData = response.request.data;
            if (requestData.TryGetValue("network", out JToken networkJToken))
            {
                string network = networkJToken.Value<string>();
                if (requestData.TryGetValue("wallet", out JToken walletAddressJToken))
                {
                    string wallet = walletAddressJToken.Value<string>();
                    Address walletAddress = new Address(wallet);
                    SuccessfulTransactionReturn result =
                        await SendIntent<SuccessfulTransactionReturn, IntentDataGetTransactionReceipt>(
                            new IntentDataGetTransactionReceipt(walletAddress, network, response.metaTxHash),
                            IntentType.GetTransactionReceipt);
                    return result;
                }
                else
                {
                    throw new Exception("Wallet address not found in response");
                }
            }
            else
            {
                throw new Exception("Network not found in response");
            }
        }
    }
}