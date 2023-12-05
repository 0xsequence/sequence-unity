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
        private string _erc20Address = "0x3c499c542cEF5E3811e1192ce70d8cC03d5c3359";
        private string _erc721Address = "0xa9a6A3626993D487d2Dbda3173cf58cA1a9D9e9f";
        private string _erc721TokenId = "27070476534167349548939059504276130568103741723583141759304527275404067148946";
        private string _erc1155Address = "0x44b3f42e2bf34f62868ff9e9dab7c2f807ba97cb";
        private string _erc1155TokenId = "86";
        private IIndexer _polygonIndexer = new ChainIndexer((int)Chain.Polygon);

        private IEthClient _client;
        private int _delayForTransactionToProcess = 10000; // Allow the indexer some time to pull new data from chain
        
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
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestGetAddress), e.Message));
            }
        }
        
        public async Task TestSignMessage(string message, Chain network)
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                string signature = await _wallet.SignMessage(message, network.AsString());
                CustomAssert.NotNull(signature, nameof(TestSignMessage), message, network);
                CustomAssert.IsTrue(WaaSWalletTests.AppearsToBeValidSignature(signature), nameof(TestSignMessage), message, network); // If a signature appears valid and comes from WaaS, it most likely is valid - validity is tested on the WaaS side
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestSignMessage), e.Message));
            }
        }

        public async Task TestSendTransaction_basicTransfer()
        {
            try
            {
                WaaSTestHarness.TestStarted?.Invoke();
                var balance = await _client.BalanceAt(_address);
                var balance2 = await _client.BalanceAt(_toAddress);

                EthTransaction transaction =
                    await new GasLimitEstimator(_client, new Address(_address)).BuildTransaction(_toAddress, value: 1);
                TransactionReceipt receipt = await _wallet.SendTransactionAndWaitForReceipt(_client, transaction);

                CustomAssert.NotNull(receipt, nameof(TestSendTransaction_basicTransfer));
                await Task.Delay(_delayForTransactionToProcess);
                var newBalance = await _client.BalanceAt(_address);
                var newBalance2 = await _client.BalanceAt(_toAddress);
                CustomAssert.IsTrue(newBalance < balance, nameof(TestSendTransaction_basicTransfer));
                CustomAssert.IsTrue(newBalance2 > balance2, nameof(TestSendTransaction_basicTransfer));
                WaaSTestHarness.TestPassed?.Invoke();
            }
            catch (Exception e)
            {
                WaaSTestHarness.TestFailed?.Invoke(new WaaSTestFailed(nameof(TestSendTransaction_basicTransfer), e.Message));
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

                ERC20 usdc = new ERC20(_erc20Address);
                TransactionReceipt receipt = await usdc.Transfer(_toAddress, 1).SendTransactionMethodAndWaitForReceipt(_wallet, _client);

                CustomAssert.NotNull(receipt, nameof(TestSendTransaction_basicTransfer));
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
    }
}