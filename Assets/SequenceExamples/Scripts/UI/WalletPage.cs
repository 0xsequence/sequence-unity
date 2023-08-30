using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class WalletPage : UIPage
    {
        [SerializeField] private int _numberOfTokenPlaceholdersToInstantiate = 3;
        [SerializeField] private GameObject _tokenPlaceHolderPrefab;
        [SerializeField] private int _numberOfTokensToFetchAtOnce = 1;
        [SerializeField] private int _numberOfNftPlaceholdersToInstantiate = 10;
        [SerializeField] private GameObject _nftPlaceHolderPrefab;
        [SerializeField] private int _numberOfNftsToFetchAtOnce = 1;
        [SerializeField] private Transform _scrollviewContentParent;
        public float TimeBetweenTokenValueRefreshesInSeconds = 5;
        private ObjectPool _tokenPool;
        private ObjectPool _nftPool;
        private ITokenContentFetcher _tokenFetcher;
        private List<TokenElement> _tokenContent = new List<TokenElement>();
        private INftContentFetcher _nftFetcher;
        private List<Texture2D> _nftContent = new List<Texture2D>();
        private RectTransform _scrollRectContent;
        private int _widthInItems = 2;
        private GridLayoutGroup _grid;
        private float _brandingBuffer = 60;
        private List<TokenUIElement> _tokenUIElements = new List<TokenUIElement>();

        protected override void Awake()
        {
            base.Awake();
            _scrollRectContent = GetComponentInChildren<ScrollRect>().content;
            _grid = GetComponentInChildren<GridLayoutGroup>();
        }

        public override void Open(params object[] args)
        {
            base.Open();

            if (_tokenFetcher == null)
            {
                throw new SystemException($"{nameof(_tokenFetcher)} must not be null. Please call {nameof(SetupContentFetchers)} before opening");
            }

            if (_nftFetcher == null)
            {
                throw new SystemException($"{nameof(_nftFetcher)} must not be null. Please call {nameof(SetupContentFetchers)} before opening");
            }

            _tokenPool =
                ObjectPool.ActivateObjectPool(_tokenPlaceHolderPrefab, _numberOfTokenPlaceholdersToInstantiate);
            _nftPool = ObjectPool.ActivateObjectPool(_nftPlaceHolderPrefab, _numberOfNftPlaceholdersToInstantiate);

            _tokenFetcher.FetchContent(_numberOfTokensToFetchAtOnce);

            StartCoroutine(RefreshTokenValues());
        }

        public override void Close()
        {
            base.Close();
            _tokenFetcher.OnTokenFetchSuccess -= HandleTokenFetchSuccess;
            _tokenFetcher = null;
            _nftFetcher.OnNftFetchSuccess -= HandleNftFetchSuccess;
            _nftFetcher = null;
            _tokenPool.Cleanup();
            _nftPool.Cleanup();
            _tokenUIElements = new List<TokenUIElement>();
        }

        public void SetupContentFetchers(ITokenContentFetcher tokenContentFetcher, INftContentFetcher nftContentFetcher)
        {
            _tokenFetcher = tokenContentFetcher;
            _tokenFetcher.OnTokenFetchSuccess += HandleTokenFetchSuccess;
            _nftFetcher = nftContentFetcher;
            _nftFetcher.OnNftFetchSuccess += HandleNftFetchSuccess;
        }

        private void HandleTokenFetchSuccess(FetchTokenContentResult result)
        {
            TokenElement[] elements = result.Content;
            int count = elements.Length;
            for (int i = 0; i < count; i++)
            {
                _tokenContent.Add(elements[i]);
                ApplyTokenContent(elements[i]);
                UpdateScrollViewSize();
            }

            if (result.MoreToFetch)
            {
                _tokenFetcher.FetchContent(_numberOfTokensToFetchAtOnce);
            }
            else
            {
                _nftFetcher.FetchContent(_numberOfNftsToFetchAtOnce);
            }
        }

        private void ApplyTokenContent(TokenElement element)
        {
            Transform tokenContainer = _tokenPool.GetNextAvailable();
            if (tokenContainer == null)
            {
                throw new SystemException(
                    $"{nameof(tokenContainer)} should not be null. {nameof(_tokenPool)} should expand.");
            }

            TokenUIElement uiElement = tokenContainer.GetComponent<TokenUIElement>();
            uiElement.Assemble(element);
            _tokenUIElements.Add(uiElement);
            tokenContainer.SetParent(_scrollviewContentParent);
            tokenContainer.localScale = new Vector3(1, 1, 1);
        }

        private void HandleNftFetchSuccess(FetchNftContentResult result)
        {
            Texture2D[] textures = result.Content;
            int count = textures.Length;
            for (int i = 0; i < count; i++)
            {
                _nftContent.Add(textures[i]);
                ApplyTexture(textures[i]);
                UpdateScrollViewSize();
            }

            if (result.MoreToFetch)
            {
                _nftFetcher.FetchContent(_numberOfNftsToFetchAtOnce);
            }
        }

        private void ApplyTexture(Texture2D texture)
        {
            Transform nftContainer = _nftPool.GetNextAvailable();
            if (nftContainer == null)
            {
                throw new SystemException(
                    $"{nameof(nftContainer)} should not be null. {nameof(_nftPool)} should expand.");
            }
            
            Image nftImage = nftContainer.GetComponent<Image>();
            nftImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(.5f, .5f));
            nftContainer.SetParent(_scrollviewContentParent);
            nftContainer.localScale = new Vector3(1, 1, 1);
        }

        private void UpdateScrollViewSize()
        {
            int itemCount = _tokenContent.Count + _nftContent.Count;
            int rowCount = Mathf.CeilToInt((float)itemCount / _widthInItems);
            float contentHeight = rowCount * _grid.cellSize.y + (rowCount - 1) * _grid.spacing.y;

            RectTransform content = _scrollRectContent;
            content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight + _brandingBuffer);
        }

        private IEnumerator RefreshTokenValues()
        {
            var waitForRefresh = new WaitForSecondsRealtime(TimeBetweenTokenValueRefreshesInSeconds);
            while (true) // Terminates on Close() (as this gameObject will be disabled)
            {
                yield return waitForRefresh;
                int count = _tokenUIElements.Count;
                for (int i = 0; i < count; i++)
                {
                    _tokenUIElements[i].RefreshCurrencyValue();
                }
            }
        }

        public int CountFungibleTokensDisplayed()
        {
            return _tokenUIElements.Count;
        }
    }
}