using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Mocks;
using Sequence.Provider;
using UnityEngine;
using UnityEngine.TestTools;

public class RPCTests
{
    string clientUrl = "http://localhost:8545/";
    float polygonBlockTimeInSeconds = 2f;

    [Test]
    public async Task TestChainId() {
        try {
            string expected = "0x7a69";
            var client = new SequenceEthClient(clientUrl);
            string chainId = await client.ChainID();
            Assert.AreEqual(expected, chainId);
        }
        catch (System.Exception ex) {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }

    [Test]
    public async Task TestNetworkId() {
        try {
            string expected = "31337";
            var client = new SequenceEthClient(clientUrl);
            string networkId = await client.NetworkId();
            Assert.AreEqual(expected, networkId);
        }
        catch (System.Exception ex) {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }

    [Test]
    public async Task TestBlockNumber() {
        try {
            var client = new SequenceEthClient(clientUrl);
            var blockNumber = await client.BlockNumber();
            await Task.Delay((int)(polygonBlockTimeInSeconds * 1000 * 3)); // Wait for more than the block time just in case
            var blockNumber2 = await client.BlockNumber();
            Assert.Less(blockNumber, blockNumber2);
        }
        catch (System.Exception ex) {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }


}
