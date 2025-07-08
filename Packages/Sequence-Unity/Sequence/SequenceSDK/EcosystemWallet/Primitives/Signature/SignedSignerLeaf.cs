namespace Sequence.EcosystemWallet.Primitives
{
    public class SignedSignerLeaf : SignerLeaf
    {
        public SignatureOfLeaf signature;

        public override byte[] Encode(bool noChainId, byte[] checkpointerData)
        {
            return signature.Encode(this);
        }
    }
}