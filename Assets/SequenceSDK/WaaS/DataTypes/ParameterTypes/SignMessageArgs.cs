using System;
using System.Text;
using Sequence.ABI;
using Sequence.Extensions;
using Sequence.Utils;
using SequenceSDK.WaaS;

namespace Sequence.WaaS
{
    [Serializable]
    public class SignMessageArgs
    {
        public string code { get; private set; } = "signMessage";
        public uint expires { get; private set; }
        public uint issued { get; private set; }
        public string message { get; private set; }
        public string network { get; private set; }
        public string wallet { get; private set; }

        public SignMessageArgs(string wallet, Chain network, string message, uint timeBeforeExpiry = 30)
        {
            int networkId = (int)network;
            this.wallet = wallet;
            this.network = networkId.ToString();
            this.message = PrepareMessage(message);
            this.issued = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            this.expires = this.issued + timeBeforeExpiry;
        }
        
        private static string PrepareMessage(string message)
        {
            return Wallet.IWallet.PrefixedMessage(Encoding.UTF8.GetBytes(message)).ByteArrayToHexString().ToUpper().EnsureHexPrefix();
        }
    }
}