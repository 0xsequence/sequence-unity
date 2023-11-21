using System;
using SequenceSDK.WaaS;

namespace Sequence.WaaS
{
    [Serializable]
    public class SignMessageArgs
    {
        public string code { get; private set; }
        public uint expires { get; private set; }
        public uint issued { get; private set; }
        public string message { get; private set; }
        public string network { get; private set; }
        public string wallet { get; private set; }

        public SignMessageArgs(string wallet, Chain network, string message, uint timeBeforeExpiry = 3000)
        {
            int networkId = (int)network;
            this.wallet = wallet;
            this.network = networkId.ToString();
            this.message = message;
            this.code = "signMessage";
            this.issued = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            this.expires = this.issued + timeBeforeExpiry;
        }
        
        public SignMessageArgs(string wallet, string networkId, string message, uint timeBeforeExpiry = 3000)
        {
            this.wallet = wallet;
            this.network = networkId;
            this.message = message;
            this.code = "signMessage";
            this.issued = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            this.expires = this.issued + timeBeforeExpiry;
        }
    }
}