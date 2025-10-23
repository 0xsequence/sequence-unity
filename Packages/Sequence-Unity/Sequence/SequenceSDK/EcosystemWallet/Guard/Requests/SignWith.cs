using System.Numerics;
using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.EcosystemWallet
{
    [JsonConverter(typeof(EnumConverter<GuardPayloadType>))]
    public enum GuardPayloadType
    {
        Calls,
        Message,
        ConfigUpdate,
        SessionImplicitAuthorize
    }
    
    [JsonConverter(typeof(EnumConverter<GuardSignatureType>))]
    public enum GuardSignatureType {
        Hash,
        Sapient,
        EthSign,
        Erc1271,
    }
    
    public struct SignWithArgs
    {
        public Address signer;
        public SignRequest request;
    }

    public struct SignWithResponse
    {
        public string sig;
    }
    
    public struct SignRequest
    {
        public BigInteger chainId;
        public string msg;
        public string auxData;
        public string wallet;
        public GuardPayloadType payloadType;
        public string payloadData;
        public GuardSignatureArgs[] signatures;
    }

    public struct GuardSignatureArgs
    {
        public Address address;
        public GuardSignatureType type;
        public string imageHash;
        public string data;
    }
}