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
        public WonkyIntentPayload request { get; private set; }
        public SimulateResult[] simulations { get; private set; }

        [JsonConstructor]
        public FailedTransactionReturn(string error, WonkyIntentPayload request, SimulateResult[] simulations)
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
            this.request = new WonkyIntentPayload(new IntentPayload("", IntentType.SendTransaction, requestJObject, null));
            this.simulations = null;
        }
    }
}