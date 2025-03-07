using System;
using System.Threading.Tasks;
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
        
        private SequenceWallet _wallet;
        private SequenceLoginWindow _loginWindow;
        private SequencePlayerProfile _playerProfile;

        private async void Start()
        {
            SequenceWallet.OnWalletCreated += HandleWalletCreated;
            await TryRecoverSessionToOpenLoginWindow();
        }

        private void OnDestroy()
        {
            SequenceWallet.OnWalletCreated -= HandleWalletCreated;
            BoilerplateFactory.CleanUp();
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
        
        /// <summary>
        /// This function is called at the start of the scene and whenever the user logged out.
        /// </summary>
        private async Task TryRecoverSessionToOpenLoginWindow()
        {
            HideFeatureSelection();
            
            var login = SequenceLogin.GetInstance();
            var wallet = await login.TryToRestoreSessionAsync();
            if (wallet != null)
                return;
            
            Debug.LogError("Failed to recover session. Please login.");
            _loginWindow = BoilerplateFactory.OpenSequenceLoginWindow(transform);
        }

        /// <summary>
        /// This function is called after the login or session recovery succeeded.
        /// </summary>
        /// <param name="wallet">Instance of SequenceWallet</param>
        private void HandleWalletCreated(SequenceWallet wallet)
        {
            _wallet = wallet;
            ShowFeatureSelection();
            
            if (_loginWindow)
                _loginWindow.Hide();
            
            wallet.OnDropSessionComplete += async s =>
            {
                if (s == wallet.SessionId)
                {
                    if (_playerProfile)
                        _playerProfile.Hide();
                        
                    await TryRecoverSessionToOpenLoginWindow();
                }
            };
        }
    }
}