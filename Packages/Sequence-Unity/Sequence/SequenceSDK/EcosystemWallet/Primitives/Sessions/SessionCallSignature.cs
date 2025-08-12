using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public abstract class SessionCallSignature
    {
        public abstract byte[] Encode();

        public static SessionCallSignature FromJson(string json)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            if (data.TryGetValue("identitySignature", out var identitySignature))
            {
                return new ImplicitSessionCallSignature
                {
                    attestation = JsonConvert.DeserializeObject<Attestation>(data["attestation"].ToString()),
                    sessionSignature = RSY.FromString((string)data["sessionSignature"]),
                    identitySignature = RSY.FromString((string)identitySignature)
                };
            }
            
            if (data.TryGetValue("permissionIndex", out var permissionIndex))
            {
                return new ExplicitSessionCallSignature
                {
                    permissionIndex = BigInteger.Parse((string)permissionIndex),
                    sessionSignature = RSY.FromString((string)data["sessionSignature"]),
                };
            }
            
            throw new Exception("Invalid signature");
        }

        public static byte[] EncodeSignatures(SessionCallSignature[] signatures, SessionsTopology sessionsTopology,
            Address[] explicitSigners, Address[] implicitSigners)
        {
            var parts = new List<byte[]>();

            if (!sessionsTopology.IsComplete())
                throw new Exception("Incomplete topology");

            sessionsTopology = sessionsTopology.Minimise(explicitSigners, implicitSigners);
            
            var encodedTopology = sessionsTopology.Encode();
            if (encodedTopology.Length.MinBytesFor() > 3)
                throw new Exception("Session topology is too large");
            
            parts.Add(encodedTopology.Length.ByteArrayFromNumber(3));
            parts.Add(encodedTopology);

            var attestationMap = new Dictionary<string, int>();
            var encodedAttestations = new List<byte[]>();

            foreach (var signature in signatures.Where(s => s is ImplicitSessionCallSignature))
            {
                if (signature is not ImplicitSessionCallSignature implicitSignature)
                    throw new Exception("Invalid implicit signature");
                
                if (implicitSignature.attestation != null)
                {
                    var attestationStr = JsonConvert.SerializeObject(implicitSignature.attestation.ToJson());
                    if (!attestationMap.ContainsKey(attestationStr))
                    {
                        attestationMap[attestationStr] = encodedAttestations.Count;
                        encodedAttestations.Add(ByteArrayExtensions.ConcatenateByteArrays(
                            implicitSignature.attestation.Encode(),
                            implicitSignature.identitySignature.Pack()
                        ));
                    }
                }
            }

            if (encodedAttestations.Count >= 128)
                throw new Exception("Too many attestations");

            parts.Add(encodedAttestations.Count.ByteArrayFromNumber(1));
            parts.Add(ByteArrayExtensions.ConcatenateByteArrays(encodedAttestations.ToArray()));

            foreach (var signature in signatures)
            {
                if (signature is ImplicitSessionCallSignature implicitCallSignature)
                {
                    var attestationStr = JsonConvert.SerializeObject(implicitCallSignature.attestation.ToJson());
                    if (!attestationMap.TryGetValue(attestationStr, out var index))
                        throw new Exception("Failed to find attestation index");

                    var packedFlag = 0x80 | index;
                    parts.Add(packedFlag.ByteArrayFromNumber(1));
                    parts.Add(implicitCallSignature.sessionSignature.Pack());
                }
                else if (signature is ExplicitSessionCallSignature explicitCallSignature)
                {
                    if (explicitCallSignature.permissionIndex > 127)
                        throw new Exception("Permission index is too large");

                    var packedFlag = explicitCallSignature.permissionIndex;
                    parts.Add(packedFlag.ByteArrayFromNumber(1));
                    parts.Add(explicitCallSignature.sessionSignature.Pack());
                }
                else
                {
                    throw new Exception("Invalid call signature");
                }
            }

            return ByteArrayExtensions.ConcatenateByteArrays(parts.ToArray());
        }
    }
}