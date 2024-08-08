using System;
namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class IntentDataGetIdToken
    {
        public string SessionId { get; private set; }
        public string Address { get; private set; }
        public string Nonce { get; private set; } 

        public IntentDataGetIdToken(string sessionId, string walletAddress, string nonce = null)
        {
            this.SessionId = sessionId;
            this.Address = walletAddress;
            this.Nonce = nonce; 
        }
    }
}