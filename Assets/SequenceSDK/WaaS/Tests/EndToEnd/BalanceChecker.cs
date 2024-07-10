using System.Numerics;
using System.Threading.Tasks;
using Sequence.Provider;

namespace Sequence.WaaS.Tests
{
    public class BalanceChecker
    {
        private BigInteger _balance;
        private IEthClient _client;
        private string _address;
        
        private BalanceChecker(IEthClient client, string address)
        {
            _client = client;
            _address = address;
        }
        
        public static async Task<BalanceChecker> CreateAsync(IEthClient client, string address)
        {
            BalanceChecker checker = new BalanceChecker(client, address);
            checker._balance = await checker._client.BalanceAt(address);
            return checker;
        }

        public async Task AssertNewValueIsLarger(string caller, params object[] args)
        {
            BigInteger newBalance = await _client.BalanceAt(_address);
            CustomAssert.IsTrue(newBalance > _balance, caller, args, $"New balance {newBalance} is not greater than old balance {_balance}");
        }

        public async Task AssertNewValueIsSmaller(string caller, params object[] args)
        {
            BigInteger newBalance = await _client.BalanceAt(_address);
            CustomAssert.IsTrue(newBalance < _balance, caller, args, $"New balance {newBalance} is not smaller than old balance {_balance}");
        }
    }
}