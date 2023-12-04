using System;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Contracts;
using Sequence.Provider;
using Sequence.Transactions;
using Sequence.Utils;
using Sequence.WaaS;
using Sequence.WaaS.Authentication;
using SequenceSDK.Ethereum.Utils;
using SequenceSDK.WaaS;
using TMPro;
using UnityEngine;
using IWallet = Sequence.Wallet.IWallet;

namespace Sequence.Demo
{
    public class WaaSDemoPage : UIPage
    {
        [SerializeField] private TextMeshProUGUI _resultText;
        
        private WaaSWallet _wallet;
        private Address _address;
        private IWallet _adapter;
        
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

            _address = _wallet.GetWalletAddress();
            
            _wallet.OnSignMessageComplete += OnSignMessageComplete;
            _wallet.OnSendTransactionComplete += OnSuccessfulTransaction;
            _wallet.OnSendTransactionFailed += OnFailedTransaction;
            _wallet.OnDropSessionComplete += OnDropSessionComplete;

            CreateAdapter();
        }

        private async Task CreateAdapter()
        {
            _adapter = new WaaSToWalletAdapter(_wallet);
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
                    new RawTransaction("0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f", DecimalNormalizer.Normalize(1)),
                    new SendERC20(
                        "0x3c499c542cEF5E3811e1192ce70d8cC03d5c3359",
                        "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                        "1"),
                    new SendERC721(
                        "0xa9a6A3626993D487d2Dbda3173cf58cA1a9D9e9f",
                        "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                        "54530968763798660137294927684252503703134533114052628080002308208148824588621"),
                    new SendERC1155(
                        "0x44b3f42e2bf34f62868ff9e9dab7c2f807ba97cb",
                        "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                        new SendERC1155Values[]
                        {
                            new SendERC1155Values("86", "1")
                        }),
                    new DelayedEncode("0x3c499c542cEF5E3811e1192ce70d8cC03d5c3359", "0", new DelayedEncodeData(
                        "transfer(address,uint256)",
                        new object[]
                        {
                            "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f", "1"
                        },
                        "transfer")),
                }));
        }

        public void SendWithAdapter()
        {
            DoSendWithAdapter();
        }

        private async Task DoSendWithAdapter()
        {
            ERC721 nft = new ERC721("0xa9a6A3626993D487d2Dbda3173cf58cA1a9D9e9f");
            var receipt = await nft.TransferFrom(_adapter.GetAddress(), "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f", 
                    "54530968763798660137294927684252503703134533114052628080002308208148824588621")
                .SendTransactionMethodAndWaitForReceipt(_adapter, new SequenceEthClient("https://polygon-bor.publicnode.com"));
            Debug.LogError($"Transaction hash: {receipt.transactionHash}");
            
        }

        public void SendMultipleWithAdapter()
        {
            DoSendMultipleWithAdapter();
        }

        private async Task DoSendMultipleWithAdapter()
        {
            ERC721 nft = new ERC721("0xa9a6A3626993D487d2Dbda3173cf58cA1a9D9e9f");
            ERC1155 sft = new ERC1155("0x44b3f42e2bf34f62868ff9e9dab7c2f807ba97cb");
            SequenceEthClient client = new SequenceEthClient("https://polygon-bor.publicnode.com");
            var nftTransfer = await nft.TransferFrom(_adapter.GetAddress(),
                "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                "54530968763798660137294927684252503703134533114052628080002308208148824588621")(client, new ContractCall(_adapter.GetAddress()));
            var sftTransfer = await sft.SafeTransferFrom(_adapter.GetAddress(),
                "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                "86",
                1)(client, new ContractCall(_adapter.GetAddress()));

            var receipt = await _adapter.SendTransactionBatchAndWaitForReceipts(client, new EthTransaction[]
            {
                nftTransfer, sftTransfer
            });
            Debug.LogError($"Transaction hash: {receipt[0].transactionHash}");
            
            // or
            
            // _wallet.SendTransaction(new SendTransactionArgs(
            //     _address, Chain.Polygon, new SequenceSDK.WaaS.Transaction[]
            //     {
            //         new RawTransaction(nftTransfer),
            //         new RawTransaction(sftTransfer),
            //     }));
        }
        
        public void SendMultipleWithAdapter2()
        {
            DoSendMultipleWithAdapter2();
        }

        private async Task DoSendMultipleWithAdapter2()
        {
            ERC721 nft = new ERC721("0xa9a6A3626993D487d2Dbda3173cf58cA1a9D9e9f");
            ERC1155 sft = new ERC1155("0x44b3f42e2bf34f62868ff9e9dab7c2f807ba97cb");
            _wallet.SendTransaction(new SendTransactionArgs(
                _address, Chain.Polygon, new SequenceSDK.WaaS.Transaction[]
                {
                    new RawTransaction(nft.Contract, "transferFrom", _adapter.GetAddress().Value,
                        "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                        BigInteger.Parse("54530968763798660137294927684252503703134533114052628080002308208148824588621")),
                    new RawTransaction(sft.Contract, "safeTransferFrom", _adapter.GetAddress().Value,
                        "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                        86,
                        1, "data".ToByteArray()), // Todo figure out why data is required
                }));
        }

        public void DelayedEncode()
        {
            // Todo fix
            _wallet.SendTransaction(new SendTransactionArgs(
                _address, Chain.Polygon, new SequenceSDK.WaaS.Transaction[]
                {
                    new DelayedEncode("0x3c499c542cEF5E3811e1192ce70d8cC03d5c3359", "0", new DelayedEncodeData(
                        "transfer(address,uint256)",
                        new object[]
                        {
                            "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f", "1"
                        },
                        "transfer")),
                }));
        }

        public void ListSessions()
        {
            _wallet.OnSessionsFound += OnSessionsListed;
            _wallet.ListSessions();
        }
        
        private void OnSessionsListed(WaaSSession[] sessions)
        {
            _resultText.text = $"Found {sessions.Length} sessions";
        }
    }
}