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

    public static class NativeTokenAddress
    {
        public static string Get(Chain chain)
        {
            return ChainDictionaries.NativeTokenAddressOf.TryGetValue(chain, out var address) ? address : null;
        }
        
        public static string Get(int chainId)
        {
            return Get((Chain)chainId);
        }
    }
    

}