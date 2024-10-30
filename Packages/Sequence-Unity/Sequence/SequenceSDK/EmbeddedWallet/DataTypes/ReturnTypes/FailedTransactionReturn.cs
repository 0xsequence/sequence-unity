using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [System.Serializable]
    public class FailedTransactionReturn : TransactionReturn
    {
        public const string IdentifyingCode = "transactionFailed";
        public string error;
        public IntentPayload request;
        public SimulateResult[] simulations;

        [Preserve]
        [JsonConstructor]
        public FailedTransactionReturn(string error, IntentPayload request, SimulateResult[] simulations)
        {
            this.error = error;
            this.request = request;
            this.simulations = simulations;
        }

        public FailedTransactionReturn() { }

        public FailedTransactionReturn(string error, IntentDataSendTransaction request)
        {
            this.error = error;
            string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            JObject requestJObject = JObject.Parse(requestJson);
            this.request = new IntentPayload("", IntentType.SendTransaction, requestJObject, null);
            this.simulations = null;
        }
    }

    public class FailedBatchTransactionReturn : FailedTransactionReturn
    {
        public SuccessfulTransactionReturn[] SuccessfulTransactionReturns;

        public FailedTransactionReturn[] FailedTransactionReturns;

        public FailedBatchTransactionReturn(SuccessfulTransactionReturn[] successfullTransactionReturns, FailedTransactionReturn[] failedTransactionReturns)
        {
            SuccessfulTransactionReturns = successfullTransactionReturns;
            FailedTransactionReturns = failedTransactionReturns;
        }
    }
}