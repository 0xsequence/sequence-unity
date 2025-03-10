using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    public class IntentResponseSessionAuthProof
    {
        public string sessionId;
        public string network;
        public string wallet;
        public string message;
        public string signature;

        [UnityEngine.Scripting.Preserve]
        public IntentResponseSessionAuthProof(string sessionId, string network, string wallet, string message, string signature)
        {
            this.sessionId = sessionId;
            this.network = network;
            this.wallet = wallet;
            this.message = message;
            this.signature = signature;
        }
    }
}