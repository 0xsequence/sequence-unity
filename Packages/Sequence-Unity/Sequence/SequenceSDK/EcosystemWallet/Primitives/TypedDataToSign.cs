using System;
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    internal class TypedDataToSign
    {
        public Domain domain;
        public Dictionary<string, NamedType[]> types;
        public string primaryType;
        public Dictionary<string, object> message;
        
        [Serializable]
        public class Domain
        {
            public string name;
            public string version;
            public BigInteger chainId;
            public Address verifyingContract;

            public Domain(string name, string version, BigInteger chainId, Address verifyingContract)
            {
                this.name = name;
                this.version = version;
                this.chainId = chainId;
                this.verifyingContract = verifyingContract;
            }

            public Domain(string name, string version, Chain chain, Address verifyingContract)
            {
                this.name = name;
                this.version = version;
                this.chainId = BigInteger.Parse(ChainDictionaries.ChainIdOf[chain]);
                this.verifyingContract = verifyingContract;
            }
        }
        
        [Serializable]
        public class NamedType
        {
            public string name;
            public string type;

            public NamedType(string name, string type)
            {
                this.name = name;
                this.type = type;
            }
        }

        [Preserve]
        [JsonConstructor]
        public TypedDataToSign(Domain domain, Dictionary<string, NamedType[]> types, string primaryType, Dictionary<string, object> message)
        {
            this.domain = domain;
            this.types = types;
            this.primaryType = primaryType;
            this.message = message;
        }

        // Todo: once we have tests working, let's refactor this so it isn't one giant function
        public TypedDataToSign(Address wallet, Chain chain, Parented payload)
        {
            this.domain = new Domain("Sequence Wallet", "3", chain, wallet);
            switch (payload.payload.type)
            {
                case PayloadType.Call:
                {
                    types = new Dictionary<string, NamedType[]>()
                    {
                        ["Calls"] = new[]
                        {
                            new TypedDataToSign.NamedType("calls", "Call[]"),
                            new TypedDataToSign.NamedType("space", "uint256"),
                            new TypedDataToSign.NamedType("nonce", "uint256"),
                            new TypedDataToSign.NamedType("wallets", "address[]"),
                        },
                        ["Call"] = new[]
                        {
                            new TypedDataToSign.NamedType("to", "address"),
                            new TypedDataToSign.NamedType("value", "uint256"),
                            new TypedDataToSign.NamedType("data", "bytes"),
                            new TypedDataToSign.NamedType("gasLimit", "uint256"),
                            new TypedDataToSign.NamedType("delegateCall", "bool"),
                            new TypedDataToSign.NamedType("onlyFallback", "bool"),
                            new TypedDataToSign.NamedType("behaviorOnError", "uint256"),
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
                        encoded[i] = new EncodeSapient.EncodedCall
                        {
                            to = call.to,
                            value = call.value,
                            data = call.data.ByteArrayToHexStringWithPrefix(),
                            gasLimit = call.gasLimit,
                            delegateCall = call.delegateCall,
                            onlyFallback = call.onlyFallback,
                            behaviorOnError = (int)call.behaviorOnError
                        };
                    }
                    message.Add("calls", encoded);
                    primaryType = "Calls";
                    break;
                }
                case PayloadType.Message:
                {
                    types = new Dictionary<string, TypedDataToSign.NamedType[]>
                    {
                        ["Message"] = new[]
                        {
                            new TypedDataToSign.NamedType("message", "bytes"),
                            new TypedDataToSign.NamedType("wallets", "address[]"),
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
                    types = new Dictionary<string, TypedDataToSign.NamedType[]>
                    {
                        ["ConfigUpdate"] = new[]
                        {
                            new TypedDataToSign.NamedType("imageHash", "bytes32"),
                            new TypedDataToSign.NamedType("wallets", "address[]"),
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
    }
}