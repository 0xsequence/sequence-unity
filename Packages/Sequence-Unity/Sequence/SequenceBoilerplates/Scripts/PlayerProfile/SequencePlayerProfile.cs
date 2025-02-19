using System;
using Sequence;
using Sequence.Demo;
using Sequence.EmbeddedWallet;
using TMPro;
using UnityEngine;

namespace SequenceSDK.Samples
{
    public class SequencePlayerProfile : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TMP_Text _etherBalanceText;
        [SerializeField] private TMP_InputField _recipientInput;
        [SerializeField] private TMP_InputField _tokenAmountInput;
        [SerializeField] private QrCodeView _qrImage;
        [SerializeField] private MessagePopup _messagePopup;
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private GameObject _overviewState;
        [SerializeField] private GameObject _sendTokenState;
        [SerializeField] private GameObject _receiveState;
        [SerializeField] private GameObject _walletsState;
        [SerializeField] private GenericObjectPool<TransactionHistoryTile> _transactionPool;
        [SerializeField] private GenericObjectPool<LinkedWalletTile> _walletsPool;
        
        private IWallet _wallet;
        private Chain _chain;
        private Action _onClose;
        private EOAWalletLinker _walletLinker;

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
        /// <param name="onClose">(Optional) Callback when the user closes this window.</param>
        public async void Show(IWallet wallet, Chain chain, Action onClose = null)
        {
            _wallet = wallet;
            _chain = chain;
            _onClose = onClose;
            gameObject.SetActive(true);
            _transactionPool.Cleanup();
            
            SetOverviewState();
            
            var walletAddress = _wallet.GetWalletAddress();
            var indexer = new ChainIndexer(_chain);
            var balance = await indexer.GetEtherBalance(walletAddress);
            
            _etherBalanceText.text = $"{balance.balanceWei} ETH";
            _messagePopup.gameObject.SetActive(false);
            EnableLoading(false);

            LoadTransactionHistory();
            LoadLinkedWallets();
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

        public async void LinkExternalWallet()
        {
            await new EOAWalletLinker(_wallet, _chain).OpenEoaWalletLink();
        }

        public void SetOverviewState()
        {
            _overviewState.SetActive(true);
            _sendTokenState.SetActive(false);
            _receiveState.SetActive(false);
            _walletsState.SetActive(false);
        }

        public async void SetReceiveState()
        {
            _receiveState.SetActive(true);
            await _qrImage.Show(_wallet.GetWalletAddress());
        }

        private void EnableLoading(bool enable)
        {
            _loadingScreen.SetActive(enable);
        }

        private async void LoadTransactionHistory()
        {
            var walletAddress = _wallet.GetWalletAddress();
            var indexer = new ChainIndexer(_chain);
            var filter = new TransactionHistoryFilter {accountAddress = walletAddress};
            var response = await indexer.GetTransactionHistory(new GetTransactionHistoryArgs(filter));
            
            _transactionPool.Cleanup();
            if (response.transactions.Length == 0)
                _transactionPool.GetObject().ShowEmpty();
            
            foreach (var transaction in response.transactions)
                _transactionPool.GetObject().Show(transaction);
        }

        private async void LoadLinkedWallets()
        {
            _walletLinker = new EOAWalletLinker(_wallet, _chain);
            var linkedWallets = await _walletLinker.GetLinkedWallets();
            
            _walletsPool.Cleanup();
            if (linkedWallets.Length == 0)
                _walletsPool.GetObject().ShowEmpty();
            
            foreach (var wallet in linkedWallets)
                _walletsPool.GetObject().Show(wallet);
        }
    }
}
