namespace Sequence.Relayer
{
    public class MetaTxn
    {
        public Address walletAddress;
        public string contract;
        public string input;

        public MetaTxn(Address walletAddress, string contract, string input)
        {
            this.walletAddress = walletAddress;
            this.contract = contract;
            this.input = input;
        }
    }
}