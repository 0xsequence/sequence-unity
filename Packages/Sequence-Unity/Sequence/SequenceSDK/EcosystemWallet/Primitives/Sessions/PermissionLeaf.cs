namespace Sequence.EcosystemWallet.Primitives
{
    public class PermissionLeaf : SessionLeaf
    {
        public override object ToJson()
        {
            return new
            {
                type = SessionPermissionsType
            };
        }

        public override byte[] Encode()
        {
            throw new System.NotImplementedException();
        }
    }
}