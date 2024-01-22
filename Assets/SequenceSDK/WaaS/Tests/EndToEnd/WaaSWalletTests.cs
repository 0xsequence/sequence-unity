using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Contracts;
using Sequence.Extensions;
using Sequence.Provider;
using Sequence.Wallet;
using Sequence.Utils;
using SequenceSDK.WaaS;
using UnityEngine;

namespace Sequence.WaaS.Tests
{
    // Note: These tests rely on the certain tokens being present in a given wallet address. These tokens are typically found in the wallet created by WaaS when logging in via
    // Google to qp@horizon.io - 0x2D566542570771c264b98959B037f4eb7534caaA
    public class WaaSWalletTests
    {
        public static Exception TestNotSetupProperly;
        
        private IWallet _wallet;
        private string _address;

        private string _toAddress = "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f";
        private string _polygonNode = "https://polygon-bor.publicnode.com";
        private string _erc20Address = "0x0d500B1d8E8eF31E21C99d1Db9A6444d3ADf1270";
        private string _erc721Address = "0xa9a6A3626993D487d2Dbda3173cf58cA1a9D9e9f";
        private string _erc721TokenId = "54530968763798660137294927684252503703134533114052628080002308208148824588621";
        private string _erc1155Address = "0x44b3f42e2bf34f62868ff9e9dab7c2f807ba97cb";
        private string _erc1155TokenId = "86";
        private IIndexer _polygonIndexer = new ChainIndexer((int)Chain.Polygon);

        private IEthClient _client;

        public WaaSWalletTests(IWallet wallet)
        {
            _wallet = wallet;
            GetAddress();
            _client = new SequenceEthClient(_polygonNode);
        }
        
        private async Task GetAddress()
        {
            var result = _wallet.GetWalletAddress();
            _address = result;
        }
        
        public async Task TestMessageSigning(string message, Chain network)
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                string signature = await _wallet.SignMessage(network, message);
                CustomAssert.NotNull(signature, nameof(TestMessageSigning), message, network);
                var isValid = await _wallet.IsValidMessageSignature(network,  message, signature);
                bool isValidSignature = isValid.isValid;
                CustomAssert.IsTrue(isValidSignature, nameof(TestMessageSigning), message, network);
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestMessageSigning), e.Message, message, network));
            }
        }

        public async Task TestTransfer()
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                BalanceChecker balanceChecker = await BalanceChecker.CreateAsync(_client, _address);
                BalanceChecker balanceChecker2 = await BalanceChecker.CreateAsync(_client, _toAddress);

                TransactionReturn result = await _wallet.SendTransaction(
                    Chain.Polygon,
                    new SequenceSDK.WaaS.Transaction[]
                    {
                        new RawTransaction(_toAddress, "1"),
                    });

                CustomAssert.IsTrue(result is SuccessfulTransactionReturn, nameof(TestTransfer));
                await Task.Delay(WaaSTestHarness.DelayForTransactionToProcess);
                await balanceChecker.AssertNewValueIsSmaller(nameof(TestTransfer));
                await balanceChecker2.AssertNewValueIsLarger(nameof(TestTransfer));
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestTransfer), e.Message));
            }
        }
        
        public async Task TestSendERC20()
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                Erc20BalanceChecker balanceChecker =
                    await Erc20BalanceChecker.CreateAsync(_polygonIndexer, _address, _erc20Address);
                Erc20BalanceChecker balanceChecker2 =
                    await Erc20BalanceChecker.CreateAsync(_polygonIndexer, _toAddress, _erc20Address);

                TransactionReturn result = await _wallet.SendTransaction(
                    Chain.Polygon,
                    new SequenceSDK.WaaS.Transaction[]
                    {
                        new SendERC20(_erc20Address, _toAddress, "1"),
                    });

                CustomAssert.IsTrue(result is SuccessfulTransactionReturn, nameof(TestSendERC20));
                await Task.Delay(WaaSTestHarness.DelayForTransactionToProcess);
                await balanceChecker.AssertNewValueIsSmaller(nameof(TestSendERC20));
                await balanceChecker2.AssertNewValueIsLarger(nameof(TestSendERC20));
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestSendERC20), e.Message));
            }
        }
        
        public async Task TestSendERC20_usingRawTransaction()
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                Erc20BalanceChecker balanceChecker =
                    await Erc20BalanceChecker.CreateAsync(_polygonIndexer, _address, _erc20Address);
                Erc20BalanceChecker balanceChecker2 =
                    await Erc20BalanceChecker.CreateAsync(_polygonIndexer, _toAddress, _erc20Address);
                
                ERC20 erc20 = new ERC20(_erc20Address);

                TransactionReturn result = await _wallet.SendTransaction(
                    Chain.Polygon,
                    new SequenceSDK.WaaS.Transaction[]
                    {
                        new RawTransaction(erc20.Transfer(_toAddress, 1)),
                    });

                CustomAssert.IsTrue(result is SuccessfulTransactionReturn, nameof(TestSendERC20_usingRawTransaction));
                await Task.Delay(WaaSTestHarness.DelayForTransactionToProcess);
                await balanceChecker.AssertNewValueIsSmaller(nameof(TestSendERC20_usingRawTransaction));
                await balanceChecker2.AssertNewValueIsLarger(nameof(TestSendERC20_usingRawTransaction));
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestSendERC20_usingRawTransaction), e.Message));
            }
        }

        // Note: since we are using real tokens, sending ERC721s and ERC1155s multiple times in a single test suite is difficult
        // as we need to send the tokens back manually after running each test. So instead of testing these functionalities individually,
        // we do them as part of batch transactions. We don't have this issue with ERC20s as we can batch multiple transactions together
        public async Task TestSendBatchTransaction_withERC721()
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                BalanceChecker balanceChecker = await BalanceChecker.CreateAsync(_client, _address);
                Erc20BalanceChecker erc20BalanceChecker =
                    await Erc20BalanceChecker.CreateAsync(_polygonIndexer, _address, _erc20Address);
                Erc721BalanceChecker erc721BalanceChecker =
                    await Erc721BalanceChecker.CreateAsync(_polygonIndexer, _address, _erc721Address);
                if (erc721BalanceChecker.GetBalance() == 0)
                {
                    string failReason =
                        $"Test {nameof(TestSendBatchTransaction_withERC721)} was not setup properly. {_address} must have an NFT from contract address: {_erc721Address}";
                    Debug.LogError(failReason);
                    WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestSendBatchTransaction_withERC721),
                        failReason));
                    throw TestNotSetupProperly;
                }

                BalanceChecker balanceChecker2 = await BalanceChecker.CreateAsync(_client, _toAddress);
                Erc20BalanceChecker erc20BalanceChecker2 =
                    await Erc20BalanceChecker.CreateAsync(_polygonIndexer, _toAddress, _erc20Address);
                Erc721BalanceChecker erc721BalanceChecker2 =
                    await Erc721BalanceChecker.CreateAsync(_polygonIndexer, _toAddress, _erc721Address);

                TransactionReturn result = await _wallet.SendTransaction(
                    Chain.Polygon,
                    new SequenceSDK.WaaS.Transaction[]
                    {
                        new SendERC20(_erc20Address, _toAddress, "1"),
                        new SendERC721(_erc721Address, _toAddress, _erc721TokenId),
                        new RawTransaction(_toAddress, "1")
                    });
                CustomAssert.IsTrue(result is SuccessfulTransactionReturn, nameof(TestSendBatchTransaction_withERC721));
                await Task.Delay(WaaSTestHarness.DelayForTransactionToProcess);

                await balanceChecker.AssertNewValueIsSmaller(nameof(TestSendBatchTransaction_withERC721));
                await erc20BalanceChecker.AssertNewValueIsSmaller(nameof(TestSendBatchTransaction_withERC721));
                await erc721BalanceChecker.AssertNewValueIsSmaller(nameof(TestSendBatchTransaction_withERC721));
                await balanceChecker2.AssertNewValueIsLarger(nameof(TestSendBatchTransaction_withERC721));
                await erc20BalanceChecker2.AssertNewValueIsLarger(nameof(TestSendBatchTransaction_withERC721));
                await erc721BalanceChecker2.AssertNewValueIsLarger(nameof(TestSendBatchTransaction_withERC721));
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestSendBatchTransaction_withERC721), e.Message));
            
            }
        }
        
        public async Task TestSendBatchTransaction_withERC1155()
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                BalanceChecker balanceChecker = await BalanceChecker.CreateAsync(_client, _address);
                Erc20BalanceChecker erc20BalanceChecker =
                    await Erc20BalanceChecker.CreateAsync(_polygonIndexer, _address, _erc20Address);
                Erc1155BalanceChecker erc1155BalanceChecker =
                    await Erc1155BalanceChecker.CreateAsync(_polygonIndexer, _address, _erc1155Address);
                if (erc1155BalanceChecker.GetBalance() == 0)
                {
                    string failReason =
                        $"Test {nameof(TestSendBatchTransaction_withERC1155)} was not setup properly. {_address} must have an SFT from contract address: {_erc1155Address}";
                    Debug.LogError(failReason);
                    throw TestNotSetupProperly;
                }

                BalanceChecker balanceChecker2 = await BalanceChecker.CreateAsync(_client, _toAddress);
                Erc20BalanceChecker erc20BalanceChecker2 =
                    await Erc20BalanceChecker.CreateAsync(_polygonIndexer, _toAddress, _erc20Address);
                Erc1155BalanceChecker erc1155BalanceChecker2 =
                    await Erc1155BalanceChecker.CreateAsync(_polygonIndexer, _toAddress, _erc1155Address);

                TransactionReturn result = await _wallet.SendTransaction(
                    Chain.Polygon,
                    new SequenceSDK.WaaS.Transaction[]
                    {
                        new SendERC20(_erc20Address, _toAddress, "1"),
                        new SendERC1155(_erc1155Address, _toAddress, new SendERC1155Values[]
                        {
                            new SendERC1155Values(_erc1155TokenId, "1")
                        }),
                        new RawTransaction(_toAddress, "1")
                    });
                CustomAssert.IsTrue(result is SuccessfulTransactionReturn,
                    nameof(TestSendBatchTransaction_withERC1155));
                await Task.Delay(WaaSTestHarness.DelayForTransactionToProcess);

                await balanceChecker.AssertNewValueIsSmaller(nameof(TestSendBatchTransaction_withERC1155));
                await erc20BalanceChecker.AssertNewValueIsSmaller(nameof(TestSendBatchTransaction_withERC1155));
                await erc1155BalanceChecker.AssertNewValueIsSmaller(nameof(TestSendBatchTransaction_withERC1155));
                await balanceChecker2.AssertNewValueIsLarger(nameof(TestSendBatchTransaction_withERC1155));
                await erc20BalanceChecker2.AssertNewValueIsLarger(nameof(TestSendBatchTransaction_withERC1155));
                await erc1155BalanceChecker2.AssertNewValueIsLarger(nameof(TestSendBatchTransaction_withERC1155));
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestSendBatchTransaction_withERC1155), e.Message));
            }
        }

        public async Task TestDelayedEncode(string abi)
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                Erc20BalanceChecker balanceChecker =
                    await Erc20BalanceChecker.CreateAsync(_polygonIndexer, _address, _erc20Address);
                Erc20BalanceChecker balanceChecker2 =
                    await Erc20BalanceChecker.CreateAsync(_polygonIndexer, _toAddress, _erc20Address);

                TransactionReturn result = await _wallet.SendTransaction(
                    Chain.Polygon,
                    new SequenceSDK.WaaS.Transaction[]
                    {
                        new DelayedEncode(_toAddress, "0", new DelayedEncodeData(
                            abi,
                            new object[]
                            {
                                _erc20Address, "1"
                            },
                            "transfer")),
                    });

                CustomAssert.IsTrue(result is SuccessfulTransactionReturn, nameof(TestDelayedEncode));
                await Task.Delay(WaaSTestHarness.DelayForTransactionToProcess);
                await balanceChecker.AssertNewValueIsSmaller(nameof(TestDelayedEncode));
                await balanceChecker2.AssertNewValueIsLarger(nameof(TestDelayedEncode));
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestDelayedEncode), e.Message, abi));
            }
        }
        
        public async Task TestSendBatchTransaction_withDelayedEncode(string abi)
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                BalanceChecker balanceChecker = await BalanceChecker.CreateAsync(_client, _address);
                Erc20BalanceChecker erc20BalanceChecker =
                    await Erc20BalanceChecker.CreateAsync(_polygonIndexer, _address, _erc20Address);
                
                BalanceChecker balanceChecker2 = await BalanceChecker.CreateAsync(_client, _toAddress);
                Erc20BalanceChecker erc20BalanceChecker2 =
                    await Erc20BalanceChecker.CreateAsync(_polygonIndexer, _toAddress, _erc20Address);
                TransactionReturn result = await _wallet.SendTransaction(
                    Chain.Polygon,
                    new SequenceSDK.WaaS.Transaction[]
                    {
                        new DelayedEncode(_toAddress, "0", new DelayedEncodeData(
                            abi,
                            new object[]
                            {
                                _erc20Address, "1"
                            },
                            "transfer")),
                        new RawTransaction(_toAddress, "1")
                    });
                CustomAssert.IsTrue(result is SuccessfulTransactionReturn,
                    nameof(TestSendBatchTransaction_withERC1155));
                await Task.Delay(WaaSTestHarness.DelayForTransactionToProcess);

                await balanceChecker.AssertNewValueIsSmaller(nameof(TestSendBatchTransaction_withDelayedEncode), abi);
                await erc20BalanceChecker.AssertNewValueIsSmaller(nameof(TestSendBatchTransaction_withDelayedEncode),
                    abi);
                await balanceChecker2.AssertNewValueIsLarger(nameof(TestSendBatchTransaction_withDelayedEncode), abi);
                await erc20BalanceChecker2.AssertNewValueIsLarger(nameof(TestSendBatchTransaction_withDelayedEncode),
                    abi);
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestSendBatchTransaction_withERC1155), e.Message, abi));
            }
        }
        
        public async Task TestDeployContract(string bytecode)
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                ContractDeploymentReturn result = await _wallet.DeployContract(Chain.Polygon, bytecode);
                CustomAssert.IsTrue(result is SuccessfulContractDeploymentReturn, nameof(TestDeployContract));
                if (result is SuccessfulContractDeploymentReturn successfulResult)
                {
                    CustomAssert.NotNull(successfulResult.TransactionReturn, nameof(TestDeployContract));
                    CustomAssert.NotNull(successfulResult.DeployedContractAddress, nameof(TestDeployContract));
                }
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestDeployContract), e.Message));
            }
        }
    }
}