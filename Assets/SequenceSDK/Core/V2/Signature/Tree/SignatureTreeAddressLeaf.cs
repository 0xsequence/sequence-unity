using System;
using System.Collections.Generic;
using System.Numerics;
using Sequence.Core;
using Sequence.Core.Signature;
using Sequence.Core.Wallet;

namespace Sequence.Core.V2.Signature.Tree
{
    public class SignatureTreeAddressLeaf : ISignatureTree
    {
        public int weight;
        public string address;

        public byte[] Data()
        {
            throw new NotImplementedException();
        }

        public ISignatureTree Join(ISignatureTree otherSignatureTree)
        {
            throw new NotImplementedException();
        }

        public (IWalletConfigTree, BigInteger) Recover(WalletContext context, Subdigest subdigest, RPCProvider provider, params SignerSignatures[] signerSignatures)
        {
            throw new NotImplementedException();
        }

        public ISignatureTree Reduce()
        {
            throw new NotImplementedException();
        }

        public ImageHash ReduceImageHash()
        {
            throw new NotImplementedException();
        }
    }
}
