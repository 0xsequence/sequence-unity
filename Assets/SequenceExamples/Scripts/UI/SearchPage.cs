using System;
using System.Collections.Generic;
using Sequence.Demo.ScriptableObjects;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sequence.Demo
{
    public class SearchPage : UIPage
    {
        public int MaxSearchElementsDisplayed = 6; // Size of panel only permits showing so many SearchElements
        
        [SerializeField] private TextMeshProUGUI _collectionCountText;
        [SerializeField] private TextMeshProUGUI _tokenCountText;
        [SerializeField] private GameObject _searchElementPrefab;
        [SerializeField] private int _numberOfSearchElementPrefabsToInstantiate = 1;
        [SerializeField] private RectTransform _elementLayoutGroupTransform;

        private ObjectPool _searchElementPool;
        
        private SearchableCollection[] _searchableCollections;
        private TokenElement[] _tokenElements;
        private SearchableQuerier _searchableQuerier;
        
        private RectTransform _collectionCountTextTransform;
        private RectTransform _tokenCountTextTransform;
        private int _nextCollectionSiblingIndex;
        private int _nextTokenSiblingIndex;

        protected override void Awake()
        {
            base.Awake();
            _collectionCountTextTransform = _collectionCountText.GetComponent<RectTransform>();
            _tokenCountTextTransform = _tokenCountText.GetComponent<RectTransform>();
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
            
            SearchableCollection[] collections = args.GetObjectOfTypeIfExists<SearchableCollection[]>();
            if (collections == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(SearchableCollection[])} as an argument");
            }
            TokenElement[] tokenElements =
                args.GetObjectOfTypeIfExists<TokenElement[]>();
            if (tokenElements == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(TokenElement[])} as an argument");
            }

            _searchableCollections = collections;
            _tokenElements = tokenElements;
            _searchableQuerier =
                new SearchableQuerier(_searchableCollections, _tokenElements, MaxSearchElementsDisplayed);

            _searchElementPool =
                ObjectPool.ActivateObjectPool(_searchElementPrefab, _numberOfSearchElementPrefabsToInstantiate);

            _nextCollectionSiblingIndex = _collectionCountTextTransform.parent.GetSiblingIndex() + 1;
            _nextTokenSiblingIndex = _tokenCountTextTransform.parent.GetSiblingIndex() + 1;
            
            Assemble();
            PopulateInitialSearchResults();
        }

        public override void Close()
        {
            base.Close();
            _searchElementPool.Cleanup();
        }

        private void Assemble()
        {
            _collectionCountText.text = $"Collections ({_searchableCollections.Length})";
            _tokenCountText.text = $"Coins ({_tokenElements.Length})";
        }

        private void PopulateInitialSearchResults()
        {
            int collections = _searchableCollections.Length;
            int tokens = _tokenElements.Length;
            int totalElements = collections + tokens;
            int toSpawn = Math.Min(totalElements, MaxSearchElementsDisplayed);
            for (int spawned = 0; spawned < toSpawn; spawned++)
            {
                Transform searchElementTransform = _searchElementPool.GetNextAvailable();
                if (searchElementTransform == null)
                {
                    throw new SystemException(
                        $"{nameof(searchElementTransform)} should not be null. {nameof(_searchElementPool)} should expand.");
                }
                
                searchElementTransform.SetParent(_elementLayoutGroupTransform);
                searchElementTransform.localScale = new Vector3(1, 1, 1);
                SearchElement searchElement = searchElementTransform.GetComponent<SearchElement>();
                ISearchable element = GetNextValidElement();
                if (element == null)
                {
                    throw new SystemException($"{nameof(element)} should not be null.");
                }
                SetAndIncrementSiblingIndex(searchElementTransform, element);
                searchElement.Assemble(element);
            }
        }

        private void SetAndIncrementSiblingIndex(Transform searchElementTransform, ISearchable element)
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

        private ISearchable GetNextValidElement()
        {
            return _searchableQuerier.GetNextValid();
        }
    }
}