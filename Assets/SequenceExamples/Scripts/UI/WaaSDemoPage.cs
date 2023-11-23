using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Sequence.Utils;
using Sequence.WaaS;
using SequenceSDK.Ethereum.Utils;
using SequenceSDK.WaaS;
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
            _wallet.OnDropSessionComplete += OnDropSessionComplete;
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

        public void DropSession()
        {
            _wallet.DropThisSession();
        }
        
        private void OnDropSessionComplete(string droppedSessionId)
        {
            Debug.Log("Session dropped: " + droppedSessionId);
            _wallet = null;
            Close();
        }

        public void SendErc20Transfer()
        {
            _wallet.SendTransaction(new SendTransactionArgs(
                _address, Chain.Polygon, new SequenceSDK.WaaS.Transaction[]
                {
                    new SendERC20(
                        "0x3c499c542cEF5E3811e1192ce70d8cC03d5c3359",
                        "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                        "1"),
                }));
        }
        
        public void SendErc721Transfer()
        {
            _wallet.SendTransaction(new SendTransactionArgs(
                _address, Chain.Polygon, new SequenceSDK.WaaS.Transaction[]
                {
                    new SendERC721(
                        "0xa9a6A3626993D487d2Dbda3173cf58cA1a9D9e9f",
                        "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                        "54530968763798660137294927684252503703134533114052628080002308208148824588621"),
                }));
        }
        
        public void SendErc1155Transfer()
        {
            // Todo fix
            _wallet.SendTransaction(new SendTransactionArgs(
                _address, Chain.Polygon, new SequenceSDK.WaaS.Transaction[]
                {
                    new SendERC1155(
                        "0x44b3f42e2bf34f62868ff9e9dab7c2f807ba97cb",
                        "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                        new SendERC1155Values[]
                        {
                            new SendERC1155Values("86", "1")
                        }),
                }));
        }

        public void SendMultipleTransferTypes()
        {
            _wallet.SendTransaction(new SendTransactionArgs(
                _address, Chain.Polygon, new SequenceSDK.WaaS.Transaction[]
                {
                    new RawTransaction("0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f", "1"),
                    new SendERC20(
                        "0x3c499c542cEF5E3811e1192ce70d8cC03d5c3359",
                        "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                        "1"),
                    new RawTransaction("0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f", "1"),
                    new SendERC721(
                        "0xa9a6A3626993D487d2Dbda3173cf58cA1a9D9e9f",
                        "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                        "54530968763798660137294927684252503703134533114052628080002308208148824588621"),
                    new RawTransaction("0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f", "1"),
                }));
        }
    }
}