using System;
using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet.UnitTests
{
    public static class SessionsUtils
    {
        public static SessionsTopology CreateSessionsTopologyWithSingleIdentity(string identitySigner)
        {
            return new SessionsTopology(new SessionBranch(
                new ImplicitBlacklistLeaf
                {
                    blacklist = Array.Empty<Address>()
                }.ToTopology(), 
                new IdentitySignerLeaf
                {
                    identitySigner = new Address(identitySigner)
                }.ToTopology())
            );
        }
    }
}