using System.Numerics;
using Newtonsoft.Json;
using StringExtensions = Sequence.Utils.StringExtensions;

namespace Sequence.WaaS
{
    [System.Serializable]
    public class RawTransaction : SequenceSDK.WaaS.Transaction
    {
        public const string TypeIdentifier = "transaction";
        public string data { get; private set; }
        public string to { get; private set; }
        public string type { get; private set; } = TypeIdentifier;
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
        
        [JsonConstructor]
        public RawTransaction(string data, string to, string type, string value)
        {
            this.data = data;
            this.to = to;
            this.type = type;
            this.value = value;
        }
    }
}
