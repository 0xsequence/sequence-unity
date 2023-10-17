using System;
using System.Threading.Tasks;

namespace Sequence.Demo
{
    public interface IContentFetcher
    {
        public event Action<FetchContentResult> OnContentFetchSuccess;
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
}