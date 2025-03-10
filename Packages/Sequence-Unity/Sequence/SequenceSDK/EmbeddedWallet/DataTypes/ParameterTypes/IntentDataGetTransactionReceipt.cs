using System;
using Newtonsoft.Json;
using Sequence;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class IntentDataGetTransactionReceipt
    {
        public string metaTxHash;
        public string network;
        public string wallet;

        [UnityEngine.Scripting.Preserve]
        [JsonConstructor]
        public IntentDataGetTransactionReceipt(string metaTxHash, string network, string wallet)
        {
            this.metaTxHash = metaTxHash;
            this.network = network;
            this.wallet = wallet;
        }

        public IntentDataGetTransactionReceipt(Address walletAddress, string network, string metaTransactionHash)
        {
            this.wallet = walletAddress;
            this.network = network;
            this.metaTxHash = metaTransactionHash;
        }
    }
}