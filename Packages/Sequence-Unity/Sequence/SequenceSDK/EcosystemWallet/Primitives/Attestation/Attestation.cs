using System;
using Newtonsoft.Json;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    internal class Attestation
    {
        public static readonly byte[] ACCEPT_IMPLICIT_REQUEST_MAGIC_PREFIX =
            SequenceCoder.KeccakHash("acceptImplicitRequest".ToByteArray());
        
        public Address approvedSigner;
        public FixedByte identityType;
        public FixedByte issuerHash;
        public FixedByte audienceHash;
        public byte[] applicationData;
        public AuthData authData;

        public Attestation(Address approvedSigner, FixedByte identityType, FixedByte issuerHash, FixedByte audienceHash, byte[] applicationData, AuthData authData)
        {
            this.approvedSigner = approvedSigner;
            AssertHasSize(identityType, 4, nameof(identityType));
            this.identityType = identityType;
            AssertHasSize(issuerHash, 32, nameof(issuerHash));
            this.issuerHash = issuerHash;
            AssertHasSize(audienceHash, 32, nameof(audienceHash));
            this.audienceHash = audienceHash;
            this.applicationData = applicationData;
            this.authData = authData;
        }

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

        public string ToJson() // Todo make this a JsonConverter
        {
            JsonAttestation jsonAble = new JsonAttestation(this);
            return JsonConvert.ToString(jsonAble);
        }

        public Attestation(JsonAttestation jsonAble)
        {
            this.approvedSigner = jsonAble.approvedSigner;
            this.identityType = new FixedByte(4, jsonAble.identityType.HexStringToByteArray());
            this.issuerHash = new FixedByte(32, jsonAble.issuerHash.HexStringToByteArray());
            this.audienceHash = new FixedByte(32, jsonAble.audienceHash.HexStringToByteArray());
            this.applicationData = jsonAble.applicationData.HexStringToByteArray();
            this.authData = jsonAble.authData;
        }

        public Attestation FromJson(string json) // Todo make this a JsonConverter
        {
            JsonAttestation jsonAble = JsonConvert.DeserializeObject<JsonAttestation>(json);
            return new Attestation(jsonAble);
        }

        public byte[] GenerateImplicitRequestMagic(Address wallet)
        {
            byte[] walletBytes = wallet.Value.HexStringToByteArray(20);
            return ByteArrayExtensions.ConcatenateByteArrays(ACCEPT_IMPLICIT_REQUEST_MAGIC_PREFIX, walletBytes,
                audienceHash.Data, issuerHash.Data);
        }

        [Serializable]
        internal class JsonAttestation
        {
            public Address approvedSigner;
            public string identityType;
            public string issuerHash;
            public string audienceHash;
            public string applicationData;
            public AuthData authData;

            [JsonConstructor]
            [Preserve]
            public JsonAttestation(Address approvedSigner, string identityType, string issuerHash, string audienceHash, string applicationData, AuthData authData)
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
                this.authData = attestation.authData;
            }
        }
    }
}