using System.Numerics;
using Sequence.Utils;

namespace Sequence
{
    public static class ChainId
    {
        public static string AsString(this Chain chain)
        {
            BigInteger chainId = (BigInteger)(int)chain;
            return chainId.BigIntegerToHexString();
        }
    }
}