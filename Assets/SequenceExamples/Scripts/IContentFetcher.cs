using System;
using System.Threading.Tasks;

namespace Sequence.Demo
{
    public interface IContentFetcher
    {
        /// <summary>
        /// Fired when wallet content, fungible tokens or owned NFT contracts, are fetched
        /// </summary>
        public event Action<FetchContentResult> OnContentFetch;
        
        /// <summary>
        /// Fired when a collection is processed; i.e. when we fetch the NFTs owned for a given collection contract address
        /// </summary>
        public event Action<CollectionProcessingResult> OnCollectionProcessing; 
        public Task FetchContent(int pageSize);
        public Task<FetchTokenContentResult> FetchTokenContent(int maxToFetch);
        public Task<FetchNftContentResult> FetchNftContent(int maxToFetch);
        public Address GetAddress();
    }
    
    public class FetchContentResult
    {
        public TokenBalance[] Content;
        public bool MoreToFetch;
        
        public FetchContentResult(TokenBalance[] content, bool moreToFetch)
        {
            this.Content = content;
            this.MoreToFetch = moreToFetch;
        }
    }

    public class CollectionProcessingResult
    {
        public TokenBalance[] Content;
        public bool MoreToFetch;
        
        public CollectionProcessingResult(TokenBalance[] content, bool moreToFetch)
        {
            this.Content = content;
            this.MoreToFetch = moreToFetch;
        }
    }
}