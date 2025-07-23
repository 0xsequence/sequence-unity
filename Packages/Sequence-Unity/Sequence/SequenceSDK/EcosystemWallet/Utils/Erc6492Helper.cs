using System;
using System.Threading.Tasks;
using Nethereum.ABI;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.Hex.HexConvertors.Extensions;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

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

        public static byte[] Wrap(byte[] signature, Address to, byte[] data)
        {
            var encoder = new ABIEncode();
            var encoded = encoder.GetABIEncodedPacked(
                new ABIValue("address", to),
                new ABIValue("bytes", data),
                new ABIValue("bytes", signature)
            );

            return HexUtils.Concat(encoded.ByteArrayToHexString(), MagicBytes).HexStringToByteArray();
        }

        public static (byte[] Signature, Erc6492 Erc6492) Decode(byte[] signature)
        {
            var magicBytes = MagicBytes.HexStringToByteArray();
            int magicLength = magicBytes.Length;
            
            if (signature.Length >= magicLength && signature[^magicLength..].ByteArrayToHexStringWithPrefix() == MagicBytes)
            {
                var raw = signature[..^magicLength];

                try
                {
                    var decoder = new ParameterDecoder();
                    var parameters = new Parameter[]
                    {
                        new ("address", 1),
                        new ("bytes", 2),
                        new ("bytes", 3),
                    };

                    var decoded = decoder.DecodeDefaultData(raw, parameters);

                    string to = decoded[0].Result?.ToString() ?? throw new Exception("Missing 'to' address");
                    byte[] data = (byte[])(decoded[1].Result ?? throw new Exception("Missing 'data'"));
                    byte[] unwrappedSignature = (byte[])(decoded[2].Result ?? throw new Exception("Missing 'signature'"));

                    return (unwrappedSignature, new Erc6492(new Address(to), data));
                }
                catch
                {
                    return (signature, null);
                }
            }

            return (signature, null);
        }
    }
}