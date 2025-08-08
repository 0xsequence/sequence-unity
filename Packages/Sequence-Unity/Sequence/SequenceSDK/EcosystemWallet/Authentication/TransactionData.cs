namespace Sequence.EcosystemWallet
{
    public struct TransactionData
    {
        public Address To;
        public byte[] Data;

        public TransactionData(Address to, byte[] data)
        {
            To = to;
            Data = data;
        }
    }
}