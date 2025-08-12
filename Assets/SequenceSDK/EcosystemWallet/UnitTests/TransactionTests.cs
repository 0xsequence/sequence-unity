using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.UnitTests
{
    public class TransactionTests
    {
        private static readonly EcosystemType Ecosystem = EcosystemType.Sequence;
        
        private static readonly Call[] ImplicitCalls = new Call[]
        {
            new (new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA"), 0, ABI.ABI.FunctionSelector("implicitEmit()").HexStringToByteArray())
        };
        
        private static readonly Call[] ExplicitCalls = new Call[]
        {
            new (new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA"), 0, ABI.ABI.FunctionSelector("explicitEmit()").HexStringToByteArray())
        };

        private static SequenceWallet Wallet => SequenceWallet.RecoverFromStorage();
        
        [Test]
        public async Task AddPermissionForUsdcTransfer()
        {
            var usdcAddress = new Address("0x7F5c764cBc14f9669B88837ca1490cCa17c31607");
            var target = new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA");
            var deadline = new BigInteger(DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000 + 1000 * 60 * 5000);
            
            var sessionBuilder = new SessionBuilder(Chain.Optimism, 1000000, deadline);
            sessionBuilder.AddPermission(usdcAddress);
            sessionBuilder.AddPermission(target);
            
            var permissions = sessionBuilder.GetPermissions();

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