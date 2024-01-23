using System.Numerics;
using Sequence.Utils;

namespace Sequence
{
    public static class ChainId
    {
        public static string AsHexString(this Chain chain)
        {
            BigInteger chainId = (BigInteger)(int)chain;
            return chainId.BigIntegerToHexString();
        }
        
        public static Chain ChainFromHexString(this string hexString)
        {
            BigInteger chainId = hexString.HexStringToBigInteger();
            return (Chain)(int)chainId;
        }
    }
}