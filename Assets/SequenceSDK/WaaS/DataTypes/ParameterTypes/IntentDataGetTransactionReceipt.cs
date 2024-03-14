using System;
using Sequence;

namespace SequenceSDK.WaaS
{
    [Serializable]
    public class IntentDataGetTransactionReceipt
    {
        public string metaTxHash { get; private set; }
        public string network { get; private set; }
        public string wallet { get; private set; }

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