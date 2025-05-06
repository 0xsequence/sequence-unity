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
    }
}