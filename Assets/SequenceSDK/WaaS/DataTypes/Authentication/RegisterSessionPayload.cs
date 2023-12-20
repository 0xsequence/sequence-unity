using System;

namespace Sequence.WaaS.Authentication
{
    [Serializable]
    public class RegisterSessionPayload
    {
        public string encryptedPayloadKey;
        public string payloadCiphertext;
        public string payloadSig;

        public RegisterSessionPayload(string encryptedPayloadKey, string payloadCiphertext, string payloadSig)
        {
            this.encryptedPayloadKey = encryptedPayloadKey;
            this.payloadCiphertext = payloadCiphertext;
            this.payloadSig = payloadSig;
        }
    }
}