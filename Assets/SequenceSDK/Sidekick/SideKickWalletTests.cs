using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Sequence;
using Sequence.EmbeddedWallet;
public class SequenceSidekickTests
{
    SequenceSidekickClient sidekick;
    string chainId;

    [SetUp]
    public void Setup()
    {
        sidekick = new SequenceSidekickClient(Chain.TestnetArbitrumSepolia);
        chainId = sidekick.Chain.GetChainId();

    }

    [Test]
    public async Task TestGetWalletAddress()
    {
        try
        {
            var sidekick = new SequenceSidekickClient();
            string response = await sidekick.GetWalletAddress();
            Assert.IsNotNull(response);
            var json = JObject.Parse(response);
            Assert.IsTrue(json.ContainsKey("walletAddress"));
        }
        catch (Exception e)
        {
            Assert.Fail("Expected no exception, but got: " + e.Message);
        }
    }

}
