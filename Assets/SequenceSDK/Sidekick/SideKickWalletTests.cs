using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Sequence.Sidekick;
public class SequenceSidekickTests
{
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
