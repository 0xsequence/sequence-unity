using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        [Test]
        public void TestConversionToNethereumWithCallsPayload()
        {
            // Arrange
            Address fromWallet = new Address("0xd0B2e0C7b8a2D267733B573Bdc87cC73f551b8A4");
            Chain chain = Chain.LocalChain;
            
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