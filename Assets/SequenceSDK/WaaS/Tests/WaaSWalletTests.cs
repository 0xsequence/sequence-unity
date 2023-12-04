using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Extensions;
using Sequence.Provider;
using Sequence.Wallet;
using Sequence.Utils;
using SequenceSDK.WaaS;
using UnityEngine;

namespace Sequence.WaaS.Tests
{
    // Note: These tests rely on the certain tokens being present in a given wallet address. These tokens are typically found in the wallet created by WaaS when logging in via
    // Google to qp@horizon.io - 0x48b0560661326cB8EECb68107CD72B4B4aB8B2fb
    public class WaaSWalletTests
    {
        public static Exception TestNotSetupProperly;
        
        private IWallet _wallet;
        private string _address;

        private string _toAddress = "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f";
        private string _polygonNode = "https://polygon-bor.publicnode.com";
        private string _erc20Address = "0x3c499c542cEF5E3811e1192ce70d8cC03d5c3359";
        private string _erc721Address = "0xa9a6A3626993D487d2Dbda3173cf58cA1a9D9e9f";
        private string _erc721TokenId = "54530968763798660137294927684252503703134533114052628080002308208148824588621";
        private string _erc1155Address = "0x44b3f42e2bf34f62868ff9e9dab7c2f807ba97cb";
        private string _erc1155TokenId = "86";
        private IIndexer _polygonIndexer = new ChainIndexer((int)Chain.Polygon);

        private IEthClient _client;
        private int _delayForTransactionToProcess = 10000; // Allow the indexer some time to pull new data from chain

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
                var result = await _wallet.SignMessage(new SignMessageArgs(_address, network, message));
                string signature = result.signature;
                CustomAssert.NotNull(signature, nameof(TestMessageSigning), message, network);
                CustomAssert.IsTrue(AppearsToBeValidSignature(signature), nameof(TestMessageSigning), message, network); // If a signature appears valid and comes from WaaS, it most likely is valid - validity is tested on the WaaS side
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestMessageSigning), e.Message, message, network));
            }
        }

        private static bool AppearsToBeValidSignature(string signature)
        {
            return signature.StartsWith("0x") && signature.IsHexFormat();
        } 

        public async Task TestTransfer()
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                var balance = await _client.BalanceAt(_address);
                var balance2 = await _client.BalanceAt(_toAddress);

                TransactionReturn result = await _wallet.SendTransaction(new SendTransactionArgs(_address,
                    Chain.Polygon,
                    new SequenceSDK.WaaS.Transaction[]
                    {
                        new RawTransaction(_toAddress, "1"),
                    }));

                CustomAssert.IsTrue(result is SuccessfulTransactionReturn, nameof(TestTransfer));
                await Task.Delay(_delayForTransactionToProcess);
                var newBalance = await _client.BalanceAt(_address);
                var newBalance2 = await _client.BalanceAt(_toAddress);
                CustomAssert.IsTrue(newBalance < balance, nameof(TestTransfer));
                CustomAssert.IsTrue(newBalance2 > balance2, nameof(TestTransfer));
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
                GetTokenBalancesReturn tokenBalances =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_address, _erc20Address));
                BigInteger balance = tokenBalances.balances[0].balance;
                GetTokenBalancesReturn tokenBalances2 =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_toAddress, _erc20Address));
                BigInteger balance2 = tokenBalances2.balances[0].balance;

                TransactionReturn result = await _wallet.SendTransaction(new SendTransactionArgs(_address,
                    Chain.Polygon,
                    new SequenceSDK.WaaS.Transaction[]
                    {
                        new SendERC20(_erc20Address, _toAddress, "1"),
                    }));

                CustomAssert.IsTrue(result is SuccessfulTransactionReturn, nameof(TestSendERC20));
                await Task.Delay(_delayForTransactionToProcess);
                GetTokenBalancesReturn newTokenBalances =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_address, _erc20Address));
                BigInteger newBalance = newTokenBalances.balances[0].balance;
                GetTokenBalancesReturn newTokenBalances2 =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_toAddress, _erc20Address));
                BigInteger newBalance2 = newTokenBalances2.balances[0].balance;
                CustomAssert.IsTrue(newBalance < balance, nameof(TestSendERC20));
                CustomAssert.IsTrue(newBalance2 > balance2, nameof(TestSendERC20));
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestSendERC20), e.Message));
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
                BigInteger balance = await _client.BalanceAt(_address);
                GetTokenBalancesReturn erc20Balances =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_address, _erc20Address));
                BigInteger erc20Balance = erc20Balances.balances[0].balance;
                GetTokenBalancesReturn erc721Balances =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_address, _erc721Address));
                BigInteger erc721Balance = erc721Balances.balances[0].balance;
                if (erc721Balance == 0)
                {
                    string failReason =
                        $"Test {nameof(TestSendBatchTransaction_withERC721)} was not setup properly. {_address} must have an NFT from contract address: {_erc721Address}";
                    Debug.LogError(failReason);
                    WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestSendBatchTransaction_withERC721),
                        failReason));
                    throw TestNotSetupProperly;
                }

                BigInteger balance2 = await _client.BalanceAt(_toAddress);
                GetTokenBalancesReturn erc20Balances2 =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_toAddress, _erc20Address));
                BigInteger erc20Balance2 = erc20Balances2.balances[0].balance;

                TransactionReturn result = await _wallet.SendTransaction(new SendTransactionArgs(_address,
                    Chain.Polygon,
                    new SequenceSDK.WaaS.Transaction[]
                    {
                        new SendERC20(_erc20Address, _toAddress, "1"),
                        new SendERC721(_erc721Address, _toAddress, _erc721TokenId),
                        new RawTransaction(_toAddress, "1")
                    }));
                CustomAssert.IsTrue(result is SuccessfulTransactionReturn, nameof(TestSendBatchTransaction_withERC721));
                await Task.Delay(_delayForTransactionToProcess);

                BigInteger newBalance = await _client.BalanceAt(_address);
                GetTokenBalancesReturn newErc20Balances =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_address, _erc20Address));
                BigInteger newErc20Balance = newErc20Balances.balances[0].balance;
                GetTokenBalancesReturn newErc721Balances =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_address, _erc721Address));
                BigInteger newBalance2 = await _client.BalanceAt(_toAddress);
                GetTokenBalancesReturn newErc20Balances2 =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_toAddress, _erc20Address));
                BigInteger newErc20Balance2 = newErc20Balances2.balances[0].balance;
                GetTokenBalancesReturn newErc721Balances2 =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_toAddress, _erc721Address));
                BigInteger newErc721Balance2 = newErc721Balances2.balances[0].balance;

                CustomAssert.IsTrue(balance > newBalance, nameof(TestSendBatchTransaction_withERC721));
                CustomAssert.IsTrue(erc20Balance > newErc20Balance, nameof(TestSendBatchTransaction_withERC721));
                CustomAssert.IsTrue(newErc721Balances.balances.Length == 0,
                    nameof(TestSendBatchTransaction_withERC721));
                CustomAssert.IsTrue(balance2 < newBalance2, nameof(TestSendBatchTransaction_withERC721));
                CustomAssert.IsTrue(erc20Balance2 < newErc20Balance2, nameof(TestSendBatchTransaction_withERC721));
                CustomAssert.IsTrue(newErc721Balance2 == 1, nameof(TestSendBatchTransaction_withERC721));
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
                BigInteger balance = await _client.BalanceAt(_address);
                GetTokenBalancesReturn erc20Balances =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_address, _erc20Address));
                BigInteger erc20Balance = erc20Balances.balances[0].balance;
                GetTokenBalancesReturn erc1155Balances =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_address, _erc1155Address));
                BigInteger erc1155Balance = erc1155Balances.balances[0].balance;
                if (erc1155Balance == 0)
                {
                    string failReason =
                        $"Test {nameof(TestSendBatchTransaction_withERC1155)} was not setup properly. {_address} must have an SFT from contract address: {_erc1155Address}";
                    Debug.LogError(failReason);
                    throw TestNotSetupProperly;
                }

                BigInteger balance2 = await _client.BalanceAt(_toAddress);
                GetTokenBalancesReturn erc20Balances2 =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_toAddress, _erc20Address));
                BigInteger erc20Balance2 = erc20Balances2.balances[0].balance;

                TransactionReturn result = await _wallet.SendTransaction(new SendTransactionArgs(_address,
                    Chain.Polygon,
                    new SequenceSDK.WaaS.Transaction[]
                    {
                        new SendERC20(_erc20Address, _toAddress, "1"),
                        new SendERC1155(_erc1155Address, _toAddress, new SendERC1155Values[]
                        {
                            new SendERC1155Values(_erc1155TokenId, "1")
                        }),
                        new RawTransaction(_toAddress, "1")
                    }));
                CustomAssert.IsTrue(result is SuccessfulTransactionReturn,
                    nameof(TestSendBatchTransaction_withERC1155));
                await Task.Delay(_delayForTransactionToProcess);

                BigInteger newBalance = await _client.BalanceAt(_address);
                GetTokenBalancesReturn newErc20Balances =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_address, _erc20Address));
                BigInteger newErc20Balance = newErc20Balances.balances[0].balance;
                GetTokenBalancesReturn newErc1155Balances =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_address, _erc1155Address));
                BigInteger newBalance2 = await _client.BalanceAt(_toAddress);
                GetTokenBalancesReturn newErc20Balances2 =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_toAddress, _erc20Address));
                BigInteger newErc20Balance2 = newErc20Balances2.balances[0].balance;
                GetTokenBalancesReturn newErc1155Balances2 =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_toAddress, _erc1155Address));
                BigInteger newErc1155Balance2 = newErc1155Balances2.balances[0].balance;

                CustomAssert.IsTrue(balance > newBalance, nameof(TestSendBatchTransaction_withERC1155));
                CustomAssert.IsTrue(erc20Balance > newErc20Balance, nameof(TestSendBatchTransaction_withERC1155));
                CustomAssert.IsTrue(newErc1155Balances.balances.Length == 0 || newErc1155Balances.balances[0].balance < erc1155Balance,
                    nameof(TestSendBatchTransaction_withERC1155));
                CustomAssert.IsTrue(balance2 < newBalance2, nameof(TestSendBatchTransaction_withERC1155));
                CustomAssert.IsTrue(erc20Balance2 < newErc20Balance2, nameof(TestSendBatchTransaction_withERC1155));
                CustomAssert.IsTrue(newErc1155Balance2 == 1, nameof(TestSendBatchTransaction_withERC1155));
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
                GetTokenBalancesReturn tokenBalances =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_address, _erc20Address));
                BigInteger balance = tokenBalances.balances[0].balance;
                GetTokenBalancesReturn tokenBalances2 =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_toAddress, _erc20Address));
                BigInteger balance2 = tokenBalances2.balances[0].balance;

                TransactionReturn result = await _wallet.SendTransaction(new SendTransactionArgs(_address,
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
                    }));

                CustomAssert.IsTrue(result is SuccessfulTransactionReturn, nameof(TestDelayedEncode));
                await Task.Delay(_delayForTransactionToProcess);
                GetTokenBalancesReturn newTokenBalances =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_address, _erc20Address));
                BigInteger newBalance = newTokenBalances.balances[0].balance;
                GetTokenBalancesReturn newTokenBalances2 =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_toAddress, _erc20Address));
                BigInteger newBalance2 = newTokenBalances2.balances[0].balance;
                CustomAssert.IsTrue(newBalance < balance, nameof(TestDelayedEncode));
                CustomAssert.IsTrue(newBalance2 > balance2, nameof(TestDelayedEncode));
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestDelayedEncode), e.Message));
            }
        }
        
        public async Task TestSendBatchTransaction_withDelayedEncode(string abi)
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                BigInteger balance = await _client.BalanceAt(_address);
                GetTokenBalancesReturn erc20Balances =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_address, _erc20Address));
                BigInteger erc20Balance = erc20Balances.balances[0].balance;

                BigInteger balance2 = await _client.BalanceAt(_toAddress);
                GetTokenBalancesReturn erc20Balances2 =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_toAddress, _erc20Address));
                BigInteger erc20Balance2 = erc20Balances2.balances[0].balance;

                TransactionReturn result = await _wallet.SendTransaction(new SendTransactionArgs(_address,
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
                    }));
                CustomAssert.IsTrue(result is SuccessfulTransactionReturn,
                    nameof(TestSendBatchTransaction_withERC1155));
                await Task.Delay(_delayForTransactionToProcess);

                BigInteger newBalance = await _client.BalanceAt(_address);
                GetTokenBalancesReturn newErc20Balances =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_address, _erc20Address));
                BigInteger newErc20Balance = newErc20Balances.balances[0].balance;
                BigInteger newBalance2 = await _client.BalanceAt(_toAddress);
                GetTokenBalancesReturn newErc20Balances2 =
                    await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_toAddress, _erc20Address));
                BigInteger newErc20Balance2 = newErc20Balances2.balances[0].balance;

                CustomAssert.IsTrue(balance > newBalance, nameof(TestSendBatchTransaction_withERC1155));
                CustomAssert.IsTrue(erc20Balance > newErc20Balance, nameof(TestSendBatchTransaction_withERC1155));
                CustomAssert.IsTrue(balance2 < newBalance2, nameof(TestSendBatchTransaction_withERC1155));
                CustomAssert.IsTrue(erc20Balance2 < newErc20Balance2, nameof(TestSendBatchTransaction_withERC1155));
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestSendBatchTransaction_withERC1155), e.Message));
            }
        }
    }
}