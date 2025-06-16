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

        public TypedDataToSign(Address wallet, Chain chain, Parented payload)
        {
            _payload = payload;
            this.domain = new Domain("Sequence Wallet", "3", chain, wallet);
            switch (payload.payload.type)
            {
                case PayloadType.Call:
                {
                    types = Calls.Types;

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
                    types = Message.Types;

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
                    types = ConfigUpdate.Types;

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

        // Todo replace nethereum (introduced after commit 871466dbc7faf88f1201f963af6a372c59263d2f in commit d44d3bedfcbe875f2e5edc335e1d143bfb1358d3) - I think we were close with our own implementation so worth looking at it, even if only as a reference
        public byte[] GetSignPayload()
        {
            var eip712TypedDataEncoder = new Eip712TypedDataEncoder();
            
            bool hasSalt = domain.HasSalt();
            byte[] encodedTypedData;
            ConvertTypedDataToSignToNethereum converter = new ConvertTypedDataToSignToNethereum(this);
            if (hasSalt)
            {
                TypedData<Nethereum.ABI.EIP712.DomainWithSalt> nethereumTypedDataWithSalt =
                    converter.ConvertToNethereumTypedDataWithSalt();
                encodedTypedData = eip712TypedDataEncoder.EncodeTypedData(nethereumTypedDataWithSalt);
            }
            else
            {
                TypedData<Nethereum.ABI.EIP712.Domain> nethereumTypedData =
                    converter.ConvertToNethereumTypedDataWithoutSalt();
                encodedTypedData = eip712TypedDataEncoder.EncodeTypedData(nethereumTypedData);
            }

            return SequenceCoder.KeccakHash(encodedTypedData);
        }
    }
}