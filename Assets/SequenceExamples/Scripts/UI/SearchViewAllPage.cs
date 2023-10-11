using System;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Demo
{
    public class SearchViewAllPage : SearchableUIPage
    {
        public enum SearchToggleStatus
        {
            Default,
            Collection,
            Token
        }

        private SearchToggleStatus _searchToggleStatus;
        
        protected override void SetAndIncrementSiblingIndex(Transform searchElementTransform, ISearchable element)
        {
            // Not needed for this page
        }

        protected override ISearchable GetNextValidElement()
        {
            if (_searchToggleStatus == SearchToggleStatus.Collection)
            {
                return _searchableQuerier.GetNextValidCollection();
            }
            if (_searchToggleStatus == SearchToggleStatus.Token)
            {
                return _searchableQuerier.GetNextValidToken();
            }

            throw new SystemException($"{GetType().Name} has an unexpected {nameof(SearchToggleStatus)}");
        }

        public override void Open(params object[] args)
        {
            SearchToggleStatus toggleStatus = args.GetObjectOfTypeIfExists<SearchToggleStatus>();
            if (toggleStatus == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(SearchToggleStatus)} as an argument");
            }

            _searchToggleStatus = toggleStatus;
            
            base.Open(args);
        }
    }
}