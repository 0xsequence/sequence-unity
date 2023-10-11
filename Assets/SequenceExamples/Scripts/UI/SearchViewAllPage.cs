using System;
using System.Collections;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class SearchViewAllPage : SearchableUIPage
    {
        private VerticalLayoutGroup _searchElementsLayoutGroup;
        public enum SearchToggleStatus
        {
            Default,
            Collection,
            Token
        }

        private SearchToggleStatus _searchToggleStatus;

        protected override void Awake()
        {
            base.Awake();
            _searchElementsLayoutGroup = _elementLayoutGroupTransform.GetComponent<VerticalLayoutGroup>();
            if (_searchElementsLayoutGroup == null)
            {
                throw new SystemException(
                    $"{GetType().Name} has been assembled incorrectly. {nameof(_elementLayoutGroupTransform)} must have a {typeof(VerticalLayoutGroup)} as a component");
            }
        }

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

        protected override void OnInputValueChanged(string newValue)
        {
            base.OnInputValueChanged(newValue);
            StartCoroutine(UpdateScrollViewSize());
        }

        private IEnumerator UpdateScrollViewSize()
        {
            yield return new WaitForEndOfFrame(); // Allow UI time to update so that the search elements can be populated
            
            float contentHeight = _searchElementsLayoutGroup.preferredHeight;
            _elementLayoutGroupTransform.sizeDelta =
                new Vector2(_elementLayoutGroupTransform.sizeDelta.x, contentHeight);
        }
    }
}