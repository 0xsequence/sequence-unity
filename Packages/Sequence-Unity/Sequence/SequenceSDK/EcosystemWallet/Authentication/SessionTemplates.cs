using System;
using System.Numerics;
using Nethereum.Util;
using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet
{
    public class SessionTemplates
    {
        private readonly Chain _chain;
        
        public SessionTemplates(Chain chain)
        {
            _chain = chain;
        }

        public SessionPermissions BuildUnrestrictivePermissions()
        {
            var target = new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA");
            var deadline = new BigInteger(DateTime.UtcNow.ToUnixTimestamp() * 1000 + 1000 * 60 * 5000);
            
            var sessionBuilder = new SessionBuilder(_chain, 0, deadline);
            sessionBuilder.AddPermission(target);
            
            return sessionBuilder.GetPermissions();
        }

        public SessionPermissions BuildBasicRestrictivePermissions()
        {
            var deadline = new BigInteger(DateTime.UtcNow.ToUnixTimestamp() * 1000 + 1000 * 60 * 5000);
            var target = new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA");
            
            var sessionBuilder = new SessionBuilder(_chain, 0, deadline);
            sessionBuilder.AddPermission(target, false, ParameterOperation.equal, 
                ABI.ABI.FunctionSelector("explicitEmit()"), 0, ParameterRule.SelectorMask);
            
            sessionBuilder.AddPermission(target, true, ParameterOperation.greaterThanOrEqual, 
                "0x1234567890123456789012345678901234567890", 4, ParameterRule.Uint256Mask);
            
            return sessionBuilder.GetPermissions();
        }
    }
}