using System;
using System.Collections.Generic;
using System.Numerics;
using Sequence.Core.Signature;
using Sequence.Core.Wallet;

namespace Sequence.Core.V2.Signature.Tree
{
    public class SignatureTreeECDSASignatureLeaf : ISignatureTree
    {
        private enum ECDSASignatureType
        {
            ECDSASignatureTypeEIP712 = 1,
            ECDSASignatureTypeEthSign = 2
        }

        public static int SignatureLength = 0;
        public int weight;
        private ECDSASignatureType type;
        public byte[] signature = new byte[SignatureLength];

        public (IWalletConfigTree, BigInteger) Recover(WalletContext context, Subdigest subdigest, RPCProvider provider, List<SignerSignatures> signerSignatures)
        {
            throw new NotImplementedException();
        }
    }
}
