using Sequence.Authentication;
using Sequence.Config;
using Sequence.EmbeddedWallet;
using Sequence.Utils.SecureStorage;
using SequenceSDK.Samples;
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
        private SequenceLoginWindow _loginWindow;
        private TransitionPanel _featureSelection;
        private WalletPanel _walletPanel;
        private SignMessagePanel _signMessagePanel;
        private SendTransactionPanel _sendTransactionPanel;
        private SendTransactionWithFeeOptionsPanel _sendTransactionWithFeeOptionsPanel;
        private SeeMarketplaceListingsPanel _seeMarketplaceListingsPanel;
        private MarketplaceItemDetailsPanel _marketplaceItemDetailsPanel;
        private SequencePlayerProfile _playerProfile;
        private SequenceDailyRewards _dailyRewards;
        private SequenceInventory _inventory;
        private SequenceInGameShop _inGameShop;
        
        private void Awake()
        {
            if (instance == null) instance = this;
                else Destroy(gameObject);

            _loginWindow = GetComponentInChildren<SequenceLoginWindow>();
            _featureSelection = GetComponentInChildren<TransitionPanel>();
            _walletPanel = GetComponentInChildren<WalletPanel>();
            _signMessagePanel = GetComponentInChildren<SignMessagePanel>();
            _sendTransactionPanel = GetComponentInChildren<SendTransactionPanel>();
            _sendTransactionWithFeeOptionsPanel = GetComponentInChildren<SendTransactionWithFeeOptionsPanel>();
            _seeMarketplaceListingsPanel = GetComponentInChildren<SeeMarketplaceListingsPanel>();
            _marketplaceItemDetailsPanel = GetComponentInChildren<MarketplaceItemDetailsPanel>();
            _playerProfile = GetComponentInChildren<SequencePlayerProfile>();
            _dailyRewards = GetComponentInChildren<SequenceDailyRewards>();
            _inventory = GetComponentInChildren<SequenceInventory>();
            _inGameShop = GetComponentInChildren<SequenceInGameShop>();

            if (!IsTesting)
            {
                //InitialPanel = _loginPanel;
            }

            SequenceWallet.OnWalletCreated += wallet =>
            {
                DisableAllUIPages();
                _featureSelection.gameObject.SetActive(true);
                
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
                return;
            
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
            
            _loginWindow.Hide();
            _playerProfile.Hide();
            _inventory.Hide();
            _inGameShop.Hide();
            _dailyRewards.Hide();
        }

        public void OpenPlayerProfile(IWallet wallet)
        {
            _playerProfile.Show(wallet);
        }
        
        public void OpenDailyRewards(IWallet wallet)
        {
            _dailyRewards.Show(wallet);
        }

        public void OpenInventory(IWallet wallet)
        {
            _inventory.Show(wallet);
        }

        public void OpenInGameShop(IWallet wallet)
        {
            _inGameShop.Show(wallet);
        }

        public void OpenWalletPanelWithDelay(float delayInSeconds, params object[] openArgs)
        {
            _walletPanel.OpenWithDelay(delayInSeconds, openArgs);
        }
        
        public void OpenSignMessagePanelWithDelay(float delayInSeconds, params object[] openArgs)
        {
            _signMessagePanel.OpenWithDelay(delayInSeconds, openArgs);
        }
        
        public void OpenSendTransactionPanelWithDelay(float delayInSeconds, params object[] openArgs)
        {
            _sendTransactionPanel.OpenWithDelay(delayInSeconds, openArgs);
        }
        
        public void OpenSendTransactionWithFeeOptionsPanelWithDelay(float delayInSeconds, params object[] openArgs)
        {
            _sendTransactionWithFeeOptionsPanel.OpenWithDelay(delayInSeconds, openArgs);
        }
        
        public void OpenSeeMarketplaceListingsPanelWithDelay(float delayInSeconds, params object[] openArgs)
        {
            _seeMarketplaceListingsPanel.OpenWithDelay(delayInSeconds, openArgs);
        }

        public void OpenSeeMarketplaceDetailsPanelWithDelay(float delayInSeconds, params object[] openArgs)
        {
            _marketplaceItemDetailsPanel.OpenWithDelay(delayInSeconds, openArgs);
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
            _loginWindow.Show(_loginHandler);
        }
    }
}