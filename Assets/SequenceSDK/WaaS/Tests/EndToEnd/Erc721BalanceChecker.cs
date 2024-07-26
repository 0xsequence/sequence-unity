using System.Numerics;
using System.Threading.Tasks;

namespace Sequence.EmbeddedWallet.Tests
{
    public class Erc721BalanceChecker
    {
        private BigInteger _balance;
        private IIndexer _indexer;
        private string _address;
        private string _contractAddress;
        
        private Erc721BalanceChecker(IIndexer indexer, string address, string contractAddress)
        {
            _indexer = indexer;
            _address = address;
            _contractAddress = contractAddress;
        }
        
        public static async Task<Erc721BalanceChecker> CreateAsync(IIndexer indexer, string address, string contractAddress)
        {
            Erc721BalanceChecker checker = new Erc721BalanceChecker(indexer, address, contractAddress);
            checker._balance = await GetBalance(indexer, address, contractAddress);
            return checker;
        }
        
        private static async Task<BigInteger> GetBalance(IIndexer indexer, string address, string contractAddress)
        {
            GetTokenBalancesReturn balances = await indexer.GetTokenBalances(new GetTokenBalancesArgs(address, contractAddress));
            BigInteger result = 0;
            for (int i = 0; i < balances.balances.Length; i++)
            {
                result += balances.balances[i].balance;
            }

            return result;
        }
        
        public async Task AssertNewValueIsLarger(string caller, params object[] args)
        {
            BigInteger newBalance = await GetBalance(_indexer, _address, _contractAddress);
            CustomAssert.IsTrue(newBalance > _balance, caller, args, $"New balance {newBalance} is not greater than old balance {_balance}");
        }
        
        public async Task AssertNewValueIsSmaller(string caller, params object[] args)
        {
            BigInteger newBalance = await GetBalance(_indexer, _address, _contractAddress);
            CustomAssert.IsTrue(newBalance < _balance, caller, args, $"New balance {newBalance} is not smaller than old balance {_balance}");
        }
        
        public BigInteger GetBalance()
        {
            return _balance;
        }
    }
}