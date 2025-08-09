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
            var signer = new Address("0xb7bE532959236170064cf099e1a3395aEf228F44");
            var deadline = new BigInteger(DateTime.UtcNow.ToUnixTimestamp() * 1000 + 1000 * 60 * 5000);
            var target = new Address("0x8F6066bA491b019bAc33407255f3bc5cC684A5a4");
            
            var sessionBuilder = new SessionBuilder(_chain, signer, 0, deadline);
            sessionBuilder.AddPermission(target);
            
            return sessionBuilder.GetPermissions();
        }

        public SessionPermissions BuildBasicRestrictivePermissions()
        {
            var signer = new Address("0xb7bE532959236170064cf099e1a3395aEf228F44");
            var deadline = new BigInteger(DateTime.UtcNow.ToUnixTimestamp() * 1000 + 1000 * 60 * 5000);
            var target = new Address("0x8F6066bA491b019bAc33407255f3bc5cC684A5a4");
            
            var sessionBuilder = new SessionBuilder(_chain, signer, 0, deadline);
            sessionBuilder.AddPermission(target, false, ParameterOperation.equal, 
                EthCrypto.HashFunctionSelector("explicitEmit()"), 0, ParameterRule.SelectorMask);
            
            sessionBuilder.AddPermission(target, true, ParameterOperation.greaterThanOrEqual, 
                "0x1234567890123456789012345678901234567890", 4, ParameterRule.Uint256Mask);
            
            return sessionBuilder.GetPermissions();
        }
    }
}