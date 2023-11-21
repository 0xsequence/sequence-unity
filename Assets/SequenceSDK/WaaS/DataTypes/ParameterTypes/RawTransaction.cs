using StringExtensions = Sequence.Utils.StringExtensions;

namespace Sequence.WaaS
{
    [System.Serializable]
    public class RawTransaction : SequenceSDK.WaaS.Transaction
    {
        public string data { get; private set; }
        public string to { get; private set; }
        public string type { get; private set; } = "transaction";
        public string value { get; private set; }

        public RawTransaction(string to, string value = null, string calldata = null)
        {
            if (to == StringExtensions.ZeroAddress)
            {
                to = WaaSZeroAddress;
            }
            this.to = to;
            this.value = value;
            this.data = calldata;
        }
    }
}
