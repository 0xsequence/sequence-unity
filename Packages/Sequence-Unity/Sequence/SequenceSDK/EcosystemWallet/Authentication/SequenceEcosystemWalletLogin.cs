using System;
using System.Numerics;
using Nethereum.Util;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Authentication
{
    public class SequenceEcosystemWalletLogin
    {
        private Chain _chain;
        private string _redirectUrl;
        private string _emitterAddress;
        
        public SequenceEcosystemWalletLogin(Chain chain)
        {
            _chain = chain;
            _redirectUrl = "";
            _emitterAddress = "0xb7bE532959236170064cf099e1a3395aEf228F44";
        }
        
        public void SignInWithEmail(string email)
        {
            CreateNewSession(GetOpenPermissions(),"email", email);
        }
        
        /// <summary>
        /// Create an implicit- or explicit session based on a given set of permissions.
        /// </summary>
        /// <param name="permissions">Leave it null to create an implicit session. Otherwise, we create an explicit session.</param>
        /// <param name="preferredLoginMethod"></param>
        /// <param name="email"></param>
        private void CreateNewSession(SessionPermissions permissions, string preferredLoginMethod, string email)
        {
            // Create new local signer and private key
        }

        private SessionPermissions GetOpenPermissions()
        {
            return new SessionPermissions
            {
                valueLimit = new BigInteger(0),
                deadline = new BigInteger(DateTime.UtcNow.Second + 1000 * 60 * 5000),
                permissions = new []
                {
                    new Permission
                    {
                        target = new Address(_emitterAddress),
                        rules = Array.Empty<ParameterRule>()
                    }
                }
            };
        }

        private SessionPermissions GetRestrictivePermissions()
        {
            return new SessionPermissions
            {
                valueLimit = new BigInteger(0),
                deadline = new BigInteger(DateTime.UtcNow.Second + 1000 * 60 * 5000),
                permissions = new []
                {
                    new Permission
                    {
                        target = new Address(_emitterAddress),
                        rules = new []
                        {
                            new ParameterRule
                            {
                                cumulative = false,
                                operation = ParameterOperation.equal,
                                value = HashFunctionSelector("explicitEmit()").HexStringToByteArray(32),
                                offset = new BigInteger(0),
                                mask = ParameterRule.SelectorMask
                            },
                            new ParameterRule
                            {
                                cumulative = true,
                                operation = ParameterOperation.greaterThanOrEqual,
                                value = "0x1234567890123456789012345678901234567890".HexStringToByteArray(32),
                                offset = new BigInteger(4),
                                mask = ParameterRule.Uint256Mask
                            }
                        }
                    }
                }
            };
        }

        private string HashFunctionSelector(string function)
        {
            var sha3 = new Sha3Keccack();
            var hash = sha3.CalculateHash(function);
            return "0x" + hash.Substring(0, 8);
        }
    }
}