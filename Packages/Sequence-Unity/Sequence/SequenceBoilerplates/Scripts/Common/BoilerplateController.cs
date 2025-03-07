using System;
using Sequence.Boilerplates.Login;
using Sequence.Boilerplates.PlayerProfile;
using Sequence.Config;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
using Sequence.Pay;
using Sequence.Utils;
using Sequence.Utils.SecureStorage;
using UnityEngine;

namespace Sequence.Boilerplates
{
    public class BoilerplateController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private Chain _chain = Chain.TestnetArbitrumSepolia;
        [SerializeField] private string _dailyRewardsApi;
        [SerializeField] private string _collectionAddress;
        [SerializeField] private string _saleContractAddress;
        [SerializeField] private int[] _itemsForSale;
        [SerializeField] private GameObject _loadingScreenPrefab;
        
        [Header("Components")]
        [SerializeField] private GameObject _featureSelection;
        
        private IWallet _wallet;
        private SequenceLoginWindow _loginWindow;
        private SequencePlayerProfile _playerProfile;
        private GameObject _loadingScreen;
        
        private void Awake()
        {
            SequenceWallet.OnFailedToRecoverSession += OnFailedToRecoverSession;
            SequenceWallet.OnWalletCreated += wallet =>
            {
                _wallet = wallet;
                Debug.Log("Signed in with embedded wallet address: " + _wallet.GetWalletAddress());
                ShowFeatureSelection();
                
                if (_loginWindow)
                    _loginWindow.Hide();
                
                wallet.OnDropSessionComplete += s =>
                {
                    if (s == wallet.SessionId)
                    {
                        if (_playerProfile)
                            _playerProfile.Hide();
                        
                        TryRecoverSessionToOpenLoginWindow();
                    }
                };
            };
        }

        private void Start()
        {
            TryRecoverSessionToOpenLoginWindow();
        }

        private void OnDestroy()
        {
            BoilerplateFactory.CleanUp();
        }

        public void OpenPlayerProfilePanel()
        {
            HideFeatureSelection();
            _playerProfile = BoilerplateFactory.OpenSequencePlayerProfile(transform, _wallet, _chain, ShowFeatureSelection);
        }

        public void OpenSignMessage()
        {
            HideFeatureSelection();
            BoilerplateFactory.OpenSequenceSignMessage(transform, _wallet, _chain, ShowFeatureSelection);
        }
        
        public void OpenDailyRewardsPanel()
        {
            HideFeatureSelection();
            BoilerplateFactory.OpenSequenceDailyRewards(transform, _wallet, _chain, _dailyRewardsApi, ShowFeatureSelection);
        }
        
        public void OpenInventoryPanel()
        {
            HideFeatureSelection();
            BoilerplateFactory.OpenSequenceInventory(transform, _wallet, _chain, _collectionAddress, ShowFeatureSelection);
        }
        
        public void OpenInGameShopPanel()
        {
            HideFeatureSelection();
            BoilerplateFactory.OpenSequenceInGameShop(transform, _wallet, _chain, _collectionAddress,
                _saleContractAddress, _itemsForSale, ShowFeatureSelection);
        }

        public void OpenCheckoutPanel(ICheckoutHelper checkoutHelper, IFiatCheckout fiatCheckout)
        {
            HideFeatureSelection();
            BoilerplateFactory.OpenCheckoutPanel(transform, checkoutHelper, fiatCheckout, ShowFeatureSelection);
        }

        public void OpenViewMarketplaceListingsPage()
        {
            HideFeatureSelection();
            BoilerplateFactory.OpenViewMarketplaceListingsPanel(transform, _wallet, _chain, ShowFeatureSelection);
        }

        public void OpenCheckoutPanel()
        {
            HideFeatureSelection();
            ShowLoadingScreen();
            DoShowCheckoutPanel();
        }

        private void ShowLoadingScreen()
        {
            if (_loadingScreen != null)
            {
                return;
            }
            
            _loadingScreen = Instantiate(_loadingScreenPrefab, transform);
        }
        
        private void HideLoadingScreen()
        {
            if (_loadingScreen == null)
            {
                return;
            }
            
            Destroy(_loadingScreen);
            _loadingScreen = null;
        }

        private void ShowFeatureSelection()
        {
            _featureSelection.SetActive(true);
        }
        
        private void HideFeatureSelection()
        {
            _featureSelection.SetActive(false);
        }

        private async void DoShowCheckoutPanel()
        {
            ERC1155Sale saleContract = new ERC1155Sale("0xe65b75eb7c58ffc0bf0e671d64d0e1c6cd0d3e5b");
            ERC1155 collection = new ERC1155("0xdeb398f41ccd290ee5114df7e498cf04fac916cb");
            Sequence.Marketplace.TokenMetadata metadata =
                await new MarketplaceReader(Chain.Polygon).GetCollectible(collection, "1");
            string imageUrl = metadata.image.Replace(".webp", ".png");
            Sprite collectibleImage = await AssetHandler.GetSpriteAsync(imageUrl);
            ICheckoutHelper checkoutHelper = await ERC1155SaleCheckout.Create(saleContract, collection, "1", 1, _chain,
                _wallet, "Demo Token Sale",
                "https://cryptologos.cc/logos/usd-coin-usdc-logo.png",
                collectibleImage);
            
            HideLoadingScreen();
            BoilerplateFactory.OpenCheckoutPanel(transform, checkoutHelper,
                new SequenceCheckout(_wallet, Chain.Polygon, saleContract, collection, "1", 1), ShowFeatureSelection);
        }
        
        private void TryRecoverSessionToOpenLoginWindow()
        {
            HideFeatureSelection();
            var config = SequenceConfig.GetConfig();
            var storeSessionInfoAndSkipLoginWhenPossible = config.StoreSessionKey();
            var loginHandler = SequenceLogin.GetInstance();
            
            if (SecureStorageFactory.IsSupportedPlatform() && storeSessionInfoAndSkipLoginWhenPossible)
            {
                loginHandler.TryToRestoreSession();
                loginHandler.SetupAuthenticator();
            }
            else
            {
                OnFailedToRecoverSession("Secure Storage disabled");
            }
        }

        private void OnFailedToRecoverSession(string error)
        {
            Debug.LogError($"Error attempting to recover Sequence session: {error}");
            _loginWindow = BoilerplateFactory.OpenSequenceLoginWindow(transform);
        }
    }
}