namespace Sequence.EcosystemWallet.Primitives
{
    public class ImplicitBlacklistLeaf : SessionLeaf
    {
        public Address[] Blacklist;
        
        public override byte[] Encode()
        {
            throw new System.NotImplementedException();
        }
    }
}