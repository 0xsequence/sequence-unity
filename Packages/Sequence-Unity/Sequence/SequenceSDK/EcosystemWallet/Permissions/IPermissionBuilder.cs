using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet
{
    public interface IPermissionBuilder
    {
        SessionPermissions GetPermissions();
    }
}