using System.Collections.Generic;
using System.Numerics;
using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet
{
    public class Permissions : IPermissions
    {
        private readonly Chain _chain;
        private readonly IPermissions[] _permissions;
        
        public Permissions(Chain chain, params IPermissions[] permissions)
        {
            _chain = chain;
            _permissions = permissions;
        }
        
        public SessionPermissions GetPermissions()
        {
            BigInteger deadline = 0;
            BigInteger valueLimit = 0;
            
            var allPermissions = new List<Permission>();
            
            foreach (var currentPermissions in _permissions)
            {
                var permissions = currentPermissions.GetPermissions();

                if (permissions.deadline > deadline)
                    deadline = permissions.deadline;
                
                valueLimit += permissions.valueLimit;
                
                allPermissions.AddRange(permissions.permissions);
            }

            return new SessionPermissions
            {
                chainId = _chain.AsBigInteger(),
                deadline = deadline,
                valueLimit = valueLimit,
                signer = new Address("0x1234567890123456789012345678901234567890"),
                permissions = allPermissions.ToArray()
            };
        }
    }
}