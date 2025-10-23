using System;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.EcosystemWallet.Primitives;
using Sequence.EcosystemWallet.Primitives.Common;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.UnitTests
{
    public class AuthenticationTests
    {
        private static readonly Chain Chain = Chain.TestnetArbitrumSepolia;
        
        [Test]
        public void Disconnect()
        {
            SequenceWallet.RecoverFromStorage().Disconnect();
        }
        
        [Test]
        public async Task CreateSessionForExplicitEmit()
        {
            var deadline = new BigInteger(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 1000 * 60);
            var permissions = new ContractPermission(Chain, new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA"),
                deadline, 0);
            
            var connect = new SequenceConnect();
            await connect.SignInWithGoogle(permissions);
        }
        
        [Test]
        public async Task CreateSessionForSafeMint()
        {
            var deadline = new BigInteger(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 1000 * 60);
            var permissions = new ContractPermission(Chain, new Address("0xd25b37e2fb07f85e9eca9d40fe3bcf60ba2dc57b"),
                deadline, 0);
            
            permissions.AddRule(new ParameterRule
            {
                cumulative = false,
                mask = "0x000000000000000000000000ffffffffffffffffffffffffffffffffffffffff".HexStringToByteArray(),
                offset = new BigInt(4),
                operation = (int)ParameterOperation.equal,
                value = "0x000000000000000000000000bd7f38b943452e0c14d7ba92b9b504a9c9fc3518".HexStringToByteArray(),
            });
            
            var connect = new SequenceConnect();
            await connect.SignInWithGoogle(permissions);
        }
        
        [Test]
        public async Task CallExplicitEmit()
        {
            var wallet = SequenceWallet.RecoverFromStorage();
            await wallet.SendTransaction(Chain,
                new Transaction(new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA"), 0, "explicitEmit()"));
        }
        
        [Test]
        public async Task CallSafeMint()
        {
            var wallet = SequenceWallet.RecoverFromStorage();
            await wallet.SendTransaction(Chain,
                new Transaction(new Address("0xd25b37e2fb07f85e9eca9d40fe3bcf60ba2dc57b"), 0, "safeMint(address)", wallet.Address));
        }
        
        [Test]
        public async Task AddUnrestrictiveExplicitSession()
        {
            var wallet = SequenceWallet.RecoverFromStorage();
            await wallet.AddSession(new ContractPermission(Chain, new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA"), 0, 0));
        }
        
        [Test]
        public async Task AddRestrictiveExplicitSession()
        {
            var wallet = SequenceWallet.RecoverFromStorage();
            await wallet.AddSession(new ContractPermission(Chain, new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA"), 0, 0));
        }
    }
}