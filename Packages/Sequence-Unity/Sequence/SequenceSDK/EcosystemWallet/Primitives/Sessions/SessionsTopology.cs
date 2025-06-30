using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sequence.Utils;
using Unity.Plastic.Newtonsoft.Json;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SessionsTopology
    {
        public const int FlagPermissions = 0;
        public const int FlagNode = 1;
        public const int FlagBranch = 2;
        public const int FlagBlacklist = 3;
        public const int FlagIdentitySigner = 4;

        public readonly SessionBranch Branch;
        public readonly SessionLeaf Leaf;

        public bool IsBranch => Branch != null;
        public bool IsLeaf => Leaf != null;
        
        public SessionsTopology(SessionBranch branch)
        {
            this.Branch = branch;
            this.Leaf = null;
        }

        public SessionsTopology(SessionLeaf leaf)
        {
            this.Branch = null;
            this.Leaf = leaf;
        }

        public string JsonSerialize()
        {
            return JsonConvert.SerializeObject(ToJson());
        }

        public object ToJson()
        {
            if (IsBranch)
                return Branch.ToJson();

            if (IsLeaf)
                return Leaf.ToJson();
            
            throw new Exception("Invalid topology.");
        }

        public byte[] Encode()
        {
            if (IsBranch)
                return Branch.Encode();

            if (IsLeaf)
                return Leaf.Encode();
            
            throw new Exception("Invalid topology.");
        }

        public static SessionsTopology FromJson(string json)
        {
            if (json.StartsWith("["))
            {
                var list = JsonConvert.DeserializeObject<List<object>>(json);
                if (list.Count < 2)
                    throw new Exception("Invalid node structure in JSON");

                var children = list.Select(i => FromJson(i.ToString())).ToArray();
                return new SessionBranch(children).ToTopology();
            }

            if (json.StartsWith("0x"))
            {
                return new SessionNodeLeaf
                {
                    Value = json.HexStringToByteArray()
                }.ToTopology();
            }
            
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            var type = (string)data["type"];

            switch (type)
            {
                case SessionLeaf.SessionPermissionsType:
                    return new PermissionLeaf
                    {
                        permissions =
                        {
                            signer = new Address((string)data["signer"]),
                            valueLimit = (BigInteger)data["valueLimit"],
                            deadline = (BigInteger)data["deadline"],
                            permissions = JsonConvert.DeserializeObject<Permission[]>(data["permissions"].ToString())
                        }
                    }.ToTopology();
                case SessionLeaf.IdentitySignerType:
                    var identitySigner = data["identitySigner"].ToString();
                    return new IdentitySignerLeaf
                    {
                        identitySigner = new Address(identitySigner)
                    }.ToTopology();
                case SessionLeaf.ImplicitBlacklistType:
                    var blacklistJson = data["blacklist"].ToString();
                    var blacklist = JsonConvert.DeserializeObject<Address[]>(blacklistJson);
                    return new ImplicitBlacklistLeaf
                    {
                        blacklist = blacklist
                    }.ToTopology();
                default:
                    throw new Exception("Invalid topology.");
            }
        }
    }
}