namespace Sequence.Pay.Transak
{
    public class TransakContractId
    {
        public string Id;
        public Address ContractAddress;
        public Chain Chain;
        public string PriceTokenSymbol;

        public TransakContractId(string id, Address contractAddress, Chain chain, string priceTokenSymbol)
        {
            Id = id;
            ContractAddress = contractAddress;
            Chain = chain;
            PriceTokenSymbol = priceTokenSymbol;
        }
    }
}