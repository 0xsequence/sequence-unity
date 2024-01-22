using System.Numerics;
using System.Threading.Tasks;

namespace Sequence.WaaS.Tests
{
    public class Erc20BalanceChecker
    {
        private BigInteger _balance;
        private IIndexer _indexer;
        private string _address;
        private string _contractAddress;
        
        private Erc20BalanceChecker(IIndexer indexer, string address, string contractAddress)
        {
            _indexer = indexer;
            _address = address;
            _contractAddress = contractAddress;
        }
        
        public static async Task<Erc20BalanceChecker> CreateAsync(IIndexer indexer, string address, string contractAddress)
        {
            Erc20BalanceChecker checker = new Erc20BalanceChecker(indexer, address, contractAddress);
            GetTokenBalancesReturn balances = await checker._indexer.GetTokenBalances(new GetTokenBalancesArgs(address, contractAddress));
            checker._balance = balances.balances[0].balance;
            return checker;
        }

        public async Task AssertNewValueIsLarger(string caller, params object[] args)
        {
            GetTokenBalancesReturn balances = await _indexer.GetTokenBalances(new GetTokenBalancesArgs(_address, _contractAddress));
            BigInteger newBalance = balances.balances[0].balance;
            CustomAssert.IsTrue(newBalance > _balance, caller, args, $"New balance {newBalance} is not greater than old balance {_balance}");
        }

        public async Task AssertNewValueIsSmaller(string caller, params object[] args)
        {
            GetTokenBalancesReturn balances = await _indexer.GetTokenBalances(new GetTokenBalancesArgs(_address, _contractAddress));
            BigInteger newBalance = balances.balances[0].balance;
            CustomAssert.IsTrue(newBalance < _balance, caller, args, $"New balance {newBalance} is not smaller than old balance {_balance}");
        }
    }
}