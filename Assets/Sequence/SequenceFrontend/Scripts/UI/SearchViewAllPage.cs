using System;
using System.Collections;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class SearchViewAllPage : SearchableUIPage
    {
        [SerializeField] private Toggle _collectionToggle;
        [SerializeField] private Toggle _tokenToggle;
        
        private VerticalLayoutGroup _searchElementsLayoutGroup;
        private string _currentSearchValue;
        
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
            
            SetupToggles();

            base.Open(args);
        }

        public override void Close()
        {
            base.Close();
            _collectionToggle.onValueChanged.RemoveListener(SwitchToViewCollections);
            _tokenToggle.onValueChanged.RemoveListener(SwitchToViewTokens);
        }

        private void SetupToggles()
        {
            if (_searchToggleStatus == SearchToggleStatus.Collection)
            {
                _collectionToggle.isOn = true;
                _tokenToggle.isOn = false;
            }
            else
            {
                _collectionToggle.isOn = false;
                _tokenToggle.isOn = true;
            }
            
            _collectionToggle.onValueChanged.AddListener(SwitchToViewCollections);
            _tokenToggle.onValueChanged.AddListener(SwitchToViewTokens);
        }

        protected override void OnInputValueChanged(string newValue)
        {
            base.OnInputValueChanged(newValue);
            _currentSearchValue = newValue;
            StartCoroutine(UpdateScrollViewSize());
        }

        private IEnumerator UpdateScrollViewSize()
        {
            yield return new WaitForEndOfFrame(); // Allow UI time to update so that the search elements can be populated
            
            float contentHeight = _searchElementsLayoutGroup.preferredHeight;
            _elementLayoutGroupTransform.sizeDelta =
                new Vector2(_elementLayoutGroupTransform.sizeDelta.x, contentHeight);
        }

        private void Refresh()
        {
            OnInputValueChanged(_currentSearchValue);
        }

        private void SwitchToViewCollections(bool toSwitch)
        {
            if (!toSwitch)
            {
                return;
            }

            _searchToggleStatus = SearchToggleStatus.Collection;
            Refresh();
        }

        private void SwitchToViewTokens(bool toSwitch)
        {
            if (!toSwitch)
            {
                return;
            }

            _searchToggleStatus = SearchToggleStatus.Token;
            Refresh();
        }
    }
}