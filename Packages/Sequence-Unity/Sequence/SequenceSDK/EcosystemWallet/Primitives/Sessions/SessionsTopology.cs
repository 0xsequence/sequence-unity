using System;

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

        public byte[] Encode()
        {
            if (IsBranch)
                return Branch.Encode();

            if (IsLeaf)
                return Leaf.Encode();
            
            throw new Exception("Invalid topology.");
        }

        public static SessionsTopology Decode(string input)
        {
            return new SessionsTopology(new IdentitySignerLeaf());
        }
    }
}