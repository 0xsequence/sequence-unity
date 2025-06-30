using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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

        [TestCase("{\"explicitSession\":{\"signer\":\"0x00000000000000000000000000000000000025a8\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]},\"sessionTopology\":[{\"type\":\"implicit-blacklist\",\"blacklist\":[]},{\"type\":\"identity-signer\",\"identitySigner\":\"0x8312fc6754389018bdD3BDfEFf226DD8eD9EcdB1\"}]}")]
        public void TestAddExplicitSession(string inputJson)
        {
            var input = JsonConvert.DeserializeObject<Dictionary<string, object>>(inputJson);
            var explicitSessionJson = input["explicitSession"].ToString();
            var sessionTopologyJson = input["sessionTopology"].ToString();

            var explicitSession = SessionPermissions.FromJson(explicitSessionJson);
            var sessionTopology = SessionsTopology.FromJson(sessionTopologyJson);

            var existingPermission = sessionTopology.FindPermissions(explicitSession.signer);
            if (existingPermission != null)
                throw new Exception("Session already exists.");

            var newTopology = sessionTopology.AddExplicitSession(explicitSession);
            Debug.Log($"{newTopology.JsonSerialize()}");
        }
    }
}