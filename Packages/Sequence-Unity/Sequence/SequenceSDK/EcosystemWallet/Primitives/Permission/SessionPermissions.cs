using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    public class SessionPermissions
    {
        public BigInteger chainId;
        public Address signer;
        public BigInteger valueLimit;
        public BigInteger deadline;
        public Permission[] permissions;

        public object ToJson()
        {
            return new
            {
                chainId = chainId,
                valueLimit = valueLimit,
                deadline = deadline,
                permissions = permissions.Select(permission => permission.ToJson()).ToArray()
            };
        }

        public byte[] Encode()
        {
            if (permissions.Length > Permission.MAX_PERMISSIONS_COUNT) {
                throw new Exception("Too many permissions");
            }

            List<byte> result = new();
            result.AddRange(signer.ToString().HexStringToByteArray().PadLeft(20));
            result.AddRange(chainId.ByteArrayFromNumber().PadLeft(32));
            result.AddRange(valueLimit.ByteArrayFromNumber(32));
            result.AddRange(deadline.ByteArrayFromNumber(8));
            result.Add((byte)permissions.Length);
            
            foreach (var permission in permissions) {
                result.AddRange(permission.Encode());
            }

            return result.ToArray();
        }

        public static SessionPermissions Decode(byte[] data)
        {
            if (data.Length < 85)
                throw new Exception("Data too short");
            
            var signer = new Address(data.AsSpan(0, 20).ToArray());
            var chainId = new BigInteger(data.AsSpan(20, 32).ToArray(), isUnsigned: true, isBigEndian: true);
            var valueLimit = new BigInteger(data.AsSpan(52, 32).ToArray(), isUnsigned: true, isBigEndian: true);
            var deadline = new BigInteger(data.AsSpan(84, 8).ToArray(), isUnsigned: true, isBigEndian: true);

            var permissionsLength = data[92];
            
            var pointer = 93;
            var permissions = new Permission[permissionsLength];
            
            for (var i = 0; i < permissionsLength; i++) {
                var (permission, consumed) = Permission.Decode(data, pointer);
                permissions[i] = permission;
                pointer += consumed;
            }
            
            return new SessionPermissions {
                signer = signer,
                chainId = chainId,
                valueLimit = valueLimit,
                deadline = deadline,
                permissions = permissions
            };
        }

        public static SessionPermissions FromJson(string json)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            return new SessionPermissions
            {
                signer = new Address((string)data["signer"]),
                valueLimit = BigInteger.Parse((string)data["valueLimit"]),
                deadline = BigInteger.Parse((string)data["deadline"]),
                permissions = JsonConvert.DeserializeObject<object[]>(data["permissions"].ToString())
                    .Select(p => Permission.FromJson(p.ToString()))
                    .ToArray()
            };
        }
    }
}