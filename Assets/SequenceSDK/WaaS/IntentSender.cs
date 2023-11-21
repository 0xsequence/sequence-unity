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
using UnityEngine;

namespace Sequence.WaaS
{
    public class IntentSender
    {
        private HttpClient _httpClient;
        private DataKey _dataKey;
        private Wallet.IWallet _sessionWallet;
        private string _sessionId;
        private int _waasProjectId;
        private string _waasVersion;

        public IntentSender(HttpClient httpClient, DataKey dataKey, Wallet.IWallet sessionWallet, string sessionId, int waasProjectId, string waasVersion)
        {
            _httpClient = httpClient;
            _dataKey = dataKey;
            _sessionWallet = sessionWallet;
            _sessionId = sessionId;
            _waasProjectId = waasProjectId;
            _waasVersion = waasVersion;
        }

        public async Task<T> SendIntent<T, T2>(T2 args)
        {
            string payload = AssemblePayloadJson(args);
            string intentPayload = await AssembleIntentPayload(payload);
            Debug.Log($"Intent Payload: {intentPayload}");
            string sendIntentPayload = AssembleSendIntentPayload(intentPayload);
            Debug.Log($"Send intent payload: {sendIntentPayload}");
            string payloadCiphertext = await PrepareEncryptedPayload(_dataKey, sendIntentPayload);
            string signedPayload = await _sessionWallet.SignMessage(sendIntentPayload);
            WaaSPayload intent = new WaaSPayload(_dataKey.Ciphertext.ByteArrayToHexStringWithPrefix(), payloadCiphertext, signedPayload);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-Sequence-Tenant", _waasProjectId.ToString());
            IntentReturn<T> result = await _httpClient.SendRequest<WaaSPayload, IntentReturn<T>>("SendIntent", intent, headers);
            return result.data;
        }

        private string AssemblePayloadJson<T>(T args)
        {
            return JsonConvert.SerializeObject(args);
        }

        private async Task<string> AssembleIntentPayload(string payload)
        {
            JObject packet = JsonConvert.DeserializeObject<JObject>(payload);
            string signedPayload = await _sessionWallet.SignMessage(SequenceCoder.KeccakHash(payload.ToByteArray()));
            Debug.Log($"Signing payload {payload} result: {signedPayload}");
            IntentPayload intentPayload = new IntentPayload(_waasVersion, packet, (_sessionId, signedPayload));
            return JsonConvert.SerializeObject(intentPayload);
        }
        
        private async Task<string> PrepareEncryptedPayload(DataKey dataKey, string payload)
        {
            byte[] encryptedPayload = Encryptor.AES256CBCEncryption(dataKey.Plaintext, payload);
            return encryptedPayload.ByteArrayToHexStringWithPrefix();
        }

        private string AssembleSendIntentPayload(string intentPayload)
        {
            SendIntentPayload sendIntentPayload = new SendIntentPayload(_sessionId, intentPayload);
            string payload = JsonConvert.SerializeObject(sendIntentPayload);
            return payload;
        }
    }
}