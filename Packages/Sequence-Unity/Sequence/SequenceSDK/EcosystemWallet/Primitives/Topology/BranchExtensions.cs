using System;
using System.Linq;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public static class BranchExtensions
    {
        public static object ToJsonObject(this IBranch branch)
        {
            return branch.Children.Select(child => child.ToJsonObject()).ToArray();
        }

        public static byte[] Encode(this IBranch branch)
        {
            var encodings = branch.Children.Select(child => child.Encode()).ToArray();
            var encoded = ByteArrayExtensions.ConcatenateByteArrays(encodings);
            var encodedSize = encoded.Length.MinBytesFor();
            if (encodedSize > 15)
                throw new Exception("Session Branch is too large.");
            
            var flag = (SessionsTopology.FlagBranch << 4) | encodedSize;
            
            return ByteArrayExtensions.ConcatenateByteArrays(
                flag.ByteArrayFromNumber(flag.MinBytesFor()), 
                encoded.Length.ByteArrayFromNumber(encodedSize), 
                encoded);
        }
    }
}