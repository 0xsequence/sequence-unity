using Newtonsoft.Json;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [System.Serializable]
    public class AdditionalFee
    {
        public string amount;
        public string receiver;
        
        [Preserve]
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