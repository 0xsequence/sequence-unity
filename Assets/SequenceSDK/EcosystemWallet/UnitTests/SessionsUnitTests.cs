using System.Collections.Generic;
using System.Linq;
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

            var newTopology = sessionTopology.AddExplicitSession(explicitSession);
            Debug.Log($"{newTopology.JsonSerialize()}");
        }

        [TestCase("{\"sessionTopology\":[[[[[[[[[[[[[[[[[[[[[{\"type\":\"implicit-blacklist\",\"blacklist\":[]},{\"type\":\"identity-signer\",\"identitySigner\":\"0x8312fc6754389018bdD3BDfEFf226DD8eD9EcdB1\"}],{\"type\":\"session-permissions\",\"signer\":\"0x00000000000000000000000000000000000025a8\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x00000000000000000000000000000000000009CD\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x0000000000000000000000000000000000002AA7\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x00000000000000000000000000000000000026eB\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x0000000000000000000000000000000000001F8C\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x000000000000000000000000000000000000435E\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x000000000000000000000000000000000000026e\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x000000000000000000000000000000000000133b\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x0000000000000000000000000000000000002B44\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x000000000000000000000000000000000000004C\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x0000000000000000000000000000000000002015\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x00000000000000000000000000000000000000E9\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x0000000000000000000000000000000000003Ec4\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x00000000000000000000000000000000000002eF\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x0000000000000000000000000000000000003703\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x0000000000000000000000000000000000003dEB\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x00000000000000000000000000000000000057Eb\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x0000000000000000000000000000000000000373\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x00000000000000000000000000000000000002be\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],{\"type\":\"session-permissions\",\"signer\":\"0x0000000000000000000000000000000000001EA6\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],\"blacklistAddress\":\"0x0000000000000000000000000000000000000269\"}")]
        public void AddBlacklistAddress(string inputJson)
        {
            var input = JsonConvert.DeserializeObject<Dictionary<string, object>>(inputJson);
            var blacklistAddress = new Address((string)input["blacklistAddress"]);
            var sessionTopologyJson = input["sessionTopology"].ToString();

            var sessionsTopology = SessionsTopology.FromJson(sessionTopologyJson);
            sessionsTopology.AddToImplicitBlacklist(blacklistAddress);
            
            Debug.Log($"{sessionsTopology.JsonSerialize()}");
        }
        
        [TestCase("{\"sessionTopology\":[[[[{\"type\":\"implicit-blacklist\",\"blacklist\":[\"0x0000000000000000000000000000000000000120\",\"0x0000000000000000000000000000000000000269\",\"0x00000000000000000000000000000000000019c7\",\"0x00000000000000000000000000000000000027D6\",\"0x000000000000000000000000000000000000289C\"]},{\"type\":\"identity-signer\",\"identitySigner\":\"0x8312fc6754389018bdD3BDfEFf226DD8eD9EcdB1\"}],[{\"type\":\"session-permissions\",\"signer\":\"0x00000000000000000000000000000000000025a8\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]},[{\"type\":\"session-permissions\",\"signer\":\"0x00000000000000000000000000000000000009CD\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]},{\"type\":\"session-permissions\",\"signer\":\"0x0000000000000000000000000000000000002AA7\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}]]],[[{\"type\":\"session-permissions\",\"signer\":\"0x00000000000000000000000000000000000026eB\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]},[{\"type\":\"session-permissions\",\"signer\":\"0x0000000000000000000000000000000000001F8C\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]},{\"type\":\"session-permissions\",\"signer\":\"0x000000000000000000000000000000000000435E\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}]],[{\"type\":\"session-permissions\",\"signer\":\"0x000000000000000000000000000000000000026e\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]},[{\"type\":\"session-permissions\",\"signer\":\"0x000000000000000000000000000000000000133b\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]},{\"type\":\"session-permissions\",\"signer\":\"0x0000000000000000000000000000000000002B44\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}]]]],[[[{\"type\":\"session-permissions\",\"signer\":\"0x000000000000000000000000000000000000004C\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]},{\"type\":\"session-permissions\",\"signer\":\"0x0000000000000000000000000000000000002015\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}],[{\"type\":\"session-permissions\",\"signer\":\"0x00000000000000000000000000000000000000E9\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]},[{\"type\":\"session-permissions\",\"signer\":\"0x0000000000000000000000000000000000003Ec4\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]},{\"type\":\"session-permissions\",\"signer\":\"0x00000000000000000000000000000000000002eF\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}]]],[[{\"type\":\"session-permissions\",\"signer\":\"0x0000000000000000000000000000000000003703\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]},[{\"type\":\"session-permissions\",\"signer\":\"0x0000000000000000000000000000000000003dEB\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]},{\"type\":\"session-permissions\",\"signer\":\"0x00000000000000000000000000000000000057Eb\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}]],[{\"type\":\"session-permissions\",\"signer\":\"0x0000000000000000000000000000000000000373\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]},[{\"type\":\"session-permissions\",\"signer\":\"0x00000000000000000000000000000000000002be\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]},{\"type\":\"session-permissions\",\"signer\":\"0x0000000000000000000000000000000000001EA6\",\"valueLimit\":\"0\",\"deadline\":\"0\",\"permissions\":[]}]]]]],\"callSignatures\":[],\"explicitSigners\":[\"0x00000000000000000000000000000000000025a8\"],\"implicitSigners\":[\"0x9ed233eCAE5E093CAff8Ff8E147DdAfc704EC619\"]}")]
        public void EncodeCallSignatures(string inputJson)
        {
            var input = JsonConvert.DeserializeObject<Dictionary<string, object>>(inputJson);
            var sessionTopologyJson = input["sessionTopology"].ToString();
            
            var signatures = JsonConvert.DeserializeObject<string[]>(input["callSignatures"]
                .ToString()).Select(SessionCallSignature.FromJson).ToArray();
            
            var explicitSigners = JsonConvert.DeserializeObject<string[]>(input["explicitSigners"]
                .ToString()).Select(v => new Address(v)).ToArray();
            
            var implicitSigners = JsonConvert.DeserializeObject<string[]>(input["implicitSigners"]
                .ToString()).Select(v => new Address(v)).ToArray();

            var sessionsTopology = SessionsTopology.FromJson(sessionTopologyJson);
            var encodedSignatures = SessionCallSignature.EncodeSignatures(signatures, sessionsTopology, explicitSigners, implicitSigners);
            
            Debug.Log($"{encodedSignatures.ByteArrayToHexStringWithPrefix()}");
        }

        [TestCase("{\"sessionTopology\":[[{\"type\":\"implicit-blacklist\",\"blacklist\":[]},{\"type\":\"identity-signer\",\"identitySigner\":\"0x8312fc6754389018bdD3BDfEFf226DD8eD9EcdB1\"}],[{\"type\":\"session-permissions\",\"signer\":\"0x9ed233eCAE5E093CAff8Ff8E147DdAfc704EC619\",\"valueLimit\":\"1000\",\"deadline\":\"2000\",\"permissions\":[{\"target\":\"0x000000000000000000000000000000000000bEEF\",\"rules\":[{\"cumulative\":false,\"operation\":0,\"value\":\"0x0000000000000000000000000000000000000000000000000000000000000000\",\"offset\":\"0\",\"mask\":\"0x0000000000000000000000000000000000000000000000000000000000000000\"}]}]},{\"type\":\"session-permissions\",\"signer\":\"0xe67ee7a9b12041BdE69ef786fa0431d0d4e59239\",\"valueLimit\":\"1000\",\"deadline\":\"2000\",\"permissions\":[{\"target\":\"0x000000000000000000000000000000000000cafE\",\"rules\":[{\"cumulative\":false,\"operation\":0,\"value\":\"0x0000000000000000000000000000000000000000000000000000000000000000\",\"offset\":\"0\",\"mask\":\"0x0000000000000000000000000000000000000000000000000000000000000000\"}]}]}]]}")]
        public void CreateSessionImageHash(string inputJson)
        {
            var input = JsonConvert.DeserializeObject<Dictionary<string, object>>(inputJson);
            var sessionTopology = SessionsTopology.FromJson(input["sessionTopology"].ToString());
            var imageHash = sessionTopology.ImageHash();
            
            Debug.Log($"{imageHash}");
            
            Assert.AreEqual(imageHash, "0x0a987248daf9d7b5372a694abfa45390e69a102c1e587c0b6ed9670909213be2");
        }
    }
}