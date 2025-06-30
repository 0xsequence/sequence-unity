namespace Sequence.EcosystemWallet.Primitives
{
    public class SessionNodeLeaf : SessionLeaf
    {
        public byte[] Value;
        
        public override byte[] Encode()
        {
            return Value;
        }
    }
}