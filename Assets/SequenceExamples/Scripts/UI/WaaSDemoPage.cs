using System;
using System.Threading.Tasks;
using Sequence.Utils;
using Sequence.WaaS;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class WaaSDemoPage : UIPage
    {
        [SerializeField] private TextMeshProUGUI _resultText;
        
        private WaaSWallet _wallet;
        private Address _address;
        
        public override void Open(params object[] args)
        {
            _wallet =
                args.GetObjectOfTypeIfExists<WaaSWallet>();
            if (_wallet == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(WaaSWallet)} as an argument");
            }
            _gameObject.SetActive(true);
            _animator.AnimateIn( _openAnimationDurationInSeconds);

            SetAddress();
            
            _wallet.OnSignMessageComplete += OnSignMessageComplete;
            _wallet.OnSendTransactionComplete += OnSuccessfulTransaction;
            _wallet.OnSendTransactionFailed += OnFailedTransaction;
        }

        private async Task SetAddress()
        {
            var addressReturn = await _wallet.GetWalletAddress(new GetWalletAddressArgs(0));
            _address = new Address(addressReturn.address);
        }
        
        public void SignMessage()
        {
            _wallet.SignMessage(new SignMessageArgs(_address, Chain.Polygon, "Hello World!"));
        }
        
        private void OnSignMessageComplete(SignMessageReturn result)
        {
            _resultText.text = result.signature;
        }

        public void SendTransfer()
        {
            _wallet.SendTransaction(new SendTransactionArgs(
                _address,
                Chain.Polygon,
                new SequenceSDK.WaaS.Transaction[]
                {
                    new RawTransaction("0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f", "1")
                }));
        }
        
        private void OnSuccessfulTransaction(SuccessfulTransactionReturn result)
        {
            _resultText.text = $"https://polygonscan.com/tx/{result.txHash}";
            Debug.Log("Transaction successful: " + result.txHash);
        }
        
        private void OnFailedTransaction(FailedTransactionReturn result)
        {
            _resultText.text = result.error;
            Debug.LogError("Transaction failed: " + result.error);
        }

        public void SendFailingTransfer()
        {
            _wallet.SendTransaction(new SendTransactionArgs(
                _address,
                Chain.Polygon,
                new SequenceSDK.WaaS.Transaction[]
                {
                    new RawTransaction("0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f", "99000000000000000000")
                }));
        }
    }
}