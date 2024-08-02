using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Contracts;
using Sequence.Provider;
using Sequence.Transactions;
using Sequence.EmbeddedWallet;

namespace Sequence.EmbeddedWallet.Tests
{
    public class SequenceWalletToEOAWalletAdapterTests
    {
        private Sequence.Wallet.IWallet _wallet;
        private string _address;
        private string _toAddress = "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f";
        private string _polygonNode = "https://polygon-bor.publicnode.com";
        private string _erc20Address = "0x0d500B1d8E8eF31E21C99d1Db9A6444d3ADf1270";
        private string _erc721Address = "0xa9a6A3626993D487d2Dbda3173cf58cA1a9D9e9f";
        private string _erc721TokenId = "18744835910876056821490721563850010263377359459933499228339117876467177683972";
        private string _erc1155Address = "0x44b3f42e2bf34f62868ff9e9dab7c2f807ba97cb";
        private string _erc1155TokenId = "86";
        private IIndexer _polygonIndexer = new ChainIndexer(Chain.Polygon);

        private IEthClient _client;
        
        public SequenceWalletToEOAWalletAdapterTests(EmbeddedWallet.IWallet wallet)
        {
            _wallet = new SequenceWalletToEOAWalletAdapter(wallet);
            GetAddress();
            _client = new SequenceEthClient(_polygonNode);
        }
        
        private async Task GetAddress()
        {
            var result = _wallet.GetAddress();
            _address = result;
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