using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.EmbeddedWallet;
using Sequence.Pay.Transak;
using Sequence.Marketplace;
using Sequence.Pay;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

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
        [SerializeField] private GameObject _payWithCreditCardPrefab;
        
        private CartItemData[] _cartItemDatas;
        private Chain _chain;
        private IWallet _wallet;
        private ICheckoutHelper _cart;
        private RectTransform _scrollViewLayoutGroupRectTransform;
        private EstimatedTotal _estimatedTotal;
        private List<TokenPaymentOption> _tokenPaymentOptions;
        private Marketplace.Currency _bestCurrency;
        private List<GameObject> _spawnedGameObjects;
        private IFiatCheckout _fiatCheckout;
        private IIndexer _indexer;

        protected override void Awake()
        {
            base.Awake();
            _scrollViewLayoutGroupRectTransform = _scrollViewLayoutGroup.GetComponent<RectTransform>();
        }

        public override void Open(params object[] args)
        {
            _cart = args.GetObjectOfTypeIfExists<ICheckoutHelper>();
            if (_cart == null)
            {
                throw new ArgumentException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(ICheckoutHelper)} as an argument");
            }
            
            _fiatCheckout = args.GetObjectOfTypeIfExists<IFiatCheckout>();
            if (_fiatCheckout == null)
            {
                Debug.LogWarning(
                    $"{GetType().Name} must be opened with a {typeof(IFiatCheckout)} as an argument in order to use fiat checkout features");
            }
            
            _indexer = args.GetObjectOfTypeIfExists<IIndexer>();
            
            Configure();
            
            CartItem.OnAmountChanged += OnCartAmountChanged;

            Assemble(args).ConfigureAwait(false);
        }

        protected void Configure()
        {
            _cartItemDatas = _cart.GetCartItemData();
            if (_cartItemDatas == null || _cartItemDatas.Length == 0)
            {
                throw new SystemException(
                    "Must have at least one cart item");
            }
            _chain = _cartItemDatas[0].Network;
            _wallet = _cart.GetWallet();
            _tokenPaymentOptions = new List<TokenPaymentOption>();
            
            if (_indexer == null)
            {
                _indexer = new ChainIndexer(_chain);
            }
        }

        public override void Close()
        {
            base.Close();
            CartItem.OnAmountChanged -= OnCartAmountChanged;
            
            foreach (var spawnedGameObject in _spawnedGameObjects)
            {
                if (spawnedGameObject == null)
                {
                    continue;
                }
                Destroy(spawnedGameObject);
            }
        }

        protected async Task Assemble(object[] args)
        {
            _loadingScreen.SetActive(true);
            
            int listings = _cartItemDatas.Length;
            _numberOfUniqueItemsText.text = $"{listings} items";
            
            _spawnedGameObjects = new List<GameObject>();
            
            AssembleCartItems(listings);
            
            await SetEstimatedTotal();
            
            AssembleNetworkBanner();

            _spawnedGameObjects.Add(Instantiate(_dividerPrefab, _cartItemsParent));
            _spawnedGameObjects.Add(Instantiate(_payWithCryptoTextPrefab, _cartItemsParent));

            await AssembleTokenPaymentOptions();
            
            _spawnedGameObjects.Add(Instantiate(_dividerPrefab, _cartItemsParent));
            
            SetupQrCode();

            if (HasAtLeastOneCryptoPaymentOption())
            {
                _completePurchaseButton.interactable = true;
            }
            else
            {
                _completePurchaseButton.interactable = false;
            }

            if (_fiatCheckout != null)
            {
                bool onRampEnabled = _fiatCheckout.OnRampEnabled();
                AddOnRamp(onRampEnabled);
            
                bool creditCardCheckoutEnabled = await _fiatCheckout.NftCheckoutEnabled();
                AddCreditCardCheckout(creditCardCheckoutEnabled);
            }
            
            base.Open(args);
            
            _loadingScreen.SetActive(false);

            await AsyncExtensions.DelayTask(.3f);
            UpdateScrollViewSize();
        }

        protected void AssembleCartItems(int listings)
        {
            for (int i = 0; i < listings; i++)
            {
                CartItemData itemData = _cartItemDatas[i];
                GameObject cartItem = Instantiate(_cartItemPrefab, _cartItemsParent);
                cartItem.GetComponent<CartItem>().Assemble(_cart, itemData);
                _spawnedGameObjects.Add(cartItem);
            }
        }

        protected async Task SetEstimatedTotal()
        {
            GameObject estimatedTotalGameObject = Instantiate(_estimatedTotalPrefab, _cartItemsParent);
            _estimatedTotal = estimatedTotalGameObject.GetComponent<EstimatedTotal>();
            await RefreshEstimatedTotal();
            _spawnedGameObjects.Add(estimatedTotalGameObject);
        }

        protected void AssembleNetworkBanner()
        {
            GameObject networkBannerGameObject = Instantiate(_networkBannerPrefab, _cartItemsParent);
            NetworkBanner networkBanner = networkBannerGameObject.GetComponent<NetworkBanner>();
            networkBanner.Assemble(_chain);
            _spawnedGameObjects.Add(networkBannerGameObject);
        }

        protected async Task AssembleTokenPaymentOptions()
        {
            Marketplace.Currency[] currencies = await _cart.GetCurrencies();
            EtherBalance etherBalance = await _indexer.GetEtherBalance(_wallet.GetWalletAddress());
            GetTokenBalancesReturn tokenBalancesReturn = await _indexer.GetTokenBalances(new GetTokenBalancesArgs(_wallet.GetWalletAddress()));

            Marketplace.Currency nativeCurrency = currencies.GetNativeCurrency();
            if (nativeCurrency != null && etherBalance.balanceWei > 0)
            {
                await AddTokenPaymentOption(nativeCurrency, new TokenBalance()
                {
                    contractAddress = Marketplace.Currency.NativeCurrencyAddress,
                    balance = etherBalance.balanceWei,
                    contractType = ContractType.NATIVE,
                    accountAddress = _wallet.GetWalletAddress(),
                    chainId = BigInteger.Parse(ChainDictionaries.ChainIdOf[_chain]),
                    tokenMetadata = new TokenMetadata()
                    {
                        decimals = 18,
                    },
                    contractInfo = new ContractInfo()
                    {
                        decimals = 18,
                    }
                });
            }
            
            TokenBalance[] balances = tokenBalancesReturn.balances;
            (Marketplace.Currency, TokenBalance)[] currenciesWithBalances = GetCurrenciesWithBalances(currencies, balances);
            await AddTokenPaymentOptions(currenciesWithBalances);

            while (tokenBalancesReturn.page.more)
            {
                tokenBalancesReturn =
                    await _indexer.GetTokenBalances(new GetTokenBalancesArgs(_wallet.GetWalletAddress(), false,
                        tokenBalancesReturn.page));
                currenciesWithBalances = GetCurrenciesWithBalances(currencies, tokenBalancesReturn.balances);
                await AddTokenPaymentOptions(currenciesWithBalances);
            }
        }

        private async Task AddTokenPaymentOptions((Marketplace.Currency, TokenBalance)[] currenciesWithBalances)
        {
            foreach (var (currency, balance) in currenciesWithBalances)
            {
                await AddTokenPaymentOption(currency, balance);
            }
        }

        private async Task AddTokenPaymentOption(Marketplace.Currency currency, TokenBalance balance)
        {
            GameObject tokenPaymentOptionGameObject = Instantiate(_tokenPaymentOptionPrefab, _cartItemsParent);
            TokenPaymentOption tokenPaymentOption = tokenPaymentOptionGameObject.GetComponent<TokenPaymentOption>();
            bool success = await RefreshTokenPaymentOption(tokenPaymentOption, currency, balance);
                
            if (!success)
            {
                Destroy(tokenPaymentOptionGameObject);
                return;
            }
                
            if (!HasAtLeastOneCryptoPaymentOption())
            {
                tokenPaymentOption.SelectCurrency();
            }
            _tokenPaymentOptions.Add(tokenPaymentOption);
            _spawnedGameObjects.Add(tokenPaymentOptionGameObject);
        }
        
        private (Marketplace.Currency, TokenBalance)[] GetCurrenciesWithBalances(Marketplace.Currency[] currencies, TokenBalance[] balances)
        {
            List<(Marketplace.Currency, TokenBalance)> currenciesWithBalances = new List<(Marketplace.Currency, TokenBalance)>();
            foreach (var currency in currencies)
            {
                int tokenIndex = balances.TokenIndex(currency.contractAddress);
                if (tokenIndex != -1)
                {
                    TokenBalance balance = balances[tokenIndex];
                    currenciesWithBalances.Add((currency, balance));
                }
            }

            return currenciesWithBalances.ToArray();
        }

        protected void SetupQrCode()
        {
            GameObject qrCodeButtonGameObject = Instantiate(_qrCodeButtonPrefab, _cartItemsParent);
            Button qrCodeButton = qrCodeButtonGameObject.GetComponent<Button>();
            qrCodeButton.onClick.AddListener(OpenQrCodePage);
            _spawnedGameObjects.Add(qrCodeButtonGameObject);
        }

        protected void AddOnRamp(bool onRampEnabled)
        {
            if (!onRampEnabled)
            {
                return;
            }
            
            GameObject fundWithCreditCardGameObject = Instantiate(_fundWithCreditCardPrefab, _cartItemsParent);
            Button fundWithCreditCardButton = fundWithCreditCardGameObject.GetComponent<Button>();
            fundWithCreditCardButton.onClick.AddListener(async () =>
            {
                string onRampLink = await _fiatCheckout.GetOnRampLink();
                Application.OpenURL(onRampLink);
            });
            _spawnedGameObjects.Add(fundWithCreditCardGameObject);
        }

        protected void AddCreditCardCheckout(bool creditCardCheckoutEnabled)
        {
            if (!creditCardCheckoutEnabled)
            {
                return;
            }
            
            GameObject payWithCreditCardGameObject = Instantiate(_payWithCreditCardPrefab, _cartItemsParent);
            Button payWithCreditCardButton = payWithCreditCardGameObject.GetComponent<Button>();
            payWithCreditCardButton.onClick.AddListener(async () =>
            {
                string checkoutLink = await _fiatCheckout.GetNftCheckoutLink();
                Application.OpenURL(checkoutLink);
            });
            _spawnedGameObjects.Add(payWithCreditCardGameObject);
        }

        private bool HasAtLeastOneCryptoPaymentOption()
        {
            return _tokenPaymentOptions.Count > 0;
        }

        protected async Task RefreshEstimatedTotal()
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
            string approximateTotalInUsd = await _cart.GetApproximateTotalInUSD();
            bool success = _estimatedTotal.Assemble(approximateTotalInUsd, estimatedCurrencyRequired, _bestCurrency.symbol, currencyIcon);
            if (!success)
            {
                _estimatedTotal = null;
            }
        }

        protected async Task<bool> RefreshTokenPaymentOption(TokenPaymentOption tokenPaymentOption, Marketplace.Currency currency, TokenBalance userBalance = null)
        {
            string quotedPrice = await _cart.GetApproximateTotalInCurrencyIfAffordable(new Address(currency.contractAddress), userBalance);
            if (string.IsNullOrWhiteSpace(quotedPrice) || quotedPrice == "")
            {
                Debug.Log($"User {_wallet.GetWalletAddress()} has insufficient balance to pay with {currency.contractAddress}. Skipping...");
                return false;
            }

            if (quotedPrice.StartsWith("Error"))
            {
                Debug.LogWarning(quotedPrice + "\nSkipping...");
                return false;
            }
                
            Sprite tokenIcon = await _cart.GetCurrencyIcon(currency);
            tokenPaymentOption.Assemble(currency, quotedPrice, tokenIcon);
            return true;
        }

        private void OnCartAmountChanged(BigInteger newAmount)
        {
            RefreshEstimatedTotal().ConfigureAwait(false);
            RefreshTokenPaymentOptions().ConfigureAwait(false);
            _fiatCheckout.SetAmountRequested(newAmount);
        }

        // Todo switch to using object pool for token payment options
        protected async Task RefreshTokenPaymentOptions()
        {
            int tokenPaymentOptionsCount = _tokenPaymentOptions.Count;
            for (int i = 0; i < tokenPaymentOptionsCount; i++)
            {
                TokenPaymentOption tokenPaymentOption = _tokenPaymentOptions[i];
                bool refreshed = await RefreshTokenPaymentOption(tokenPaymentOption, tokenPaymentOption.Currency);

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

        protected async Task DoCheckout()
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
        
        protected async Task<bool> CheckoutWithCart()
        {
            try
            {
                TransactionReturn transactionReturn = await _cart.Checkout();
                if (transactionReturn is FailedTransactionReturn failedTransactionReturn)
                {
                    Debug.LogError(failedTransactionReturn.error);
                    return false;
                }
                else if (transactionReturn is SuccessfulTransactionReturn result)
                {
                    Debug.Log($"Checkout successful. See transaction here: {ChainDictionaries.BlockExplorerOf[_chain].AppendTrailingSlashIfNeeded()}tx/{result?.txHash}");
                    return true;
                }
                else
                {
                    throw new Exception(
                        $"Encountered unexpected result type {transactionReturn.GetType()} when checking out");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        protected void OpenQrCodePage()
        {
            if (_panel is CheckoutPanel checkoutPanel)
            {
                checkoutPanel.OpenQrCodePage(new QrCodeParams(new Address(_bestCurrency.contractAddress), _chain, _wallet.GetWalletAddress()));
            }
        }
    }
}