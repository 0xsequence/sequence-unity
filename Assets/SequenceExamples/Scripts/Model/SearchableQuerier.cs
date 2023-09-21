using System;
using System.Collections.Generic;

namespace Sequence.Demo
{
    public class SearchableQuerier
    {
        private SearchableCollection[] _allCollections;
        private int _totalCollections;
        private TokenElement[] _allTokens;
        private int _totalTokens;
        private List<SearchableCollection> _collectionsMeetingCriteria;
        private List<TokenElement> _tokensMeetingCriteria;
        private int _maxSearchablesToReturn;

        private Queue<ISearchable> _nextSearchables;

        public SearchableQuerier(SearchableCollection[] allCollections, TokenElement[] allTokens, int maxSearchablesToReturn)
        {
            _allCollections = allCollections;
            _totalCollections = allCollections.Length;
            _allTokens = allTokens;
            _totalTokens = allTokens.Length;
            _maxSearchablesToReturn = maxSearchablesToReturn;
            SetNewCriteria("");
        }
        
        public void SetNewCriteria(string criteria)
        {
            FilterCollectionsAndTokens(criteria);
            (int collectionsToReturn, int tokensToReturn) = GetRatioToReturn();
            AssembleNextSearchable(collectionsToReturn, tokensToReturn);
        }
        
        private void FilterCollectionsAndTokens(string criteria)
        {
            _collectionsMeetingCriteria = new List<SearchableCollection>();
            for (int i = 0; i < _totalCollections; i++)
            {
                if (_allCollections[i].GetName().StartsWith(criteria))
                {
                    _collectionsMeetingCriteria.Add(_allCollections[i]);
                }
            }
            
            _tokensMeetingCriteria = new List<TokenElement>();
            for (int i = 0; i < _totalTokens; i++)
            {
                if (_allTokens[i].GetName().StartsWith(criteria))
                {
                    _tokensMeetingCriteria.Add(_allTokens[i]);
                }
            }
        }

        /// <summary>
        /// Returns the ratio of collections to tokens to return
        /// We favour having an even split, with Collections getting the remainder if necessary
        /// </summary>
        /// <returns></returns>
        private (int, int) GetRatioToReturn()
        {
            int idealRatio = _maxSearchablesToReturn / 2;
            int remainder = _maxSearchablesToReturn % 2;
            int availableCollections = _collectionsMeetingCriteria.Count;
            int availableTokens = _tokensMeetingCriteria.Count;
            int collectionsToReturn = Math.Min(idealRatio + remainder, availableCollections);
            int tokensToReturn = Math.Min(idealRatio, availableTokens);
            
            // If there aren't enough availableCollections or availableTokens to return the ideal ratio, return extra of the other (if possible)
            collectionsToReturn = Math.Min(collectionsToReturn + _maxSearchablesToReturn - tokensToReturn,
                availableCollections);
            tokensToReturn = Math.Min(tokensToReturn + _maxSearchablesToReturn - collectionsToReturn, availableTokens);
            
            return (collectionsToReturn, tokensToReturn);
        }

        private void AssembleNextSearchable(int collectionsToReturn, int tokensToReturn)
        {
            _nextSearchables = new Queue<ISearchable>();
            for (int i = 0; i < collectionsToReturn; i++)
            {
                _nextSearchables.Enqueue(_collectionsMeetingCriteria[i]);
            }
            for (int i = 0; i < tokensToReturn; i++)
            {
                _nextSearchables.Enqueue(_tokensMeetingCriteria[i]);
            }
        }

        public ISearchable GetNextValid()
        {
            return _nextSearchables.Dequeue();
        }
    }
}