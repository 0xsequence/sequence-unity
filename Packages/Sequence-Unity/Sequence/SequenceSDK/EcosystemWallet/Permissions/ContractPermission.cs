using System;
using System.Collections.Generic;
using System.Numerics;
using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet
{
    public class ContractPermission : IPermissions
    {
        private readonly Chain _chain;
        private readonly Address _contract;
        private readonly BigInteger _deadline;
        private readonly BigInteger _valueLimit;
        private readonly List<ParameterRule> _rules = new();
        
        public ContractPermission(Address contract, BigInteger deadline, BigInteger valueLimit)
        {
            _contract = contract;
            _deadline = deadline;
            _valueLimit = valueLimit;
        }
        
        public ContractPermission(Chain chain, Address contract, BigInteger deadline, BigInteger valueLimit)
        {
            _chain = chain;
            _contract = contract;
            _deadline = deadline;
            _valueLimit = valueLimit;
        }

        public void AddRule(ParameterRule rule)
        {
            _rules.Add(rule);
        }
        
        public SessionPermissions GetPermissions()
        {
            var permissions = new[] { new Permission { target = _contract, rules = _rules.ToArray() } };

            return new SessionPermissions
            {
                chainId = _chain.IsActive() ? _chain.AsBigInteger() : 0,
                deadline = _deadline,
                valueLimit = _valueLimit,
                signer = new Address("0x1234567890123456789012345678901234567890"),
                permissions = permissions
            };
        }
    }
}