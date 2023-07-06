using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Provider;
using UnityEngine;
using UnityEngine.TestTools;

public class RPCTests
{
    string clientUrl = "http://localhost:8545/";
    float polygonBlockTimeInSeconds = 2f;
    
    [UnityTest]
    public IEnumerator TestChainID()
    {
        string expected = "0x7a69";
        var client = new SequenceEthClient(clientUrl);

        Task<string> chainIdTask = client.ChainID();
        while (!chainIdTask.IsCompleted)
        {
            yield return null;
        }

        if (chainIdTask.Exception != null)
        {
            Assert.Fail("Unexpected exception: " + chainIdTask.Exception.Message);
            yield break;
        }
        string chainId = chainIdTask.Result;
        Assert.AreEqual(expected, chainId);
    }

    [UnityTest]
    public IEnumerator TestNetworkId()
    {
        string expected = "31337";
        var client = new SequenceEthClient(clientUrl);

        Task<string> networkIdTask = client.NetworkId();
        while (!networkIdTask.IsCompleted)
        {
            yield return null;
        }

        if (networkIdTask.Exception != null)
        {
            Assert.Fail("Unexpected exception: " + networkIdTask.Exception.Message);
            yield break;
        }
        string networkId = networkIdTask.Result;
        Assert.AreEqual(expected, networkId);
    }

    [UnityTest]
    public IEnumerator TestBlockNumber()
    {
            var client = new SequenceEthClient(clientUrl);

            Task<string> blockNumberTask = client.BlockNumber();
            while (!blockNumberTask.IsCompleted)
            {
                yield return null;
            }

            if (blockNumberTask.Exception != null)
            {
                Assert.Fail("Unexpected exception: " + blockNumberTask.Exception.Message);
                yield break;
            }
            var blockNumber = blockNumberTask.Result;

            yield return new WaitForSecondsRealtime(polygonBlockTimeInSeconds * 3); // Wait for more than the block time just in case

            Task<string> blockNumberTask2 = client.BlockNumber();
            while (!blockNumberTask2.IsCompleted)
            {
                yield return null;
            }

            if (blockNumberTask2.Exception != null)
            {
                Assert.Fail("Unexpected exception: " + blockNumberTask2.Exception.Message);
                yield break;
            }
            var blockNumber2 = blockNumberTask2.Result;

            Assert.Less(blockNumber, blockNumber2);
    }

    [UnityTest]
    public IEnumerator TestERC20MintAndTransfer()
    {
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestBlockByNumber()
    {
        yield return null;
    }

    [UnityTest]
    public IEnumerator ExampleBatchCall()
    {
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestETHRPC()
    {
        yield return null;
    }

}
