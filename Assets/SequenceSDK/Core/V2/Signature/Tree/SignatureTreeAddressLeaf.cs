using System;
using System.Collections.Generic;
using System.Numerics;
using Sequence.Core.Provider;
using Sequence.Core.Signature;
using Sequence.Core.Wallet;

namespace Sequence.Core.V2.Signature.Tree
{
    public class SignatureTreeAddressLeaf : ISignatureTree
    {
        public int weight;
        public string address;

        public (IWalletConfigTree, BigInteger) Recover(WalletContext context, Subdigest subdigest, RPCProvider provider, List<SignerSignatures> signerSignatures)
        {
            throw new NotImplementedException();
        }
    }
}
