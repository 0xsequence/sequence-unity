using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Scripting;
using Nethereum.ABI.EIP712;
using Nethereum.Util;

namespace Sequence.EcosystemWallet.Primitives
{
    public class TypedDataToSign
    {
        public Domain domain;
        public Dictionary<string, NamedType[]> types;
        public string primaryType;
        public Dictionary<string, object> message;
        
        private string[] _types;
        private Parented _payload;
        
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is TypedDataToSign))
            {
                return false;
            }
            
            TypedDataToSign other = (TypedDataToSign)obj;
            if (!other.domain.Equals(domain))
            {
                return false;
            }

            if (types.GetKeys().Length.Equals(other.types.GetKeys().Length))
            {
                foreach (var key in types.GetKeys())
                {
                    NamedType[] namedTypes = types[key];
                    NamedType[] otherNamedTypes;
                    if (other.types.TryGetValue(key, out otherNamedTypes))
                    {
                        int length = namedTypes.Length;
                        if (length != otherNamedTypes.Length)
                        {
                            return false;
                        }

                        for (int i = 0; i < length; i++)
                        {
                            if (!namedTypes[i].Equals(otherNamedTypes[i]))
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            
            if (!primaryType.Equals(other.primaryType))
            {
                return false;
            }

            if (message.GetKeys().Length.Equals(other.message.GetKeys().Length))
            {
                foreach (var key in message.GetKeys())
                {
                    object value = message[key];
                    object otherValue;
                    if (other.message.TryGetValue(key, out otherValue))
                    {
                        if (value is IEnumerable)
                        {
                            if (!(otherValue is IEnumerable))
                            {
                                return false;
                            }
                            
                            IEnumerator valueEnumerator = ((IEnumerable)value).GetEnumerator();
                            IEnumerator otherValueEnumerator = ((IEnumerable)otherValue).GetEnumerator();
                            
                            while (valueEnumerator.MoveNext() && otherValueEnumerator.MoveNext())
                            {
                                if (valueEnumerator.Current != null && !valueEnumerator.Current.Equals(otherValueEnumerator.Current))
                                {
                                    return false;
                                }
                                if (valueEnumerator.Current == null && otherValueEnumerator.Current != null)
                                {
                                    return false;
                                }
                            }
                            
                            if (valueEnumerator.MoveNext() || otherValueEnumerator.MoveNext())
                            {
                                return false;
                            }
                            continue;
                        }
                        if (!value.Equals(otherValue))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            
            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("TypedDataToSign {");
            sb.AppendLine($"  PrimaryType: {primaryType}");
            
            // Append domain information
            sb.AppendLine($"  Domain: {domain}");
            
            // Append types
            sb.AppendLine("  Types: {");
            foreach (var typeEntry in types)
            {
                sb.AppendLine($"    {typeEntry.Key}: [");
                foreach (var namedType in typeEntry.Value)
                {
                    sb.AppendLine($"      {namedType}");
                }
                sb.AppendLine("    ]");
            }
            sb.AppendLine("  }");
            
            // Append message
            sb.AppendLine("  Message: {");
            foreach (var msgEntry in message)
            {
                if (msgEntry.Value is byte[])
                {
                    // Convert byte arrays to hex string
                    byte[] byteArray = (byte[])msgEntry.Value;
                    string hexValue = byteArray.ByteArrayToHexStringWithPrefix();
                    sb.AppendLine($"    {msgEntry.Key}: {hexValue}");
                }
                else if (msgEntry.Value is IEnumerable && !(msgEntry.Value is string))
                {
                    sb.AppendLine($"    {msgEntry.Key}: [");
                    foreach (var item in (IEnumerable)msgEntry.Value)
                    {
                        if (item is byte[])
                        {
                            // Convert byte arrays to hex string
                            byte[] byteArray = (byte[])item;
                            string hexValue = byteArray.ByteArrayToHexStringWithPrefix();
                            sb.AppendLine($"      {hexValue}");
                        }
                        else
                        {
                            sb.AppendLine($"      {item}");
                        }
                    }
                    sb.AppendLine("    ]");
                }
                else
                {
                    sb.AppendLine($"    {msgEntry.Key}: {msgEntry.Value}");
                }
            }
            sb.AppendLine("  }");
            
            // Append payload information if available
            if (_payload != null)
            {
                sb.AppendLine($"  Payload: {_payload}");
            }
            
            sb.AppendLine("}");
            
            return sb.ToString();
        }

        [Preserve]
        [JsonConstructor]
        public TypedDataToSign(Domain domain, Dictionary<string, NamedType[]> types, string primaryType, Dictionary<string, object> message)
        {
            this.domain = domain;
            this.types = types;
            this.primaryType = primaryType;
            this.message = message;
            if (!types.ContainsKey(primaryType) || !types.ContainsKey(primaryType))
            {
                throw new ArgumentException(
                    $"{nameof(primaryType)} {primaryType} not found in {nameof(types)}. Please check the provided types.");
            }
        }

        // Todo: once we have tests working, let's refactor this so it isn't one giant function
        public TypedDataToSign(Address wallet, Chain chain, Parented payload)
        {
            _payload = payload;
            this.domain = new Domain("Sequence Wallet", "3", chain, wallet);
            switch (payload.payload.type)
            {
                case PayloadType.Call:
                {
                    types = new Dictionary<string, NamedType[]>()
                    {
                        ["Calls"] = new[]
                        {
                            new NamedType("calls", "Call[]"),
                            new NamedType("space", "uint256"),
                            new NamedType("nonce", "uint256"),
                            new NamedType("wallets", "address[]"),
                        },
                        ["Call"] = new[]
                        {
                            new NamedType("to", "address"),
                            new NamedType("value", "uint256"),
                            new NamedType("data", "bytes"),
                            new NamedType("gasLimit", "uint256"),
                            new NamedType("delegateCall", "bool"),
                            new NamedType("onlyFallback", "bool"),
                            new NamedType("behaviorOnError", "uint256"),
                        }
                    };

                    // We ensure 'behaviorOnError' is turned into a numeric value
                    Calls callPayload = (Calls)payload.payload;
                    message = new Dictionary<string, object>
                    {
                        ["space"] = callPayload.space.ToString(),
                        ["nonce"] = callPayload.nonce.ToString(),
                        ["wallets"] = payload.parentWallets
                    };
                    int callsLength = callPayload.calls.Length;
                    EncodeSapient.EncodedCall[] encoded = new EncodeSapient.EncodedCall[callsLength];
                    for (int i = 0; i < callsLength; i++)
                    {
                        Call call = callPayload.calls[i];
                        encoded[i] = new EncodeSapient.EncodedCall(call);
                    }
                    message.Add("calls", encoded);
                    primaryType = "Calls";
                    break;
                }
                case PayloadType.Message:
                {
                    types = new Dictionary<string, NamedType[]>
                    {
                        ["Message"] = new[]
                        {
                            new NamedType("message", "bytes"),
                            new NamedType("wallets", "address[]"),
                        }
                    };

                    var messagePayload = (Message)payload.payload;
                    message = new Dictionary<string, object>
                    {
                        ["message"] = messagePayload.message.ByteArrayToHexStringWithPrefix(),
                        ["wallets"] = payload.parentWallets
                    };
                    primaryType = "Message";
                    break;
                }
                case PayloadType.ConfigUpdate:
                {
                    types = new Dictionary<string, NamedType[]>
                    {
                        ["ConfigUpdate"] = new[]
                        {
                            new NamedType("imageHash", "bytes32"),
                            new NamedType("wallets", "address[]"),
                        }
                    };

                    var configPayload = (ConfigUpdate)payload.payload;
                    message = new Dictionary<string, object>
                    {
                        ["imageHash"] = configPayload.imageHash,
                        ["wallets"] = payload.parentWallets
                    };
                    primaryType = "ConfigUpdate";
                    break;
                }
                case PayloadType.Digest:
                    throw new ArgumentException(
                        $"{nameof(PayloadType.Digest)} is not supported, use {nameof(PayloadType.Message)} instead");
                default:
                    throw new SystemException($"Encountered unexpected {nameof(PayloadType)}: {payload.payload.type}");
            }
        }

        // Todo clean up this AI slop
        // Todo replace Nethereum
        public byte[] GetSignPayload()
        {
            try
            {
                // Use Nethereum's EIP-712 encoder
                var eip712TypedDataEncoder = new Eip712TypedDataEncoder();
                byte[] encodedTypedData;
                
                // Check if we have salt data to determine which domain type to use
                if (domain.salt?.Data != null && domain.salt.Data.Length > 0)
                {
                    // Convert to Nethereum's TypedData format with salt
                    var nethereumTypedDataWithSalt = ConvertToNethereumTypedDataWithSalt();
                    encodedTypedData = eip712TypedDataEncoder.EncodeTypedData(nethereumTypedDataWithSalt);
                }
                else
                {
                    // Convert to Nethereum's TypedData format without salt
                    var nethereumTypedData = ConvertToNethereumTypedDataWithoutSalt();
                    encodedTypedData = eip712TypedDataEncoder.EncodeTypedData(nethereumTypedData);
                }
                
                Debug.Log($"EIP-712 encoded payload: {encodedTypedData.ByteArrayToHexStringWithPrefix()}");
                
                return SequenceCoder.KeccakHash(encodedTypedData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error encoding EIP-712 data: {ex.Message}");
                throw;
            }
        }
        
        private TypedData<Nethereum.ABI.EIP712.DomainWithSalt> ConvertToNethereumTypedDataWithSalt()
        {
            // Convert domain using Nethereum's DomainWithSalt
            var nethereumDomain = new Nethereum.ABI.EIP712.DomainWithSalt()
            {
                Name = domain.name,
                Version = domain.version,
                ChainId = domain.chainId,
                VerifyingContract = domain.verifyingContract,
                Salt = domain.salt.Data
            };
            
            // Convert types to Nethereum format
            var nethereumTypes = new Dictionary<string, MemberDescription[]>();
            
            // Add the required EIP712Domain type definition with salt
            var domainMembers = new List<MemberDescription>
            {
                new MemberDescription { Name = "name", Type = "string" },
                new MemberDescription { Name = "version", Type = "string" },
                new MemberDescription { Name = "chainId", Type = "uint256" },
                new MemberDescription { Name = "verifyingContract", Type = "address" },
                new MemberDescription { Name = "salt", Type = "bytes32" }
            };
            
            nethereumTypes["EIP712Domain"] = domainMembers.ToArray();
            
            foreach (var typeEntry in types)
            {
                var members = new MemberDescription[typeEntry.Value.Length];
                for (int i = 0; i < typeEntry.Value.Length; i++)
                {
                    members[i] = new MemberDescription
                    {
                        Name = typeEntry.Value[i].name,
                        Type = typeEntry.Value[i].type
                    };
                }
                nethereumTypes[typeEntry.Key] = members;
            }
            
            // Convert message to proper format
            var convertedMessage = ConvertMessageForNethereum();
            
            return new TypedData<Nethereum.ABI.EIP712.DomainWithSalt>
            {
                Domain = nethereumDomain,
                Types = nethereumTypes,
                PrimaryType = primaryType,
                Message = convertedMessage
            };
        }
        
        private TypedData<Nethereum.ABI.EIP712.Domain> ConvertToNethereumTypedDataWithoutSalt()
        {
            // Convert domain using Nethereum's Domain (without salt)
            var nethereumDomain = new Nethereum.ABI.EIP712.Domain()
            {
                Name = domain.name,
                Version = domain.version,
                ChainId = (int)domain.chainId,
                VerifyingContract = domain.verifyingContract.ToString()
            };
            
            // Convert types to Nethereum format
            var nethereumTypes = new Dictionary<string, MemberDescription[]>();
            
            // Add the required EIP712Domain type definition without salt
            var domainMembers = new List<MemberDescription>
            {
                new MemberDescription { Name = "name", Type = "string" },
                new MemberDescription { Name = "version", Type = "string" },
                new MemberDescription { Name = "chainId", Type = "uint256" },
                new MemberDescription { Name = "verifyingContract", Type = "address" }
            };
            
            nethereumTypes["EIP712Domain"] = domainMembers.ToArray();
            
            foreach (var typeEntry in types)
            {
                var members = new MemberDescription[typeEntry.Value.Length];
                for (int i = 0; i < typeEntry.Value.Length; i++)
                {
                    members[i] = new MemberDescription
                    {
                        Name = typeEntry.Value[i].name,
                        Type = typeEntry.Value[i].type
                    };
                }
                nethereumTypes[typeEntry.Key] = members;
            }
            
            // Convert message to proper format
            var convertedMessage = ConvertMessageForNethereum();
            
            return new TypedData<Nethereum.ABI.EIP712.Domain>
            {
                Domain = nethereumDomain,
                Types = nethereumTypes,
                PrimaryType = primaryType,
                Message = convertedMessage
            };
        }
        
        private MemberValue[] ConvertMessageForNethereum()
        {
            var memberValues = new List<MemberValue>();
            
            // Get the primary type definition to ensure correct ordering
            if (!types.TryGetValue(primaryType, out NamedType[] namedTypes))
            {
                Debug.LogError($"Primary type '{primaryType}' not found in types dictionary");
                return memberValues.ToArray();
            }
            
            // Process fields in the order they appear in the type definition
            foreach (var namedType in namedTypes)
            {
                if (!message.TryGetValue(namedType.name, out object value))
                {
                    Debug.LogError($"Field '{namedType.name}' not found in message for type '{primaryType}'");
                    continue;
                }
                
                object convertedValue;
                
                // Skip null values for bytes32 and other non-nullable types
                if (value == null)
                {
                    if (namedType.type == "bytes32" || namedType.type == "address" || namedType.type.StartsWith("uint"))
                    {
                        Debug.LogError($"Null value found for non-nullable type '{namedType.type}' in field '{namedType.name}'");
                        continue;
                    }
                }
                
                Debug.Log($"Converting field '{namedType.name}' of type '{namedType.type}' with value: {value}");
                
                // Handle different value types for Nethereum compatibility
                if (value is string strValue)
                {
                    // Check if it's a hex string that should be converted to BigInteger
                    if (strValue.StartsWith("0x"))
                    {
                        // Convert based on the specific type
                        if (namedType.type == "uint256")
                        {
                            convertedValue = BigInteger.Parse(strValue.Substring(2), System.Globalization.NumberStyles.HexNumber);
                        }
                        else if (namedType.type == "bytes32")
                        {
                            // Convert bytes32 hex string to byte array
                            convertedValue = strValue.HexStringToByteArray();
                        }
                        else
                        {
                            // Keep as hex string for other types (address, bytes)
                            convertedValue = strValue;
                        }
                    }
                    else if (BigInteger.TryParse(strValue, out BigInteger bigIntValue))
                    {
                        convertedValue = bigIntValue;
                    }
                    else
                    {
                        convertedValue = strValue;
                    }
                }
                else if (value is Address[] addresses)
                {
                    convertedValue = addresses.Select(addr => addr.ToString()).ToArray();
                }
                else if (value is EncodeSapient.EncodedCall[] calls)
                {
                    convertedValue = calls.Select(ConvertCallForNethereum).ToArray();
                }
                else if (value is byte[] bytes)
                {
                    convertedValue = bytes.ByteArrayToHexStringWithPrefix();
                }
                else
                {
                    convertedValue = value;
                }
                
                Debug.Log($"Converted field '{namedType.name}' to: {convertedValue}");
                
                memberValues.Add(new MemberValue
                {
                    TypeName = namedType.type,
                    Value = convertedValue
                });
            }
            
            Debug.Log($"Created {memberValues.Count} member values for type '{primaryType}'");
            return memberValues.ToArray();
        }
        
        private Dictionary<string, object> ConvertCallForNethereum(EncodeSapient.EncodedCall call)
        {
            return new Dictionary<string, object>
            {
                ["to"] = call.to.ToString(),
                ["value"] = call.value,
                ["data"] = call.data,
                ["gasLimit"] = call.gasLimit,
                ["delegateCall"] = call.delegateCall,
                ["onlyFallback"] = call.onlyFallback,
                ["behaviorOnError"] = call.behaviorOnError
            };
        }
        
        private string GetFieldType(string fieldName)
        {
            // Find the field type in the primary type definition
            if (types.TryGetValue(primaryType, out NamedType[] namedTypes))
            {
                foreach (var namedType in namedTypes)
                {
                    if (namedType.name == fieldName)
                    {
                        return namedType.type;
                    }
                }
            }
            return "string"; // default fallback
        }
    }
}