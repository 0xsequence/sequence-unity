using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    [System.Serializable]
    public class AdditionalFee
    {
        public string amount;
        public string receiver;
        
        [JsonConstructor]
        public AdditionalFee(string amount, string receiver)
        {
            this.amount = amount;
            this.receiver = receiver;
        }
        
        public AdditionalFee(string amount, Address receiver)
        {
            this.amount = amount;
            this.receiver = receiver;
        }

        public AdditionalFee(Address receiver, float amount, int decimals = 18)
        {
            this.amount = DecimalNormalizer.Normalize(amount, decimals);
            this.receiver = receiver;
        }
    }
}