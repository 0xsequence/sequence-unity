using System;
using System.Collections.Generic;
using System.Numerics;
using Sequence.Core;
using Sequence.Core.Signature;
using Sequence.Core.Wallet;

namespace Sequence.Core.V2.Signature.Tree
{
    public class SignatureTreeNestedLeaf : ISignatureTree
    {
        public int weight;
        public int threshold;
        public ISignatureTree tree;

        public (IWalletConfigTree, BigInteger) Recover(WalletContext context, Subdigest subdigest, RPCProvider provider, List<SignerSignatures> signerSignatures)
        {
            throw new NotImplementedException();
        }
    }
}
