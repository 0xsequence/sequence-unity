using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [System.Serializable]
    [Obsolete("It is recommended to use SequenceContractCall instead")]
    public class DelayedEncode : Transaction
    {
        public const string TypeIdentifier = "delayedEncode";
        public DelayedEncodeData data;
        public string to;
        public string type = TypeIdentifier;
        public string value;
        
        [Preserve]
        public DelayedEncode(string contractAddress, string value, DelayedEncodeData data)
        {
            this.to = contractAddress;
            this.value = value;
            this.data = data;
        }
    }
}