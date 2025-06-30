using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;
using Sequence.EcosystemWallet.IntegrationTests;
using Sequence.EcosystemWallet.Primitives;
using Sequence.EcosystemWallet.Utils;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.UnitTests
{
    public class ConfigSignatureTests
    {
        private const string ConfigInput =
            "{\"input\":{\"threshold\":\"2\",\"checkpoint\":\"22850\",\"topology\":{\"type\":\"signer\",\"address\":\"0xD461055c456f50E9A3B6A497C5AA8027c0e3884D\",\"weight\":\"2\"},\"checkpointer\":\"0x0000000000000000000000000000000000001bE9\"},\"signatures\":\"0xD461055c456f50E9A3B6A497C5AA8027c0e3884D:hash:0x13a31d1a2ec622361e3285cc7377bb05ad8a7ee7db48551f9d94e5036a1306de:0x1d615daff8918cd9180a9ac57c6dd590beb8d567b4ad8ecc4ca7b05296895916:27\",\"chainId\":true,\"checkpointerData\":\"0x000000000000000000000000000000000000000000000000000000000001a12a\"}";

        private const string RandomConfigParams =
            "{\"maxDepth\":54,\"seed\":\"3313\",\"minThresholdOnNested\":1,\"skewed\":\"right\"}";
        
        [Test]
        public void TestConfigSignature()
        {
            var parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(ConfigInput);
            var input = parameters["input"].ToString();
            var config = Primitives.Config.FromJson(input);

            var signatures = parameters["signatures"].ToString();
            var noChainId = !parameters.TryGetValue("chainId", out var chainIdValue) || !(bool)chainIdValue;
            var checkpointerData = parameters.TryGetValue("checkpointerData", out var checkpointerDataValue) ? 
                checkpointerDataValue.ToString().HexStringToByteArray() : null;

            var encoded = SignatureUtils.EncodeSignatureFromInput(input, signatures, noChainId, checkpointerData);
            Debug.Log($"Encoded Signature {encoded}");

            var encodedSignature = encoded.HexStringToByteArray();
            var decoded = RawSignature.Decode(encodedSignature);

            Debug.Log($"Decoded Signature {JsonConvert.SerializeObject(decoded)}");

            Assert.AreEqual(config.checkpoint, decoded.configuration.checkpoint);
            Assert.AreEqual(config.threshold, decoded.configuration.threshold);
            Assert.AreEqual(config.checkpointer, decoded.configuration.checkpointer);
        }

        [Test]
        public void TestRecoverRandomConfig()
        {
            var parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(RandomConfigParams);
            
            var maxDepth = int.Parse(parameters["maxDepth"].ToString());
            var seed = (string)parameters["seed"];
            var minThresholdOnNested = int.Parse(parameters["minThresholdOnNested"].ToString());
            var checkpointer = parameters.TryGetValue("checkpointer", out var checkpointerObj) ? checkpointerObj.ToString() : "no";
            var skewed = parameters.TryGetValue("skewed", out var skewedObj) ? skewedObj.ToString() : "none";

            var options = new DevTools.RandomOptions
            {
                seededRandom = string.IsNullOrEmpty(seed) ? null : DevTools.CreateSeededRandom(seed),
                checkpointer = checkpointer,
                minThresholdOnNested = minThresholdOnNested,
                skewed = skewed
            };

            var config = DevTools.CreateRandomConfig(maxDepth, options);
            var signature = new RawSignature
            {
                checkpointerData = null,
                configuration = config,
            };

            var encodedSignature = signature.Encode();
            
            Debug.Log($"Encoded Signature {encodedSignature.ByteArrayToHexStringWithPrefix()}");

            var decodedSignature = RawSignature.Decode(encodedSignature);
            
            Debug.Log($"Decoded Signature: {decodedSignature.ToJson()}");
            
            Assert.AreEqual(
                encodedSignature.ByteArrayToHexStringWithPrefix(), 
                decodedSignature.Encode().ByteArrayToHexStringWithPrefix());
            
            Assert.AreEqual(
                config.HashConfiguration().ByteArrayToHexStringWithPrefix(),
                decodedSignature.configuration.HashConfiguration().ByteArrayToHexStringWithPrefix());
        }
    }
}