using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet
{
    public interface IPermissions
    {
        SessionPermissions GetPermissions();
    }
}