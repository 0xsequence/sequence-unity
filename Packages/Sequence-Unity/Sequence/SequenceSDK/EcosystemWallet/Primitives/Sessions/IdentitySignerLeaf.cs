namespace Sequence.EcosystemWallet.Primitives
{
    public class IdentitySignerLeaf : SessionLeaf
    {
        public Address IdentitySigner;
        
        public override byte[] Encode()
        {
            throw new System.NotImplementedException();
        }
    }
}