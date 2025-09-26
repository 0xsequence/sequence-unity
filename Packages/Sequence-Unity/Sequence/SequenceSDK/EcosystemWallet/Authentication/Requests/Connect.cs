using System.Numerics;
using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet
{
    public struct ConnectArgs
    {
        public string preferredLoginMethod;
        public string email;
        public string origin;
        public bool includeImplicitSession;
        public SessionArgs session;
    }
    
    public struct ConnectResponse
    {
        public Address walletAddress;
        public GuardConfig guard;
        public string userEmail;
        public string loginMethod;
        public Attestation attestation;
        public RSY signature;
    }

    public class GuardConfig
    {
        public string url;
        public ModuleAddresses moduleAddresses;  
    }

    public class ModuleAddresses
    {
        public bool isMap;
        public string[][] data;
    }
    
    public class SessionArgs
    {
        public Address sessionAddress;

        public SessionArgs(Address sessionAddress)
        {
            this.sessionAddress = sessionAddress;
        }
    }
    
    public class ExplicitSessionArgs : SessionArgs
    {
        public BigInteger chainId;
        public BigInteger valueLimit;
        public BigInteger deadline;
        public Permission[] permissions;

        public ExplicitSessionArgs(SessionPermissions sessionPermissions) : base(sessionPermissions.sessionAddress)
        {
            chainId = sessionPermissions.chainId;
            valueLimit = sessionPermissions.valueLimit;
            deadline = sessionPermissions.deadline;
            permissions = sessionPermissions.permissions;
        }
    }
}