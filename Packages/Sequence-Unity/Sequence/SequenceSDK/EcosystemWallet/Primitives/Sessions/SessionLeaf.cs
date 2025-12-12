namespace Sequence.EcosystemWallet.Primitives
{
    public abstract class SessionLeaf : ILeaf
    {
        public const string ImplicitBlacklistType = "implicit-blacklist";
        public const string IdentitySignerType = "identity-signer";
        public const string SessionPermissionsType = "session-permissions";

        public abstract object ToJsonObject();
        public abstract byte[] Encode();
        public abstract byte[] EncodeRaw();

        public SessionsTopology ToTopology()
        {
            return new SessionsTopology(this);
        }
    }
}