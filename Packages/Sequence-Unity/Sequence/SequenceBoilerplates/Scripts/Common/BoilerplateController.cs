using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Sequence.Boilerplates.Login;
using Sequence.Boilerplates.PlayerProfile;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.Marketplace;
using Sequence.Pay;
using UnityEngine;

namespace Sequence.Boilerplates
{
    public class BoilerplateController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private BoilerplateConfigData _config;
        
        [Header("Components")]
        [SerializeField] private GameObject _featureSelection;
        [SerializeField] private GenericObjectPool<FeatureSelectionButton> _buttonPool;
        [SerializeField] private GameObject _loadingScreenPrefab;
        
        [Header("Texts")]
        [SerializeField] private string _playerProfileDescription = "Send & Receive ETH. Manage linked wallets & Sign Out.";
        [SerializeField] private string _signMessageDescription = "Sign messages with your wallet.";
        [SerializeField] private string _dailyRewardsDescription = "Get rewarded for consecutive days and claim one token every day!";
        [SerializeField] private string _inventoryDescription = "View all items you own for a specified contract address.";
        [SerializeField] private string _saleDescription = "Buy ERC1155 tokens via a Primary Sale contract.";
        [SerializeField] private string _marketplaceDescription = "Browse and interact with listings on a Peer-to-Peer, Secondary Sales marketplace.";
        [SerializeField] private string _checkoutDescription = "Buy an ERC1155 token via a Primary Sales contract using the Checkout Panel - pay with crypto or fiat.";

        private IWallet _wallet;
        private SequenceLoginWindow _loginWindow;
        private SequencePlayerProfile _playerProfile;
        private Chain _chain;
        private GameObject _loadingScreen;
        private Chain _marketplaceChain = Chain.Polygon;
        private string _marketplaceCollectionAddress = "0x0ee3af1874789245467e7482f042ced9c5171073";
        
        private void Awake()
        {
            SequenceWallet.OnFailedToRecoverSession += OnFailedToRecoverSession;
            SequenceWallet.OnWalletCreated += wallet =>
            {
                _wallet = wallet;
                ShowDefaultWindow();
                
                if (_loginWindow)
                    _loginWindow.gameObject.SetActive(false);
                
                wallet.OnDropSessionComplete += s =>
                {
                    if (s == wallet.SessionId)
                    {
                        if (_playerProfile)
                            _playerProfile.gameObject.SetActive(false);
                        
                        TryRecoverSessionToOpenLoginWindow();
                    }
                };
            };
        }

        private void Start()
        {
            SetupScene();
            TryRecoverSessionToOpenLoginWindow();
        }

#if UNITY_EDITOR
        [ContextMenu("Encode Boilerplate Config")]
        public void EncodeBoilerplateConfig()
        {
            var json = JsonConvert.SerializeObject(_config);
            var data = Encoding.UTF8.GetBytes(json);
            var encoded = System.Convert.ToBase64String(data);

            GUIUtility.systemCopyBuffer = encoded;
            Debug.Log($"Copied: (len {encoded.Length}) {encoded}");
        }
#endif

        private void OnDestroy()
        {
            BoilerplateFactory.CleanUp();
        }

        private void ShowDefaultWindow()
        {
            _featureSelection.SetActive(true);
        }
        
        private void HideFeatureSelection()
        {
            _featureSelection.SetActive(false);
        }
        
        private async void TryRecoverSessionToOpenLoginWindow()
        {
            HideFeatureSelection();
            
            var loginHandler = SequenceLogin.GetInstance();
            var (storageEnabled, wallet) = await loginHandler.TryToRestoreSessionAsync();
            if (!storageEnabled)
                OnFailedToRecoverSession("Secure storage is disabled");
        }

        private void OnFailedToRecoverSession(string error)
        {
            Debug.Log($"There's no session to recover from storage. Reason: {error}");
            _loginWindow = BoilerplateFactory.OpenSequenceLoginWindow(transform);
        }

        private void SetupScene()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            try
            {
                var url = Application.absoluteURL;
                if (string.IsNullOrEmpty(url))
                    throw new System.Exception();
            
                var query = new System.Uri(url).Query;
                if (query.Contains("config="))
                {
                    var encodedConfig = query.Split("config=")[1];
                    var decodedConfig = System.Convert.FromBase64String(encodedConfig);
                    var json = Encoding.UTF8.GetString(decodedConfig);
                    
                    _config = JsonConvert.DeserializeObject<BoilerplateConfigData>(json);
                    SetupConfig();
                }
                else
                    SetupConfig();
            }
            catch (System.Exception e)
            {
                SetupConfig();
                throw;
            }
#else
            SetupConfig();
#endif
        }

        private void SetupConfig()
        {
            _chain = ChainDictionaries.ChainById.GetValueOrDefault(_config.chainId, Chain.TestnetArbitrumSepolia);
            _buttonPool.Cleanup();

            if (_config.playerProfile)
                ShowPlayerProfileButton();
            
            if (_config.signMessage)
                ShowSignMessageButton();
            
            if (!string.IsNullOrEmpty(_config.rewardsApi) && _config.rewardsApi.StartsWith("https://"))
                ShowDailyRewardsButton();
            
            if (_config.collections.Length > 0)
                ShowInventoryButton();
            
            foreach (var sale in _config.primarySales)
                ShowPrimarySaleButton(sale);

            if (_config.secondarySale != null)
            {
                _marketplaceChain = _config.secondarySale.chain;
                _marketplaceCollectionAddress = _config.secondarySale.collectionAddress;
                ShowSecondarySaleButton();
            }
            
            if (_config.checkout)
                _buttonPool.GetObject().Show("Checkout Panel", _checkoutDescription, OpenCheckoutPanel);
        }

        private void ShowPlayerProfileButton()
        {
            _buttonPool.GetObject().Show("Player Profile", _playerProfileDescription, OpenPlayerProfilePanel);
        }
        
        private void ShowSignMessageButton()
        {
            _buttonPool.GetObject().Show("Sign Message", _signMessageDescription, OpenSignMessage);
        }
        
        private void ShowDailyRewardsButton()
        {
            _buttonPool.GetObject().Show("Daily Rewards", _dailyRewardsDescription, OpenDailyRewardsPanel);
        }
        
        private void ShowInventoryButton()
        {
            _buttonPool.GetObject().Show("Inventory", _inventoryDescription, OpenInventoryPanel);
        }

        private void ShowSecondarySaleButton()
        {
            _buttonPool.GetObject().Show("Secondary Sales Marketplace", _marketplaceDescription, OpenViewMarketplaceListingsPage);
        }
        
        private void ShowPrimarySaleButton(PrimarySaleConfig sale)
        {
            _buttonPool.GetObject().Show($"{sale.name} Shop", _saleDescription, () =>
            {
                OpenInGameShopPanel(sale);
            });
        }
        
        private void OpenPlayerProfilePanel()
        {
            HideFeatureSelection();
            _playerProfile = BoilerplateFactory.OpenSequencePlayerProfile(transform, _wallet, _chain, new Address("0x85acb5646a9d73952347174ef928c2c9a174156f"), "STB", ShowDefaultWindow);
        }
        
        private void OpenDailyRewardsPanel()
        {
            HideFeatureSelection();
            BoilerplateFactory.OpenSequenceDailyRewards(transform, _wallet, _chain, _config.rewardsApi, ShowDefaultWindow);
        }
        
        private void OpenInventoryPanel()
        {
            HideFeatureSelection();
            BoilerplateFactory.OpenSequenceInventory(transform, _wallet, _chain, _config.collections, ShowDefaultWindow);
        }
        
        private void OpenInGameShopPanel(PrimarySaleConfig sale)
        {
            HideFeatureSelection();
            BoilerplateFactory.OpenSequenceInGameShop(transform, _wallet, _chain, sale.collectionAddress,
                sale.saleAddress, sale.itemsForSale, ShowDefaultWindow);
        }

        private void OpenSignMessage()
        {
            HideFeatureSelection();
            BoilerplateFactory.OpenSequenceSignMessage(transform, _wallet, _chain, ShowDefaultWindow);
        }

        public void OpenCheckoutPanel(ICheckoutHelper checkoutHelper, IFiatCheckout fiatCheckout)
        {
            HideFeatureSelection();
            BoilerplateFactory.OpenCheckoutPanel(transform, _chain, checkoutHelper, fiatCheckout, ShowDefaultWindow);
        }

        public void OpenViewMarketplaceListingsPage()
        {
            HideFeatureSelection();
            BoilerplateFactory.OpenViewMarketplaceListingsPanel(transform, _wallet, _marketplaceChain, new Address(_marketplaceCollectionAddress), ShowDefaultWindow);
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

        private async void DoShowCheckoutPanel()
        {
            ERC1155Sale saleContract = new ERC1155Sale("0xe65b75eb7c58ffc0bf0e671d64d0e1c6cd0d3e5b");
            ERC1155 collection = new ERC1155("0xdeb398f41ccd290ee5114df7e498cf04fac916cb");
            Sequence.Marketplace.TokenMetadata metadata =
                await new MarketplaceReader(Chain.Polygon).GetCollectible(collection, "1");
            string imageUrl = metadata.image.Replace(".webp", ".png");
            Sprite collectibleImage = await AssetHandler.GetSpriteAsync(imageUrl);
            ICheckoutHelper checkoutHelper = await ERC1155SaleCheckout.Create(saleContract, collection, "1", 1, Chain.Polygon,
                _wallet, "Demo Token Sale",
                "https://cryptologos.cc/logos/usd-coin-usdc-logo.png",
                collectibleImage);
            
            HideLoadingScreen();
            BoilerplateFactory.OpenCheckoutPanel(transform, _chain, checkoutHelper,
                new SequenceCheckout(_wallet, Chain.Polygon, saleContract, collection, "1", 1), ShowDefaultWindow);
        }
    }
}
