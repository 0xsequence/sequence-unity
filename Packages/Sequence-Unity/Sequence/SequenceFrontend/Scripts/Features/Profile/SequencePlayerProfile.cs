using Sequence;
using Sequence.Demo;
using Sequence.EmbeddedWallet;
using TMPro;
using UnityEngine;

namespace SequenceSDK.Samples
{
    public class SequencePlayerProfile : MonoBehaviour
    {
        [Header("Config")] 
        [SerializeField] private Chain _chain = Chain.TestnetArbitrumSepolia;
        
        [Header("Components")]
        [SerializeField] private TMP_Text _etherBalanceText;
        [SerializeField] private TMP_InputField _recipientInput;
        [SerializeField] private TMP_InputField _tokenAmountInput;
        [SerializeField] private QrCodeView _qrImage;
        [SerializeField] private MessagePopup _messagePopup;
        [SerializeField] private LoadingScreen _loadingScreen;
        [SerializeField] private GameObject _backButton;
        [SerializeField] private GameObject _overviewState;
        [SerializeField] private GameObject _sendTokenState;
        [SerializeField] private GameObject _receiveState;
        [SerializeField] private GenericObjectPool<TransactionHistoryTile> _transactionPool;
        
        private IWallet _wallet;

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public async void Show(IWallet wallet)
        {
            _wallet = wallet;
            
            gameObject.SetActive(true);
            SetOverviewState();
            
            var walletAddress = _wallet.GetWalletAddress();
            var indexer = new ChainIndexer(_chain);
            var balance = await indexer.GetEtherBalance(walletAddress);
            
            _etherBalanceText.text = $"{balance.balanceWei} ETH";
            EnableLoading(false);
            _messagePopup.gameObject.SetActive(false);

            var filter = new TransactionHistoryFilter {accountAddress = walletAddress};
            var response = await indexer.GetTransactionHistory(new GetTransactionHistoryArgs(filter));
            
            _transactionPool.Cleanup();
            if (response.transactions.Length == 0)
                _transactionPool.GetObject().ShowEmpty();
            
            foreach (var transaction in response.transactions)
                _transactionPool.GetObject().Show(transaction);
        }

        public void CopyWalletAddress()
        {
            GUIUtility.systemCopyBuffer = _wallet.GetWalletAddress();
            _messagePopup.Show("Copied");
        }

        public async void SignOut()
        {
            EnableLoading(true);
            await _wallet.DropThisSession();
            EnableLoading(false);
            Hide();
        }

        public async void SendToken()
        {
            var recipient = _recipientInput.text;
            var input = _tokenAmountInput.text;
            if (!uint.TryParse(input, out uint t))
            {
                _messagePopup.Show("Invalid amount.", true);
                return;
            }

            EnableLoading(true);
            var response = await _wallet.SendTransaction(_chain, new Transaction[] {
                new RawTransaction(recipient, t.ToString())
            });
            
            EnableLoading(false);

            if (response is FailedTransactionReturn failed)
                _messagePopup.Show(failed.error, true);
            else if (response is SuccessfulTransactionReturn)
                _messagePopup.Show("Sent successfully.");
        }

        public void SetOverviewState()
        {
            _backButton.SetActive(false);
            _overviewState.SetActive(true);
            _sendTokenState.SetActive(false);
            _receiveState.SetActive(false);
        }

        public async void SetReceiveState()
        {
            _receiveState.SetActive(true);
            await _qrImage.Show(_wallet.GetWalletAddress());
        }

        private void EnableLoading(bool enable)
        {
            _loadingScreen.gameObject.SetActive(enable);
        }
    }
}
