using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.ABI;
using Sequence.Contracts;
using Sequence.Provider;
using Sequence.Wallet;
using Sequence.EmbeddedWallet;
using UnityEngine;

namespace Sequence.EmbeddedWallet.Tests
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
        private IIndexer _polygonIndexer = new ChainIndexer(Chain.Polygon);

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