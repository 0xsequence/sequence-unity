using System;
using System.Collections.Generic;
using System.Linq;

namespace Sequence.EcosystemWallet.Primitives
{
    public static class SessionsTopologyUtils
    {
        public static SessionsTopology BalanceSessionsTopology(SessionsTopology topology)
        {
            var flattened = FlattenSessionsTopology(topology);

            var blacklist = flattened.FirstOrDefault(l => l.IsLeaf && l.Leaf is ImplicitBlacklistLeaf);
            var identitySigner = flattened.FirstOrDefault(l => l.IsLeaf && l.Leaf is IdentitySignerLeaf);
            var leaves = flattened.Where(l => l.IsLeaf && l.Leaf is PermissionLeaf).ToArray();

            if (blacklist == null || identitySigner == null)
            {
                throw new Exception("No blacklist or identity signer");
            }

            var elements = new List<SessionsTopology> { blacklist, identitySigner };
            elements.AddRange(leaves);

            return BuildBalancedSessionsTopology(elements.ToArray());
        }

        private static SessionsTopology[] FlattenSessionsTopology(SessionsTopology topology)
        {
            if (topology.IsLeaf)
                return new [] { topology };

            if (!topology.IsBranch)
                throw new Exception("Invalid topology structure");
            
            var result = new List<SessionsTopology>();
            foreach (var child in topology.Branch.Children)
                result.AddRange(FlattenSessionsTopology(child));
                
            return result.ToArray();

        }

        private static SessionsTopology BuildBalancedSessionsTopology(SessionsTopology[] topologies)
        {
            var len = topologies.Length;
            if (len == 0)
                throw new Exception("Cannot build a topology from an empty list");
            
            if (len == 1)
                return topologies[0];

            var mid = len / 2;
            var left = topologies.Take(mid).ToArray();
            var right = topologies.Skip(mid).ToArray();

            var leftTopo = BuildBalancedSessionsTopology(left);
            var rightTopo = BuildBalancedSessionsTopology(right);

            return new SessionBranch(leftTopo, rightTopo).ToTopology();
        }
    }
}