using System.Numerics;
using Sequence.EcosystemWallet.Primitives;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet
{
    [Preserve]
    public struct ConnectArgs
    {
        public string preferredLoginMethod;
        public string email;
        public string origin;
        public bool includeImplicitSession;
        public SessionArgs session;
    }
    
    [Preserve]
    public struct ConnectResponse
    {
        public Address walletAddress;
        public GuardConfig guard;
        public string userEmail;
        public string loginMethod;
        public Attestation attestation;
        public RSY signature;
    }

    [Preserve]
    public class GuardConfig
    {
        public string url;
        public ModuleAddresses moduleAddresses;  
    }

    [Preserve]
    public class ModuleAddresses
    {
        public bool isMap;
        public Address[][] data;
    }
    
    [Preserve]
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
        public string valueLimit;
        public BigInteger deadline;
        public Permission[] permissions;

        public ExplicitSessionArgs(SessionPermissions sessionPermissions) : base(sessionPermissions.sessionAddress)
        {
            chainId = sessionPermissions.chainId;
            valueLimit = sessionPermissions.valueLimit.ToString();
            deadline = sessionPermissions.deadline;
            permissions = sessionPermissions.permissions;
        }
    }
}