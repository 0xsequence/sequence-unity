using System;
using System.Linq;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class ImplicitBlacklistLeaf : SessionLeaf
    {
        public Address[] blacklist;

        public override object ToJson()
        {
            return new
            {
                type = ImplicitBlacklistType,
                blacklist = blacklist.Select(a => a.Value).ToArray() ,
            };
        }

        public override byte[] Encode()
        {
            var encoded = EncodeBlacklist();

            var count = blacklist.Length;
            if (count >= 0x0f)
            {
                if (count > 0xffff)
                    throw new Exception("Blacklist too large");

                var flag = (SessionsTopology.FlagBlacklist << 4) | 0x0f;
                return ByteArrayExtensions.ConcatenateByteArrays(
                    flag.ByteArrayFromNumber(flag.MinBytesFor()),
                    count.ByteArrayFromNumber(2),
                    encoded
                );
            }

            var flagByte = (SessionsTopology.FlagBlacklist << 4) | count;
            return ByteArrayExtensions.ConcatenateByteArrays(flagByte.ByteArrayFromNumber(flagByte.MinBytesFor()), encoded);
        }
        
        public override byte[] EncodeRaw()
        {
            return ByteArrayExtensions.ConcatenateByteArrays(
                SessionsTopology.FlagBlacklist.ByteArrayFromNumber(1), 
                EncodeBlacklist());
        }

        private byte[] EncodeBlacklist()
        {
            return ByteArrayExtensions.ConcatenateByteArrays(blacklist
                .Select(hex => hex.Value.HexStringToByteArray())
                .ToArray());
        }
    }
}