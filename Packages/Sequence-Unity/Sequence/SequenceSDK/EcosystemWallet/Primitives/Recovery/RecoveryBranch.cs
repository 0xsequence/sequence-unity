using System;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class RecoveryBranch : IBranch
    {
        public ITopology[] Children { get; }
        
        public RecoveryTopology Left => Children[0] as RecoveryTopology;
        public RecoveryTopology Right => Children[1] as RecoveryTopology;

        public RecoveryBranch(RecoveryTopology left, RecoveryTopology right)
        {
            Children = new ITopology[] { left, right };
        }
        
        public byte[] Encode()
        {
            var encodedLeft = Left.Encode();
            var encodedRight = Right.Encode();

            if (!Right.IsBranch())
                return ByteArrayExtensions.ConcatenateByteArrays(encodedLeft, encodedRight);
            
            if (encodedRight.Length > 16777215)
                throw new Exception("Branch too large");
            
            return ByteArrayExtensions.ConcatenateByteArrays(
                encodedLeft,
                RecoveryTopology.FlagBranch.ByteArrayFromNumber(1), 
                encodedRight.Length.ByteArrayFromNumber(3), 
                encodedRight);
        }
    }
}