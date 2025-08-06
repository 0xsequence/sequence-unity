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
            await wallet.SendTransaction(Array.Empty<Call>());
        }
    }
}