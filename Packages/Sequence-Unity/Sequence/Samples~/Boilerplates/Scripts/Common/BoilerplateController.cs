using System;
using Sequence.Boilerplates.Login;
using Sequence.Boilerplates.PlayerProfile;
using Sequence.Config;
using Sequence.EmbeddedWallet;
using Sequence.Utils.SecureStorage;
using UnityEngine;
using UnityEngine.Events;

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
        [SerializeField] private string _defaultFeatures;
        
        [Header("Components")]
        [SerializeField] private GameObject _featureSelection;
        [SerializeField] private FeatureSelectionButton[] _featureButtons;
        
        private IWallet _wallet;
        private SequenceLoginWindow _loginWindow;
        private SequencePlayerProfile _playerProfile;
        private UnityAction _openDefaultWindow;
        
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
            EnableFeatures();
            TryRecoverSessionToOpenLoginWindow();
        }

        private void OnDestroy()
        {
            BoilerplateFactory.CleanUp();
        }

        public void OpenPlayerProfilePanel()
        {
            HideFeatureSelection();
            _playerProfile = BoilerplateFactory.OpenSequencePlayerProfile(transform, _wallet, _chain, ShowDefaultWindow);
        }
        
        public void OpenDailyRewardsPanel()
        {
            HideFeatureSelection();
            BoilerplateFactory.OpenSequenceDailyRewards(transform, _wallet, _chain, _dailyRewardsApi, ShowDefaultWindow);
        }
        
        public void OpenInventoryPanel()
        {
            HideFeatureSelection();
            BoilerplateFactory.OpenSequenceInventory(transform, _wallet, _chain, _collectionAddress, ShowDefaultWindow);
        }
        
        public void OpenInGameShopPanel()
        {
            HideFeatureSelection();
            BoilerplateFactory.OpenSequenceInGameShop(transform, _wallet, _chain, _collectionAddress,
                _saleContractAddress, _itemsForSale, ShowDefaultWindow);
        }

        public void OpenSignMessage()
        {
            HideFeatureSelection();
            BoilerplateFactory.OpenSequenceSignMessage(transform, _wallet, _chain, ShowDefaultWindow);
        }

        private void SetDefaultWindow(UnityAction openDefaultWindow)
        {
            _openDefaultWindow = openDefaultWindow;
        }

        private void ShowDefaultWindow()
        {
            _openDefaultWindow?.Invoke();
        }

        private void ShowFeatureSelection()
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

        private void EnableFeatures()
        {
            var features = GetFeatures();
            if (features.Length == 1)
            {
                var feature = features[0];
                var button = Array.Find(_featureButtons, b => b.Key == feature);
                SetDefaultWindow(button.ExecuteClick);
            }
            else
            {
                SetDefaultWindow(ShowFeatureSelection);
                Array.ForEach(_featureButtons, b => b.EnableIfExists(features));
            }
        }

        private string[] GetFeatures()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            var url = Application.absoluteURL;
#else
            var url = _defaultFeatures;
#endif
            
            if (string.IsNullOrEmpty(url))
                return Array.Empty<string>();

            try
            {
                var uri = new Uri(url);
                var parts = uri.Query.Split("features=");
                if (parts.Length != 2)
                    throw new Exception();
                
                return parts[1].Split("+");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return Array.Empty<string>();
            }
        }
    }
}