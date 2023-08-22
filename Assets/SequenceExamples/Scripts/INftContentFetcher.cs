using System.Threading.Tasks;
using UnityEngine;

namespace Sequence.Demo
{
    public interface INftContentFetcher
    {
        public Task<Texture2D[]> FetchContent();
    }
}