using System;
using System.Numerics;
using Sequence.Adapter;
using Sequence.EmbeddedWallet;
using Sequence.Utils;
using TMPro;
using UnityEngine;

namespace Sequence.Boilerplates.PlayerProfile
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
        
        private readonly EmbeddedWalletAdapter _adapter = EmbeddedWalletAdapter.GetInstance();
        
        private Action _onClose;
        private Address _currency;
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
        /// <param name="currency">Define a custom ERC20 currency. Leave it null to use the chains native token.</param>
        /// <param name="onClose">(Optional) Callback when the user closes this window.</param>
        public async void Show(Address currency = null, string currencySymbol = null, Action onClose = null)
        {
            _onClose = onClose;
            _currency = currency ?? Address.ZeroAddress;
            gameObject.SetActive(true);
            _transactionPool.Cleanup();
            
            SetOverviewState();
            SetBalance(0, string.Empty);

            if (_currency.IsZeroAddress())
            {
                var balance = await _adapter.GetMyNativeTokenBalance();
                SetNativeBalance(balance);
            }
            else
            {
                var balances = await _adapter.GetMyTokenBalances(currency);
                var balance = balances.Length > 0 ? balances[0].balance : 0;
                SetBalance(balance, currencySymbol);
            }
            
            _messagePopup.gameObject.SetActive(false);
            EnableLoading(false);

            LoadTransactionHistory();
            LoadLinkedWallets();
        }

        public void CopyWalletAddress()
        {
            GUIUtility.systemCopyBuffer = _adapter.WalletAddress;
            _messagePopup.Show("Copied");
        }

        public async void SignOut()
        {
            EnableLoading(true);
            await _adapter.SignOut();
            EnableLoading(false);
        }

        public void OpenLoginWindowForFederation()
        {
            gameObject.SetActive(false);
            BoilerplateFactory.OpenSequenceLoginWindow(transform.parent, 
                () => gameObject.SetActive(true));
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

            try
            {
                await _adapter.SendToken(recipient, t, _currency);
                _messagePopup.Show("Sent successfully.");
            }
            catch (Exception ex)
            {
                _messagePopup.Show(ex.Message, true);
            }
            
            EnableLoading(false);
        }

        public async void LinkExternalWallet()
        {
            await new EOAWalletLinker(_adapter.Wallet, _adapter.Chain).OpenEoaWalletLink();
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
            await _qrImage.Show(_adapter.WalletAddress);
        }

        private void EnableLoading(bool enable)
        {
            _loadingScreen.SetActive(enable);
        }

        private async void LoadTransactionHistory()
        {
            var walletAddress = _adapter.WalletAddress;
            var indexer = new ChainIndexer(_adapter.Chain);
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
            _walletsPool.Cleanup();
            _walletLinker = new EOAWalletLinker(_adapter.Wallet, _adapter.Chain);
            var linkedWallets = await _walletLinker.GetLinkedWallets();
            
            if (linkedWallets.Length == 0)
                _walletsPool.GetObject().ShowEmpty();
            
            foreach (var wallet in linkedWallets)
                _walletsPool.GetObject().Show(wallet, () => UnlinkWallet(wallet));
        }

        private async void UnlinkWallet(LinkedWalletData wallet)
        {
            var success = await _walletLinker.UnlinkWallet(wallet.linkedWalletAddress);
            _messagePopup.Show(success ? "Unlinked." : "Failed to unlink", !success);
        }

        private void SetNativeBalance(BigInteger balance)
        {
            var symbol = ChainDictionaries.GasCurrencyOf[_adapter.Chain];
            SetBalance(balance, symbol);
        }

        private void SetBalance(BigInteger balance, string symbol)
        {
            var decimals = DecimalNormalizer.ReturnToNormalString(balance);
            _etherBalanceText.text = $"{decimals} {symbol}";
        }
    }
}
