using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SessionsTopology : ITopology
    {
        public const int FlagPermissions = 0;
        public const int FlagNode = 1;
        public const int FlagBranch = 2;
        public const int FlagBlacklist = 3;
        public const int FlagIdentitySigner = 4;

        public IBranch Branch { get; }
        public ILeaf Leaf { get; }
        public INode Node { get; }
        
        public SessionsTopology(SessionBranch branch)
        {
            this.Branch = branch;
        }

        public SessionsTopology(SessionLeaf leaf)
        {
            this.Leaf = leaf;
        }
        
        public SessionsTopology(SessionNodeLeaf node)
        {
            this.Node = node;
        }

        public string JsonSerialize()
        {
            return JsonConvert.SerializeObject(this.ToJsonObject());
        }
        
        public string ImageHash()
        {
            return this.Hash(true);
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
            if (this.IsBranch())
            {
                var branchList = ((SessionsTopology[])Branch.Children)
                    .Select(branch => (ITopology)branch.Minimise(explicitSigners, implicitSigners))
                    .ToArray();
                
                if (branchList.All(b => b.Node is SessionNodeLeaf))
                {
                    var nodeBytes = branchList
                        .Select(node => (node.Node as SessionNodeLeaf)?.Value)
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
                    Value = this.Hash(true).HexStringToByteArray()
                }.ToTopology();
            }

            if (Leaf is ImplicitBlacklistLeaf)
            {
                if (implicitSigners.Length == 0)
                {
                    return new SessionNodeLeaf
                    {
                        Value = this.Hash(true).HexStringToByteArray()
                    }.ToTopology();
                }

                return this;
            }

            if (Leaf is IdentitySignerLeaf || Node is SessionNodeLeaf)
                return this;

            throw new Exception("Invalid topology");
        }
        
        public bool IsComplete()
        {
            return this.FindLeaf<IdentitySignerLeaf>(_ => true) != null && 
                   this.FindLeaf<ImplicitBlacklistLeaf>(_ => true) != null;
        }

        public SessionsTopology AddExplicitSession(SessionPermissions session)
        {
            var existingPermission = this.FindLeaf<PermissionLeaf>(leaf => 
                leaf.permissions.signer.Equals(session.signer));
            
            if (existingPermission != null)
                throw new Exception("Session already exists.");

            return SessionsTopologyUtils.BalanceSessionsTopology(MergeSessionsTopologies(this, new PermissionLeaf
            {
                permissions = session
            }.ToTopology()));
        }

        [CanBeNull]
        public SessionsTopology RemoveExplicitSession(Address address)
        {
            if (this.IsLeaf() && Leaf is PermissionLeaf permissionLeaf)
                return permissionLeaf.permissions.signer.Equals(address) ? null : this;

            if (this.IsBranch())
            {
                var newChildren = new List<SessionsTopology>();
                foreach (var child in (SessionsTopology[])Branch.Children)
                {
                    var updatedChild = child.RemoveExplicitSession(address);
                    if (updatedChild != null)
                        newChildren.Add(updatedChild);
                }

                if (newChildren.Count == 0)
                    return null;

                if (newChildren.Count == 1)
                    return newChildren[0];

                return new SessionBranch(newChildren.ToArray()).ToTopology();
            }

            return this;
        }

        public Address GetIdentitySigner()
        {
            return this.FindLeaf<IdentitySignerLeaf>()?.identitySigner;
        }

        public Address[] GetImplicitBlacklist()
        {
            return this.FindLeaf<ImplicitBlacklistLeaf>().blacklist;
        }

        public Address[] GetExplicitSigners()
        {
            return GetExplicitSigners(Array.Empty<Address>());
        }
        
        public Address[] GetExplicitSigners(Address[] current)
        {
            if (this.IsLeaf() && Leaf is PermissionLeaf permissionLeaf)
                return current.AddToArray(permissionLeaf.permissions.signer);

            if (this.IsBranch())
            {
                var children = (SessionsTopology[])Branch.Children;
                foreach (var child in children)
                    current = child.GetExplicitSigners(current);
            }
            
            return current;
        }
        
        public void AddToImplicitBlacklist(Address address)
        {
            var existingLeaf = this.FindLeaf<ImplicitBlacklistLeaf>();
            if (existingLeaf == null)
                throw new Exception("No blacklist found.");

            if (existingLeaf.blacklist.Any(b => b.Equals(address)))
                return;
            
            var blacklist = existingLeaf.blacklist.ToList();
            blacklist.Add(address);
            existingLeaf.blacklist = blacklist.OrderBy(h => h.Value.HexStringToByteArray(), new ByteArrayComparer()).ToArray();
        }

        public void RemoveFromImplicitBlacklist(Address address)
        {
            var leaf = this.FindLeaf<ImplicitBlacklistLeaf>(_ => true);
            if (leaf == null)
                throw new Exception("No blacklist found.");

            var newBlacklist = leaf.blacklist.Where(a => !a.Equals(address)).ToArray();
            leaf.blacklist = newBlacklist;
        }

        public static SessionLeaf DecodeLeaf(string value)
        {
            var data = value.HexStringToByteArray();
            if (data.Length == 0)
                throw new ArgumentException("Bytes array is empty");

            var flagBytes = new[] { data[0] };
            var flag = flagBytes.ToInteger();
            
            if (flag == FlagBlacklist)
            {
                var blacklist = new Address[data.Length - 1];
                var index = 0;
                
                for (int i = 1; i < data.Length; i += 20)
                {
                    byte[] slice = data.Skip(i).Take(20).ToArray();
                    blacklist[index] = new Address(slice.ByteArrayToHexStringWithPrefix());
                }
                
                return new ImplicitBlacklistLeaf
                {
                    blacklist = blacklist
                };
            }

            if (flag == FlagIdentitySigner)
            {
                string identitySigner = data.Skip(1).Take(20).ToArray().ByteArrayToHexStringWithPrefix();
                
                return new IdentitySignerLeaf
                {
                    identitySigner = new Address(identitySigner)
                };
            }

            if (flag == FlagPermissions)
            {
                byte[] slice = data.Skip(1).ToArray();
                var permissions = SessionPermissions.Decode(slice);
                
                return new PermissionLeaf
                {
                    permissions = permissions
                };
            }

            throw new Exception("Invalid leaf");
        }
        
        public static SessionsTopology MergeSessionsTopologies(SessionsTopology a, SessionsTopology b)
        {
            return new SessionsTopology(new SessionBranch(a, b));
        }

        public static SessionsTopology FromTree(string tree)
        {
            if (tree.StartsWith("["))
            {
                var list = JsonConvert.DeserializeObject<List<object>>(tree);
                if (list.Count < 2)
                    throw new Exception("Invalid node structure in JSON");

                var children = list.Select(i => FromTree(i.ToString())).ToArray();
                return new SessionBranch(children).ToTopology();
            }
            
            if (tree.StartsWith("0x"))
                throw new Exception("Unknown in configuration tree");
            
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(tree)["data"];
            return DecodeLeaf(data).ToTopology();
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