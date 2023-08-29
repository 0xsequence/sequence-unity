using System;
using System.Threading.Tasks;

namespace Sequence.Demo
{
    public interface ITokenContentFetcher
    {
        public event Action<FetchTokenContentResult> OnTokenFetchSuccess; 
        public Task FetchContent(int maxToFetch);
    }

    public class FetchTokenContentResult
    {
        public TokenElement[] Content;
        public bool MoreToFetch;

        public FetchTokenContentResult(TokenElement[] content, bool moreToFetch)
        {
            this.Content = content;
            this.MoreToFetch = moreToFetch;
        }
    }
}