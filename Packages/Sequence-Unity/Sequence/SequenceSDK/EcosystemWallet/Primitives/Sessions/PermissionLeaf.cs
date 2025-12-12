using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class PermissionLeaf : SessionLeaf
    {
        public SessionPermissions permissions;
        
        public override object ToJsonObject()
        {
            return permissions.ToJson();
        }

        public override byte[] Encode()
        {
            var flag = SessionsTopology.FlagPermissions << 4;
            return ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(flag.MinBytesFor()), permissions.Encode());
        }

        public override byte[] EncodeRaw()
        {
            return ByteArrayExtensions.ConcatenateByteArrays(SessionsTopology.FlagPermissions.ByteArrayFromNumber(1), permissions.Encode());
        }
    }
}