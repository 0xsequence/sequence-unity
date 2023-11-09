using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Sequence.Demo
{
    public interface INftContentFetcher
    {
        public event Action<FetchNftContentResult> OnNftFetchSuccess; 
        public Task FetchContent(int maxToFetch);
        public void Refresh();
    }

    public class FetchNftContentResult
    {
        public NftElement[] Content;
        public bool MoreToFetch;

        public FetchNftContentResult(NftElement[] content, bool moreToFetch)
        {
            this.Content = content;
            this.MoreToFetch = moreToFetch;
        }
    }
}