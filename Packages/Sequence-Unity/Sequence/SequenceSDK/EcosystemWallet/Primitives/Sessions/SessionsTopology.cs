using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using Sequence.ABI;
using Sequence.Utils;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

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

        /// <summary>
        /// Optimise the configuration tree by rolling unused signers into nodes.
        /// </summary>
        /// <param name="explicitSigners">The list of explicit signers to consider.</param>
        /// <param name="implicitSigners">The list of implicit signers to consider.</param>
        /// <returns>New reference to the compromised topology.</returns>
        /// <exception cref="Exception"></exception>
        public SessionsTopology Minimise(Address[] explicitSigners, Address[] implicitSigners)
        {
            if (IsBranch)
            {
                var branchList = Branch.Children
                    .Select(branch => branch.Minimise(explicitSigners, implicitSigners))
                    .ToArray();

                if (branchList.All(b => b.IsLeaf && b.Leaf is SessionNodeLeaf))
                {
                    var nodeBytes = branchList
                        .Select(node => node.Encode())
                        .ToArray();

                    var concatenated = ByteArrayExtensions.ConcatenateByteArrays(nodeBytes);
                    return new SessionNodeLeaf
                    {
                        Value = SequenceCoder.KeccakHash(concatenated)
                    }.ToTopology();
                }

                return new SessionBranch(branchList).ToTopology();
            }

            if (Leaf is PermissionLeaf permissionLeaf)
            {
                if (explicitSigners.Contains(permissionLeaf.permissions.signer))
                    return this;

                return new SessionNodeLeaf
                {
                    Value = Hash().HexStringToByteArray()
                }.ToTopology();
            }

            if (Leaf is ImplicitBlacklistLeaf)
            {
                if (implicitSigners.Length == 0)
                {
                    return new SessionNodeLeaf
                    {
                        Value = Hash().HexStringToByteArray()
                    }.ToTopology();
                }

                return this;
            }

            if (Leaf is IdentitySignerLeaf or SessionNodeLeaf)
                return this;

            throw new Exception("Invalid topology");
        }

        public string Hash()
        {
            if (IsBranch)
            {
                var children = Branch.Children;
                if (children.Length == 0)
                    throw new Exception("Empty branch");

                var hashedChildren = children.Select(child => child.Hash()).ToArray();

                var childBytes = hashedChildren[0].HexStringToByteArray();
                for (var i = 1; i < hashedChildren.Length; i++)
                {
                    var nextBytes = hashedChildren[i].HexStringToByteArray();
                    childBytes = SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(childBytes, nextBytes));
                }

                return childBytes.ByteArrayToHexStringWithPrefix();
            }

            if (IsLeaf && Leaf is SessionNodeLeaf nodeLeaf)
                return nodeLeaf.Value.ByteArrayToHexStringWithPrefix();

            if (IsLeaf)
                return SequenceCoder.KeccakHash(Leaf.Encode()).ByteArrayToHexStringWithPrefix();

            throw new Exception("Invalid tree structure");
        }
        
        public bool IsComplete()
        {
            return FindLeaf<IdentitySignerLeaf>(_ => true) != null && 
                   FindLeaf<ImplicitBlacklistLeaf>(_ => true) != null;
        }

        public T FindLeaf<T>(Func<T, bool> check) where T : SessionLeaf
        {
            if (Leaf is T leaf && check(leaf))
                return leaf;

            if (!IsBranch) 
                return null;

            return Branch.Children.Select(child => child.FindLeaf(check))
                .FirstOrDefault(childLeaf => childLeaf != null);
        }

        public SessionsTopology AddExplicitSession(SessionPermissions session)
        {
            var existingPermission = FindLeaf<PermissionLeaf>(leaf => 
                leaf.permissions.signer.Equals(session.signer));
            
            if (existingPermission != null)
                throw new Exception("Session already exists.");

            return SessionsTopologyUtils.BalanceSessionsTopology(MergeSessionsTopologies(this, new PermissionLeaf
            {
                permissions = session
            }.ToTopology()));
        }

        public SessionsTopology RemoveExplicitSession(SessionPermissions session)
        {
            throw new Exception("Not implemented.");
        }

        public void AddToImplicitBlacklist(Address address)
        {
            var existingLeaf = FindLeaf<ImplicitBlacklistLeaf>(_ => true);
            if (existingLeaf == null)
                throw new Exception("No blacklist found.");

            if (existingLeaf.blacklist.Any(b => b.Equals(address)))
                return;
            
            var blacklist = existingLeaf.blacklist.ToList();
            blacklist.Add(address);
            existingLeaf.blacklist = blacklist.OrderBy(h => h.Value.HexStringToByteArray(), new ByteArrayComparer()).ToArray();
        }

        public SessionsTopology RemoveFromImplicitBlacklist(Address address)
        {
            throw new Exception("Not implemented.");
        }

        public static SessionsTopology MergeSessionsTopologies(SessionsTopology a, SessionsTopology b)
        {
            return new SessionsTopology(new SessionBranch(a, b));
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
                        permissions = new()
                        {
                            signer = new Address((string)data["signer"]),
                            valueLimit = BigInteger.Parse((string)data["valueLimit"]),
                            deadline = BigInteger.Parse((string)data["deadline"]),
                            permissions = JsonConvert.DeserializeObject<List<object>>(data["permissions"].ToString())
                                .Select(i => Permission.FromJson(i.ToString()))
                                .ToArray()
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
                    var blacklist = JsonConvert.DeserializeObject<string[]>(blacklistJson);
                    return new ImplicitBlacklistLeaf
                    {
                        blacklist = blacklist.Select(b => new Address(b)).ToArray()
                    }.ToTopology();
                default:
                    throw new Exception("Invalid topology.");
            }
        }
    }
}