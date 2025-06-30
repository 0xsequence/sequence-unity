using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class PermissionLeaf : SessionLeaf
    {
        public SessionPermissions permissions;
        
        public override object ToJson()
        {
            return new
            {
                type = SessionPermissionsType,
                signer = permissions.signer,
                valueLimit = permissions.valueLimit,
                deadline = permissions.deadline,
                permissions = permissions.permissions,
            };
        }

        public override byte[] Encode()
        {
            var flag = SessionsTopology.FlagPermissions << 4;
            return ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(flag.MinBytesFor()), permissions.Encode());
        }
    }
}