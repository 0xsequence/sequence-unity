using System;
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
        
        public ContractPermission(Chain chain, Address contract, BigInteger deadline, BigInteger valueLimit)
        {
            _chain = chain;
            _contract = contract;
            _deadline = deadline;
            _valueLimit = valueLimit;
        }
        
        public SessionPermissions GetPermissions()
        {
            var permissions = new[] { new Permission { target = _contract, rules = Array.Empty<ParameterRule>() } };

            return new SessionPermissions
            {
                chainId = BigInteger.Parse(ChainDictionaries.ChainIdOf[_chain]),
                deadline = _deadline,
                valueLimit = _valueLimit,
                signer = new Address("0x1234567890123456789012345678901234567890"),
                permissions = permissions
            };
        }
    }
}