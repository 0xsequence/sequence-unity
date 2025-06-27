namespace Sequence.EcosystemWallet.Primitives
{
    public class SignedSapientSignerLeaf : SapientSignerLeaf
    {
        public SignatureType signature;

        public override byte[] Encode(bool noChainId, byte[] checkpointerData)
        {
            return signature.Encode(this);
        }
    }
}