using System;
using System.Collections.Generic;
using Sequence.Demo.ScriptableObjects;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sequence.Demo
{
    public class SearchPage : SearchableUIPage
    {

        private SearchViewAllPage _viewAllPage;
        
        private RectTransform _collectionCountTextTransform;
        private RectTransform _tokenCountTextTransform;
        private int _nextCollectionSiblingIndex;
        private int _nextTokenSiblingIndex;

        private Queue<SearchElement> _activeElements;

        protected override void Awake()
        {
            base.Awake();
            _collectionCountTextTransform = _collectionCountText.GetComponent<RectTransform>();
            _tokenCountTextTransform = _tokenCountText.GetComponent<RectTransform>();
            _viewAllPage = FindObjectOfType<SearchViewAllPage>();
        }

        protected override void SetAndIncrementSiblingIndex(Transform searchElementTransform, ISearchable element)
        {
            if (element is SearchableCollection)
            {
                searchElementTransform.SetSiblingIndex(_nextCollectionSiblingIndex);
                _nextCollectionSiblingIndex++;
                _nextTokenSiblingIndex++;
            }
            else if (element is TokenElement)
            {
                searchElementTransform.SetSiblingIndex(_nextTokenSiblingIndex);
                _nextTokenSiblingIndex++;
            }
            else
            {
                throw new SystemException(
                    $"{nameof(element)} of type {element.GetType()} is an unexpected implementation type of {nameof(ISearchable)}");
            }
        }

        protected override ISearchable GetNextValidElement()
        {
            return _searchableQuerier.GetNextValid();
        }

        protected override void ResetSearchElements()
        {
            base.ResetSearchElements();
            _nextCollectionSiblingIndex = _collectionCountTextTransform.parent.GetSiblingIndex() + 1;
            _nextTokenSiblingIndex = _tokenCountTextTransform.parent.GetSiblingIndex() + 1;
        }

        public void ViewAllCollections()
        {
            OpenViewAllPage(SearchViewAllPage.SearchToggleStatus.Collection);
        }

        public void ViewAllTokens()
        {
            OpenViewAllPage(SearchViewAllPage.SearchToggleStatus.Token);
        }

        private void OpenViewAllPage(SearchViewAllPage.SearchToggleStatus toggleStatus)
        {
            _panel.StartCoroutine(_panel.SetUIPage(_viewAllPage, _searchableCollections, _tokenElements, _searchBar.text, toggleStatus));
        }
    }
}