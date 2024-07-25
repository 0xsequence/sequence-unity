using System.Numerics;
using Sequence.Utils;

namespace Sequence
{
    public static class ChainId
    {
        public static string AsHexString(this Chain chain)
        {
            string chainIdString = chain.GetChainId();
            BigInteger chainId = BigInteger.Parse(chainIdString);
            return chainId.BigIntegerToHexString();
        }
        
        public static Chain ChainFromHexString(this string hexString)
        {
            BigInteger chainId = hexString.HexStringToBigInteger();
            string chainIdString = chainId.ToString();
            return ChainDictionaries.ChainById[chainIdString];
        }

        public static string GetChainId(this Chain chain)
        {
            return ChainDictionaries.ChainIdOf[chain];
        }
    }
}