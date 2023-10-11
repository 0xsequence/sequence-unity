using System;
using System.Collections.Generic;
using Sequence.Utils;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public abstract class SearchableUIPage : UIPage
    {
        public int MaxSearchElementsDisplayed = Int32.MaxValue; // Size of panel may only permit showing so many SearchElements - in which case you should set this value in inspector
        
        [SerializeField] private GameObject _searchElementPrefab;
        [SerializeField] private int _numberOfSearchElementPrefabsToInstantiate = 1;
        [SerializeField] private RectTransform _elementLayoutGroupTransform;
        [SerializeField] protected TMP_InputField _searchBar;
        [SerializeField] protected TextMeshProUGUI _collectionCountText;
        [SerializeField] protected TextMeshProUGUI _tokenCountText;

        private ObjectPool _searchElementPool;
        
        protected SearchableCollection[] _searchableCollections;
        protected TokenElement[] _tokenElements;
        protected SearchableQuerier _searchableQuerier;

        private Queue<SearchElement> _activeElements;

        protected override void Awake()
        {
            base.Awake();
            _searchBar.onValueChanged.AddListener(OnInputValueChanged);
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
            
            _activeElements = new Queue<SearchElement>();
            
            SetInitialSearchBarText(args.GetObjectOfTypeIfExists<string>());
            
            SetCountTexts();
        }

        private void SetInitialSearchBarText(string initialText)
        {
            if (initialText != default)
            {
                _searchBar.text = initialText;
            }
            OnInputValueChanged(_searchBar.text);
        }

        public override void Close()
        {
            base.Close();
            _searchElementPool.Cleanup();
        }

        public override void Back()
        {
            _panel.Back(_searchBar.text);
        }

        private void PopulateSearchElements()
        {
            int collections = _searchableCollections.Length;
            int tokens = _tokenElements.Length;
            int totalElements = collections + tokens;
            int toSpawn = Math.Min(totalElements, MaxSearchElementsDisplayed);
            for (int spawned = 0; spawned < toSpawn; spawned++)
            {
                ISearchable element = GetNextValidElement();
                if (element == null)
                {
                    break; // We have run out of ISearchables that meet the search criteria
                }
                
                Transform searchElementTransform = _searchElementPool.GetNextAvailable();
                if (searchElementTransform == null)
                {
                    throw new SystemException(
                        $"{nameof(searchElementTransform)} should not be null. {nameof(_searchElementPool)} should expand.");
                }
                
                searchElementTransform.SetParent(_elementLayoutGroupTransform);
                searchElementTransform.localScale = new Vector3(1, 1, 1);
                SearchElement searchElement = searchElementTransform.GetComponent<SearchElement>();
                SetAndIncrementSiblingIndex(searchElementTransform, element);
                searchElement.Assemble(element);
                _activeElements.Enqueue(searchElement);
            }
        }

        protected abstract void SetAndIncrementSiblingIndex(Transform searchElementTransform, ISearchable element);
        protected abstract ISearchable GetNextValidElement();

        protected void OnInputValueChanged(string newValue)
        {
            _searchableQuerier.SetNewCriteria(newValue);
            ResetSearchElements();
            PopulateSearchElements();
            SetCountTexts();
        }

        protected virtual void ResetSearchElements()
        {
            while (_activeElements.Count > 0)
            {
                SearchElement element = _activeElements.Dequeue();
                _searchElementPool.DeactivateObject(element.gameObject);
            }
        }

        private void SetCountTexts()
        {
            _collectionCountText.text = $"Collections ({_searchableQuerier.GetNumberOfCollectionsMatchingCriteria()})";
            _tokenCountText.text = $"Coins ({_searchableQuerier.GetNumberOfTokensMatchingCriteria()})";
        }

        public SearchableQuerier GetQuerier()
        {
            return _searchableQuerier;
        }
    }
}