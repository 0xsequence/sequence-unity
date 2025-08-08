using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.EcosystemWallet.Primitives;

namespace Sequence.EcosystemWallet.UnitTests
{
    public class TransactionTests
    {
        [Test]
        public async Task SendTransaction()
        {
            var connect = new SequenceConnect(Chain.TestnetArbitrumSepolia, EcosystemType.Sequence);
            var wallet = connect.GetWallet();

            var calls = new Call[]
            {
                new (wallet.Address, 1000, Array.Empty<byte>())
            };
            
            await wallet.SendTransaction(calls);
        }
    }
}