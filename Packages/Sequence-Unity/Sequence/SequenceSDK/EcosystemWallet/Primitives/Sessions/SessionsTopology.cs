using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Sequence.ABI;
using Sequence.Utils;

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
            return JsonConvert.SerializeObject(ToJson());
        }

        public object ToJson()
        {
            if (this.IsBranch())
                return Branch.ToJson();

            if (this.IsLeaf())
                return Leaf.ToJson();

            if (this.IsNode())
                return Node.ToJson();
            
            throw new Exception("Invalid topology.");
        }
        
        public string ImageHash()
        {
            return this.Hash(true);
        }

        public byte[] Encode()
        {
            if (this.IsBranch())
                return Branch.Encode();

            if (this.IsLeaf())
                return Leaf.Encode();

            if (this.IsNode())
                return Node.Encode();
            
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

        public void AddToImplicitBlacklist(Address address)
        {
            var existingLeaf = this.FindLeaf<ImplicitBlacklistLeaf>(_ => true);
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