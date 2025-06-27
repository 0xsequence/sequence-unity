namespace Sequence.EcosystemWallet.Primitives
{
    public class SignedSignerLeaf : SignerLeaf
    {
        public SignatureType signature;

        public override byte[] Encode(bool noChainId, byte[] checkpointerData)
        {
            return signature.Encode(this);
        }
    }
}