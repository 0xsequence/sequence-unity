using System;
using System.Collections.Generic;
using System.Numerics;
using Sequence.Core.Signature;

namespace Sequence.Core.V2.Wallet.ConfigTree
{
    public class WalletConfigTreeNode : IWalletConfigTree
    {
        public IWalletConfigTree Left { get; set; }
        public IWalletConfigTree Right { get; set; }

        public ISignatureTree BuildSignatureTree(Dictionary<Address, SignerSignature> signerSignatures)
        {
            throw new NotImplementedException();
        }

        public ImageHash ImageHash()
        {
            throw new NotImplementedException();
        }

        public BigInteger MaxWeight()
        {
            throw new NotImplementedException();
        }

        public Dictionary<Address, ushort> ReadSignersIntoMap()
        {
            throw new NotImplementedException();
        }

        public BigInteger UnverifiedWeight(Dictionary<Address, ushort> signers)
        {
            throw new NotImplementedException();
        }
    }
}
