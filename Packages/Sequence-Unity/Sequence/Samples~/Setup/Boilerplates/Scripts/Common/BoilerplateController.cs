using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Sequence.Boilerplates.Login;
using Sequence.Boilerplates.PlayerProfile;
using Sequence.Config;
using Sequence.EmbeddedWallet;
using Sequence.Utils.SecureStorage;
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
        
        [Header("Texts")]
        [SerializeField] private string _playerProfileDescription;
        [SerializeField] private string _signMessageDescription;
        [SerializeField] private string _dailyRewardsDescription;
        [SerializeField] private string _inventoryDescription;
        [SerializeField] private string _saleDescription;

        private IWallet _wallet;
        private SequenceLoginWindow _loginWindow;
        private SequencePlayerProfile _playerProfile;
        private Chain _chain;
        
        private void Awake()
        {
            SequenceWallet.OnFailedToRecoverSession += OnFailedToRecoverSession;
            SequenceWallet.OnWalletCreated += wallet =>
            {
                _wallet = wallet;
                ShowDefaultWindow();
                
                if (_loginWindow)
                    _loginWindow.Hide();
                
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
            var sequenceConfig = SequenceConfig.GetConfig();
            if (_config.useProjectKeys)
            {
                sequenceConfig.WaaSConfigKey = _config.waasConfigKey;
                sequenceConfig.BuilderAPIKey = _config.projectAccessKey;
                sequenceConfig.GoogleClientId = _config.googleClientId;
                sequenceConfig.AppleClientId = _config.appleClientId;
            }
            
            _chain = ChainDictionaries.ChainById.GetValueOrDefault(_config.chainId, Chain.TestnetArbitrumSepolia);
            _buttonPool.Cleanup();

            if (_config.playerProfile)
                ShowPlayerProfileButton();
            
            if (_config.signMessage)
                ShowSignMessageButton();
            
            if (_config.rewardsApi.StartsWith("https://"))
                ShowDailyRewardsButton();
            
            if (_config.collections.Length > 0)
                ShowInventoryButton();
            
            foreach (var sale in _config.primarySales)
                ShowPrimarySaleButton(sale);
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
            _playerProfile = BoilerplateFactory.OpenSequencePlayerProfile(transform, _wallet, _chain, ShowDefaultWindow);
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
    }
}