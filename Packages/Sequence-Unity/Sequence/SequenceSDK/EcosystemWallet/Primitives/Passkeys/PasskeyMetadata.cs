using Sequence.ABI;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives.Passkeys
{
    public class PasskeyMetadata
    {
        public bool IsValid => !string.IsNullOrEmpty(credentialId) || !string.IsNullOrEmpty(hash);
        
        public string credentialId;
        public string hash;

        public byte[] Encode()
        {
            if (!string.IsNullOrEmpty(credentialId))
                return SequenceCoder.KeccakHash(credentialId.ToByteArray());
            
            if (!string.IsNullOrEmpty(hash))
                return hash.HexStringToByteArray();

            return "0x00".HexStringToByteArray(32);
        }
        
        public static PasskeyMetadata FromCredentialId(string credentialId)
        {
            return new PasskeyMetadata
            {
                credentialId = credentialId
            };
        }
        
        public static PasskeyMetadata FromHash(string hash)
        {
            return new PasskeyMetadata
            {
                hash = hash
            };
        }
    }
}