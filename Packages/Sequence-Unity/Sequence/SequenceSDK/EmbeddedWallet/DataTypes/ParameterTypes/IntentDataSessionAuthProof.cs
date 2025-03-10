using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class IntentDataSessionAuthProof
    {
        public string network;
        public string nonce = null;
        public string wallet;

        [UnityEngine.Scripting.Preserve]
        [JsonConstructor]
        public IntentDataSessionAuthProof(string network, string wallet, string nonce)
        {
            this.network = network;
            this.wallet = wallet;
            this.nonce = nonce;
        }
        
        public IntentDataSessionAuthProof(Chain network, Address walletAddress, string nonce = null)
        {
            this.network = network.GetChainId();
            this.wallet = walletAddress.Value;
            this.nonce = nonce;
        }
    }
}