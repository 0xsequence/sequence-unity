using System;
using System.Numerics;
using Sequence.ABI;

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
    }
}