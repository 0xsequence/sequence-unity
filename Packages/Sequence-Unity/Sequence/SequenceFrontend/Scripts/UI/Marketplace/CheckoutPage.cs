using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sequence.EmbeddedWallet;
using Sequence.Integrations.Transak;
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
        [SerializeField] private GameObject _payWithCryptoTextPrefab;
        [SerializeField] private GameObject _tokenPaymentOptionPrefab;
        [SerializeField] private Button _completePurchaseButton;
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private GameObject _qrCodeButtonPrefab;
        [SerializeField] private GameObject _fundWithCreditCardPrefab;
        
        private CollectibleOrder[] _listings;
        private Chain _chain;
        private IWallet _wallet;
        private ICheckout _checkout;
        private Dictionary<string, Sprite> _collectibleImagesByOrderId;
        private Dictionary<string, uint> _amountsRequestedByOrderId;
        private ICheckoutHelper _cart;
        private RectTransform _scrollViewLayoutGroupRectTransform;
        private EstimatedTotal _estimatedTotal;
        private List<TokenPaymentOption> _tokenPaymentOptions;
        private Marketplace.Currency _bestCurrency;
        private List<GameObject> _spawnedGameObjects;

        protected override void Awake()
        {
            base.Awake();
            _scrollViewLayoutGroupRectTransform = _scrollViewLayoutGroup.GetComponent<RectTransform>();
        }

        public override void Open(params object[] args)
        {
            base.Open(args);
            _cart = args.GetObjectOfTypeIfExists<ICheckoutHelper>();
            if (_cart == null)
            {
                throw new ArgumentException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(ICheckoutHelper)} as an argument");
            }
            _listings = _cart.GetListings();
            
            _chain = ChainDictionaries.ChainById[_listings[0].order.chainId.ToString()];
            
            _wallet = _cart.GetWallet();

            _collectibleImagesByOrderId = _cart.GetCollectibleImagesByOrderId();

            _amountsRequestedByOrderId = _cart.GetAmountsRequestedByOrderId();

            _tokenPaymentOptions = new List<TokenPaymentOption>();
            
            _checkout = _cart.GetICheckout();
            
            CartItem.OnAmountChanged += OnCartAmountChanged;

            Assemble().ConfigureAwait(false);
        }

        public override void Close()
        {
            base.Close();
            CartItem.OnAmountChanged -= OnCartAmountChanged;
            
            foreach (var spawnedGameObject in _spawnedGameObjects)
            {
                Destroy(spawnedGameObject);
            }
        }

        private async Task Assemble()
        {
            int listings = _listings.Length;
            _numberOfUniqueItemsText.text = $"{listings} items";
            
            _spawnedGameObjects = new List<GameObject>();
            
            for (int i = 0; i < listings; i++)
            {
                CollectibleOrder listing = _listings[i];
                GameObject cartItem = Instantiate(_cartItemPrefab, _cartItemsParent);
                cartItem.GetComponent<CartItem>().Assemble(_cart, listing.order.orderId);
                _spawnedGameObjects.Add(cartItem);
            }
            
            GameObject estimatedTotalGameObject = Instantiate(_estimatedTotalPrefab, _cartItemsParent);
            _estimatedTotal = estimatedTotalGameObject.GetComponent<EstimatedTotal>();
            await RefreshEstimatedTotal();
            _spawnedGameObjects.Add(estimatedTotalGameObject);
            
            GameObject networkBannerGameObject = Instantiate(_networkBannerPrefab, _cartItemsParent);
            NetworkBanner networkBanner = networkBannerGameObject.GetComponent<NetworkBanner>();
            networkBanner.Assemble(_chain);
            _spawnedGameObjects.Add(networkBannerGameObject);

            _spawnedGameObjects.Add(Instantiate(_dividerPrefab, _cartItemsParent));
            _spawnedGameObjects.Add(Instantiate(_payWithCryptoTextPrefab, _cartItemsParent));
            
            Marketplace.Currency[] currencies = await _cart.GetCurrencies();
            int currenciesLength = currencies.Length;
            for (int i = 0; i < currenciesLength; i++)
            {
                Marketplace.Currency currency = currencies[i];
                GameObject tokenPaymentOptionGameObject = Instantiate(_tokenPaymentOptionPrefab, _cartItemsParent);
                TokenPaymentOption tokenPaymentOption = tokenPaymentOptionGameObject.GetComponent<TokenPaymentOption>();
                bool success = await RefreshTokenPaymentOption(tokenPaymentOption, currency);
                
                if (!success)
                {
                    Destroy(tokenPaymentOptionGameObject);
                    continue;
                }
                
                if (!HasAtLeastOneCryptoPaymentOption())
                {
                    tokenPaymentOption.SelectCurrency();
                }
                _tokenPaymentOptions.Add(tokenPaymentOption);
                _spawnedGameObjects.Add(tokenPaymentOptionGameObject);
            }
            
            _spawnedGameObjects.Add(Instantiate(_dividerPrefab, _cartItemsParent));
            
            GameObject qrCodeButtonGameObject = Instantiate(_qrCodeButtonPrefab, _cartItemsParent);
            Button qrCodeButton = qrCodeButtonGameObject.GetComponent<Button>();
            qrCodeButton.onClick.AddListener(OpenQrCodePage);
            _spawnedGameObjects.Add(qrCodeButtonGameObject);

            if (HasAtLeastOneCryptoPaymentOption())
            {
                _completePurchaseButton.interactable = true;
            }
            else
            {
                _completePurchaseButton.interactable = false;
            }

            Marketplace.CheckoutOptions options = null;
            try
            {
                options = await _checkout.GetCheckoutOptions(_listings.ToOrderArray());
            }
            catch (Exception e)
            {
                string message =  $"Error fetching checkout options: {e.Message}";
                Debug.LogError(message);
            }
            
            if (options != null && options.onRamp != null && options.onRamp.Length > 0)
            {
                GameObject fundWithCreditCardGameObject = Instantiate(_fundWithCreditCardPrefab, _cartItemsParent);
                Button fundWithCreditCardButton = fundWithCreditCardGameObject.GetComponent<Button>();
                fundWithCreditCardButton.onClick.AddListener(() =>
                {
                    if (options.onRamp.Contains(TransactionOnRampProvider.transak))
                    {
                        TransakOnRamp transakOnRamp = new TransakOnRamp(_wallet.GetWalletAddress());
                        transakOnRamp.OpenTransakLink();
                    }
                });
                _spawnedGameObjects.Add(fundWithCreditCardGameObject);
            }
            else
            {
                Debug.LogWarning($"No {nameof(Marketplace.CheckoutOptions)} found. Assuming crypto only checkout.");
            }
            
            // Todo instantiate credit card based checkout stuff (only if we have one cart item)

            await AsyncExtensions.DelayTask(.1f);
            UpdateScrollViewSize();
        }

        private bool HasAtLeastOneCryptoPaymentOption()
        {
            return _tokenPaymentOptions.Count > 0;
        }

        private async Task RefreshEstimatedTotal()
        {
            if (_estimatedTotal == null)
            {
                return;
            }
            _bestCurrency = await _cart.GetBestCurrency();
            string estimatedCurrencyRequired = await _cart.GetApproximateTotalInCurrency(new Address(_bestCurrency.contractAddress));
            if (estimatedCurrencyRequired.StartsWith("Error"))
            {
                Debug.LogError(estimatedCurrencyRequired);
            }
            Sprite currencyIcon = await _cart.GetCurrencyIcon(_bestCurrency);
            _estimatedTotal.Assemble(_cart.GetApproximateTotalInUSD(), estimatedCurrencyRequired, _bestCurrency.symbol, currencyIcon);
        }

        private async Task<bool> RefreshTokenPaymentOption(TokenPaymentOption tokenPaymentOption, Marketplace.Currency currency)
        {
            string quotedPrice = await _cart.GetApproximateTotalInCurrencyIfAffordable(new Address(currency.contractAddress));
            if (string.IsNullOrWhiteSpace(quotedPrice) || quotedPrice == "")
            {
                Debug.Log($"User {_wallet.GetWalletAddress()} has insufficient balance to pay with {currency.contractAddress}. Skipping...");
                return false;
            }

            if (quotedPrice.StartsWith("Error"))
            {
                Debug.LogError(quotedPrice + "\nSkipping...");
                return false;
            }
                
            Sprite tokenIcon = await _cart.GetCurrencyIcon(currency);
            tokenPaymentOption.Assemble(currency, quotedPrice, tokenIcon);
            return true;
        }

        private void OnCartAmountChanged()
        {
            RefreshEstimatedTotal().ConfigureAwait(false);
            RefreshTokenPaymentOptions().ConfigureAwait(false);
        }

        // Todo switch to using object pool for token payment options
        private async Task RefreshTokenPaymentOptions()
        {
            Marketplace.Currency[] currencies = await _cart.GetCurrencies();
            int tokenPaymentOptionsCount = _tokenPaymentOptions.Count;
            for (int i = 0; i < tokenPaymentOptionsCount; i++)
            {
                TokenPaymentOption tokenPaymentOption = _tokenPaymentOptions[i];
                bool refreshed = false;
                foreach (var currency in currencies)
                {
                    if (tokenPaymentOption.UsesCurrency(currency))
                    {
                        refreshed = await RefreshTokenPaymentOption(tokenPaymentOption, currency);
                    }
                }

                if (!refreshed)
                {
                    Destroy(tokenPaymentOption.gameObject);
                }
            }
        }

        private void UpdateScrollViewSize()
        {
            float contentHeight = _scrollViewLayoutGroup.preferredHeight;
            _scrollViewLayoutGroupRectTransform.sizeDelta =
                new Vector2(_scrollViewLayoutGroupRectTransform.sizeDelta.x, contentHeight);
            
            _scrollView.verticalNormalizedPosition = 1f;
        }

        public void Checkout()
        {
            DoCheckout().ConfigureAwait(false);
        }

        private async Task DoCheckout()
        {
            _loadingScreen.SetActive(true);
            bool success = await CheckoutWithCart();
            _loadingScreen.SetActive(false);
            if (success)
            {
                _panel.Close();
            }
            else
            {
                // todo do something here
            }
        }
        
        private async Task<bool> CheckoutWithCart()
        {
            try
            {
                TransactionReturn transactionReturn = await _cart.Checkout();
                if (transactionReturn is FailedTransactionReturn failedTransactionReturn)
                {
                    Debug.LogError(failedTransactionReturn.error);
                    return false;
                }
                else
                {
                    Debug.Log("Checkout successful");
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        private void OpenQrCodePage()
        {
            if (_panel is CheckoutPanel checkoutPanel)
            {
                checkoutPanel.OpenQrCodePage(new QrCodeParams(new Address(_bestCurrency.contractAddress), _chain, _wallet.GetWalletAddress()));
            }
        }
    }
}