using System;
using System.Threading.Tasks;

namespace Sequence.Marketplace
{
    public interface IReader
    {
        public event Action<Currency[]> OnListCurrenciesReturn;
        public event Action<string> OnListCurrenciesError;
        
        /// <summary>
        /// Fetch an array of whitelisted Currencies (ERC20 tokens) that can be used in the marketplace.
        /// </summary>
        /// <returns></returns>
        public Task<Currency[]> ListCurrencies();

        public event Action<ListCollectiblesReturn> OnListCollectibleOrdersReturn;
        public event Action<string> OnListCollectibleOrdersError;
        
        /// <summary>
        /// List collectible listings for a given contract address with the listings in the array sorted in terms of increasing prices.
        /// </summary>
        /// <param name="contractAddress">the collection contract address</param>
        /// <param name="filter">apply filters on the queried orders</param>
        /// <param name="page">used for response pagination</param>
        /// <returns></returns>
        public Task<ListCollectiblesReturn> ListCollectibleListingsWithLowestPricedListingsFirst(string contractAddress,
            CollectiblesFilter filter = default, Page page = default);

        /// <summary>
        /// List collectible listings for a given contract address with the listings in the array sorted in terms of increasing prices.
        ///
        /// Same as ListCollectibleListingsWithLowestPricedListingsFirst except we will continue to make the requests until we have all listings.
        /// Useful helper method to avoid having to deal with pagination.
        /// Be careful to use this method only when you are sure that the number of listings is not too large or it will lead to a long wait time and high memory usage.
        /// </summary>
        /// <param name="contractAddress">the collection contract address</param>
        /// <param name="filter">apply filters on the queried orders</param>
        /// <returns></returns>
        public Task<CollectibleOrder[]> ListAllCollectibleListingsWithLowestPricedListingsFirst(string contractAddress,
            CollectiblesFilter filter = default);
        
        /// <summary>
        /// List collectible offers for a given contract address with the offers in the array sorted in terms of decreasing prices.
        /// </summary>
        /// <param name="contractAddress">the collection contract address</param>
        /// <param name="filter">apply filters on the queried orders</param>
        /// <param name="page">used for response pagination</param>
        /// <returns></returns>
        public Task<ListCollectiblesReturn> ListCollectibleOffersWithHighestPricedOfferFirst(string contractAddress,
            CollectiblesFilter filter = default, Page page = default);

        /// <summary>
        /// List collectible offers for a given contract address with the offers in the array sorted in terms of decreasing prices.
        ///
        /// Same as ListCollectibleOffersWithHighestPricedOfferFirst except we will continue to make the requests until we have all offers.
        /// Useful helper method to avoid having to deal with pagination.
        /// Be careful to use this method only when you are sure that the number of offers is not too large or it will lead to a long wait time and high memory usage.
        /// </summary>
        /// <param name="contractAddress">the collection contract address</param>
        /// <param name="filter">apply filters on the queried orders</param>
        /// <returns></returns>
        public Task<CollectibleOrder[]> ListAllCollectibleOffersWithHighestPricedOfferFirst(string contractAddress,
            CollectiblesFilter filter = default);
        
        public event Action<TokenMetadata> OnGetCollectibleReturn;
        public event Action<string> OnGetCollectibleError;

        /// <summary>
        /// Fetch the collectible/TokenMetadata associated with the given contract address and token id
        /// </summary>
        /// <param name="contractAddress">the collection contract address</param>
        /// <param name="tokenId">the collectible token id</param>
        /// <returns></returns>
        public Task<TokenMetadata> GetCollectible(Address contractAddress, string tokenId);
        
        public event Action<Order> OnGetCollectibleOrderReturn;
        public event Action<string> OnGetCollectibleOrderError;

        /// <summary>
        /// Fetch just the lowest priced offer for a collectible (identified by given contract address and token id).
        /// </summary>
        /// <param name="contractAddress">the collection contract address</param>
        /// <param name="tokenId">the collectible token id</param>
        /// <param name="filter">apply filters on the queried orders</param>
        /// <returns></returns>
        public Task<Order> GetLowestPriceOfferForCollectible(Address contractAddress, string tokenId,
            OrderFilter filter = null);

        /// <summary>
        /// Fetch just the highest priced offer for a collectible (identified by given contract address and token id).
        /// </summary>
        /// <param name="contractAddress">the collection contract address</param>
        /// <param name="tokenId">the collectible token id</param>
        /// <param name="filter">apply filters on the queried orders</param>
        /// <returns></returns>
        public Task<Order> GetHighestPriceOfferForCollectible(Address contractAddress, string tokenId,
            OrderFilter filter = null);

        /// <summary>
        /// Fetch just the lowest priced listing for a collectible (identified by given contract address and token id).
        /// </summary>
        /// <param name="contractAddress">the collection contract address</param>
        /// <param name="tokenId">the collectible token id</param>
        /// <param name="filter">apply filters on the queried orders</param>
        /// <returns></returns>
        public Task<Order> GetLowestPriceListingForCollectible(Address contractAddress, string tokenId,
            OrderFilter filter = null);

        /// <summary>
        /// Fetch just the highest priced listing for a collectible (identified by given contract address and token id).
        /// </summary>
        /// <param name="contractAddress">the collection contract address</param>
        /// <param name="tokenId">the collectible token id</param>
        /// <param name="filter">apply filters on the queried orders</param>
        /// <returns></returns>
        public Task<Order> GetHighestPriceListingForCollectible(Address contractAddress, string tokenId,
            OrderFilter filter = null);
        
        public event Action<ListCollectibleListingsReturn> OnListCollectibleListingsReturn;
        public event Action<string> OnListCollectibleListingsError;

        /// <summary>
        /// List all of the listings for a collectible (identified by given contract address and token id).
        /// </summary>
        /// <param name="contractAddress">the collection contract address</param>
        /// <param name="tokenId">the collectible token id</param>
        /// <param name="filter">apply filters on the queried orders</param>
        /// <param name="page">used for response pagination</param>
        /// <returns></returns>
        public Task<ListCollectibleListingsReturn> ListListingsForCollectible(Address contractAddress,
            string tokenId, OrderFilter filter = null, Page page = null);


        /// <summary>
        /// List all of the listings for a collectible (identified by given contract address and token id).
        ///
        /// Same as ListListingsForCollectible except we will continue to make the requests until we have all listings.
        /// Useful helper method to avoid having to deal with pagination.
        /// Be careful to use this method only when you are sure that the number of listings is not too large or it will lead to a long wait time and high memory usage.
        /// </summary>
        /// <param name="contractAddress">the collection contract address</param>
        /// <param name="tokenId">the collectible token id</param>
        /// <param name="filter">apply filters on the queried orders</param>
        /// <returns></returns>
        public Task<Order[]> ListAllListingsForCollectible(Address contractAddress, string tokenId,
            OrderFilter filter = null);
        
        public event Action<ListCollectibleOffersReturn> OnListCollectibleOffersReturn;
        public event Action<string> OnListCollectibleOffersError;

        /// <summary>
        /// List all of the offers for a collectible (identified by given contract address and token id).
        /// </summary>
        /// <param name="contractAddress">the collection contract address</param>
        /// <param name="tokenId">the collectible token id</param>
        /// <param name="filter">apply filters on the queried orders</param>
        /// <param name="page">used for response pagination</param>
        /// <returns></returns>
        public Task<ListCollectibleOffersReturn> ListOffersForCollectible(Address contractAddress,
            string tokenId, OrderFilter filter = null, Page page = null);


        /// <summary>
        /// List all of the offers for a collectible (identified by given contract address and token id).
        ///
        /// Same as ListOffersForCollectible except we will continue to make the requests until we have all offers.
        /// Useful helper method to avoid having to deal with pagination.
        /// Be careful to use this method only when you are sure that the number of offers is not too large or it will lead to a long wait time and high memory usage.
        /// </summary>
        /// <param name="contractAddress">the collection contract address</param>
        /// <param name="tokenId">the collectible token id</param>
        /// <param name="filter">apply filters on the queried orders</param>
        /// <returns></returns>
        public Task<Order[]> ListAllOffersForCollectible(Address contractAddress, string tokenId,
            OrderFilter filter = null);

        /// <summary>
        /// Fetch the listing with the lowest price on a given collection
        /// </summary>
        /// <param name="contractAddress">the collection contract address</param>
        /// <param name="filter">apply filters on the queried orders</param>
        /// <returns></returns>
        public Task<CollectibleOrder> GetFloorOrder(Address contractAddress, CollectiblesFilter filter = null);

        /// <summary>
        /// List collectible offers for a given contract address with the offers in the array sorted in terms of decreasing prices. Apply filters such that:
        /// - sellableBy has the collectibles the offers are looking to buy
        /// - sellableBy did not create the offers and is therefore able to fill the offers
        ///
        /// Same as ListAllCollectibleOffersWithHighestPricedOfferFirst except we apply additional filters (above).
        /// Useful helper method to avoid having to deal with pagination and filtering.
        /// Be careful to use this method only when you are sure that the number of offers is not too large or it will lead to a long wait time and high memory usage.
        /// </summary>
        /// <param name="sellableBy">wallet address of the user looking to sell collectibles</param>
        /// <param name="collection">the collection contract address</param>
        /// <param name="filter">apply filters on the queried orders</param>
        /// <returns></returns>
        public Task<CollectibleOrder[]> ListAllSellableOffers(Address sellableBy, Address collection,
            CollectiblesFilter filter = default);

        /// <summary>
        /// List collectible listings for a given contract address with the listings in the array sorted in terms of increasing prices. Apply filters such that:
        /// - purchasableBy did not create the listings and is therefore able to fill the listings
        /// Then, filter out any listings that the user cannot afford after fetching their token balances using the provided indexer
        ///
        /// Similar to ListAllCollectibleListingsWithLowestPricedListingsFirst except we apply additional filters (above).
        /// Useful helper method to avoid having to deal with pagination, filtering, and fetching token balances.
        /// Be careful to use this method only when you are sure that the number of listings is not too large or it will lead to a long wait time and high memory usage.
        ///
        /// Then, filter out any listings that the user cannot afford after fetching their token balances using the provided indexer
        /// </summary>
        /// <param name="purchasableBy">wallet address of the user looking to sell collectibles</param>
        /// <param name="collection">the collection contract address</param>
        /// <param name="indexer">the indexer used to query token balances - one will be created for you if none is provided</param>
        /// <param name="filter">apply filters on the queried orders</param>
        /// <returns></returns>
        public Task<CollectibleOrder[]> ListAllPurchasableListings(Address purchasableBy, Address collection,
            IIndexer indexer = null, CollectiblesFilter filter = null);
    }
}