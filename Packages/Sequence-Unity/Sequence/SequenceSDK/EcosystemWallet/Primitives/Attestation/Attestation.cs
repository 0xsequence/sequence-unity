using System;
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using Sequence.ABI;
using Sequence.EcosystemWallet.Primitives.Common;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Preserve]
    public class Attestation
    {
        public static readonly byte[] ACCEPT_IMPLICIT_REQUEST_MAGIC_PREFIX =
            SequenceCoder.KeccakHash("acceptImplicitRequest".ToByteArray());
        
        public Address approvedSigner;
        public Bytes identityType;
        public Bytes issuerHash;
        public Bytes audienceHash;
        public Bytes applicationData;
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
            byte[] applicationDataLengthBytes = applicationData.Data.Length.ByteArrayFromNumber(3);
            
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
                applicationData = applicationData.Data.ByteArrayToHexStringWithPrefix(),
                authData = new
                {
                    redirectUrl = authData.redirectUrl,
                    issuedAt = authData.issuedAt.ToString()
                }
            };
        }

        public byte[] GenerateImplicitRequestMagic(Address wallet)
        {
            byte[] walletBytes = wallet.Value.HexStringToByteArray(20);
            return ByteArrayExtensions.ConcatenateByteArrays(ACCEPT_IMPLICIT_REQUEST_MAGIC_PREFIX, walletBytes,
                audienceHash.Data, issuerHash.Data);
        }
    }
}