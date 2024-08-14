using System;
using System.Threading.Tasks;
using Sequence.Utils;

namespace Sequence.Marketplace
{
    public class ListCollectibles
    {
        private Chain _chain;
        private HttpClient _client;
        
        public ListCollectibles(Chain chain)
        {
            _chain = chain;
            _client = new HttpClient();
        }
        
        public Action<ListCollectiblesWithLowestListingReturn> OnListCollectiblesWithLowestListingReturn;
        public Action<string> OnListCollectiblesWithLowestListingError;
        public async Task<ListCollectiblesWithLowestListingReturn> ListCollectiblesWithLowestListing(string contractAddress, CollectiblesFilter filter = default, Page page = default)
        {
            string endpoint = "ListCollectiblesWithLowestListing";
            ListCollectiblesWithLowestListingArgs args =
                new ListCollectiblesWithLowestListingArgs(contractAddress, filter, page);

            try
            {
                ListCollectiblesWithLowestListingReturn result =
                    await _client
                        .SendRequest<ListCollectiblesWithLowestListingArgs, ListCollectiblesWithLowestListingReturn>(
                            _chain, endpoint, args);
                OnListCollectiblesWithLowestListingReturn?.Invoke(result);
                return result;
            }
            catch (Exception e)
            {
                OnListCollectiblesWithLowestListingError?.Invoke(e.Message);
                return null;
            }
        }

        public async Task<CollectibleOrder[]> ListAllCollectibleWithLowestListing(string contractAddress,
            CollectiblesFilter filter = default)
        {
            ListCollectiblesWithLowestListingReturn result = await ListCollectiblesWithLowestListing(contractAddress, filter);
            if (result == null)
            {
                return null;
            }

            CollectibleOrder[] collectibles = result.collectibles;
            while (result.page != null && result.page.more)
            {
                collectibles = ArrayUtils.CombineArrays(collectibles, result.collectibles);
                result = await ListCollectiblesWithLowestListing(contractAddress, filter, result.page);
                if (result == null)
                {
                    return collectibles;
                }
            }

            return collectibles;
        }
    }
}