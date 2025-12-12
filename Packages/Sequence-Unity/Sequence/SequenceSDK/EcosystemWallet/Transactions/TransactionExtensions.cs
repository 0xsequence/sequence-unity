using System.Linq;
using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet
{
    internal static class TransactionExtensions
    {
        public static Call[] GetCalls(this ITransaction[] transactions)
        {
            return transactions.Select(t => t.GetCall()).ToArray();
        }
    }
}