using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Utils;
using Sequence.WaaS;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class SendTransactionWithFeeOptionsPage : DemoPage
    {
        [SerializeField] private TMP_InputField _toAddressInputField;
        [SerializeField] private TMP_InputField _amountInputField;
        [SerializeField] private TextMeshProUGUI _transactionHashText;
        [SerializeField] private TextMeshProUGUI _sendText;
        
        private ChainIndexer _indexer;
        private BigInteger _maxAmount = 0;
        
        public override void Open(params object[] args)
        {
            base.Open(args);
            
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

        public void GetFeeOptions()
        {
            Address toAddress = GetAddress();
            string amount = DecimalNormalizer.Normalize(float.Parse(_amountInputField.text));

            _wallet.GetFeeOptions(_chain, new Transaction[]
            {
                new RawTransaction(toAddress, amount)
            });
        }

        private Address GetAddress()
        {
            Address toAddress;
            try
            {
                toAddress = new Address(_toAddressInputField.text);
            }
            catch (Exception e)
            {
                Debug.LogError($"Invalid address: {_toAddressInputField.text}");
                return null;
            }

            return toAddress;
        }
        
        public void SendTransaction()
        {
            Address toAddress = GetAddress();

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