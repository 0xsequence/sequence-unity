namespace Sequence.Marketplace
{
    public class CartItemData
    {
        public string Name;
        public string TokenId;
        public Address Collection;
        public Chain Network;
        
        public CartItemData(string name, string tokenId, Address collection, Chain network)
        {
            Name = name;
            TokenId = tokenId;
            Collection = collection;
            Network = network;
        }
    }
}