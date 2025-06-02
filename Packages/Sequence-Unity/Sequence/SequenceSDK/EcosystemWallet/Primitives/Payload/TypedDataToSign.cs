using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine.Scripting;

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

        public byte[] GetSignPayload()
        {
            _types = types.GetKeys();
            SortTypes();
            string encodeType = CalculateEncodeType();
            byte[] encodeData = CalculateEncodeData();
            string hashedEncodeType = SequenceCoder.KeccakHashASCII(encodeType);
            byte[] hashStruct = SequenceCoder.KeccakHash(
                ByteArrayExtensions.ConcatenateByteArrays(hashedEncodeType.HexStringToByteArray(),
                    encodeData));
            byte[] domainSeparator = domain.GetDomainSeparator();

            byte[] signablePayload = ByteArrayExtensions.ConcatenateByteArrays(
                SequenceCoder.HexStringToByteArray("0x19"),
                SequenceCoder.HexStringToByteArray("0x01"),
                domainSeparator,
                hashStruct);
            byte[] hashedMessage = SequenceCoder.KeccakHash(signablePayload);
            return hashedMessage;
        }
        
        private void SortTypes()
        {
            int typesCount = _types.Length;
            if (typesCount == 0)
            {
                throw new ArgumentException($"Must have at least one entry in {nameof(types)} dictionary");
            }
            List<string> newTypes = new List<string>();
            newTypes.Add(primaryType);
            
            Array.Sort(_types);
            for (int i = 0; i < typesCount; i++)
            {
                if (_types[i] == primaryType)
                {
                    continue;
                }
                newTypes.Add(_types[i]);
            }

            _types = newTypes.ToArray();
        }

        private string CalculateEncodeType()
        {
            StringBuilder encodeType = new StringBuilder();
            int typeCount = _types.Length;
            for (int i = 0; i < typeCount; i++)
            {
                string type = _types[i];
                encodeType.Append(type);
                encodeType.Append("(");
                foreach (var namedTypes in types[type])
                {
                    encodeType.Append(namedTypes.type);
                    encodeType.Append(" ");
                    encodeType.Append(namedTypes.name);
                    encodeType.Append(",");
                }
                encodeType.Remove(encodeType.Length - 1, 1); // remove last comma
                encodeType.Append(")");
            }

            return encodeType.ToString();
        }

        private byte[] CalculateEncodeData()
        {
            return _payload.GetEIP712EncodeData();
        }
    }
}