using System;
using System.Numerics;
using Sequence.ABI;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    public class Domain
    {
        public string name;
        public string version;
        public BigInteger chainId;
        public Address verifyingContract;
        public FixedByte salt;

        public Domain(string name, string version, BigInteger chainId, Address verifyingContract, FixedByte salt = null)
        {
            this.name = name;
            this.version = version;
            this.chainId = chainId;
            this.verifyingContract = verifyingContract;
            
            if (salt != null)
            {
                if (!salt.IsFixedSize || salt.Count != 32)
                {
                    throw new ArgumentException($"If provided, {nameof(salt)} must be a fixed byte array of length 32.");
                }
                this.salt = salt;
            }
        }

        public Domain(string name, string version, Chain chain, Address verifyingContract, FixedByte salt = null)
        {
            this.name = name;
            this.version = version;
            this.chainId = BigInteger.Parse(ChainDictionaries.ChainIdOf[chain]);
            this.verifyingContract = verifyingContract;
            
            if (salt != null)
            {
                if (!salt.IsFixedSize || salt.Count != 32)
                {
                    throw new ArgumentException($"If provided, {nameof(salt)} must be a fixed byte array of length 32.");
                }
                this.salt = salt;
            }
        }

        public byte[] GetDomainSeparator()
        {
            string encodeType =
                "EIP712Domain(string name,string version,uint256 chainId,address verifyingContract,bytes32 salt)";
            byte[] typeHash = SequenceCoder.KeccakHash(encodeType).HexStringToByteArray();
            byte[] nameHash = new TupleCoder().EncodeToString(name, new[] { "string" }).HexStringToByteArray();
            byte[] versionHash = new TupleCoder().EncodeToString(version, new[] { "string" }).HexStringToByteArray();
            byte[] chainIdHash = new NumberCoder().Encode(chainId);
            byte[] verifyingContractHash = new AddressCoder().Encode(verifyingContract);
            byte[] saltHash = salt != null ? new FixedBytesCoder().Encode(salt) : null;
            byte[] encoded = ByteArrayExtensions.ConcatenateByteArrays(typeHash, nameHash, versionHash,
                chainIdHash, verifyingContractHash, saltHash);
            return encoded;
        }
    }
}