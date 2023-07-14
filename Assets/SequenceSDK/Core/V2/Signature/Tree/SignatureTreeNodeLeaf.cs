using System;
using System.Collections.Generic;
using System.Numerics;
using Sequence.Core;
using Sequence.Core.Signature;
using Sequence.Core.Wallet;

namespace Sequence.Core.V2.Signature.Tree
{
    public class SignatureTreeNodeLeaf : ISignatureTree
    {
        public ImageHash imageHash { get; set; }

        public (IWalletConfigTree, BigInteger) Recover(WalletContext context, Subdigest subdigest, RPCProvider provider, List<SignerSignatures> signerSignatures)
        {
            throw new NotImplementedException();
        }
    }
}
