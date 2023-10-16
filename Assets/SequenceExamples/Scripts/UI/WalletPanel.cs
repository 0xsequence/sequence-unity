using System;
using System.Collections;
using System.Collections.Generic;
using Sequence.Demo.ScriptableObjects;
using Sequence.Utils;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class WalletPanel : UIPanel
    {
        [SerializeField] private GameObject _searchButton;
        [SerializeField] private GameObject _backButton;
        [SerializeField] private TextMeshProUGUI _walletAddressText;
        
        private WalletPage _walletPage;
        private TransitionPanel _transitionPanel;
        private TokenInfoPage _tokenInfoPage;
        private NftInfoPage _nftInfoPage;
        private SearchPage _searchPage;
        private WalletDropdown _walletDropdown;
        private CollectionNftMapper _collectionNftMapper;
        private INftContentFetcher _nftContentFetcher;
        private CollectionInfoPage _collectionInfoPage;
        private ITokenContentFetcher _tokenContentFetcher;
        private List<TokenElement> _fetchedTokenElements;
        private Address _walletAddress;

        public enum TopBarMode
        {
            Search,
            Back
        }

        protected override void Awake()
        {
            base.Awake();
            _walletPage = GetComponentInChildren<WalletPage>();
            _transitionPanel = FindObjectOfType<TransitionPanel>();
            _tokenInfoPage = GetComponentInChildren<TokenInfoPage>();
            _nftInfoPage = GetComponentInChildren<NftInfoPage>();
            _collectionInfoPage = GetComponentInChildren<CollectionInfoPage>();
            _searchPage = GetComponentInChildren<SearchPage>();
            _walletDropdown = GetComponentInChildren<WalletDropdown>();
            _collectionNftMapper = new CollectionNftMapper();
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
            _fetchedTokenElements = new List<TokenElement>();
            _walletAddress = args.GetObjectOfTypeIfExists<Address>();
            if (_walletAddress == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(Address)} as an argument");
            }
            
            _walletAddressText.text = _walletAddress.CondenseForUI();
        }

        public override void Close()
        {
            base.Close();
            _walletPage.Close();
            _tokenContentFetcher.OnTokenFetchSuccess -= HandleTokenFetchSuccess;
            _nftContentFetcher.OnNftFetchSuccess -= _collectionNftMapper.HandleNftFetch;
            _collectionNftMapper.Evict();
        }

        public override void Back(params object[] injectAdditionalParams)
        {
            base.Back(injectAdditionalParams);
            if (_page is WalletPage)
            {
                SetTopBarMode(TopBarMode.Search);
            }
        }

        public override IEnumerator OpenInitialPage(params object[] openArgs)
        {
            _collectionNftMapper.Evict();
            openArgs = SetupContentFetchers(openArgs);
            return base.OpenInitialPage(openArgs);
        }

        private object[] SetupContentFetchers(params object[] args)
        {
            ITokenContentFetcher tokenFetcher = args.GetObjectOfTypeIfExists<ITokenContentFetcher>();
            if (tokenFetcher == null)
            {
                tokenFetcher = new MockTokenContentFetcher();
                args.AppendObject(tokenFetcher);
            }

            INftContentFetcher nftFetcher = args.GetObjectOfTypeIfExists<INftContentFetcher>();
            if (nftFetcher == null)
            {
                nftFetcher = new MockNftContentFetcher();
                args.AppendObject(nftFetcher);
            }

            _tokenContentFetcher = tokenFetcher;
            _tokenContentFetcher.OnTokenFetchSuccess += HandleTokenFetchSuccess;

            _nftContentFetcher = nftFetcher;
            _nftContentFetcher.OnNftFetchSuccess += _collectionNftMapper.HandleNftFetch;

            return args;
        }

        public void OpenTransitionPanel()
        {
            _transitionPanel.OpenWithDelay(_closeAnimationDurationInSeconds);
        }

        public void SetTopBarMode(TopBarMode mode)
        {
            switch (mode)
            {
                case TopBarMode.Search:
                    _searchButton.SetActive(true);
                    _backButton.SetActive(false);
                    break;
                case TopBarMode.Back:
                    _searchButton.SetActive(false);
                    _backButton.SetActive(true);
                    break;
            }
        }

        public void OpenTokenInfoPage(TokenElement tokenElement, NetworkIcons networkIcons, ITransactionDetailsFetcher transactionDetailsFetcher)
        {
            OpenInfoPage(_tokenInfoPage, tokenElement, networkIcons, transactionDetailsFetcher);
        }
        
        public void OpenNftInfoPage(NftElement nftElement, NetworkIcons networkIcons, ITransactionDetailsFetcher transactionDetailsFetcher)
        {
            OpenInfoPage(_nftInfoPage, nftElement, networkIcons, transactionDetailsFetcher);
        }

        public void OpenCollectionInfoPage(NetworkIcons networkIcons, CollectionInfo collectionInfo)
        {
            OpenInfoPage(_collectionInfoPage, networkIcons, collectionInfo);
        }

        private void OpenInfoPage(UIPage infoPage, params object[] openArgs)
        {
            StartCoroutine(SetUIPage(infoPage, openArgs));
            SetTopBarMode(TopBarMode.Back);
        }

        public List<NftElement> GetNftsFromCollection(CollectionInfo collection)
        {
            return _collectionNftMapper.GetNftsFromCollection(collection);
        }

        private void HandleTokenFetchSuccess(FetchTokenContentResult result)
        {
            TokenElement[] tokens = result.Content;
            int count = tokens.Length;
            for (int i = 0; i < count; i++)
            {
                _fetchedTokenElements.Add(tokens[i]);
            }
        }

        public void OpenSearchPage()
        {
            CollectionInfo[] collections = _collectionNftMapper.GetCollections();
            int collectionsLength = collections.Length;
            SearchableCollection[] searchableCollections = new SearchableCollection[collectionsLength];
            for (int i = 0; i < collectionsLength; i++)
            {
                searchableCollections[i] = new SearchableCollection(collections[i], _collectionNftMapper);
            }

            StartCoroutine(SetUIPage(_searchPage, searchableCollections, _fetchedTokenElements.ToArray()));
            SetTopBarMode(TopBarMode.Back);
        }
        
        public void OpenWalletDropdown()
        {
            OpenPageOverlaid(_walletDropdown, _walletAddress);
        }
    }
}