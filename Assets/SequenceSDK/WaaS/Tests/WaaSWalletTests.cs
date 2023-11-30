using System.Numerics;
using System.Threading.Tasks;
using Sequence.Provider;
using Sequence.Wallet;
using Sequence.Utils;
using SequenceSDK.WaaS;

namespace Sequence.WaaS.Tests
{
    public class WaaSWalletTests
    {
        private IWallet _wallet;
        private string _address;

        private string _toAddress = "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f";
        private string _polygonNode = "https://polygon-bor.publicnode.com";
        private string _erc20Address = "0x3c499c542cEF5E3811e1192ce70d8cC03d5c3359";
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
            var result = await _wallet.GetWalletAddress(new GetWalletAddressArgs(0));
            _address = result.address;
        }
        
        public async Task TestMessageSigning(string message, Chain network)
        {
            WaaSTestHarness.TestStarted?.Invoke();
            var result = await _wallet.SignMessage(new SignMessageArgs(_address, network, message));
            string signature = result.signature;
            CustomAssert.NotNull(signature, nameof(TestMessageSigning), message, network);
            var ethWallet = new EthWallet();
            var networkId = (BigInteger)(int)network;
            var isValid = await ethWallet.IsValidSignature(signature, message, networkId.BigIntegerToHexString()); // Todo replace: this checks if the signature was signed by the EthWallet, I want to check that it was signed by my WaaS wallet
            CustomAssert.IsTrue(isValid, nameof(TestMessageSigning), message, network);
            WaaSTestHarness.TestPassed?.Invoke();
        }

        public async Task TestTransfer()
        {
            WaaSTestHarness.TestStarted?.Invoke();
            
            var balance = await _client.BalanceAt(_address);
            var balance2 = await _client.BalanceAt(_toAddress);
            TransactionReturn result = await _wallet.SendTransaction(new SendTransactionArgs(_address, Chain.Polygon,
                new SequenceSDK.WaaS.Transaction[]
                {
                    new RawTransaction(_toAddress, "1"),
                }));
            var newBalance = await _client.BalanceAt(_address);
            var newBalance2 = await _client.BalanceAt(_toAddress);
            CustomAssert.IsTrue(result is SuccessfulTransactionReturn, nameof(TestTransfer));
            CustomAssert.IsTrue(newBalance < balance, nameof(TestTransfer));
            CustomAssert.IsTrue(newBalance2 > balance2, nameof(TestTransfer));
            WaaSTestHarness.TestPassed?.Invoke();
        }
        
        public async Task TestSendERC20()
        {
            WaaSTestHarness.TestStarted?.Invoke();
            GetTokenBalancesReturn tokenBalances =
                await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_address, _erc20Address));
            BigInteger balance = tokenBalances.balances[0].balance;
            GetTokenBalancesReturn tokenBalances2 =
                await _polygonIndexer.GetTokenBalances(new GetTokenBalancesArgs(_toAddress, _erc20Address));
            BigInteger balance2 = tokenBalances2.balances[0].balance;
            TransactionReturn result = await _wallet.SendTransaction(new SendTransactionArgs(_address, Chain.Polygon,
                new SequenceSDK.WaaS.Transaction[]
                {
                    new SendERC20(_erc20Address, _toAddress, "1"),
                }));
            CustomAssert.IsTrue(result is SuccessfulTransactionReturn, nameof(TestSendERC20));
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
    }
}