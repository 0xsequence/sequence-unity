using System;
using Newtonsoft.Json;

namespace Sequence.WaaS
{
    [Serializable]
    public class IntentDataSessionAuthProof
    {
        public string network;
        public string nonce = null;
        public string wallet;

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