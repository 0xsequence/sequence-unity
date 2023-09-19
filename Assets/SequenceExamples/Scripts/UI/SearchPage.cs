using System;
using System.Collections.Generic;
using Sequence.Demo.ScriptableObjects;
using Sequence.Utils;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class SearchPage : UIPage
    {
        [SerializeField] private TextMeshProUGUI _collectionCountText;
        [SerializeField] private TextMeshProUGUI _tokenCountText;
        [SerializeField] private GameObject _searchElementPrefab;
        [SerializeField] private int _numberOfSearchElementPrefabsToInstantiate = 1;
        [SerializeField] private int _maxSearchElementsDisplayed = 6; // Size of panel only permits showing so many SearchElements
        [SerializeField] private RectTransform _elementLayoutGroupTransform;

        private ObjectPool _searchElementPool;
        
        private CollectionInfo[] _collections;
        private TokenElement[] _tokenElements;
        private NetworkIcons _networkIcons;
        
        private List<SearchElement> _elementsMeetingCriteria;
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
            
            CollectionInfo[] collections = args.GetObjectOfTypeIfExists<CollectionInfo[]>();
            if (collections == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(CollectionInfo[])} as an argument");
            }
            TokenElement[] tokenElements =
                args.GetObjectOfTypeIfExists<TokenElement[]>();
            if (tokenElements == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(TokenElement[])} as an argument");
            }
            NetworkIcons networkIcons = args.GetObjectOfTypeIfExists<NetworkIcons>();
            if (networkIcons == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(NetworkIcons)} as an argument");
            }

            _collections = collections;
            _tokenElements = tokenElements;
            _networkIcons = networkIcons;

            _searchElementPool =
                ObjectPool.ActivateObjectPool(_searchElementPrefab, _numberOfSearchElementPrefabsToInstantiate);

            _nextCollectionSiblingIndex = _collectionCountTextTransform.GetSiblingIndex() + 1;
            _nextTokenSiblingIndex = _tokenCountTextTransform.GetSiblingIndex() + 1;
            
            Assemble();
            PopulateInitialSearchResults();
        }

        public override void Close()
        {
            base.Close();
            _searchElementPool.Cleanup();
        }
        
        public Sprite GetNetworkIcon(Chain network)
        {
            return _networkIcons.GetIcon(network);
        }

        private void Assemble()
        {
            _collectionCountText.text = $"Collections ({_collections.Length})";
            _tokenCountText.text = $"Coins ({_tokenElements.Length})";
        }

        private void PopulateInitialSearchResults()
        {
            int collections = _collections.Length;
            int tokens = _tokenElements.Length;
            int totalElements = collections + tokens;
            int toSpawn = Math.Min(totalElements, _maxSearchElementsDisplayed);
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
                if (element is CollectionInfo)
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
                searchElement.Assemble(element, _networkIcons);
                _elementsMeetingCriteria.Add(searchElement);
            }
        }

        private ISearchable GetNextValidElement()
        {
            throw new NotImplementedException();
        }
    }
}