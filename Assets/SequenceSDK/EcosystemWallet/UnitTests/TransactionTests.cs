using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.UnitTests
{
    public class TransactionTests
    {
        private static readonly Call[] ImplicitCalls = new Call[]
        {
            new (new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA"), 0, ABI.ABI.FunctionSelector("implicitEmit()").HexStringToByteArray())
        };
        
        private static readonly Call[] ExplicitCalls = new Call[]
        {
            new (new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA"), 0, ABI.ABI.FunctionSelector("explicitEmit()").HexStringToByteArray())
        };

        private static SequenceWallet Wallet
        {
            get
            {
                var connect = new SequenceConnect(Chain.TestnetArbitrumSepolia, EcosystemType.Sequence);
                return connect.GetWallet();
            }
        }
        
        [Test]
        public async Task SendImplicitTransaction()
        {
            await Wallet.SendTransaction(ImplicitCalls);
        }
        
        [Test]
        public async Task SendExplicitTransaction()
        {
            await Wallet.SendTransaction(ExplicitCalls);
        }
        
        [Test]
        public async Task GetFeeOptionsForImplicitCalls()
        {
            await Wallet.GetFeeOption(ImplicitCalls);
        }
        
        [Test]
        public async Task GetFeeOptionsForExplicitCalls()
        {
            await Wallet.GetFeeOption(ExplicitCalls);
        }
    }
}