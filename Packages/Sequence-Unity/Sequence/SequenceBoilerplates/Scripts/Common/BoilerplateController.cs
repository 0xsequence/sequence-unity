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
            _playerProfile.Show(_wallet);
        }
        
        public void OpenDailyRewardsPanel()
        {
            _featureSelection.SetActive(false);
            _dailyRewards.Show(_wallet);
        }
        
        public void OpenInventoryPanel()
        {
            _featureSelection.SetActive(false);
            _inventory.Show(_wallet);
        }
        
        public void OpenInGameShopPanel()
        {
            _featureSelection.SetActive(false);
            _inGameShop.Show(_wallet);
        }

        public void OpenSignMessage()
        {
            _featureSelection.SetActive(false);
            _signMessage.Show(_wallet);
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