using System;
using Sequence.ABI;

namespace Sequence.EcosystemWallet.Primitives
{
    internal class Attestation
    {
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
    }
}