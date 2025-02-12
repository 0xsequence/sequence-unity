using Sequence.Authentication;
using Sequence.Boilerplates.SignMessage;
using Sequence.Config;
using Sequence.EmbeddedWallet;
using Sequence.Utils.SecureStorage;
using SequenceSDK.Samples;
using UnityEngine;

namespace Sequence.Demo
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
        [SerializeField] private SequenceLoginWindow _loginWindow;
        [SerializeField] private SequencePlayerProfile _playerProfile;
        [SerializeField] private SequenceDailyRewards _dailyRewards;
        [SerializeField] private SequenceInventory _inventory;
        [SerializeField] private SequenceInGameShop _inGameShop;
        [SerializeField] private SequenceSignMessage _signMessage;
        
        private IWallet _wallet;
        private ILogin _loginHandler;
        private GameObject _lastOpenedWindow;
        
        private void Awake()
        {
            SequenceWallet.OnWalletCreated += wallet =>
            {
                _wallet = wallet;
                _loginWindow.Hide();
                _featureSelection.gameObject.SetActive(true);
                
                wallet.OnDropSessionComplete += s =>
                {
                    if (s == wallet.SessionId)
                    {
                        TryRecoverSession();
                    }
                };
            };
        }

        private void Start()
        {
            HideAll();
            TryRecoverSession();
        }

        public void OpenPlayerProfilePanel()
        {
            _featureSelection.SetActive(false);
            _playerProfile.Show(_wallet, _chain);
        }
        
        public void OpenDailyRewardsPanel()
        {
            _featureSelection.SetActive(false);
            _dailyRewards.Show(_wallet, _chain, _dailyRewardsApi);
        }
        
        public void OpenInventoryPanel()
        {
            _featureSelection.SetActive(false);
            _inventory.Show(_wallet, _chain, _collectionAddress);
        }
        
        public void OpenInGameShopPanel()
        {
            _featureSelection.SetActive(false);
            _inGameShop.Show(_wallet, _chain, _collectionAddress, _saleContractAddress, _itemsForSale);
        }

        public void OpenSignMessage()
        {
            _featureSelection.SetActive(false);
            _signMessage.Show(_wallet, _chain);
        }

        private void HideAll()
        {
            _featureSelection.SetActive(false);
            _loginWindow.Hide();
            _playerProfile.Hide();
            _dailyRewards.Hide();
            _inventory.Hide();
            _inGameShop.Hide();
            _signMessage.Hide();
        }
        
        private void TryRecoverSession()
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
            
            HideAll();
            _loginWindow.Show(_loginHandler);
        }
    }
}