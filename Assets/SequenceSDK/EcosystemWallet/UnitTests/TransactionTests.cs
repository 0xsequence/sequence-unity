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
            new (new Address("0x33985d320809E26274a72E03268c8a29927Bc6dA"), 0, EthCrypto.HashFunctionSelector("implicitEmit()").HexStringToByteArray())
        };
        
        [Test]
        public async Task SendTransaction()
        {
            var connect = new SequenceConnect(Chain.TestnetArbitrumSepolia, EcosystemType.Sequence);
            var wallet = connect.GetWallet();

            await wallet.SendTransaction(ImplicitCalls);
        }
        
        [Test]
        public async Task GetFeeOptions()
        {
            var connect = new SequenceConnect(Chain.TestnetArbitrumSepolia, EcosystemType.Sequence);
            var wallet = connect.GetWallet();

            await wallet.GetFeeOption(ImplicitCalls);
        }
    }
}