using NUnit.Framework;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.UnitTests
{
    public class SessionsUnitTests
    {
        [TestCase("0x8312fc6754389018bdD3BDfEFf226DD8eD9EcdB1")]
        public void TestSessionEncoding(string identitySigner)
        {
            var topology = SessionsUtils.CreateSessionsTopologyWithSingleIdentity(identitySigner);
            var encoded = topology.Encode().ByteArrayToHexStringWithPrefix();
            Debug.Log($"Encoded Sessions Topology: {encoded}");
            
            Debug.Log($"Sessions Topology: {topology.JsonSerialize()}");

            var newTopology = SessionsTopology.FromJson(topology.JsonSerialize());
            
            Assert.AreEqual(topology.JsonSerialize(), newTopology.JsonSerialize());
        }
    }
}