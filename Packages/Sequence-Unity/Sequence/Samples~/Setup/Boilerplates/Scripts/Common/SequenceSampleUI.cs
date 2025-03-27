using System;
using Sequence.Authentication;
using Sequence.Boilerplates;
using Sequence.Boilerplates.DailyRewards;
using Sequence.Boilerplates.InGameShop;
using Sequence.Boilerplates.Inventory;
using Sequence.Boilerplates.Login;
using Sequence.Boilerplates.Marketplace;
using Sequence.Boilerplates.PlayerProfile;
using Sequence.Config;
using Sequence.EmbeddedWallet;
using Sequence.Utils.SecureStorage;
using UnityEngine;

namespace Sequence.Demo
{
    public class SequenceSampleUI : MonoBehaviour
    {
        #region TestConfig
        public static bool IsTesting = false;
        public static UIPanel InitialPanel;
        public static object[] InitialPanelOpenArgs;
        #endregion

        public static SequenceSampleUI instance;

        private ILogin _loginHandler;
        private LoginPanel _loginWindow;
        private WalletPanel _walletPanel;
        private SequencePlayerProfile _playerProfile;
        private SequenceDailyRewards _dailyRewards;
        private SequenceInventory _inventory;
        private SequenceInGameShop _inGameShop;
        private ViewMarketplaceListingsPanel _viewMarketplaceListingsPanel;
        
        private void Awake()
        {
            if (instance == null) instance = this;
                else Destroy(gameObject);

            _loginWindow = GetComponentInChildren<LoginPanel>();
            _walletPanel = GetComponentInChildren<WalletPanel>();
            _playerProfile = GetComponentInChildren<SequencePlayerProfile>();
            _dailyRewards = GetComponentInChildren<SequenceDailyRewards>();
            _inventory = GetComponentInChildren<SequenceInventory>();
            _inGameShop = GetComponentInChildren<SequenceInGameShop>();
            _viewMarketplaceListingsPanel = GetComponentInChildren<ViewMarketplaceListingsPanel>();

            if (!IsTesting)
            {
                //InitialPanel = _loginPanel;
            }

            SequenceWallet.OnWalletCreated += wallet =>
            {
                DisableAllUIPages();
                
                wallet.OnDropSessionComplete += s =>
                {
                    if (s == wallet.SessionId)
                    {
                        ReplaceWithLoginPanel();
                    }
                };
            };
        }

        public void Start()
        {
            if (IsTesting)
            {
                return;
            }
            DisableAllUIPages();
            ReplaceWithLoginPanel();
        }

        private void DisableAllUIPages()
        {
            UIPage[] pages = GetComponentsInChildren<UIPage>();
            int count = pages.Length;
            for (int i = 0; i < count; i++)
            {
                pages[i].gameObject.SetActive(false);
            }
        }

        public void OpenPlayerProfile(IWallet wallet)
        {
        }
        
        public void OpenDailyRewards(IWallet wallet)
        {
        }

        public void OpenInventory(IWallet wallet)
        {
        }

        public void OpenInGameShop(IWallet wallet)
        {
        }

        public void OpenWalletPanelWithDelay(float delayInSeconds, params object[] openArgs)
        {
            _walletPanel.OpenWithDelay(delayInSeconds, openArgs);
        }
        
        private void ReplaceWithLoginPanel()
        {
            SequenceWallet.OnFailedToRecoverSession += OnFailedToRecoverSession;

            var config = SequenceConfig.GetConfig();
            var storeSessionInfoAndSkipLoginWhenPossible = config.StoreSessionKey();
            _loginHandler = SequenceLogin.GetInstance();
            
            if (SecureStorageFactory.IsSupportedPlatform() && storeSessionInfoAndSkipLoginWhenPossible)
            {
                _loginHandler.TryToRestoreSession();
                _loginHandler.SetupAuthenticator();
            }
            else
            {
                OnFailedToRecoverSession("Secure Storage disabled");
            }
        }

        private void OnFailedToRecoverSession(string error)
        {
            SequenceWallet.OnFailedToRecoverSession -= OnFailedToRecoverSession;
            Debug.LogError($"Error attempting to recover Sequence session: {error}");
            
            DisableAllUIPages();
            _loginWindow.Close();
        }
    }
}