using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;
using Sequence.EcosystemWallet.Primitives.Passkeys;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.UnitTests
{
    public class PasskeysUnitTests
    {
        [TestCase("{\"x\":\"0xe4dcb71b15cfb8be619221a74bb2590c6090a63d267bbe973b84a0b65cf5f93e\",\"y\": \"0xe7c0b7bdb5e8bd0676decea02326a732ce434b10550e7109817b38f37bfa13a2\",\"requireUserVerification\":true,\"metadataHash\":\"0x00000000000000000000000000000000000000000000000000000000000057a4\",\"r\": \"0xfc5768348f7bb3939658d2fb14dfce15c58a770fbb752b27a41a9abd7448a09a\",\"s\":\"0x5099a7c5aed7c93e21c80135815a4fc475736f37ad08a89a8c6461ac973ea887\",\"authenticatorData\":\"0x00000000000000000000000000000000000000000000000000000000000015b9050000763c\",\"clientDataJson\":{\"type\":\"webauthn.get\",\"challenge\":\"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUXI\",\"origin\":\"?Wbl5!!-_3!k0!?rMWUMKO!V!I70M-L_-?0-_LE!?!_G-J0!y-76ba_3!u0y60hO63K28?_l!!_N?QOj-0!_?2Cbb5w!!nh?7?Y1x-9m?96q?ho!M4NH3!?CmAa?z5!_-?!q-1C2?!0_O1_??cUI7?_-!!Ps?1?6FT04W_?xQ!Fd??-p?ZX4_6JG3n!?!-k-_6D-e!?l9o!tB62OK2z-_F4-!!X!J!a?!-c?wA_66lvA3-Q1_-97FBjhX3_Cu!e_\"},\"embedMetadata\":true}")]
        [TestCase("{\"x\":\"0x27da369dc69d1fa5a7d815e10ec765ccb6d1d1a1ac383c0a43e2befe5f2b5e1b\",\"y\":\"0x78e9505cb8dcac86878065bcaf8f199ef77f0c3afd458c6c46448af9550ed818\",\"requireUserVerification\":false,\"metadataHash\":\"0x5c59a01b79c5d8c4364f17a42a4808e005a93526c5084f0e7529ce3e6bb7131a\",\"r\":\"0x339d9b9e8ffc8f23a2a8545a8877f96bb660ec76205476a56ea9ae7a63e505dc\",\"s\":\"0x4db2e79efd31a6d7fbb424f0ebefc73903a948856197bcbeacea5233fd7c2962\",\"authenticatorData\":\"0x220ee2806d87a214f76e76e790c1a3bb55e4d83b966f9b6c576c40a38aff383501000cdd31\",\"clientDataJson\":{\"type\":\"webauthn.get\",\"challenge\":\"Pqr3Arso8Ef_SX0fQwTCJUk6VhZddkpI7qmMeQGgXAY\",\"origin\":\"-e!0-?98oYa3t?-CsHcY-!-?5VSna4l_?-fE7g63eL-?P06?0-inM?9d5D--5??lv?OJVw!Fy598ZyVV_E_6ug5v60CBOT?7_fL?_r?2_HY7cp7z_B?C82jz?__!?qT2-_925_-T__C5_U_-?8L-6D7_!7dDb2!?9JC178V!u26_H9?N7Mo_6oz!7tqK-g_!8?iJ3_??A??P4K94-8L?!-?mF!10-R6B0?8D__0T!-R-uR_9MQCD82?7-4j1s2!_g85Y4Q??W68dl?A?xAnCA8d_wF2?__k\"},\"embedMetadata\":true}")]
        public void EncodeSignature(string inputJson)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(inputJson);
            var encoded = PasskeysHelper.EncodeSignature(new PasskeysArgs
            {
                x = (string)data["x"],
                y = (string)data["y"],
                r = (string)data["r"],
                s = (string)data["s"],
                requireUserVerification = (bool)data["requireUserVerification"],
                embedMetadata = (bool)data["embedMetadata"],
                credentialId = data.TryGetValue("credentialId", out var credentialIdValue) ? (string)credentialIdValue : null,
                metadataHash = data.TryGetValue("metadataHash", out var metadataHashValue) ? (string)metadataHashValue : null,
                authenticatorData = (string)data["authenticatorData"],
                clientDataJson = data["clientDataJson"].ToString(),
            });
            
            Debug.Log($"Encoded Passkey: {encoded}");
            
            var decoded = DecodedSignature.Decode(encoded.HexStringToByteArray());
            
            Debug.Log($"Decoded Passkey: {JsonConvert.SerializeObject(decoded)}");
            
            Assert.AreEqual(encoded, decoded.Encode().ByteArrayToHexStringWithPrefix());
        }

        [TestCase("0x405c59a01b79c5d8c4364f17a42a4808e005a93526c5084f0e7529ce3e6bb7131a25220ee2806d87a214f76e76e790c1a3bb55e4d83b966f9b6c576c40a38aff383501000cdd3100ffff339d9b9e8ffc8f23a2a8545a8877f96bb660ec76205476a56ea9ae7a63e505dc4db2e79efd31a6d7fbb424f0ebefc73903a948856197bcbeacea5233fd7c296227da369dc69d1fa5a7d815e10ec765ccb6d1d1a1ac383c0a43e2befe5f2b5e1b78e9505cb8dcac86878065bcaf8f199ef77f0c3afd458c6c46448af9550ed818")]
        public void DecodeSignature(string encodedSignature)
        {
            var decoded = PasskeysHelper.DecodeSignature(encodedSignature);
            
            Debug.Log($"Decoded Signature: {decoded}");
        }
        
        [TestCase("{\"x\":\"0xe4dcb71b15cfb8be619221a74bb2590c6090a63d267bbe973b84a0b65cf5f93e\",\"y\":\"0xe7c0b7bdb5e8bd0676decea02326a732ce434b10550e7109817b38f37bfa13a2\",\"requireUserVerification\":true,\"metadataHash\":\"0x00000000000000000000000000000000000000000000000000000000000057a4\"}")]
        [TestCase("{\"x\":\"0x27da369dc69d1fa5a7d815e10ec765ccb6d1d1a1ac383c0a43e2befe5f2b5e1b\",\"y\":\"0x78e9505cb8dcac86878065bcaf8f199ef77f0c3afd458c6c46448af9550ed818\",\"requireUserVerification\":false,\"metadataHash\":\"0x5c59a01b79c5d8c4364f17a42a4808e005a93526c5084f0e7529ce3e6bb7131a\"}")]
        [TestCase("{\"x\":\"0xe018db753bcfb28da9770e68723e0f1bff3eaaf702bb28f047ff497d1f4304c2\",\"y\":\"0x25493a56165d764a48eea98c7901a05c06545c59a01b79c5d8c4364f17a42a48\",\"requireUserVerification\":true,\"credentialId\":\"p-e2-GDn2?-0v!G2?v7!v43_hPDRDv_UkG?RE!rI6w_4j!_R26_7X4U!09?!r6d_ckZ-l_-!B0\"}")]
        public void ComputeRoot(string inputJson)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(inputJson);
            var x = (string)data["x"];
            var y = (string)data["y"];
            var requireUserVerification = (bool)data["requireUserVerification"];
            var credentialId = data.TryGetValue("credentialId", out var credentialIdValue) ? (string)credentialIdValue : null;
            var metadataHash = data.TryGetValue("metadataHash", out var metadataHashValue) ? (string)metadataHashValue : null;

            var result = PasskeysHelper.ComputeRoot(new PasskeysArgs
            {
                x = x,
                y = y,
                requireUserVerification = requireUserVerification,
                credentialId = credentialId,
                metadataHash = metadataHash
            });

            Debug.Log($"Computed Root: {result}");
        }
    }
}