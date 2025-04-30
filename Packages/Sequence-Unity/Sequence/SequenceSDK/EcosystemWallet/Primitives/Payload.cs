namespace Sequence.EcosystemWallet.Primitives
{
    internal abstract class Payload
    {
        public abstract PayloadType type { get; }
        public Address[] parentWallets { get; set; }

        public bool isCalls => type == PayloadType.Call;
        public bool isMessage => type == PayloadType.Message;
        public bool isConfigUpdate => type == PayloadType.ConfigUpdate;
        public bool isDigest => type == PayloadType.Digest;
    }
}