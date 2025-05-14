namespace Sequence.EcosystemWallet.Primitives
{
    internal class RawSignature
    {
        public bool noChainId;
        public byte[] checkpointerData;
        public RawConfig configuration;
        public RawSignature[] suffix;
        public Erc6492 erc6492;

        public class Erc6492
        {
            public Address to;
            public byte[] data;
        }
    }
}