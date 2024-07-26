using System;
using System.Threading.Tasks;

namespace Sequence.Demo
{
    public class TokenContentFetcher : ITokenContentFetcher
    {
        private IContentFetcher _contentFetcher;

        public TokenContentFetcher(IContentFetcher contentFetcher)
        {
            _contentFetcher = contentFetcher;
        }

        public event Action<FetchTokenContentResult> OnTokenFetchSuccess;
        
        public async Task FetchContent(int maxToFetch)
        {
            FetchTokenContentResult result = await _contentFetcher.FetchTokenContent(maxToFetch);
            OnTokenFetchSuccess?.Invoke(result);
        }

        public void Refresh()
        {
            _contentFetcher.RefreshTokens();
        }
    }
}