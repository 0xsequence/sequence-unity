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
        [SerializeField] private Chain _chain = Chain.TestnetArbitrumSepolia;
        [SerializeField] private string _dailyRewardsApi;
        [SerializeField] private string _collectionAddress;
        [SerializeField] private string _saleContractAddress;
        [SerializeField] private int[] _itemsForSale;
        
        [Header("Components")]
        [SerializeField] private GameObject _featureSelection;
        
        private IWallet _wallet;
        private SequenceLoginWindow _loginWindow;
        private SequencePlayerProfile _playerProfile;
        
        private void Awake()
        {
            SequenceWallet.OnFailedToRecoverSession += OnFailedToRecoverSession;
            SequenceWallet.OnWalletCreated += wallet =>
            {
                _wallet = wallet;
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

        public void OpenPlayerProfilePanel()
        {
            HideFeatureSelection();
            _playerProfile = BoilerplateFactory.OpenSequencePlayerProfile(transform, _wallet, _chain, ShowFeatureSelection);
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

        public void OpenSignMessage()
        {
            HideFeatureSelection();
            BoilerplateFactory.OpenSequenceSignMessage(transform, _wallet, _chain, ShowFeatureSelection);
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
    }
}