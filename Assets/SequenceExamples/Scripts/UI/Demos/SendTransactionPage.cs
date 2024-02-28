using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Utils;
using Sequence.WaaS;
using SequenceSDK.WaaS;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class SendTransactionPage : UIPage
    {
        [SerializeField] private TextMeshProUGUI _walletAddressText;
        [SerializeField] private TMP_InputField _toAddressInputField;
        [SerializeField] private TMP_InputField _amountInputField;
        [SerializeField] private TextMeshProUGUI _transactionHashText;
        [SerializeField] private TextMeshProUGUI _sendText;
        [SerializeField] private Chain _chain;
        
        private IWallet _wallet;
        private ChainIndexer _indexer;
        private BigInteger _maxAmount = 0;
        
        public override void Open(params object[] args)
        {
            base.Open(args);
            _wallet = args.GetObjectOfTypeIfExists<IWallet>();
            if (_wallet == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(IWallet)} as an argument");
            }

            _walletAddressText.text = _wallet.GetWalletAddress();
            
            _wallet.OnSendTransactionComplete += OnTransactionSuccess;
            _wallet.OnSendTransactionFailed += OnTransactionFailed;

            _sendText.text = $"Send {ChainDictionaries.GasCurrencyOf[_chain]}";

            _indexer = new ChainIndexer(_chain);
            GetMaxAmount();
        }
        
        public override void Close()
        {
            base.Close();
            
            _wallet.OnSendTransactionComplete -= OnTransactionSuccess;
            _wallet.OnSendTransactionFailed -= OnTransactionFailed;
        }
        
        private void OnTransactionSuccess(SuccessfulTransactionReturn transactionReturn)
        {
            _transactionHashText.text = $"{ChainDictionaries.BlockExplorerOf[_chain]}tx/{transactionReturn.txHash}";
        }
        
        private void OnTransactionFailed(FailedTransactionReturn transactionReturn)
        {
            Debug.LogError($"Transaction failed: {transactionReturn.error}");
        }
        
        public void SendTransaction()
        {
            Address toAddress;
            try
            {
                toAddress = new Address(_toAddressInputField.text);
            }
            catch (Exception e)
            {
                Debug.LogError($"Invalid address: {_toAddressInputField.text}");
                return;
            }

            string amount = DecimalNormalizer.Normalize(float.Parse(_amountInputField.text));
            _wallet.SendTransaction(_chain, new Transaction[]
            {
                new RawTransaction(toAddress, amount)
            });
        }

        public void OpenBlockExplorer()
        {
            Application.OpenURL(_transactionHashText.text);
        }

        private async Task GetMaxAmount()
        {
            EtherBalance ethBalance = await _indexer.GetEtherBalance(_wallet.GetWalletAddress());
            _maxAmount = ethBalance.balanceWei;
        }

        public void Max()
        {
            _amountInputField.text = DecimalNormalizer.ReturnToNormalString(_maxAmount);
        }
    }
}