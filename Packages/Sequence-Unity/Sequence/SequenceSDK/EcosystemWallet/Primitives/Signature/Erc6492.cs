namespace Sequence.EcosystemWallet.Primitives
{
    public class Erc6492
    {
        public Address to;
        public byte[] data;

        public Erc6492(Address to, byte[] data)
        {
            this.to = to;
            this.data = data;
        }
    }
}