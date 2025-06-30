namespace Sequence.EcosystemWallet.Primitives
{
    public abstract class SessionLeaf
    {
        public const string ImplicitBlacklistType = "implicit-blacklist";
        public const string IdentitySignerType = "identity-signer";
        public const string SessionPermissionsType = "session-permissions";

        public abstract byte[] Encode();
    }
}