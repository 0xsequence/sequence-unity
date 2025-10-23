using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet
{
    public interface ITransaction
    {
        Call GetCall();
    }
}