using System.Collections.Generic;
using System.Numerics;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet
{
    public class SessionBuilder
    {
        public Chain Chain;
        public Address Signer;
        public BigInteger ValueLimit;
        public BigInteger Deadline;

        private Dictionary<Address, List<ParameterRule>> _permissions = new();

        public SessionBuilder(Chain chain, Address signer, BigInteger valueLimit, BigInteger deadline)
        {
            this.Chain = chain;
            this.Signer = signer;
            this.ValueLimit = valueLimit;
            this.Deadline = deadline;
        }

        public void AddPermission(Address target)
        {
            if (!_permissions.ContainsKey(target))
                _permissions.Add(target, new List<ParameterRule>());
        }

        public void AddPermission(Address target, bool cumulative, ParameterOperation operation, string value, BigInteger offset, byte[] mask)
        {
            var rule = new ParameterRule
            {
                cumulative = cumulative,
                operation = (int)operation,
                value = value.HexStringToByteArray().PadRight(32),
                offset = offset,
                mask = mask
            };
            
            if (!_permissions.ContainsKey(target))
                _permissions.Add(target, new List<ParameterRule>());
            
            _permissions[target].Add(rule);
        }

        public SessionPermissions GetPermissions()
        {
            var targets = _permissions.GetKeys();
            var permissions = new Permission[targets.Length];
            
            for (var i = 0; i < targets.Length; i++)
            {
                permissions[i] = new Permission
                {
                    target = targets[i],
                    rules = _permissions[targets[i]].ToArray()
                };
            }
            
            return new SessionPermissions
            {
                chainId = new BigInteger((int)Chain),
                signer = new Address(Signer),
                valueLimit = ValueLimit,
                deadline = Deadline,
                permissions = permissions
            };
        }
    }
}