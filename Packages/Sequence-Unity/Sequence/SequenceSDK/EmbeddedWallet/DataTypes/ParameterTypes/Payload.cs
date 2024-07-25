using System;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class Payload
    {
        public string encryptedPayloadKey;
        public string payloadCiphertext;
        public string payloadSig;

        public Payload(string encryptedPayloadKey, string payloadCiphertext, string payloadSig)
        {
            this.encryptedPayloadKey = encryptedPayloadKey;
            this.payloadCiphertext = payloadCiphertext;
            this.payloadSig = payloadSig;
        }
    }
}