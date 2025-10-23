using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Nethereum.ABI.EIP712;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Primitives
{
    public class ConvertTypedDataToSignToNethereum
    {
        private Domain domain;
        private Dictionary<string, NamedType[]> types;
        private string primaryType;
        private Dictionary<string, object> message;
        
        public ConvertTypedDataToSignToNethereum(TypedDataToSign typedDataToSign)
        {
            if (typedDataToSign == null)
            {
                throw new ArgumentNullException(nameof(typedDataToSign), "TypedDataToSign cannot be null.");
            }
            
            this.domain = typedDataToSign.domain;
            this.types = typedDataToSign.types;
            this.primaryType = typedDataToSign.primaryType;
            this.message = typedDataToSign.message;

            if (domain == null || types == null || string.IsNullOrEmpty(primaryType) || message == null)
            {
                throw new ArgumentException("Invalid TypedDataToSign structure.");
            }
        }
        
        public TypedData<Nethereum.ABI.EIP712.DomainWithSalt> ConvertToNethereumTypedDataWithSalt()
        {
            if (domain.salt == null || domain.salt.Data == null || domain.salt.Data.Length == 0)
            {
                throw new ArgumentException("Domain salt is required for EIP-712 with salt.");
            }

            DomainWithSalt nethereumDomain = new Nethereum.ABI.EIP712.DomainWithSalt()
            {
                Name = domain.name,
                Version = domain.version,
                ChainId = domain.chainId,
                VerifyingContract = domain.verifyingContract.ToString(),
                Salt = domain.salt.Data
            };
            
            var nethereumTypes = new Dictionary<string, MemberDescription[]>()
            {
                {
                    "EIP712Domain", new MemberDescription[] 
                    {
                        new MemberDescription { Name = "name", Type = "string" },
                        new MemberDescription { Name = "version", Type = "string" },
                        new MemberDescription { Name = "chainId", Type = "uint256" },
                        new MemberDescription { Name = "verifyingContract", Type = "address" },
                        new MemberDescription { Name = "salt", Type = "bytes32" }
                    }
                }
            };
            
            nethereumTypes = AddNamedTypesInNethereumFormat(nethereumTypes);
            
            var convertedMessage = ConvertMessageForNethereum();
            
            return new TypedData<Nethereum.ABI.EIP712.DomainWithSalt>
            {
                Domain = nethereumDomain,
                Types = nethereumTypes,
                PrimaryType = primaryType,
                Message = convertedMessage
            };
        }

        public TypedData<Nethereum.ABI.EIP712.Domain> ConvertToNethereumTypedDataWithoutSalt()
        {
            var nethereumDomain = new Nethereum.ABI.EIP712.Domain()
            {
                Name = domain.name,
                Version = domain.version,
                ChainId = domain.chainId,
                VerifyingContract = domain.verifyingContract.ToString()
            };

            var nethereumTypes = new Dictionary<string, MemberDescription[]>()
            {
                {
                    "EIP712Domain", new MemberDescription[] 
                    {
                        new MemberDescription { Name = "name", Type = "string" },
                        new MemberDescription { Name = "version", Type = "string" },
                        new MemberDescription { Name = "chainId", Type = "uint256" },
                        new MemberDescription { Name = "verifyingContract", Type = "address" }
                    }
                }
            };

            nethereumTypes = AddNamedTypesInNethereumFormat(nethereumTypes);

            var convertedMessage = ConvertMessageForNethereum();

            return new TypedData<Nethereum.ABI.EIP712.Domain>
            {
                Domain = nethereumDomain,
                Types = nethereumTypes,
                PrimaryType = primaryType,
                Message = convertedMessage
            };
        }
        
        private Dictionary<string, MemberDescription[]> AddNamedTypesInNethereumFormat(Dictionary<string, MemberDescription[]> nethereumTypes)
        {
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

            return nethereumTypes;
        }
        
        public MemberValue[] ConvertMessageForNethereum()
        {
            var memberValues = new List<MemberValue>();
        
            if (!types.TryGetValue(primaryType, out NamedType[] namedTypes))
            {
                throw new ArgumentException($"Primary type {primaryType} not found int the provided types.");
            }
            
            foreach (var namedType in namedTypes)
            {
                if (!message.TryGetValue(namedType.name, out object value))
                {
                    Debug.LogError($"Field '{namedType.name}' not found in message for type '{primaryType}'");
                    continue;
                }

                if (value == null)
                {
                    if (namedType.type == "bytes32" || namedType.type == "address" || namedType.type.StartsWith("uint"))
                    {
                        Debug.LogError($"Null value found for non-nullable type '{namedType.type}' in field '{namedType.name}'");
                        continue;
                    }
                }

                MemberValue memberValue = new ConvertNamedTypeToMemberValue(namedType, message, value).ConvertToNethereumMemberValue();
                
                memberValues.Add(memberValue);
            }
            
            return memberValues.ToArray();
        }

        private class ConvertNamedTypeToMemberValue
        {
            private string name;
            private string type;
            private Dictionary<string, object> message;
            private object value;
            
            private object convertedValue;

            public ConvertNamedTypeToMemberValue(NamedType type, Dictionary<string, object> message, object value)
            {
                this.name = type.name;
                this.type = type.type;
                this.message = message;
                this.value = value;
            }

            public MemberValue ConvertToNethereumMemberValue()
            {
                if (value is string valueString)
                {
                    ConvertFromString(valueString);
                }
                else if (value is Address[] addresses)
                {
                    convertedValue = addresses.Select(addr => addr.ToString()).ToArray();
                }
                else if (value is EncodeSapient.EncodedCall[] calls)
                {
                    var callMemberArrays = calls.Select(call => new MemberValue[]
                    {
                        new MemberValue { TypeName = "address", Value = call.to.ToString() },
                        new MemberValue { TypeName = "uint256", Value = call.value },
                        new MemberValue { TypeName = "bytes", Value = call.data.HexStringToByteArray() },
                        new MemberValue { TypeName = "uint256", Value = call.gasLimit },
                        new MemberValue { TypeName = "bool", Value = call.delegateCall },
                        new MemberValue { TypeName = "bool", Value = call.onlyFallback },
                        new MemberValue { TypeName = "uint256", Value = call.behaviorOnError }
                    }).ToArray();

                    convertedValue = callMemberArrays;
                }
                else if (value is byte[] bytes)
                {
                    convertedValue = bytes.ByteArrayToHexStringWithPrefix();
                }
                else
                {
                    convertedValue = value;
                }

                return new MemberValue
                {
                    TypeName = type,
                    Value = convertedValue
                };
            }

            private void ConvertFromString(string valueString)
            {
                if (valueString.IsHexFormat())
                {
                    ConvertFromHex(valueString);
                }
                else if (BigInteger.TryParse(valueString, out BigInteger bigIntValue))
                {
                    convertedValue = bigIntValue;
                }
                else
                {
                    convertedValue = valueString;
                }
            }

            private void ConvertFromHex(string hex)
            {
                if (type == "uint256")
                {
                    convertedValue = hex.HexStringToBigInteger();
                }
                else if (type == "bytes32")
                {
                    convertedValue = hex.HexStringToByteArray();
                }
                else
                {
                    convertedValue = hex;
                }
            }
        }
    }
}