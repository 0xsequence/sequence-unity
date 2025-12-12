using System;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    [JsonConverter(typeof(DomainConverter))]
    public class Domain
    {
        public string name;
        public string version;
        public BigInteger chainId;
        public Address verifyingContract;
        public FixedByte salt;
        
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Domain))
            {
                return false;
            }
            
            Domain other = (Domain)obj;
            if (salt != null && other.salt != null)
            {
                if (!salt.Equals(other.salt))
                {
                    return false;
                }
            }
            else if (salt != null && other.salt == null ||
                     salt == null && other.salt != null)
            {
                return false;
            }
            return name.Equals(other.name) && version.Equals(other.version) && chainId.Equals(other.chainId) &&
                   verifyingContract.Equals(other.verifyingContract);
        }
        
        public override string ToString()
        {
            string saltStr = salt != null ? salt.ToString() : "null";
            return $"Domain {{ name: {name}, version: {version}, chainId: {chainId}, verifyingContract: {verifyingContract}, salt: {saltStr} }}";
        }

        public Domain(string name, string version, BigInteger chainId, Address verifyingContract, FixedByte salt = null)
        {
            this.name = name;
            this.version = version;
            this.chainId = chainId;
            this.verifyingContract = verifyingContract;
            
            if (salt != null)
            {
                if (!salt.IsFixedSize || salt.Count != 32)
                {
                    throw new ArgumentException($"If provided, {nameof(salt)} must be a fixed byte array of length 32.");
                }
                this.salt = salt;
            }
        }

        public Domain(string name, string version, Chain chain, Address verifyingContract, FixedByte salt = null)
        {
            this.name = name;
            this.version = version;
            this.chainId = BigInteger.Parse(ChainDictionaries.ChainIdOf[chain]);
            this.verifyingContract = verifyingContract;
            
            if (salt != null)
            {
                if (!salt.IsFixedSize || salt.Count != 32)
                {
                    throw new ArgumentException($"If provided, {nameof(salt)} must be a fixed byte array of length 32.");
                }
                this.salt = salt;
            }
        }

        public byte[] GetDomainSeparator()
        {
            string encodeType =
                "EIP712Domain(string name,string version,uint256 chainId,address verifyingContract,bytes32 salt)";
            byte[] typeHash = SequenceCoder.KeccakHashASCII(encodeType).HexStringToByteArray();
            byte[] nameHash = SequenceCoder.KeccakHashASCII(name).HexStringToByteArray();
            byte[] versionHash = SequenceCoder.KeccakHashASCII(version).HexStringToByteArray();
            byte[] chainIdHash = new NumberCoder().Encode(chainId);
            byte[] verifyingContractHash = new AddressCoder().Encode(verifyingContract);
            byte[] saltHash = salt != null ? new FixedBytesCoder().Encode(salt) : new FixedBytesCoder().Encode(new byte[32]);
            byte[] encoded = ByteArrayExtensions.ConcatenateByteArrays(typeHash, nameHash, versionHash,
                chainIdHash, verifyingContractHash, saltHash);
            return encoded;
        }

        public bool HasSalt()
        {
            return salt?.Data != null && salt.Data.Length > 0;
        }
    }
    
    [Preserve]
    internal class DomainConverter : JsonConverter<Domain>
    {
        public override void WriteJson(JsonWriter writer, Domain value, JsonSerializer serializer)
        {
            var jsonObject = new JObject
            {
                ["name"] = value.name,
                ["version"] = value.version,
                ["chainId"] = ulong.Parse(value.chainId.ToString()),
                ["verifyingContract"] = value.verifyingContract.ToString(),
            };
            
            if (value.salt != null)
            {
                jsonObject["salt"] = value.salt.Data.ByteArrayToHexStringWithPrefix();
            }

            jsonObject.WriteTo(writer);
        }

        public override Domain ReadJson(JsonReader reader, Type objectType, Domain existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            BigInteger chainId = jsonObject["chainId"]?.Value<ulong>() ?? 0;
            string name = jsonObject["name"]?.Value<string>();
            string version = jsonObject["version"]?.Value<string>();
            Address verifyingContract = new Address(jsonObject["verifyingContract"]?.Value<string>());
            string salt = new Address(jsonObject["salt"]?.Value<string>());
            
            return new Domain(name, version, chainId, verifyingContract, null);
        }
    }
}