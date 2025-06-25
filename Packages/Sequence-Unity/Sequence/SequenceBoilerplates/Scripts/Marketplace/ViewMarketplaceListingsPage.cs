using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
using Sequence.Pay.Transak;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Currency = Sequence.Marketplace.Currency;

namespace Sequence.Boilerplates
{
    public class ViewMarketplaceListingsPage : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _collectionAddressInputField;
        [SerializeField] private Chain _chain = Chain.TestnetArbitrumSepolia;
        [SerializeField] private TextMeshProUGUI _errorText;
        [SerializeField] private string _defaultCollectionAddress = "0x0ee3af1874789245467e7482f042ced9c5171073";
        [SerializeField] private bool _useMockReader = false;
        [SerializeField] private GenericObjectPool<MarketplaceTile> _tilePool;

        private RectTransform _scrollRectContent;
        private int _widthInItems;
        private GridLayoutGroup _grid;
        private float _brandingBuffer = 60;
        private IMarketplaceReader _marketplaceReader;
        private int _items = 0;
        private Dictionary<string, Sprite> _currencyIcons = new Dictionary<string, Sprite>();
        private bool _currenciesFetched = false;
        private IWallet _wallet;
        private CheckoutPage _checkoutPanel;
        private Action _onClose;

        protected void Awake()
        {
            _collectionAddressInputField.onValueChanged.AddListener(OnInputValueChanged);
            _scrollRectContent = GetComponentInChildren<ScrollRect>().content;
            _grid = GetComponentInChildren<GridLayoutGroup>();
            _widthInItems = _grid.constraintCount;
            _marketplaceReader = new MarketplaceReader(_chain);
            _checkoutPanel = FindObjectOfType<CheckoutPage>();

            DestroyGridChildren();
        }
        
        public void Hide()
        {
            Clear();
            gameObject.SetActive(false);
            _onClose?.Invoke();
        }

        public void Show(IWallet wallet, Chain chain, Address marketplaceCollectionAddress, Action onClose = null)
        {
            _wallet = wallet;
            _onClose = onClose;
            
            if (_wallet == null)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(IWallet)} as an argument");
            }
            
            gameObject.SetActive(true);

            if (chain != default)
            {
                _chain = chain;
                _marketplaceReader = new MarketplaceReader(_chain);
            }
            
#if UNITY_EDITOR
            if (_useMockReader)
            {
                Debug.LogWarning("Using mock marketplace reader. If you want to use a real marketplaceReader, make sure to uncheck the _useMockReader field in the inspector.");
                _marketplaceReader = new MockMarketplaceReaderReturnsRandomFakeListings(_chain);
            }
#endif

            if (marketplaceCollectionAddress != null)
                _defaultCollectionAddress = marketplaceCollectionAddress;
            
            _items = 0;
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
            
            ListCollectiblesReturn result = await _marketplaceReader.ListCollectibleListingsWithLowestPricedListingsFirst(_collectionAddressInputField.text, null, page);
            if (result == null || result.collectibles == null || result.collectibles.Length == 0)
            {
                _errorText.text = "No orders founds for collection" + result.ToString();
                return;
            }
            
            int length = result.collectibles.Length;
            for (int i = 0; i < length; i++)
            {
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
                
                var marketplaceTile = _tilePool.GetObject();
                marketplaceTile.GetComponent<MarketplaceTile>().Assemble(result.collectibles[i], currencyIcon, _wallet, NFTType.ERC1155);
                marketplaceTile.transform.localScale = Vector3.one;
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
            var position = content.position;
            position = new Vector3(position.x, 0, position.z);
            content.position = position;
        }

        private void Clear()
        {
            _tilePool.Cleanup();
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
            int count = _tilePool.Parent.childCount;
            for (int i = 0; i < count; i++)
            {
                Destroy(_tilePool.Parent.GetChild(i).gameObject);
            }
        }
        
        private async Task FetchCurrencyIcons()
        {
            Currency[] currencies = await _marketplaceReader.ListCurrencies();
            foreach (Currency currency in currencies)
            {
                Sprite icon = await AssetHandler.GetSpriteAsync(currency.imageUrl);
                _currencyIcons.Add(currency.contractAddress, icon);
            }
            _currenciesFetched = true;
        }
    }
}