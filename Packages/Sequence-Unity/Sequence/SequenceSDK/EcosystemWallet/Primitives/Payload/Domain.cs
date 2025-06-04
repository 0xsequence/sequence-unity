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
        
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Domain))
            {
                return false;
            }
            
            Domain other = (Domain)obj;
            if (salt != null && other.salt != null)
            {
                if (!salt.Equals(other.salt))
                {
                    return false;
                }
            }
            else if (salt != null && other.salt == null ||
                     salt == null && other.salt != null)
            {
                return false;
            }
            return name.Equals(other.name) && version.Equals(other.version) && chainId.Equals(other.chainId) &&
                   verifyingContract.Equals(other.verifyingContract);
        }
        
        public override string ToString()
        {
            string saltStr = salt != null ? salt.ToString() : "null";
            return $"Domain {{ name: {name}, version: {version}, chainId: {chainId}, verifyingContract: {verifyingContract}, salt: {saltStr} }}";
        }

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
            byte[] typeHash = SequenceCoder.KeccakHashASCII(encodeType).HexStringToByteArray();
            byte[] nameHash = name.EncodeStringAsBytes();
            byte[] versionHash = version.EncodeStringAsBytes();
            byte[] chainIdHash = new NumberCoder().Encode(chainId);
            byte[] verifyingContractHash = new AddressCoder().Encode(verifyingContract);
            byte[] saltHash = salt != null ? new FixedBytesCoder().Encode(salt) : new byte[]{};
            byte[] encoded = ByteArrayExtensions.ConcatenateByteArrays(typeHash, nameHash, versionHash,
                chainIdHash, verifyingContractHash, saltHash);
            return encoded;
        }
    }
}