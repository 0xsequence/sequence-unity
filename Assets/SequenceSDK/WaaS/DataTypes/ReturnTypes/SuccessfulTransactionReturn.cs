using Newtonsoft.Json.Linq;
using SequenceSDK.WaaS;

namespace Sequence.WaaS
{
    [System.Serializable]
    public class SuccessfulTransactionReturn : TransactionReturn
    {
        public const string IdentifyingCode = "transactionReceipt";
        public string txHash;
        public string metaTxHash;
        public IntentPayload request;
        public MetaTxnReceipt receipt;
        public JObject nativeReceipt;
        public SimulateResult[] simulations;

        public SuccessfulTransactionReturn(string txHash, string metaTxHash, IntentPayload request, MetaTxnReceipt receipt, JObject nativeReceipt = null, SimulateResult[] simulations = null)
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