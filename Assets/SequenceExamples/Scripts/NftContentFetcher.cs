using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.Utils;

namespace Sequence.Demo
{
    public class NftContentFetcher : INftContentFetcher
    {
        private List<Chain> _excludeChains;
        private List<Chain> _fetchableChains;
        private List<IIndexer> _indexers;

        public NftContentFetcher(params Chain[] excludeChains)
        {
            _excludeChains = excludeChains.ConvertToList();
            
            _fetchableChains = EnumExtensions.GetEnumValuesAsList<Chain>();
            _fetchableChains = _fetchableChains.RemoveItemsInList(_excludeChains);
            
            _indexers = new List<IIndexer>();
            int chains = _fetchableChains.Count;
            for (int i = 0; i < chains; i++)
            {
                _indexers.Add(new ChainIndexer((int)_fetchableChains[i]));
            }
        }
        
        public event Action<FetchNftContentResult> OnNftFetchSuccess;
        public Task FetchContent(int maxToFetch)
        {
            throw new NotImplementedException(); // Todo implement
        }

        public void Refresh()
        {
            // Do nothing, not needed
        }
    }
}