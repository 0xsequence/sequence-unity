using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using NUnit.Framework;
using Sequence.ABI;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;
using UnityEngine;
using Nethereum.ABI.EIP712;

namespace Sequence.EcosystemWallet.UnitTests
{
    // AI generated, but can be useful for debugging
    public class NethereumConversionTests
    {
        private Address[] _testAddressArray = new Address[]
        {
            new Address("0x04b8b03dba0d960415ce5177accf9d399ed852eb"),
            new Address("0xb4cb75becc7d6c25a8815d3a31d997105772d12b"),
            new Address("0x6aa42b1d0f63dae3a5bf785101c2d2bf26679463")
        };

        [TestCase("0x7b22646f6d61696e223a7b226e616d65223a2253657175656e63652057616c6c6574222c2276657273696f6e223a2233222c22636861696e4964223a3432313631342c22766572696679696e67436f6e7472616374223a22307862643766333862393433343532653063313464376261393262396235303461396339666333353138227d2c227479706573223a7b2243616c6c73223a5b7b226e616d65223a2263616c6c73222c2274797065223a2243616c6c5b5d227d2c7b226e616d65223a227370616365222c2274797065223a2275696e74323536227d2c7b226e616d65223a226e6f6e6365222c2274797065223a2275696e74323536227d2c7b226e616d65223a2277616c6c657473222c2274797065223a22616464726573735b5d227d5d2c2243616c6c223a5b7b226e616d65223a22746f222c2274797065223a2261646472657373227d2c7b226e616d65223a2276616c7565222c2274797065223a2275696e74323536227d2c7b226e616d65223a2264617461222c2274797065223a226279746573227d2c7b226e616d65223a226761734c696d6974222c2274797065223a2275696e74323536227d2c7b226e616d65223a2264656c656761746543616c6c222c2274797065223a22626f6f6c227d2c7b226e616d65223a226f6e6c7946616c6c6261636b222c2274797065223a22626f6f6c227d2c7b226e616d65223a226265686176696f724f6e4572726f72222c2274797065223a2275696e74323536227d5d7d2c227072696d61727954797065223a2243616c6c73222c226d657373616765223a7b227370616365223a2230222c226e6f6e6365223a223139222c2277616c6c657473223a5b5d2c2263616c6c73223a5b7b226761734c696d6974223a302c2264656c656761746543616c6c223a66616c73652c2264617461223a2230783063303334306663222c226f6e6c7946616c6c6261636b223a66616c73652c22746f223a22307833333938356433323038303945323632373461373245303332363863386132393932374263366441222c2276616c7565223a302c226265686176696f724f6e4572726f72223a317d5d7d7d")]
        [TestCase("0x7b22646f6d61696e223a7b226e616d65223a2253657175656e63652057616c6c6574222c2276657273696f6e223a2233222c22636861696e4964223a3432313631342c22766572696679696e67436f6e7472616374223a22307862643766333862393433343532653063313464376261393262396235303461396339666333353138227d2c226d657373616765223a7b2263616c6c73223a5b7b22746f223a22307830303030303030303030434335383831306333334633613044373861413145643830466144634438222c2276616c7565223a2230222c2264617461223a223078343264653134313830303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303230303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030316566333838663933306436663664306163376365383861363962363231653363663062303434643836383164616338613663646239643335303832343936396334306430393763333030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030303030222c226761734c696d6974223a2230222c2264656c656761746543616c6c223a66616c73652c226f6e6c7946616c6c6261636b223a66616c73652c226265686176696f724f6e4572726f72223a2231227d2c7b22746f223a22307844323562333745326642303766383545396563413964343046453342634636304241326463353762222c2276616c7565223a2230222c2264617461223a223078343064303937633330303030303030303030303030303030303030303030303062643766333862393433343532653063313464376261393262396235303461396339666333353138222c226761734c696d6974223a2230222c2264656c656761746543616c6c223a66616c73652c226f6e6c7946616c6c6261636b223a66616c73652c226265686176696f724f6e4572726f72223a2231227d5d2c227370616365223a2230222c226e6f6e6365223a223139222c2277616c6c657473223a5b5d7d2c227072696d61727954797065223a2243616c6c73222c227479706573223a7b2243616c6c73223a5b7b226e616d65223a2263616c6c73222c2274797065223a2243616c6c5b5d227d2c7b226e616d65223a227370616365222c2274797065223a2275696e74323536227d2c7b226e616d65223a226e6f6e6365222c2274797065223a2275696e74323536227d2c7b226e616d65223a2277616c6c657473222c2274797065223a22616464726573735b5d227d5d2c2243616c6c223a5b7b226e616d65223a22746f222c2274797065223a2261646472657373227d2c7b226e616d65223a2276616c7565222c2274797065223a2275696e74323536227d2c7b226e616d65223a2264617461222c2274797065223a226279746573227d2c7b226e616d65223a226761734c696d6974222c2274797065223a2275696e74323536227d2c7b226e616d65223a2264656c656761746543616c6c222c2274797065223a22626f6f6c227d2c7b226e616d65223a226f6e6c7946616c6c6261636b222c2274797065223a22626f6f6c227d2c7b226e616d65223a226265686176696f724f6e4572726f72222c2274797065223a2275696e74323536227d5d7d7d")]
        public void DecodePayload(string payload)
        {
            var json = UTF8Encoding.UTF8.GetString(payload.HexStringToByteArray());
            Debug.Log(json);
        }

        [TestCase("0x061902118a261c1c227f653f26b468615afa440c6f7063760129687799953e775d96612db72c0800ae6fe69bf4a0bc458e843cf638f723a226e589d38a6da244956b941a4cf531d3779b3ff6a0f5a6cdead3578634c1d625a56500004101adb1fd39133e9191cfa66b191fa626bfa19c2abac25221649ec040de3bebad8f64d48e0e9b9d14f55ec267ea5ad22fe4a642a1d111335d2da34721bf39c75e58416062ff00003a30bf3a81c77c768d877d9cb46558300d6bae59d96b273a5bb7bd6d0dfddb574e5b650000151118002fc09def9a47437cc64e270843de094f598430ec392c0ac580a8ce28adb765a25fabb17ccc651b7c73f8f14b2d7fb63d2f8bed")]
        [TestCase("0x060c0201b3dbccd2784bf59c7556334ae64e218ee26d33f74d45c2fcac36f6cd40928b6e708898cd5cecd9955c2e519c8016bdb027e0af3f127743dd65f72eaa0dd06f3111301d73c0b825fad1c357f71deb9a9fb25022c33f65000041010561398216aabd75798fc5c816197228bbc044c8f008472fbe5545e018da185f82770de0730e6aed7dfcf9fec27c3c9f42c937009954184c8eba813bf21d0e26416062ff00003a30c709bf58e602374f1b3ee95a065b9412ca01d93a5916409e377c3ed3316e8b6b650000151118002fc09def9a47437cc64e270843de094f5984303e54bf05a9620f3d8416a229c9fd457f7c07db8282d71015599cccc4d43df683")]
        public void DecodeSignatureFromConfigUpdate(string signature)
        {
            var sig = RawSignature.Decode(signature.HexStringToByteArray());
            Debug.Log(sig.Encode().ByteArrayToHexString());
        }
        
        [Test]
        public void TestConversionToNethereumWithCallsPayload()
        {
            // Arrange
            Address fromWallet = new Address("0xd0B2e0C7b8a2D267733B573Bdc87cC73f551b8A4");
            BigInteger chain = new BigInteger((int)Chain.LocalChain);
            
            Parented parented = new Parented(_testAddressArray,
                new Calls(BigInteger.Parse("1266736520029721018202413622017"), BigInteger.Parse("55846603721928660"),
                    new Call[]
                    {
                        new Call(new Address("0x6810c263f45be5dc8e8e6ffd2ab9bd6f152412ed"),
                            BigInteger.Parse("19035696402805033763977015876"),
                            "0xf51267fb7a1ff020a04971203a2f57eb5ab06b45dcf3e824145375ab46af51334f823837f90c2d459f147b3fb3ea6e07a4af02201f85aa995aec66e632"
                                .HexStringToByteArray(), BigInteger.Parse("29"), false, true, BehaviourOnError.abort),
                        new Call(new Address("0x7fa9385be102ac3eac297483dd6233d62b3e1496"), BigInteger.Parse("0"),
                            "0x001122".HexStringToByteArray(), BigInteger.Parse("1000000"), false, false,
                            BehaviourOnError.ignore),
                    }));

            TypedDataToSign typedData = new TypedDataToSign(fromWallet, chain, parented);

            var convertedWithoutSalt = CallConvertToNethereumTypedDataWithoutSalt(typedData);
            
            Assert.IsNotNull(convertedWithoutSalt);
            Assert.AreEqual("Calls", convertedWithoutSalt.PrimaryType);
            Assert.IsNotNull(convertedWithoutSalt.Types);
            Assert.IsNotNull(convertedWithoutSalt.Message);
            Assert.IsTrue(convertedWithoutSalt.Types.ContainsKey("EIP712Domain"));
            Assert.IsTrue(convertedWithoutSalt.Types.ContainsKey("Calls"));
            Assert.IsTrue(convertedWithoutSalt.Types.ContainsKey("Call"));
            
            // Validate message structure thoroughly
            var message = convertedWithoutSalt.Message;
            Assert.AreEqual(4, message.Length, "Calls message should have exactly 4 fields");
            
            // Find each field by TypeName and validate
            var callsField = Array.Find(message, m => m.TypeName == "Call[]");
            Assert.IsNotNull(callsField, "Should have Call[] field");
            Assert.IsInstanceOf<MemberValue[][]>(callsField.Value, "Call[] value should be MemberValue[][]");
            var callsArray = (MemberValue[][])callsField.Value;
            Assert.AreEqual(2, callsArray.Length, "Should have 2 calls");
            
            // Validate first call structure
            var firstCall = callsArray[0];
            Assert.AreEqual(7, firstCall.Length, "Call should have exactly 7 fields");
            
            // Validate call field structure (each field is a MemberValue)
            var toField = Array.Find(firstCall, f => f.TypeName == "address");
            Assert.IsNotNull(toField, "Call should have 'to' field with address type");
            Assert.IsInstanceOf<string>(toField.Value, "Call 'to' should be string");
            
            var dataField = Array.Find(firstCall, f => f.TypeName == "bytes");
            Assert.IsNotNull(dataField, "Call should have 'data' field with bytes type");
            Assert.IsInstanceOf<byte[]>(dataField.Value, "Call 'data' should be byte[]");
            
            // Validate counts of field types (3 uint256, 2 bool, 1 address, 1 bytes)
            Assert.AreEqual(1, firstCall.Count(f => f.TypeName == "address"), "Call should have exactly 1 address field");
            Assert.AreEqual(1, firstCall.Count(f => f.TypeName == "bytes"), "Call should have exactly 1 bytes field");
            Assert.AreEqual(3, firstCall.Count(f => f.TypeName == "uint256"), "Call should have exactly 3 uint256 fields");
            Assert.AreEqual(2, firstCall.Count(f => f.TypeName == "bool"), "Call should have exactly 2 bool fields");
            
            // Validate uint256 fields have BigInteger values
            var uint256Fields = firstCall.Where(f => f.TypeName == "uint256").ToArray();
            foreach (var field in uint256Fields)
            {
                Assert.IsInstanceOf<BigInteger>(field.Value, "All uint256 fields should have BigInteger values");
            }
            
            // Validate bool fields have bool values
            var boolFields = firstCall.Where(f => f.TypeName == "bool").ToArray();
            foreach (var field in boolFields)
            {
                Assert.IsInstanceOf<bool>(field.Value, "All bool fields should have bool values");
            }
            
            var gasLimitField = Array.Find(firstCall, f => f.TypeName == "uint256" && IsCallGasLimitField(firstCall, f));
            Assert.IsNotNull(gasLimitField, "Call should have 'gasLimit' field with uint256 type");
            Assert.IsInstanceOf<BigInteger>(gasLimitField.Value, "Call 'gasLimit' should be BigInteger");
            
            var delegateCallField = Array.Find(firstCall, f => f.TypeName == "bool" && IsCallDelegateCallField(firstCall, f));
            Assert.IsNotNull(delegateCallField, "Call should have 'delegateCall' field with bool type");
            Assert.IsInstanceOf<bool>(delegateCallField.Value, "Call 'delegateCall' should be bool");
            
            var onlyFallbackField = Array.Find(firstCall, f => f.TypeName == "bool" && IsCallOnlyFallbackField(firstCall, f));
            Assert.IsNotNull(onlyFallbackField, "Call should have 'onlyFallback' field with bool type");
            Assert.IsInstanceOf<bool>(onlyFallbackField.Value, "Call 'onlyFallback' should be bool");
            
            var behaviorOnErrorField = Array.Find(firstCall, f => f.TypeName == "uint256" && IsCallBehaviorOnErrorField(firstCall, f));
            Assert.IsNotNull(behaviorOnErrorField, "Call should have 'behaviorOnError' field with uint256 type");
            Assert.IsInstanceOf<BigInteger>(behaviorOnErrorField.Value, "Call 'behaviorOnError' should be BigInteger");
            
            var spaceField = Array.Find(message, m => m.TypeName == "uint256" && IsSpaceField(message, m));
            Assert.IsNotNull(spaceField, "Should have space field with uint256 type");
            Assert.IsInstanceOf<BigInteger>(spaceField.Value, "Space value should be BigInteger");
            
            var nonceField = Array.Find(message, m => m.TypeName == "uint256" && IsNonceField(message, m));
            Assert.IsNotNull(nonceField, "Should have nonce field with uint256 type");
            Assert.IsInstanceOf<BigInteger>(nonceField.Value, "Nonce value should be BigInteger");
            
            var walletsField = Array.Find(message, m => m.TypeName == "address[]");
            Assert.IsNotNull(walletsField, "Should have wallets field with address[] type");
            Assert.IsInstanceOf<string[]>(walletsField.Value, "Wallets value should be string[]");
            var walletsArray = (string[])walletsField.Value;
            Assert.AreEqual(3, walletsArray.Length, "Should have 3 wallet addresses");
            
            // Log detailed information
            Debug.Log($"Message validation passed. Fields found:");
            foreach (var member in message)
            {
                Debug.Log($"  Field: TypeName={member.TypeName}, Value type={member.Value?.GetType()}, Value={member.Value}");
            }
        }

        [Test]
        public void TestConversionToNethereumWithMessagePayload()
        {
            // Arrange
            Address fromWallet = new Address("0x5615dEB798BB3E4dFa0139dFa1b3D433Cc23b72f");
            Chain chain = Chain.LocalChain;
            
            Parented parented = new Parented(_testAddressArray,
                new Message(new byte[]
                {
                    194, 99, 244, 91, 229, 220, 142, 142, 111,
                    253, 42, 185, 189, 111, 21, 36, 18, 237,
                    182, 97, 17, 182, 245, 110, 57, 164, 44,
                    105, 68, 5, 129, 240, 232, 111, 245, 18,
                    103, 251, 122, 31, 240, 32, 160, 73
                }));

            TypedDataToSign typedData = new TypedDataToSign(fromWallet, chain, parented);

            // Act & Assert
            var convertedWithoutSalt = CallConvertToNethereumTypedDataWithoutSalt(typedData);
            Assert.IsNotNull(convertedWithoutSalt);
            Assert.AreEqual("Message", convertedWithoutSalt.PrimaryType);
            Assert.IsNotNull(convertedWithoutSalt.Types);
            Assert.IsNotNull(convertedWithoutSalt.Message);
            Assert.IsTrue(convertedWithoutSalt.Types.ContainsKey("EIP712Domain"));
            Assert.IsTrue(convertedWithoutSalt.Types.ContainsKey("Message"));
            
            // Validate message structure thoroughly
            var message = convertedWithoutSalt.Message;
            Assert.AreEqual(2, message.Length, "Message payload should have exactly 2 fields");
            
            // Find and validate message field
            var messageField = Array.Find(message, m => m.TypeName == "bytes");
            Assert.IsNotNull(messageField, "Should have message field with bytes type");
            Assert.IsInstanceOf<string>(messageField.Value, "Message value should be hex string");
            var messageHex = (string)messageField.Value;
            Assert.IsTrue(messageHex.StartsWith("0x"), "Message should be hex string with 0x prefix");
            
            // Find and validate wallets field
            var walletsField = Array.Find(message, m => m.TypeName == "address[]");
            Assert.IsNotNull(walletsField, "Should have wallets field with address[] type");
            Assert.IsInstanceOf<string[]>(walletsField.Value, "Wallets value should be string[]");
            var walletsArray = (string[])walletsField.Value;
            Assert.AreEqual(3, walletsArray.Length, "Should have 3 wallet addresses");
            
            // Log detailed information
            Debug.Log($"Message payload validation passed. Fields found:");
            foreach (var member in message)
            {
                Debug.Log($"  Field: TypeName={member.TypeName}, Value type={member.Value?.GetType()}, Value={member.Value}");
            }
        }

        [Test]
        public void TestConversionToNethereumWithConfigUpdatePayload()
        {
            // Arrange
            Address fromWallet = new Address("0x5615dEB798BB3E4dFa0139dFa1b3D433Cc23b72f");
            Chain chain = Chain.LocalChain;
            
            Parented parented = new Parented(_testAddressArray,
                new ConfigUpdate("0x6810c263f45be5dc8e8e6ffd2ab9bd6f152412edb66111b6f56e39a42c694405"));

            TypedDataToSign typedData = new TypedDataToSign(fromWallet, chain, parented);

            // Act & Assert
            var convertedWithoutSalt = CallConvertToNethereumTypedDataWithoutSalt(typedData);
            Assert.IsNotNull(convertedWithoutSalt);
            Assert.AreEqual("ConfigUpdate", convertedWithoutSalt.PrimaryType);
            Assert.IsNotNull(convertedWithoutSalt.Types);
            Assert.IsNotNull(convertedWithoutSalt.Message);
            Assert.IsTrue(convertedWithoutSalt.Types.ContainsKey("EIP712Domain"));
            Assert.IsTrue(convertedWithoutSalt.Types.ContainsKey("ConfigUpdate"));
            
            // Validate message structure thoroughly
            var message = convertedWithoutSalt.Message;
            Assert.AreEqual(2, message.Length, "ConfigUpdate payload should have exactly 2 fields");
            
            // Find and validate imageHash field
            var imageHashField = Array.Find(message, m => m.TypeName == "bytes32");
            Assert.IsNotNull(imageHashField, "Should have imageHash field with bytes32 type");
            Assert.IsInstanceOf<byte[]>(imageHashField.Value, "ImageHash value should be byte[]");
            var imageHashBytes = (byte[])imageHashField.Value;
            Assert.AreEqual(32, imageHashBytes.Length, "ImageHash should be exactly 32 bytes");
            
            // Find and validate wallets field
            var walletsField = Array.Find(message, m => m.TypeName == "address[]");
            Assert.IsNotNull(walletsField, "Should have wallets field with address[] type");
            Assert.IsInstanceOf<string[]>(walletsField.Value, "Wallets value should be string[]");
            var walletsArray = (string[])walletsField.Value;
            Assert.AreEqual(3, walletsArray.Length, "Should have 3 wallet addresses");
            
            // Log detailed information
            Debug.Log($"ConfigUpdate payload validation passed. Fields found:");
            foreach (var member in message)
            {
                Debug.Log($"  Field: TypeName={member.TypeName}, Value type={member.Value?.GetType()}, Value={member.Value}");
            }
        }

        [Test]
        public void TestConversionToNethereumWithSalt()
        {
            // Arrange
            Address fromWallet = new Address("0x5615dEB798BB3E4dFa0139dFa1b3D433Cc23b72f");
            Chain chain = Chain.LocalChain;
            
            // Create a domain with salt
            var saltData = new byte[32];
            for (int i = 0; i < 32; i++) saltData[i] = (byte)i; // Fill with test data
            var salt = new FixedByte(32, saltData);
            
            var domain = new Sequence.EcosystemWallet.Primitives.Domain("Sequence Wallet", "3", chain, fromWallet, salt);
            var types = new Dictionary<string, NamedType[]>
            {
                ["ConfigUpdate"] = new[]
                {
                    new NamedType("imageHash", "bytes32"),
                    new NamedType("wallets", "address[]")
                }
            };
            var message = new Dictionary<string, object>
            {
                ["imageHash"] = "0x6810c263f45be5dc8e8e6ffd2ab9bd6f152412edb66111b6f56e39a42c694405",
                ["wallets"] = _testAddressArray
            };

            TypedDataToSign typedData = new TypedDataToSign(domain, types, "ConfigUpdate", message);

            // Act & Assert
            var convertedWithSalt = CallConvertToNethereumTypedDataWithSalt(typedData);
            Assert.IsNotNull(convertedWithSalt);
            Assert.AreEqual("ConfigUpdate", convertedWithSalt.PrimaryType);
            Assert.IsNotNull(convertedWithSalt.Types);
            Assert.IsNotNull(convertedWithSalt.Message);
            Assert.IsTrue(convertedWithSalt.Types.ContainsKey("EIP712Domain"));
            Assert.IsTrue(convertedWithSalt.Types.ContainsKey("ConfigUpdate"));
            Assert.IsNotNull(convertedWithSalt.Domain.Salt);
            
            // Validate message structure thoroughly
            var messageData = convertedWithSalt.Message;
            Assert.AreEqual(2, messageData.Length, "ConfigUpdate payload should have exactly 2 fields");
            
            // Find and validate imageHash field
            var imageHashField = Array.Find(messageData, m => m.TypeName == "bytes32");
            Assert.IsNotNull(imageHashField, "Should have imageHash field with bytes32 type");
            Assert.IsInstanceOf<byte[]>(imageHashField.Value, "ImageHash value should be byte[]");
            var imageHashBytes = (byte[])imageHashField.Value;
            Assert.AreEqual(32, imageHashBytes.Length, "ImageHash should be exactly 32 bytes");
            
            // Find and validate wallets field
            var walletsField = Array.Find(messageData, m => m.TypeName == "address[]");
            Assert.IsNotNull(walletsField, "Should have wallets field with address[] type");
            Assert.IsInstanceOf<string[]>(walletsField.Value, "Wallets value should be string[]");
            var walletsArray = (string[])walletsField.Value;
            Assert.AreEqual(3, walletsArray.Length, "Should have 3 wallet addresses");
            
            // Validate domain salt
            Assert.AreEqual(32, convertedWithSalt.Domain.Salt.Length, "Domain salt should be exactly 32 bytes");
            
            // Log detailed information
            Debug.Log($"ConfigUpdate with salt payload validation passed. Fields found:");
            foreach (var member in messageData)
            {
                Debug.Log($"  Field: TypeName={member.TypeName}, Value type={member.Value?.GetType()}, Value={member.Value}");
            }
        }

        [Test]
        public void TestMessageConversionForCallsPayload()
        {
            // Arrange
            Address fromWallet = new Address("0xd0B2e0C7b8a2D267733B573Bdc87cC73f551b8A4");
            Chain chain = Chain.LocalChain;
            
            Parented parented = new Parented(_testAddressArray,
                new Calls(BigInteger.Parse("100"), BigInteger.Parse("200"),
                    new Call[]
                    {
                        new Call(new Address("0x1234567890123456789012345678901234567890"),
                            BigInteger.Parse("1000"), "0x1234".HexStringToByteArray(), 
                            BigInteger.Parse("50000"), false, false, BehaviourOnError.ignore)
                    }));

            TypedDataToSign typedData = new TypedDataToSign(fromWallet, chain, parented);

            // Act & Assert
            var convertedMessage = CallConvertMessageForNethereum(typedData);
            Assert.IsNotNull(convertedMessage);
            Assert.Greater(convertedMessage.Length, 0);
            
            // Verify we have the expected fields for Calls
            Assert.IsTrue(Array.Exists(convertedMessage, m => m.TypeName == "Call[]"));
            Assert.IsTrue(Array.Exists(convertedMessage, m => m.TypeName == "uint256"));
            Assert.IsTrue(Array.Exists(convertedMessage, m => m.TypeName == "address[]"));
            
            // Validate message structure thoroughly
            Assert.AreEqual(4, convertedMessage.Length, "Calls message should have exactly 4 fields");
            
            // Find and validate calls field
            var callsField = Array.Find(convertedMessage, m => m.TypeName == "Call[]");
            Assert.IsNotNull(callsField, "Should have Call[] field");
            Assert.IsInstanceOf<MemberValue[][]>(callsField.Value, "Call[] value should be MemberValue[][]");
            var callsArray = (MemberValue[][])callsField.Value;
            Assert.AreEqual(1, callsArray.Length, "Should have 1 call");
            
            // Validate call structure
            var call = callsArray[0];
            Assert.AreEqual(7, call.Length, "Call should have exactly 7 fields");
            
            // Validate call has all required field types
            Assert.IsTrue(Array.Exists(call, f => f.TypeName == "address"), "Call should have address field");
            Assert.IsTrue(Array.Exists(call, f => f.TypeName == "bytes"), "Call should have bytes field");
            Assert.AreEqual(3, call.Count(f => f.TypeName == "uint256"), "Call should have exactly 3 uint256 fields");
            Assert.AreEqual(2, call.Count(f => f.TypeName == "bool"), "Call should have exactly 2 bool fields");
            
            Debug.Log($"Message conversion validation passed. Converted message has {convertedMessage.Length} members");
            foreach (var member in convertedMessage)
            {
                Debug.Log($"  Field: TypeName={member.TypeName}, Value type={member.Value?.GetType()}, Value={member.Value}");
            }
        }
        
        // Helper methods using the converter class directly
        private TypedData<Nethereum.ABI.EIP712.Domain> CallConvertToNethereumTypedDataWithoutSalt(TypedDataToSign instance)
        {
            var converter = new ConvertTypedDataToSignToNethereum(instance);
            return converter.ConvertToNethereumTypedDataWithoutSalt();
        }
        
        private TypedData<Nethereum.ABI.EIP712.DomainWithSalt> CallConvertToNethereumTypedDataWithSalt(TypedDataToSign instance)
        {
            var converter = new ConvertTypedDataToSignToNethereum(instance);
            return converter.ConvertToNethereumTypedDataWithSalt();
        }
        
        private MemberValue[] CallConvertMessageForNethereum(TypedDataToSign instance)
        {
            var converter = new ConvertTypedDataToSignToNethereum(instance);
            return converter.ConvertMessageForNethereum();
        }
        
        // Helper methods to identify space and nonce fields since they have the same type
        private bool IsSpaceField(MemberValue[] message, MemberValue field)
        {
            // Space should be the first uint256 field in the Calls struct order
            var uint256Fields = message.Where(m => m.TypeName == "uint256").ToArray();
            return uint256Fields.Length >= 1 && uint256Fields[0] == field;
        }
        
        private bool IsNonceField(MemberValue[] message, MemberValue field)
        {
            // Nonce should be the second uint256 field in the Calls struct order
            var uint256Fields = message.Where(m => m.TypeName == "uint256").ToArray();
            return uint256Fields.Length >= 2 && uint256Fields[1] == field;
        }

        private bool IsCallValueField(MemberValue[] call, MemberValue field)
        {
            return field.TypeName == "uint256" &&
                   call.Any(f => f.TypeName == "uint256" && f.Value.Equals(field.Value));
        }

        private bool IsCallGasLimitField(MemberValue[] call, MemberValue field)
        {
            return field.TypeName == "uint256" &&
                   call.Any(f => f.TypeName == "uint256" && f.Value.Equals(field.Value));
        }

        private bool IsCallDelegateCallField(MemberValue[] call, MemberValue field)
        {
            return field.TypeName == "bool" &&
                   call.Any(f => f.TypeName == "bool" && f.Value.Equals(field.Value));
        }

        private bool IsCallOnlyFallbackField(MemberValue[] call, MemberValue field)
        {
            return field.TypeName == "bool" &&
                   call.Any(f => f.TypeName == "bool" && f.Value.Equals(field.Value));
        }

        private bool IsCallBehaviorOnErrorField(MemberValue[] call, MemberValue field)
        {
            return field.TypeName == "uint256" &&
                   call.Any(f => f.TypeName == "uint256" && f.Value.Equals(field.Value));
        }
    }
} 