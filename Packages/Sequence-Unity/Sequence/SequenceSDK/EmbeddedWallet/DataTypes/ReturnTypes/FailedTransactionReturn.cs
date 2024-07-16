using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.EmbeddedWallet
{
    [System.Serializable]
    public class FailedTransactionReturn : TransactionReturn
    {
        public const string IdentifyingCode = "transactionFailed";
        public string error { get; private set; }
        public IntentPayload request { get; private set; }
        public SimulateResult[] simulations { get; private set; }

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