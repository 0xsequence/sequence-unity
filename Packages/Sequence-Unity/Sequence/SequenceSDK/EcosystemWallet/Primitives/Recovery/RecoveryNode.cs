using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class RecoveryNode : INode
    {
        public byte[] Value { get; set; }
        
        public object ToJsonObject()
        {
            return Value.ByteArrayToHexStringWithPrefix();
        }

        public byte[] Encode()
        {
            return ByteArrayExtensions.ConcatenateByteArrays(
                RecoveryTopology.FlagNode.ByteArrayFromNumber(1), 
                Value);
        }
    }
}