using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.UnitTests
{
    public class RawTransactionTests
    {
        private static readonly EcosystemType Ecosystem = EcosystemType.Sequence;
        
        private static readonly ITransaction[] ImplicitCalls = new []
        {
            new Transaction(new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA"), 0, "implicitEmit()")
        };
        
        private static readonly ITransaction[] ExplicitCalls = new []
        {
            new Transaction(new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA"), 0, "explicitEmit()")
        };

        private static IWallet Wallet => SequenceWallet.RecoverFromStorage();
        
        [Test]
        public async Task AddPermissionForUsdcTransfer()
        {
            var deadline = new BigInteger(DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000 + 1000 * 60 * 5000);
            
            var permissions = new Permissions(
                new ContractPermission(Chain.Optimism, new Address("0x7F5c764cBc14f9669B88837ca1490cCa17c31607"), deadline, 1000000),
                new ContractPermission(Chain.Optimism, new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA"), deadline, 0));
            
            await Wallet.AddSession(Chain.Optimism, permissions);
        }
        
        [Test]
        public async Task SendExplicitTransactionWithFeeOption()
        {
            var feeOptions = await Wallet.GetFeeOption(Chain.Optimism, ExplicitCalls);
            var feeOption = feeOptions.First(o => o.token.symbol == "USDC");
            if (feeOption == null)
                throw new Exception($"Fee option 'USDC' not available");
            
            await Wallet.SendTransaction(Chain.Optimism, ExplicitCalls, feeOption);
        }
        
        [Test]
        public async Task SendImplicitTransaction()
        {
            await Wallet.SendTransaction(Chain.TestnetArbitrumSepolia, ImplicitCalls);
        }
        
        [Test]
        public async Task SendExplicitTransaction()
        {
            await Wallet.SendTransaction(Chain.TestnetArbitrumSepolia, ExplicitCalls);
        }
        
        [Test]
        public async Task GetFeeOptionsForImplicitCalls()
        {
            await Wallet.GetFeeOption(Chain.Optimism, ImplicitCalls);
        }
        
        [Test]
        public async Task GetFeeOptionsForExplicitCalls()
        {
            await Wallet.GetFeeOption(Chain.Optimism, ExplicitCalls);
        }
    }
}