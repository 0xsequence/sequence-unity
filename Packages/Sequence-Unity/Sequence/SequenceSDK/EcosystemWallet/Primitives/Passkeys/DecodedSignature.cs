using System;
using System.Collections.Generic;
using System.Text;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Primitives.Passkeys
{
    public class DecodedSignature
    {
        public PublicKey publicKey;
        public byte[] r;
        public byte[] s;
        public byte[] authenticatorData;
        public string clientDataJson;
        public int challengeIndex;
        public int typeIndex;
        public bool embedMetadata;

        public byte[] Encode()
        {
            clientDataJson = clientDataJson.Replace("\n", "").Replace(" ", "");
            int challengeIndex = clientDataJson.IndexOf("\"challenge\"");
            int typeIndex = clientDataJson.IndexOf("\"type\"");
            
            int authDataSize = authenticatorData.Length;
            int clientDataJSONSize = clientDataJson.Length;

            if (authDataSize > 65535)
                throw new Exception("Authenticator data size is too large");
            if (clientDataJSONSize > 65535)
                throw new Exception("Client data JSON size is too large");

            int bytesAuthDataSize = authDataSize <= 255 ? 1 : 2;
            int bytesClientDataJSONSize = clientDataJSONSize <= 255 ? 1 : 2;
            int bytesChallengeIndex = challengeIndex <= 255 ? 1 : 2;
            int bytesTypeIndex = typeIndex <= 255 ? 1 : 2;

            byte flags = 0;
            if (publicKey.requireUserVerification)
                flags |= 0x01;
            
            flags |= (byte)((bytesAuthDataSize - 1) << 1);
            flags |= (byte)((bytesClientDataJSONSize - 1) << 2);
            flags |= (byte)((bytesChallengeIndex - 1) << 3);
            flags |= (byte)((bytesTypeIndex - 1) << 4);

            if (embedMetadata)
                flags |= (1 << 6); // 0x40

            var result = new List<byte[]> { new [] { flags } };

            if (embedMetadata)
            {
                if (!publicKey.metadata.IsValid)
                    throw new Exception("Metadata is not present in the public key");

                result.Add(publicKey.metadata.Encode());
            }

            result.Add(authDataSize.ByteArrayFromNumber(bytesAuthDataSize));
            result.Add(authenticatorData);

            var clientDataBytes = Encoding.UTF8.GetBytes(clientDataJson);
            result.Add(clientDataBytes.Length.ByteArrayFromNumber(bytesClientDataJSONSize));
            result.Add(clientDataBytes);
            
            result.Add(challengeIndex.ByteArrayFromNumber(bytesChallengeIndex));
            result.Add(typeIndex.ByteArrayFromNumber(bytesTypeIndex));

            result.Add(r.PadLeft(32));
            result.Add(s.PadLeft(32));

            result.Add(publicKey.x.HexStringToByteArray());
            result.Add(publicKey.y.HexStringToByteArray());

            return ByteArrayExtensions.ConcatenateByteArrays(result.ToArray());
        }
        
        public static DecodedSignature Decode(byte[] data)
        {
            int offset = 0;

            if (data.Length < 1)
                throw new Exception("Data too short");

            byte flags = data[0];
            offset += 1;

            bool requireUserVerification = (flags & 0x01) != 0x00;
            int bytesAuthDataSize = ((flags >> 1) & 0x01) + 1;
            int bytesClientDataJSONSize = ((flags >> 2) & 0x01) + 1;
            int bytesChallengeIndex = ((flags >> 3) & 0x01) + 1;
            int bytesTypeIndex = ((flags >> 4) & 0x01) + 1;
            bool hasMetadata = ((flags >> 6) & 0x01) == 0x01;

            if ((flags & 0x20) != 0)
                throw new Exception("Fallback to abi decode is not supported in this implementation");

            string? metadata = null;

            if (hasMetadata)
            {
                var metadataBytes = data.SubArray(offset, 32);
                metadata = metadataBytes.ByteArrayToHexStringWithPrefix();
                offset += 32;
            }

            int authDataSize = data.SubArray(offset, bytesAuthDataSize).ToInteger();
            offset += bytesAuthDataSize;
            var authenticatorData = data.SubArray(offset, authDataSize);
            offset += authDataSize;

            int clientDataJSONSize = data.SubArray(offset, bytesClientDataJSONSize).ToInteger();
            offset += bytesClientDataJSONSize;
            var clientDataJSONBytes = data.SubArray(offset, clientDataJSONSize);
            offset += clientDataJSONSize;
            string clientDataJSON = Encoding.UTF8.GetString(clientDataJSONBytes).Replace("\\", "");

            int challengeIndex = data.SubArray(offset, bytesChallengeIndex).ToInteger();
            offset += bytesChallengeIndex;
            int typeIndex = data.SubArray(offset, bytesTypeIndex).ToInteger();
            offset += bytesTypeIndex;

            var r = data.SubArray(offset, 32);
            offset += 32;
            var s = data.SubArray(offset, 32);
            offset += 32;

            var xBytes = data.SubArray(offset, 32);
            offset += 32;
            var yBytes = data.SubArray(offset, 32);

            return new DecodedSignature
            {
                publicKey = new PublicKey
                {
                    requireUserVerification = requireUserVerification,
                    x = xBytes.ByteArrayToHexStringWithPrefix(),
                    y = yBytes.ByteArrayToHexStringWithPrefix(),
                    metadata = PasskeyMetadata.FromHash(metadata)
                },
                r = r,
                s = s,
                authenticatorData = authenticatorData,
                clientDataJson = clientDataJSON,
                challengeIndex = challengeIndex,
                typeIndex = typeIndex,
                embedMetadata = hasMetadata
            };
        }
    }
}