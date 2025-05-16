using System;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.Sidekick
{
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
                string walletAddress = await sidekick.GetWalletAddress();
                Assert.IsNotNull(walletAddress);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected no exception, but got: " + e.Message);
            }
        }

    }
}