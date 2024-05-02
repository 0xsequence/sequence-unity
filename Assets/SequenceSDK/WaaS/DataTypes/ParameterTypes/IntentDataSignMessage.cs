using System;
using System.Text;
using Sequence.Utils;

namespace Sequence.WaaS
{
    [Serializable]
    public class IntentDataSignMessage
    {
        public string message;
        public string network;
        public string wallet;

        public IntentDataSignMessage(string walletAddress, Chain network, string message)
        {
            int networkId = (int)network;
            this.wallet = walletAddress;
            this.network = networkId.ToString();
            this.message = PrepareMessage(message);
        }
        
        public IntentDataSignMessage(string walletAddress, string networkId, string message)
        {
            this.wallet = walletAddress;
            this.network = networkId;
            this.message = PrepareMessage(message);
        }
        
        private static string PrepareMessage(string message)
        {
            return Wallet.IWallet.PrefixedMessage(Encoding.UTF8.GetBytes(message)).ByteArrayToHexString().ToUpper().EnsureHexPrefix();
        }
    }
}