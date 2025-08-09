namespace Sequence.EcosystemWallet
{
    public struct TransactionData
    {
        public Address To;
        public string Data;

        public TransactionData(Address to, string data)
        {
            To = to;
            Data = data;
        }
    }
}