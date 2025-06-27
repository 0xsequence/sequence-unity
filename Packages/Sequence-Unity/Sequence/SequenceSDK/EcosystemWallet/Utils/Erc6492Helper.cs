using System;
using System.Threading.Tasks;
using Nethereum.ABI;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;

namespace Sequence.EcosystemWallet.Utils
{
    public static class Erc6492Helper
    {
        private const string DeployCode = "0x60806040523480...";
        private static readonly string MagicBytes = "0x6492649264926492649264926492649264926492649264926492649264926492";

        public class Context
        {
            public Address factory;
            public Address stage1;
            public Address stage2;
            public string creationCode;
        }
        
        public static (string To, string Data) Deploy(string deployHash, Context context)
        {
            var encoded = EncodeDeploy(context.stage1, deployHash);
            return (context.factory, encoded);
        }

        private static string EncodeDeploy(string stage1, string deployHash)
        {
            var function = new FunctionABI("deploy", false);
            function.InputParameters = new[]
            {
                new Parameter("address", "stage1"),
                new Parameter("bytes", "hash")
            };

            var encoder = new FunctionCallEncoder();
            return encoder.EncodeRequest(function.Sha3Signature, Array.Empty<Parameter>(), stage1, deployHash.HexToByteArray());
        }

        public static string Wrap(string signature, string to, string data)
        {
            var encoder = new ABIEncode();
            var encoded = encoder.GetABIEncodedPacked(
                new ABIValue("address", to),
                new ABIValue("bytes", data.HexToByteArray()),
                new ABIValue("bytes", signature.HexToByteArray())
            ).ToHex();

            return encoded + MagicBytes[2..];
        }

        public static (string Signature, (string To, string Data)? Erc6492) Decode(string signature)
        {
            if (signature.EndsWith(MagicBytes[2..]))
            {
                try
                {
                    var trimmed = signature[..^MagicBytes[2..].Length];
                    var decoder = new ParameterDecoder();
                    var parameters = new[] {
                        new Parameter("address", "to"),
                        new Parameter("bytes", "data"),
                        new Parameter("bytes", "signature")
                    };

                    var decoded = decoder.DecodeDefaultData(trimmed, parameters);
                    var to = (string)decoded[0].Result;
                    var data = ((byte[])decoded[1].Result).ToHex();
                    var innerSig = ((byte[])decoded[2].Result).ToHex();

                    return (innerSig, (to, data));
                }
                catch
                {
                    // fallback to raw signature
                }
            }
            return (signature, null);
        }

        public static async Task<bool> IsValidAsync(
            string address,
            string messageHash,
            string encodedSignature,
            Web3 web3)
        {
            /*var encoder = new ABIEncode();
            var calldata = encoder.GetABIEncoded(
                new ABIValue("address", address),
                new ABIValue("bytes32", messageHash.HexToByteArray()),
                new ABIValue("bytes", encodedSignature.HexToByteArray())
            ).ToHex();

            var fullData = DeployCode + calldata[2..];

            var result = await .Transactions.Call.SendRequestAsync(new Nethereum.RPC.Eth.DTOs.CallInput
            {
                Data = fullData,
                To = null // To is null for `eth_call` with deploy bytecode
            }, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());*/

            return Convert.ToInt32("result", 16) == 1;
        }
    }
}