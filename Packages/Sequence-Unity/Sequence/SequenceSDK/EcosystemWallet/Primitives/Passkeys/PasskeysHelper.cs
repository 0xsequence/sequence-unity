using System;
using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives.Passkeys
{
    public static class PasskeysHelper
    {
        public static string EncodeSignature(PasskeysArgs args)
        {
            var publicKey = new PublicKey
            {
                x = args.x,
                y = args.y,
                requireUserVerification = args.requireUserVerification,
                metadata = !string.IsNullOrEmpty(args.credentialId)
                    ? PasskeyMetadata.FromCredentialId(args.credentialId)
                    : PasskeyMetadata.FromHash(args.metadataHash)
            };
            
            var signature = new DecodedSignature
            {
                publicKey = publicKey,
                r = args.r.HexStringToByteArray(),
                s = args.s.HexStringToByteArray(),
                authenticatorData = args.authenticatorData.HexStringToByteArray(),
                clientDataJson = args.clientDataJson,
                embedMetadata = args.embedMetadata
            };
            
            return signature.Encode().ByteArrayToHexStringWithPrefix();
        }

        public static string DecodeSignature(string encodedSignature)
        {
            var encoded = encodedSignature.HexStringToByteArray();
            var decoded = DecodedSignature.Decode(encoded);
            
            return JsonConvert.SerializeObject(decoded);
        }

        public static string ComputeRoot(PasskeysArgs args)
        {
            if (!string.IsNullOrEmpty(args.credentialId) && !string.IsNullOrEmpty(args.metadataHash))
                throw new Exception("Cannot provide both credential-id and metadata-hash");

            var publicKey = new PublicKey
            {
                x = args.x,
                y = args.y,
                requireUserVerification = args.requireUserVerification,
                metadata = !string.IsNullOrEmpty(args.credentialId)
                    ? PasskeyMetadata.FromCredentialId(args.credentialId)
                    : PasskeyMetadata.FromHash(args.metadataHash)
            };
            
            return publicKey.Hash();
        }

        public static string ValidateSignature(PasskeysArgs args)
        {
            return string.Empty;
        }
    }
}