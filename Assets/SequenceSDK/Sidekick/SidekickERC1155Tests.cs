using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence;
using Sequence.Sidekick;
public class SidekickERC1155MintTest
{
    private SequenceSidekickClient sidekick;
    private string chainId;
    private string walletAddress;

    [SetUp]
    public void Setup()
    {
        sidekick = new SequenceSidekickClient(Chain.TestnetArbitrumSepolia);
        chainId = sidekick.Chain.GetChainId();
    }


    [Test]
    public async Task DeployERC1155()
    {
        try
        {
            walletAddress = await sidekick.GetWalletAddress();
            string recipientAddress = JObject.Parse(walletAddress)["address"]?.ToString();

            var deployBody = new
            {
                defaultAdmin = recipientAddress,
                minter = recipientAddress,
                name = "MYTESTERC1155"
            };


            string deployJson = JsonConvert.SerializeObject(deployBody);

            string deployResult = await sidekick.DeployERC1155(deployJson);

            Debug.Log("Deploy result: " + deployResult);
            Assert.IsNotNull(deployResult, "Deploy result must not be null");
        }
        catch (Exception e)
        {
            Assert.Fail("ERC1155 deploy flow failed: " + e.Message);
        }
    }


    [Test]
    public async Task MintERC1155()
    {
        try
        {
            walletAddress = await sidekick.GetWalletAddress();

            string recipientAddress = JObject.Parse(walletAddress)["address"]?.ToString();

            var mintBody = new
            {
                recipient = recipientAddress,
                id = "0",
                amount = "1",
                data = "0x00"
            };
            string mintJson = JsonConvert.SerializeObject(mintBody);
            string mintResult = await sidekick.MintERC1155("0x63c12baa017b2bcb6855d83506500edcac423c3c", mintJson);

            Debug.Log("Mint result: " + mintResult);

            Assert.IsNotNull(mintResult, "Mint result must not be null");
        }
        catch (Exception e)
        {
            Assert.Fail("ERC1155 flow failed: " + e.Message);
        }
    }
}
