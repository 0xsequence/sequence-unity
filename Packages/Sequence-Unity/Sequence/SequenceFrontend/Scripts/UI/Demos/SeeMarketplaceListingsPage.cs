using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.Marketplace;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class SeeMarketplaceListingsPage : UIPage
    {
        [SerializeField] private TMP_InputField _collectionAddressInputField;
        [SerializeField] private GameObject _marketplaceTilePrefab;
        [SerializeField] private int _numberOfMarketplaceTilesToInstantiate = 10;
        [SerializeField] private Transform _scrollviewContentParent;
        [SerializeField] private Chain _chain = Chain.ArbitrumNova;
        [SerializeField] private TextMeshProUGUI _errorText;

        private ObjectPool _marketplaceTilePool;
        private RectTransform _scrollRectContent;
        private int _widthInItems;
        private GridLayoutGroup _grid;
        private float _brandingBuffer = 60;
        private MarketplaceReader _collectibles;
        private int _items = 0;

        protected override void Awake()
        {
            base.Awake();
            _scrollRectContent = GetComponentInChildren<ScrollRect>().content;
            _grid = GetComponentInChildren<GridLayoutGroup>();
            _widthInItems = _grid.constraintCount;
            _collectibles = new MarketplaceReader(_chain);

            DestroyGridChildren();
        }

        public override void Open(params object[] args)
        {
            base.Open(args);

            _items = 0;

            _marketplaceTilePool =
                ObjectPool.ActivateObjectPool(_marketplaceTilePrefab, _numberOfMarketplaceTilesToInstantiate);
            _collectionAddressInputField.onValueChanged.AddListener(OnInputValueChanged);
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
            ListCollectiblesReturn result = await _collectibles.ListCollectiblesWithLowestListing(_collectionAddressInputField.text, null, page);
            if (result == null || result.collectibles == null || result.collectibles.Length == 0)
            {
                _errorText.text = "No orders founds for collection";
                return;
            }
            
            int length = result.collectibles.Length;
            for (int i = 0; i < length; i++)
            {
                Transform marketplaceTile = _marketplaceTilePool.GetNextAvailable();
                marketplaceTile.SetParent(_scrollviewContentParent);
                marketplaceTile.GetComponent<MarketplaceTile>().Assemble(result.collectibles[i]);
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
        /// This method ensures there are no children in the grid when the SeeMarketplaceListingsPage is opened, resolving the issue
        /// </summary>
        private void DestroyGridChildren()
        {
            int count = _scrollviewContentParent.childCount;
            for (int i = 0; i < count; i++)
            {
                Destroy(_scrollviewContentParent.GetChild(i).gameObject);
            }
        }
    }
}