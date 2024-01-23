using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Contracts;
using Sequence.Provider;
using Sequence.Transactions;
using SequenceSDK.WaaS;

namespace Sequence.WaaS.Tests
{
    public class WaaSToWalletAdapterTests
    {
        private Wallet.IWallet _wallet;
        private string _address;
        private string _toAddress = "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f";
        private string _polygonNode = "https://polygon-bor.publicnode.com";
        private string _erc20Address = "0x0d500B1d8E8eF31E21C99d1Db9A6444d3ADf1270";
        private string _erc721Address = "0xa9a6A3626993D487d2Dbda3173cf58cA1a9D9e9f";
        private string _erc721TokenId = "18744835910876056821490721563850010263377359459933499228339117876467177683972";
        private string _erc1155Address = "0x44b3f42e2bf34f62868ff9e9dab7c2f807ba97cb";
        private string _erc1155TokenId = "86";
        private IIndexer _polygonIndexer = new ChainIndexer((int)Chain.Polygon);

        private IEthClient _client;
        
        public WaaSToWalletAdapterTests(WaaS.IWallet wallet)
        {
            _wallet = new WaaSToWalletAdapter(wallet);
            GetAddress();
            _client = new SequenceEthClient(_polygonNode);
        }
        
        private async Task GetAddress()
        {
            var result = _wallet.GetAddress();
            _address = result;
        }

        public void TestGetAddress(string expected)
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                Address address = _wallet.GetAddress();
                CustomAssert.IsEqual(expected, address.Value, nameof(TestGetAddress), expected);
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestGetAddress), e.Message, expected));
            }
        }
        
        public async Task TestSignMessage_withAdapter(string message, Chain network)
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                string signature = await _wallet.SignMessage(message, network.AsHexString());
                CustomAssert.NotNull(signature, nameof(TestSignMessage_withAdapter), message, network);
                bool isValid = await _wallet.IsValidSignature(signature, message, network);
                CustomAssert.IsTrue(isValid, nameof(TestSignMessage_withAdapter), message, network);
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestSignMessage_withAdapter), e.Message, message, network));
            }
        }

        public async Task TestSendTransaction_withAdapter()
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                BalanceChecker balanceChecker = await BalanceChecker.CreateAsync(_client, _address);
                BalanceChecker balanceChecker2 = await BalanceChecker.CreateAsync(_client, _toAddress);

                EthTransaction transaction =
                    await new GasLimitEstimator(_client, new Address(_address)).BuildTransaction(_toAddress, value: 1);
                TransactionReceipt receipt = await _wallet.SendTransactionAndWaitForReceipt(_client, transaction);

                CustomAssert.NotNull(receipt, nameof(TestSendTransaction_withAdapter));
                await Task.Delay(WaaSTestHarness.DelayForTransactionToProcess);
                await balanceChecker.AssertNewValueIsSmaller(nameof(TestSendTransaction_withAdapter));
                await balanceChecker2.AssertNewValueIsLarger(nameof(TestSendTransaction_withAdapter));
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestSendTransaction_withAdapter), e.Message));
            }
        }
        
        public async Task TestSendERC20_withAdapter()
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                Erc20BalanceChecker balanceChecker =
                    await Erc20BalanceChecker.CreateAsync(_polygonIndexer, _address, _erc20Address);
                Erc20BalanceChecker balanceChecker2 =
                    await Erc20BalanceChecker.CreateAsync(_polygonIndexer, _toAddress, _erc20Address);
                
                ERC20 usdc = new ERC20(_erc20Address);
                TransactionReceipt receipt = await usdc.Transfer(_toAddress, 1).SendTransactionMethodAndWaitForReceipt(_wallet, _client);

                CustomAssert.NotNull(receipt, nameof(TestSendTransaction_withAdapter));
                await Task.Delay(WaaSTestHarness.DelayForTransactionToProcess);
                await balanceChecker.AssertNewValueIsSmaller(nameof(TestSendERC20_withAdapter));
                await balanceChecker2.AssertNewValueIsLarger(nameof(TestSendERC20_withAdapter));
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestSendERC20_withAdapter), e.Message));
            }
        }

        public async Task TestBatchTransactions_withAdapter()
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                BalanceChecker balanceChecker = await BalanceChecker.CreateAsync(_client, _address);
                BalanceChecker balanceChecker2 = await BalanceChecker.CreateAsync(_client, _toAddress);
                Erc20BalanceChecker erc20BalanceChecker = await Erc20BalanceChecker.CreateAsync(_polygonIndexer, _address, _erc20Address);
                Erc20BalanceChecker erc20BalanceChecker2 = await Erc20BalanceChecker.CreateAsync(_polygonIndexer, _toAddress, _erc20Address);
                Erc721BalanceChecker erc721BalanceChecker = await Erc721BalanceChecker.CreateAsync(_polygonIndexer, _address, _erc721Address);
                Erc721BalanceChecker erc721BalanceChecker2 = await Erc721BalanceChecker.CreateAsync(_polygonIndexer, _toAddress, _erc721Address);
                Erc1155BalanceChecker erc1155BalanceChecker = await Erc1155BalanceChecker.CreateAsync(_polygonIndexer, _address, _erc1155Address);
                Erc1155BalanceChecker erc1155BalanceChecker2 = await Erc1155BalanceChecker.CreateAsync(_polygonIndexer, _toAddress, _erc1155Address);
                
                EthTransaction transfer =
                    await new GasLimitEstimator(_client, new Address(_address)).BuildTransaction(_toAddress, value: 1);
                ERC20 usdc = new ERC20(_erc20Address);
                EthTransaction erc20Transfer = await usdc.Transfer(_toAddress, 1).Create(_client, new ContractCall(new Address(_address)));
                ERC721 erc721 = new ERC721(_erc721Address);
                EthTransaction erc721Transfer = await erc721.TransferFrom(_address, _toAddress, _erc721TokenId).Create(_client, new ContractCall(new Address(_address)));
                ERC1155 erc1155 = new ERC1155(_erc1155Address);
                EthTransaction erc1155Transfer = await erc1155.SafeTransferFrom(_address, _toAddress, _erc1155TokenId, 1).Create(_client, new ContractCall(new Address(_address)));
                TransactionReceipt[] receipts = await _wallet.SendTransactionBatchAndWaitForReceipts(_client,
                    new EthTransaction[]
                    {
                        transfer, erc20Transfer, erc721Transfer, erc1155Transfer
                    });
                CustomAssert.IsEqual(1, receipts.Length, nameof(TestBatchTransactions_withAdapter));
                TransactionReceipt receipt = receipts[0];
                CustomAssert.NotNull(receipt, nameof(TestBatchTransactions_withAdapter));
                
                await Task.Delay(WaaSTestHarness.DelayForTransactionToProcess);
                await balanceChecker.AssertNewValueIsSmaller(nameof(TestBatchTransactions_withAdapter));
                await balanceChecker2.AssertNewValueIsLarger(nameof(TestBatchTransactions_withAdapter));
                await erc20BalanceChecker.AssertNewValueIsSmaller(nameof(TestBatchTransactions_withAdapter));
                await erc20BalanceChecker2.AssertNewValueIsLarger(nameof(TestBatchTransactions_withAdapter));
                await erc721BalanceChecker.AssertNewValueIsSmaller(nameof(TestBatchTransactions_withAdapter));
                await erc721BalanceChecker2.AssertNewValueIsLarger(nameof(TestBatchTransactions_withAdapter));
                await erc1155BalanceChecker.AssertNewValueIsSmaller(nameof(TestBatchTransactions_withAdapter));
                await erc1155BalanceChecker2.AssertNewValueIsLarger(nameof(TestBatchTransactions_withAdapter));
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestSendERC20_withAdapter), e.Message));
            }
        }

        public async Task TestDeployContract_withAdapter(string bytecode)
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                ContractDeploymentResult result = await ContractDeployer.Deploy(_client, _wallet, bytecode);
                CustomAssert.NotNull(result, nameof(TestDeployContract_withAdapter));
                CustomAssert.NotNull(result.DeployedContractAddress, nameof(TestDeployContract_withAdapter));
                CustomAssert.IsEqual(result.Receipt.contractAddress, result.DeployedContractAddress.Value, nameof(TestDeployContract_withAdapter));
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestDeployContract_withAdapter), e.Message));
            }
        }
    }
}