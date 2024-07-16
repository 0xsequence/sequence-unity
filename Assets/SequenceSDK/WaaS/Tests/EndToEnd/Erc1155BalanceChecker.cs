using System.Numerics;
using System.Threading.Tasks;

namespace Sequence.EmbeddedWallet.Tests
{
    public class Erc1155BalanceChecker
    {
        private BigInteger _balance;
        private IIndexer _indexer;
        private string _address;
        private string _contractAddress;
        
        private Erc1155BalanceChecker(IIndexer indexer, string address, string contractAddress)
        {
            _indexer = indexer;
            _address = address;
            _contractAddress = contractAddress;
        }
        
        public static async Task<Erc1155BalanceChecker> CreateAsync(IIndexer indexer, string address, string contractAddress)
        {
            Erc1155BalanceChecker checker = new Erc1155BalanceChecker(indexer, address, contractAddress);
            checker._balance = await GetBalance(indexer, address, contractAddress);
            return checker;
        }
        
        private static async Task<BigInteger> GetBalance(IIndexer indexer, string address, string contractAddress)
        {
            GetTokenBalancesReturn balances = await indexer.GetTokenBalances(new GetTokenBalancesArgs(address, contractAddress));
            if (balances.balances.Length == 0)
            {
                return 0;
            }
            else
            {
                return balances.balances[0].balance;
            }
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