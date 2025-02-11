using Sequence;
using Sequence.Demo;
using Sequence.EmbeddedWallet;
using Sequence.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceSDK.Samples
{
    public class SequenceInventory : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private Chain _chain = Chain.TestnetArbitrumSepolia;
        [SerializeField] private string _contractAddress = "0x00";
        
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
        private TokenBalance _selectedBalance;
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public void Show(IWallet wallet)
        {
            _wallet = wallet;
            gameObject.SetActive(true);
            _messagePopup.gameObject.SetActive(false);
            _loadingScreen.SetActive(false);
            _tilePool.Cleanup();
            
            SetOverviewState();
            LoadBalances();
        }

        public void SetOverviewState()
        {
            _backButton.SetActive(false);
            _overviewState.SetActive(true);
            _detailsState.SetActive(false);
            _sendState.SetActive(false);
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

        private async void LoadBalances()
        {
            var indexer = new ChainIndexer(_chain);
            var args = new GetTokenBalancesArgs(_wallet.GetWalletAddress(), _contractAddress, true);
            var response = await indexer.GetTokenBalances(args);
            LoadTiles(response.balances);
        }

        private void LoadTiles(TokenBalance[] balances)
        {
            var empty = balances.Length == 0;
            _messageList.SetActive(empty);
            _tokenList.SetActive(!empty);
            _messagePool.Cleanup();
            _tilePool.Cleanup();
            
            if (empty)
                _messagePool.GetObject();
            
            foreach (var balance in balances)
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
