using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.Demo.Mocks;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class ViewMarketplaceListingsPage : UIPage
    {
        [SerializeField] private TMP_InputField _collectionAddressInputField;
        [SerializeField] private GameObject _marketplaceTilePrefab;
        [SerializeField] private int _numberOfMarketplaceTilesToInstantiate = 10;
        [SerializeField] private Transform _scrollviewContentParent;
        [SerializeField] private Chain _chain = Chain.ArbitrumNova;
        [SerializeField] private TextMeshProUGUI _errorText;
        [SerializeField] private string _defaultCollectionAddress = "0x0ee3af1874789245467e7482f042ced9c5171073";
        [SerializeField] private bool _useMockReader = false;

        private ObjectPool _marketplaceTilePool;
        private RectTransform _scrollRectContent;
        private int _widthInItems;
        private GridLayoutGroup _grid;
        private float _brandingBuffer = 60;
        private IReader _reader;
        private int _items = 0;
        private Dictionary<string, Sprite> _currencyIcons = new Dictionary<string, Sprite>();
        private bool _currenciesFetched = false;
        private IWallet _wallet;

        protected override void Awake()
        {
            base.Awake();
            _scrollRectContent = GetComponentInChildren<ScrollRect>().content;
            _grid = GetComponentInChildren<GridLayoutGroup>();
            _widthInItems = _grid.constraintCount;
            _reader = new MarketplaceReader(_chain);

            DestroyGridChildren();
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
            
            _wallet = args.GetObjectOfTypeIfExists<IWallet>();
            if (_wallet == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(IWallet)} as an argument");
            }

            Chain chain = args.GetObjectOfTypeIfExists<Chain>();
            if (chain != default)
            {
                _chain = chain;
                _reader = new MarketplaceReader(_chain);
            }
            
#if UNITY_EDITOR
            if (_useMockReader)
            {
                _reader = new MockMarketplaceReaderReturnsRandomFakeListings(_chain);
            }
#endif

            _items = 0;

            _marketplaceTilePool =
                ObjectPool.ActivateObjectPool(_marketplaceTilePrefab, _numberOfMarketplaceTilesToInstantiate);
            _collectionAddressInputField.onValueChanged.AddListener(OnInputValueChanged);

            _collectionAddressInputField.text = _defaultCollectionAddress;
            
            FetchCurrencyIcons().ConfigureAwait(false);
        }
        
        private void OnInputValueChanged(string value)
        {
            if (value.IsAddress())
            {
                Clear();
                FetchCollection().ConfigureAwait(false);
                _errorText.text = "";
            }
            else
            {
                _errorText.text = "Please enter a valid collection address";
            }
        }

        private async Task FetchCollection(Page page = null)
        {
            while (!_currenciesFetched)
            {
                await Task.Yield();
            }
            
            ListCollectiblesReturn result = await _reader.ListCollectibleListingsWithLowestPricedListingsFirst(_collectionAddressInputField.text, null, page);
            if (result == null || result.collectibles == null || result.collectibles.Length == 0)
            {
                _errorText.text = "No orders founds for collection" + result.ToString();
                return;
            }
            
            int length = result.collectibles.Length;
            for (int i = 0; i < length; i++)
            {
                Transform marketplaceTile = _marketplaceTilePool.GetNextAvailable();
                marketplaceTile.SetParent(_scrollviewContentParent);
                Sprite currencyIcon = null;
                if (_currencyIcons.TryGetValue(result.collectibles[i].order.priceCurrencyAddress, out var icon))
                {
                    currencyIcon = icon;
                }
                else
                {
                    Debug.LogError($"Currency icon not found for {result.collectibles[i].order.priceCurrencyAddress} which is listed as the price currency address for {result.collectibles[i].order.orderId}. Skipping as this order is invalid.");
                    continue;
                }
                marketplaceTile.GetComponent<MarketplaceTile>().Assemble(result.collectibles[i], currencyIcon, _wallet);
                marketplaceTile.localScale = Vector3.one;
                _items++;
                UpdateScrollViewSize();
            }
            if (result.page != null && result.page.more)
            {
                await FetchCollection(result.page);
            }
        }

        private void UpdateScrollViewSize()
        {
            int rowCount = Mathf.CeilToInt((float)_items / _widthInItems);
            float contentHeight = rowCount * _grid.cellSize.y + (rowCount - 1) * _grid.spacing.y;

            RectTransform content = _scrollRectContent;
            content.sizeDelta = new Vector2(content.sizeDelta.x, contentHeight + _brandingBuffer);
        }

        public override void Close()
        {
            base.Close();
            Clear();
            _collectionAddressInputField.onValueChanged.RemoveListener(OnInputValueChanged);
        }

        private void Clear()
        {
            _marketplaceTilePool.Cleanup();
            _items = 0;
        }

        /// <summary>
        /// Sometimes it's possible that when playmode ends, we can end up creating children in the grid
        /// Or similarly, if you are editing the UI, you may forget to delete children in the grid
        /// If this happens, the UI looks all wonky because you have all these extra items in the grid being displayed
        /// This method ensures there are no children in the grid when the ViewMarketplaceListingsPage is opened, resolving the issue
        /// </summary>
        private void DestroyGridChildren()
        {
            int count = _scrollviewContentParent.childCount;
            for (int i = 0; i < count; i++)
            {
                Destroy(_scrollviewContentParent.GetChild(i).gameObject);
            }
        }
        
        private async Task FetchCurrencyIcons()
        {
            Marketplace.Currency[] currencies = await _reader.ListCurrencies();
            foreach (Marketplace.Currency currency in currencies)
            {
                Sprite icon = await SpriteFetcher.Fetch(currency.imageUrl);
                _currencyIcons.Add(currency.contractAddress, icon);
            }
            _currenciesFetched = true;
        }
    }
}