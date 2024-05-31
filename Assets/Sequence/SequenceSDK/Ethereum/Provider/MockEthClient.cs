using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence;
using Sequence.Provider;

namespace SequenceSDK.Ethereum.Provider
{
    public class MockEthClient : IEthClient
    {
        private string _chainId;
        private TransactionReceipt _receipt;
        
        public MockEthClient(string chainId, TransactionReceipt receipt)
        {
            _chainId = chainId;
            _receipt = receipt;
        }
        
        public async Task<string> ChainID()
        {
            return _chainId;
        }

        public Task<Block> BlockByHash(string blockHash)
        {
            throw new System.NotImplementedException();
        }

        public Task<Block> BlockByNumber(string blockNumber)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> BlockNumber()
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Block>> BlockRange(string start = "earliest", string end = "earliest")
        {
            throw new System.NotImplementedException();
        }

        public Task<BigInteger> PeerCount()
        {
            throw new System.NotImplementedException();
        }

        public Task<Block> HeaderByHash(string blockHash)
        {
            throw new System.NotImplementedException();
        }

        public Task<Block> HeaderByNumber(string blockNumber)
        {
            throw new System.NotImplementedException();
        }

        public async Task<TransactionReceipt> WaitForTransactionReceipt(string transactionHashint, int maxWaitTimeInMilliseconds = 15000,
            int timeBetweenChecksInMilliseconds = 500)
        {
            return _receipt;
        }

        public Task<ClientTransaction> TransactionByHash(string transactionHash)
        {
            throw new System.NotImplementedException();
        }

        public Task<ClientTransaction> TransactionSender(string blockHash, BigInteger transactionIndex)
        {
            throw new System.NotImplementedException();
        }

        public Task<BigInteger> TransactionCount(string blockHash)
        {
            throw new System.NotImplementedException();
        }

        public Task<ClientTransaction> TransactionInBlock(string blockHash, BigInteger transactionIndex)
        {
            throw new System.NotImplementedException();
        }

        public Task<TransactionReceipt> TransactionReceipt(string transactionHash)
        {
            throw new System.NotImplementedException();
        }

        public Task<SyncStatus> SyncProgress()
        {
            throw new System.NotImplementedException();
        }

        public Task<string> NetworkId()
        {
            throw new System.NotImplementedException();
        }

        public Task<BigInteger> BalanceAt(string address, string blockNumber = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> StorageAt(string address, BigInteger storagePosition, string blockNumber)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> CodeAt(string address, string blockNumber)
        {
            throw new System.NotImplementedException();
        }

        public Task<BigInteger> NonceAt(string address, string blockNumber = "latest")
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Log>> FilterLogs(Filter filter)
        {
            throw new System.NotImplementedException();
        }

        public Task<BigInteger> PendingBalanceAt(string address)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> PendingStorageAt(string address, BigInteger storagePosition)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> PendingCodeAt(string address)
        {
            throw new System.NotImplementedException();
        }

        public Task<BigInteger> PendingNonceAt(string address)
        {
            throw new System.NotImplementedException();
        }

        public Task<BigInteger> PendingTransactionCount()
        {
            throw new System.NotImplementedException();
        }

        public Task<string> CallContract(params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> CallContractAtHash()
        {
            throw new System.NotImplementedException();
        }

        public Task<string> PendingCallContract(TransactionCall transactionCall)
        {
            throw new System.NotImplementedException();
        }

        public Task<BigInteger> SuggestGasPrice()
        {
            throw new System.NotImplementedException();
        }

        public Task<BigInteger> SuggestGasTipCap()
        {
            throw new System.NotImplementedException();
        }

        public Task<FeeHistoryResult> FeeHistory(string blockCount, string newestBlock, int[] REWARDPERCENTILES)
        {
            throw new System.NotImplementedException();
        }

        public Task<BigInteger> EstimateGas(TransactionCall transactionCall)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> SendTransaction(ClientTransaction clientTransaction)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> SendRawTransaction(string signedTransactionData)
        {
            throw new System.NotImplementedException();
        }
    }
}