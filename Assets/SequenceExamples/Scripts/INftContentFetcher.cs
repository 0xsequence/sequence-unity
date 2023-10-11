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
        public Texture2D[] Content;
        public bool MoreToFetch;

        public FetchNftContentResult(Texture2D[] content, bool moreToFetch)
        {
            this.Content = content;
            this.MoreToFetch = moreToFetch;
        }
    }
}