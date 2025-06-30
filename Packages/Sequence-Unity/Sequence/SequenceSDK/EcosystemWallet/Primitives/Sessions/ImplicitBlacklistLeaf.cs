namespace Sequence.EcosystemWallet.Primitives
{
    public class ImplicitBlacklistLeaf : SessionLeaf
    {
        public Address[] Blacklist;

        public override object ToJson()
        {
            return new
            {
                type = ImplicitBlacklistType,
                blacklist = Blacklist,
            };
        }

        public override byte[] Encode()
        {
            throw new System.NotImplementedException();
        }
    }
}