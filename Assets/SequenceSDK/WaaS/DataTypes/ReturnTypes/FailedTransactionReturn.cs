using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SequenceSDK.WaaS;

namespace Sequence.WaaS
{
    [System.Serializable]
    public class FailedTransactionReturn : TransactionReturn
    {
        public const string IdentifyingCode = "transactionFailed";
        public string error;
        public IntentPayload request;
        public SimulateResult[] simulations;

        [JsonConstructor]
        public FailedTransactionReturn(string error, IntentPayload request, SimulateResult[] simulations)
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
            this.request = new IntentPayload("", IntentType.SendTransaction, requestJObject, null);
            this.simulations = null;
        }
    }
}