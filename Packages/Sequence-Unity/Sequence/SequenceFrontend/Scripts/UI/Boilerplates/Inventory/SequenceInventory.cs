using System;
using System.Threading.Tasks;
using Sequence.EmbeddedWallet;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sequence.Boilerplates.Inventory
{
    public class SequenceInventory : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private RawImage _tokenDetailsImage;
        [SerializeField] private TMP_Text _tokenDetailsNameText;
        [SerializeField] private TMP_Text _tokenDetailsDescriptionText;
        [SerializeField] private TMP_InputField _sendAmountInput;
        [SerializeField] private TMP_InputField _sendRecipientInput;
        [SerializeField] private MessagePopup _messagePopup;
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private GameObject _backButton;
        [SerializeField] private GameObject _overviewState;
        [SerializeField] private GameObject _detailsState;
        [SerializeField] private GameObject _sendState;
        [SerializeField] private GameObject _messageList;
        [SerializeField] private GameObject _tokenList;
        [SerializeField] private GenericObjectPool<Component> _messagePool;
        [SerializeField] private GenericObjectPool<SequenceInventoryTile> _tilePool;

        private IWallet _wallet;
        private Chain _chain;
        private string[] _collections;
        private Action _onClose;
        private TokenBalance _selectedBalance;
        
        /// <summary>
        /// This function is called when the user clicks the close button.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            _onClose?.Invoke();
        }
        
        /// <summary>
        /// Required function to configure this Boilerplate.
        /// </summary>
        /// <param name="wallet">This Wallet instance will perform transactions.</param>
        /// <param name="chain">Chain used to get balances and send transactions.</param>
        /// <param name="collections">The inventory will show items from these contracts.</param>
        /// <param name="onClose">(Optional) Callback when the user closes this window.</param>
        public void Show(IWallet wallet, Chain chain, string[] collections, Action onClose = null)
        {
            _wallet = wallet;
            _chain = chain;
            _collections = collections;
            _onClose = onClose;
            gameObject.SetActive(true);
            _messagePopup.gameObject.SetActive(false);
            _loadingScreen.SetActive(false);
            _tilePool.Cleanup();
            
            SetOverviewState();
            LoadAllCollections();
        }

        public void SetOverviewState()
        {
            _backButton.SetActive(false);
            _overviewState.SetActive(true);
            _detailsState.SetActive(false);
            _sendState.SetActive(false);
        }

        public void OpenListingPanel()
        {
            BoilerplateFactory.OpenListItemPanel(transform, new Sequence.Marketplace.Checkout(_wallet, _chain), _selectedBalance);
        }
        
        public async void SendToken()
        {
            var recipient = _sendRecipientInput.text;
            var amountInput = _sendAmountInput.text;
            if (!uint.TryParse(amountInput, out uint amount) || amount == 0)
            {
                _messagePopup.Show("Invalid amount.", true);
                return;
            }

            if (!recipient.IsAddress())
            {
                _messagePopup.Show("Invalid wallet address.", true);
                return;
            }
            
            if (amount > _selectedBalance.balance)
            {
                _messagePopup.Show("This is too much.", true);
                return;
            }
            
            _loadingScreen.SetActive(true);

            var metadata = _selectedBalance.tokenMetadata;
            var response = await _wallet.SendTransaction(_chain, new Transaction[] {
                new SendERC1155(metadata.contractAddress, recipient, new []
                {
                    new SendERC1155Values(metadata.tokenId.ToString(), amountInput)
                })
            });
            
            _loadingScreen.SetActive(false);
            
            if (response is FailedTransactionReturn failed)
                _messagePopup.Show(failed.error, true);
            else if (response is SuccessfulTransactionReturn)
                _messagePopup.Show("Sent successfully.");
        }
        
        private async void LoadAllCollections()
        {
            _messagePool.Cleanup();
            _tilePool.Cleanup();
            
            foreach (var collection in _collections)
                await LoadCollection(collection);
            
            var empty = _tilePool.Parent.childCount == 0;
            _messageList.SetActive(empty);
            _tokenList.SetActive(!empty);
            _messagePool.Cleanup();
            
            if (empty)
                _messagePool.GetObject();
        }

        private async Task LoadCollection(string collection)
        {
            var indexer = new ChainIndexer(_chain);
            var args = new GetTokenBalancesArgs(_wallet.GetWalletAddress(), collection, true);
            var response = await indexer.GetTokenBalances(args);
            
            foreach (var balance in response.balances)
                _tilePool.GetObject().Load(balance, () => ShowToken(balance));
        }

        private async void ShowToken(TokenBalance balance)
        {
            _selectedBalance = balance;
            _backButton.SetActive(true);
            _overviewState.SetActive(false);
            _detailsState.SetActive(true);
            _sendState.SetActive(false);

            _tokenDetailsNameText.text = $"{balance.tokenMetadata.name}";
            _tokenDetailsDescriptionText.text = balance.tokenMetadata.description;
            _tokenDetailsImage.texture = await AssetHandler.GetTexture2DAsync(balance.tokenMetadata.image);
        }
    }
}
