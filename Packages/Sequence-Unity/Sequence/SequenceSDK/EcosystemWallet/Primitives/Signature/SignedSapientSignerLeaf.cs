using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SignedSapientSignerLeaf : SapientSignerLeaf
    {
        public SignatureOfLeaf signature;

        public override byte[] Encode(bool noChainId, byte[] checkpointerData)
        {
            return signature.Encode(this);
        }
    }
}