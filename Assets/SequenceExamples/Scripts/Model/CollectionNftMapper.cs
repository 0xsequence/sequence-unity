using System.Collections.Generic;

namespace Sequence.Demo
{
    public class CollectionNftMapper
    {
        private Dictionary<CollectionInfo, List<NftElement>> _collections =
            new Dictionary<CollectionInfo, List<NftElement>>();

        public void Evict()
        {
            _collections = new Dictionary<CollectionInfo, List<NftElement>>();
        }

        public List<NftElement> GetNftsFromCollection(CollectionInfo collection)
        {
            if (_collections.TryGetValue(collection, out var fromCollection))
            {
                return fromCollection;
            }

            return new List<NftElement>();
        }

        public void HandleNftFetch(FetchNftContentResult result)
        {
            NftElement[] nfts = result.Content;
            int count = nfts.Length;
            for (int i = 0; i < count; i++)
            {
                AddNftToCollection(nfts[i].Collection, nfts[i]);
            }
        }

        private void AddNftToCollection(CollectionInfo collection, NftElement nft)
        {
            if (_collections.ContainsKey(collection))
            {
                _collections[collection].Add(nft);
            }
            else
            {
                _collections[collection] = new List<NftElement>();
                _collections[collection].Add(nft);
            }
        }
    }
}