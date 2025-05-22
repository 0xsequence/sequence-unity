namespace Sequence.EcosystemWallet.Primitives
{
    public abstract class Payload
    {
        public abstract PayloadType type { get; }
        public Address[] parentWallets { get; set; }

        public bool isCalls => type == PayloadType.Call;
        public bool isMessage => type == PayloadType.Message;
        public bool isConfigUpdate => type == PayloadType.ConfigUpdate;
        public bool isDigest => type == PayloadType.Digest;

        public abstract byte[] GetEIP712EncodeData();
    }
}