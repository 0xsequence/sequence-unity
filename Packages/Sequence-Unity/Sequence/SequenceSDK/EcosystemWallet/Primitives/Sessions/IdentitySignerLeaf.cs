namespace Sequence.EcosystemWallet.Primitives
{
    public class IdentitySignerLeaf : SessionLeaf
    {
        public Address IdentitySigner;

        public override object ToJson()
        {
            return new
            {
                type = IdentitySignerType,
                identitySigner = IdentitySigner,
            };
        }

        public override byte[] Encode()
        {
            throw new System.NotImplementedException();
        }
    }
}