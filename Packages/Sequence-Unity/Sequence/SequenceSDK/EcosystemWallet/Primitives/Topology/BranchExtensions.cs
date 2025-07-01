using System.Linq;

namespace Sequence.EcosystemWallet.Primitives
{
    public static class BranchExtensions
    {
        public static object ToJsonObject(this IBranch branch)
        {
            return branch.Children.Select(child => child.ToJsonObject()).ToArray();
        }
    }
}