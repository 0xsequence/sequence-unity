using Newtonsoft.Json.Linq;
using SequenceSDK.WaaS;

namespace Sequence.WaaS
{
    [System.Serializable]
    public class SuccessfulTransactionReturn : TransactionReturn
    {
        public const string IdentifyingCode = "transactionReceipt";
        public string txHash { get; private set; }
        public string metaTxHash { get; private set; }
        public JObject request { get; private set; }  // Todo replace with IntentPayload once response structure is updated
        public MetaTxnReceipt receipt { get; private set; }
        public JObject nativeReceipt { get; private set; }
        public SimulateResult[] simulations { get; private set; }

        public SuccessfulTransactionReturn(string txHash, string metaTxHash, JObject request, MetaTxnReceipt receipt, JObject nativeReceipt = null, SimulateResult[] simulations = null)
        {
            this.txHash = txHash;
            this.metaTxHash = metaTxHash;
            this.request = request;
            this.receipt = receipt;
            this.nativeReceipt = nativeReceipt;
            this.simulations = simulations;
        }
    }
}