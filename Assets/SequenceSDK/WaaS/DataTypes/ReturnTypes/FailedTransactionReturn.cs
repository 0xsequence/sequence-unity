using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SequenceSDK.WaaS;

namespace Sequence.WaaS
{
    [System.Serializable]
    public class FailedTransactionReturn : TransactionReturn
    {
        public const string IdentifyingCode = "transactionFailed";
        public string error { get; private set; }
        public JObject request { get; private set; } // Todo replace with IntentPayload once response structure is updated
        public SimulateResult[] simulations { get; private set; }

        [JsonConstructor]
        public FailedTransactionReturn(string error, JObject request, SimulateResult[] simulations)
        {
            this.error = error;
            this.request = request;
            this.simulations = simulations;
        }
        
        public FailedTransactionReturn(string error, IntentDataSendTransaction request)
        {
            this.error = error;
            string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            JObject requestJObject = JObject.Parse(requestJson);
            IntentPayload payload = new IntentPayload("", IntentType.SendTransaction, requestJObject, null);
            string payloadJson = JsonConvert.SerializeObject(payload);
            this.request = JObject.Parse(payloadJson);
            this.simulations = null;
        }
    }
}