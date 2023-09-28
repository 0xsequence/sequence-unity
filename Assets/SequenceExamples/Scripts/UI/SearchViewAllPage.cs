using System;
using UnityEngine;

namespace Sequence.Demo
{
    public class SearchViewAllPage : SearchableUIPage
    {
        private enum PageMode
        {
            Collection,
            Token
        }

        private PageMode _pageMode;
        
        protected override void SetAndIncrementSiblingIndex(Transform searchElementTransform, ISearchable element)
        {
            // Not needed for this page
        }

        protected override ISearchable GetNextValidElement()
        {
            if (_pageMode == PageMode.Collection)
            {
                return _searchableQuerier.GetNextValidCollection();
            }
            if (_pageMode == PageMode.Token)
            {
                return _searchableQuerier.GetNextValidToken();
            }

            throw new SystemException($"{GetType().Name} has an unexpected {nameof(PageMode)}");
        }
    }
}