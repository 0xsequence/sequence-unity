using System;
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    public class Attestation
    {
        public static readonly byte[] ACCEPT_IMPLICIT_REQUEST_MAGIC_PREFIX =
            SequenceCoder.KeccakHash("acceptImplicitRequest".ToByteArray());
        
        public Address approvedSigner;
        public FixedByte identityType;
        public FixedByte issuerHash;
        public FixedByte audienceHash;
        public byte[] applicationData;
        public AuthData authData;

        private void AssertHasSize(FixedByte obj, int size, string argumentName)
        {
            if (obj.Count != size)
            {
                throw new ArgumentException(
                    $"{argumentName} with size of {obj.Count} exceeds allowed size of {size} bytes");
            }
        }

        public byte[] Encode()
        {
            byte[] authDataBytes = authData.Encode();
            byte[] approvedSignerBytes = approvedSigner.Value.HexStringToByteArray(20);
            byte[] identityTypeBytes = identityType.Data.Slice(0, 4).PadLeft(4);
            byte[] issuerHashBytes = issuerHash.Data.PadLeft(32);
            byte[] audienceHashBytes = audienceHash.Data.PadLeft(32);
            byte[] applicationDataLengthBytes = applicationData.Length.ByteArrayFromNumber(3);
            
            return ByteArrayExtensions.ConcatenateByteArrays(approvedSignerBytes, identityTypeBytes, issuerHashBytes,
                audienceHashBytes, applicationDataLengthBytes, applicationData, authDataBytes);
        }

        public byte[] Hash()
        {
            byte[] encoded = Encode();
            return SequenceCoder.KeccakHash(encoded);
        }

        public object ToJson()
        {
            return new
            {
                approvedSigner = approvedSigner.Value,
                identityType = identityType.Data.ByteArrayToHexStringWithPrefix(),
                issuerHash = issuerHash.Data.ByteArrayToHexStringWithPrefix(),
                audienceHash = audienceHash.Data.ByteArrayToHexStringWithPrefix(),
                applicationData = applicationData.ByteArrayToHexStringWithPrefix(),
                authData = new
                {
                    redirectUrl = authData.redirectUrl,
                    issuedAt = authData.issuedAt.ToString()
                }
            };
        }

        public static Attestation FromJson(string json)
        {
            var jsonAble = JsonConvert.DeserializeObject<JsonAttestation>(json);
            return new Attestation
            {
                approvedSigner = jsonAble.approvedSigner,
                identityType = new FixedByte(4, jsonAble.identityType.HexStringToByteArray()),
                issuerHash = new FixedByte(32, jsonAble.issuerHash.HexStringToByteArray()),
                audienceHash = new FixedByte(32, jsonAble.audienceHash.HexStringToByteArray()),
                applicationData = jsonAble.applicationData.HexStringToByteArray(),
                authData = new AuthData(jsonAble.authData.redirectUrl, BigInteger.Parse(jsonAble.authData.issuedAt))
            };
        }

        public byte[] GenerateImplicitRequestMagic(Address wallet)
        {
            byte[] walletBytes = wallet.Value.HexStringToByteArray(20);
            return ByteArrayExtensions.ConcatenateByteArrays(ACCEPT_IMPLICIT_REQUEST_MAGIC_PREFIX, walletBytes,
                audienceHash.Data, issuerHash.Data);
        }

        [Serializable]
        public class JsonAttestation
        {
            public Address approvedSigner;
            public string identityType;
            public string issuerHash;
            public string audienceHash;
            public string applicationData;
            public JsonAuthData authData;

            [JsonConstructor]
            [Preserve]
            public JsonAttestation(Address approvedSigner, string identityType, string issuerHash, string audienceHash, string applicationData, JsonAuthData authData)
            {
                this.approvedSigner = approvedSigner;
                this.identityType = identityType;
                this.issuerHash = issuerHash;
                this.audienceHash = audienceHash;
                this.applicationData = applicationData;
                this.authData = authData;
            }

            public JsonAttestation(Attestation attestation)
            {
                this.approvedSigner = attestation.approvedSigner;
                this.identityType = attestation.identityType.Data.ByteArrayToHexStringWithPrefix();
                this.issuerHash = attestation.issuerHash.Data.ByteArrayToHexStringWithPrefix();
                this.audienceHash = attestation.audienceHash.Data.ByteArrayToHexStringWithPrefix();
                this.applicationData = attestation.applicationData.ByteArrayToHexStringWithPrefix();
                this.authData = new JsonAuthData(attestation.authData.redirectUrl, attestation.authData.issuedAt.ToString());
            }
        }

        public class JsonAuthData
        {
            public string redirectUrl;
            public string issuedAt;
            
            public JsonAuthData(string redirectUrl, string issuedAt)
            {
                this.redirectUrl = redirectUrl;
                this.issuedAt = issuedAt;
            }
        }
    }
}