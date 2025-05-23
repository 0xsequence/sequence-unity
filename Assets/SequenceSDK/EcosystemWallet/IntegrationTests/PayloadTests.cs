using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.ABI;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public class PayloadTests
    {
        public async Task<string> PayloadToAbi(Dictionary<string, object> parameters)
        {
            string inputPayload = parameters["payload"] as string;
            throw new NotImplementedException("Not implemented");
        }

        public async Task<string> PayloadToPacked(Dictionary<string, object> parameters)
        {
            string inputPayload = (string)parameters["payload"];
            Address wallet = parameters.TryGetValue("wallet", out var walletValue) && walletValue is string walletStr
                ? new Address(walletStr)
                : null;
            string result = DoConvertToPacked(inputPayload, wallet);
            return result;
        }

        private string DoConvertToPacked(string payload, Address wallet)
        {
            Parented decodedPayload = Decode(payload);

            if (decodedPayload.payload.isCalls)
            {
                byte[] packed = ((Calls)decodedPayload.payload).Encode(wallet);
                return packed.ByteArrayToHexStringWithPrefix();
            }
            
            throw new Exception("Not implemented or unsupported payload format");
        }
        
        public async Task<string> PayloadToJson(Dictionary<string, object> parameters)
        {
            string inputPayload = (string)parameters["payload"];
            Address wallet = new Address((string)parameters["wallet"]);
            string result = DoConvertToJson(inputPayload);
            return result;
        }

        private string DoConvertToJson(string payload)
        {
            SolidityDecoded decoded = SolidityDecoded.FromSolidityEncoding(payload);
            return JsonConvert.SerializeObject(decoded);
        }
        
        public async Task<string> PayloadHashFor(Dictionary<string, object> parameters)
        {
            string inputPayload = (string)parameters["payload"];
            Address wallet = new Address((string)parameters["wallet"]);
            string chainId = (string)parameters["chainId"];
            string result = DoHash(inputPayload, wallet, chainId);
            return result;
        }

        private string DoHash(string payload, Address wallet, string chainId)
        {
            Parented decodedPayload = Decode(payload);

            byte[] hashed = decodedPayload.Hash(wallet, ChainDictionaries.ChainById[chainId]);
            return hashed.ByteArrayToHexStringWithPrefix();
        }

        private Parented Decode(string payload)
        {
            Parented decodedPayload = null;
            try
            {
                decodedPayload = Parented.DecodeFromSolidityEncoding(payload);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to decode payload: {ex.Message}", ex);
            }

            if (decodedPayload == null || decodedPayload.payload == null)
            {
                throw new Exception("Decoded payload is null or invalid");
            }

            return decodedPayload;
        }
    }
}