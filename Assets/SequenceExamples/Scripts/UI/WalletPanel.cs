using System.Collections;
using System.Collections.Generic;
using Sequence.Demo.ScriptableObjects;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Demo
{
    public class WalletPanel : UIPanel
    {
        [SerializeField] private GameObject _searchButton;
        [SerializeField] private GameObject _backButton;
        
        private WalletPage _walletPage;
        private TransitionPanel _transitionPanel;
        private TokenInfoPage _tokenInfoPage;
        private NftInfoPage _nftInfoPage;
        private CollectionNftMapper _collectionNftMapper;
        private INftContentFetcher _nftContentFetcher;
        private CollectionInfoPage _collectionInfoPage;

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
            _collectionNftMapper = new CollectionNftMapper();
        }

        public override void Close()
        {
            base.Close();
            _walletPage.Close();
            _nftContentFetcher.OnNftFetchSuccess -= _collectionNftMapper.HandleNftFetch;
            _collectionNftMapper.Evict();
        }

        public override void Back()
        {
            base.Back();
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
                args.AppendObject(new MockTokenContentFetcher());
            }

            INftContentFetcher nftFetcher = args.GetObjectOfTypeIfExists<INftContentFetcher>();
            if (nftFetcher == null)
            {
                args.AppendObject(new MockNftContentFetcher());
            }
            _nftContentFetcher = args.GetObjectOfTypeIfExists<INftContentFetcher>();
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

        public CollectionInfo[] GetCollections()
        {
            return _collectionNftMapper.GetCollections();
        }
    }
}