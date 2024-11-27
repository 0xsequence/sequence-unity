using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Demo
{
    public class CheckoutPage : UIPage
    {
        [SerializeField] private GameObject _cartItemPrefab;
        [SerializeField] private Transform _cartItemsParent;
        [SerializeField] private TextMeshProUGUI _numberOfUniqueItemsText;
        [SerializeField] private VerticalLayoutGroup _scrollViewLayoutGroup;
        [SerializeField] private ScrollRect _scrollView;
        [SerializeField] private GameObject _networkBannerPrefab;
        [SerializeField] private GameObject _estimatedTotalPrefab;
        [SerializeField] private GameObject _dividerPrefab;
        
        private CollectibleOrder[] _listings;
        private Chain _chain;
        private IWallet _wallet;
        private ICheckout _checkout;
        private Dictionary<string, Sprite> _collectibleImagesByOrderId;
        private Dictionary<string, uint> _amountsRequestedByOrderId;
        private Cart _cart;
        private RectTransform _scrollViewLayoutGroupRectTransform;

        protected override void Awake()
        {
            base.Awake();
            _scrollViewLayoutGroupRectTransform = _scrollViewLayoutGroup.GetComponent<RectTransform>();
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
            _cart = args.GetObjectOfTypeIfExists<Cart>();
            if (_cart == null)
            {
                throw new ArgumentException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(Cart)} as an argument");
            }
            _listings = _cart.Listings;
            
            _chain = ChainDictionaries.ChainById[_listings[0].order.chainId.ToString()];
            
            _wallet = args.GetObjectOfTypeIfExists<IWallet>();
            if (_wallet == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(IWallet)} as an argument");
            }

            _collectibleImagesByOrderId = _cart.CollectibleImagesByOrderId;

            _amountsRequestedByOrderId = _cart.AmountsRequestedByOrderId;
            
            _checkout = new Checkout(_wallet, _chain);

            Assemble().ConfigureAwait(false);
        }

        private async Task Assemble()
        {
            int listings = _listings.Length;
            _numberOfUniqueItemsText.text = $"{listings} items";
            
            // Todo add back in; use a mock for now
            // Marketplace.CheckoutOptions options = await _checkout.GetCheckoutOptions(_listings.ToOrderArray());
            
            for (int i = 0; i < listings; i++)
            {
                CollectibleOrder listing = _listings[i];
                GameObject cartItem = Instantiate(_cartItemPrefab, _cartItemsParent);
                cartItem.GetComponent<CartItem>().Assemble(_cart, listing.order.orderId);
            }
            
            GameObject estimatedTotalGameObject = Instantiate(_estimatedTotalPrefab, _cartItemsParent);
            EstimatedTotal estimatedTotal = estimatedTotalGameObject.GetComponent<EstimatedTotal>();
            Marketplace.Currency bestCurrency = _cart.GetBestCurrency();
            string estimatedCurrencyRequired = await _cart.GetApproximateTotalInCurrency(new Address(bestCurrency.contractAddress));
            Sprite currencyIcon = await AssetHandler.GetSpriteAsync(bestCurrency.imageUrl);
            estimatedTotal.Assemble(_cart.GetApproximateTotalInUSD(), estimatedCurrencyRequired, bestCurrency.symbol, currencyIcon);
            
            GameObject networkBannerGameObject = Instantiate(_networkBannerPrefab, _cartItemsParent);
            NetworkBanner networkBanner = networkBannerGameObject.GetComponent<NetworkBanner>();
            networkBanner.Assemble(_chain);

            Instantiate(_dividerPrefab, _cartItemsParent);

            await AsyncExtensions.DelayTask(.1f);
            UpdateScrollViewSize();
        }

        private void UpdateScrollViewSize()
        {
            float contentHeight = _scrollViewLayoutGroup.preferredHeight;
            _scrollViewLayoutGroupRectTransform.sizeDelta =
                new Vector2(_scrollViewLayoutGroupRectTransform.sizeDelta.x, contentHeight);
            
            _scrollView.verticalNormalizedPosition = 1f;
        }
    }
}