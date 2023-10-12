using UnityEngine;

namespace Sequence.Demo
{
    public class SearchableCollection : ISearchable
    {
        private CollectionInfo _collection;
        private CollectionNftMapper _mapper;

        public SearchableCollection(CollectionInfo collection, CollectionNftMapper mapper)
        {
            _collection = collection;
            _mapper = mapper;
        }

        public Sprite GetIcon()
        {
            return _collection.IconSprite;
        }

        public string GetName()
        {
            return _collection.Name;
        }

        public Chain GetNetwork()
        {
            return _collection.Network;
        }

        public uint GetNumberOwned()
        {
            return (uint)_mapper.GetNftsFromCollection(_collection).Count;
        }

        public CollectionInfo GetCollection()
        {
            return _collection;
        }
    }
}