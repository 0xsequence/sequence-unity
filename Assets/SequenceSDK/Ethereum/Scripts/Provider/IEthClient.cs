using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Wallet;
using UnityEngine;

namespace Sequence.Provider
{
	// Standard Ethereum JSON-RPC methods:
	// https://ethereum.org/en/developers/docs/apis/json-rpc/
	//
	// web3_clientVersion
	// web3_sha3
	// net_version
	// net_listening
	// net_peerCount
	// eth_protocolVersion
	// eth_syncing
	// eth_coinbase
	// eth_mining
	// eth_hashrate
	// eth_gasPrice
	// eth_accounts
	// eth_blockNumber
	// eth_getBalance
	// eth_getStorageAt
	// eth_getTransactionCount
	// eth_getBlockTransactionCountByHash
	// eth_getBlockTransactionCountByNumber
	// eth_getUncleCountByBlockHash
	// eth_getUncleCountByBlockNumber
	// eth_getCode
	// eth_sign
	// eth_signTransaction
	// eth_sendTransaction
	// eth_sendRawTransaction
	// eth_call
	// eth_estimateGas
	// eth_getBlockByHash
	// eth_getBlockByNumber
	// eth_getTransactionByHash
	// eth_getTransactionByBlockHashAndIndex
	// eth_getTransactionByBlockNumberAndIndex
	// eth_getTransactionReceipt
	// eth_getUncleByBlockHashAndIndex
	// eth_getUncleByBlockNumberAndIndex
	// eth_getCompilers
	// eth_compileSolidity
	// eth_compileLLL
	// eth_compileSerpent
	// eth_newFilter
	// eth_newBlockFilter
	// eth_newPendingTransactionFilter
	// eth_uninstallFilter
	// eth_getFilterChanges
	// eth_getFilterLogs
	// eth_getLogs
	// eth_getWork
	// eth_submitWork
	// eth_submitHashrate
	//
	//
	// Unstandard JSON-RPC Methods:
	//
	// eth_getBlockRange -- Optimism
	public interface IEthClient
	{


		/// <summary>
		/// ChainID = eth_chainId
		/// </summary>
		/// <returns></returns>
		Task<string> ChainID();

		/// <summary>
		/// BlockByHash = eth_getBlockByHash (true)
		/// </summary>
		/// <param name="blockHash"></param>
		/// <returns></returns>
		Task<Block> BlockByHash(string blockHash);


		/// <summary>
		/// BlockByNumber = eth_getBlockByNumber (true)
		/// </summary>
		/// <param name="blockNumber"></param>
		/// <returns></returns>
		Task<Block> BlockByNumber(string blockNumber);


		/// <summary>
		/// BlockNumber = eth_blockNumber
		/// </summary>
		/// <returns></returns>
		Task<string> BlockNumber();

	
		/// <summary>
		/// BlockRange = eth_getBlockRange
		/// https://community.optimism.io/docs/developers/build/json-rpc/#eth-getblockrange
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="full"></param>
		/// <returns></returns>
		Task<List<Block>> BlockRange(string start = "earliest", string end = "earliest", bool? full= true );


		/// <summary>
		/// PeerCount = net_peerCount
		/// </summary>
		/// <returns></returns>
		Task<BigInteger> PeerCount();


		/// <summary>
		/// HeaderByHash = eth_getBlockByHash (false)
		/// </summary>
		/// <param name="blockHash"></param>
		/// <returns></returns>
		Task<Block> HeaderByHash(string blockHash);

 
		/// <summary>
		/// HeaderByNumber = eth_getBlockByHash (true)
		/// </summary>
		/// <param name="blockNumber"></param>
		/// <returns></returns>
		Task<Block> HeaderByNumber(string blockNumber);

		/// <summary>
		/// Continually calls eth_getTransactionReceipt until a response is received or timeout
		/// </summary>
		/// <param name="transactionHash"></param>
		/// <returns></returns>
		Task<TransactionReceipt> WaitForTransactionReceipt(string transactionHashint, int maxWaitTimeInMilliseconds = 15000, int timeBetweenChecksInMilliseconds = 500);

        /// <summary>
        /// TransactionByHash = eth_getTransactionByHash
        /// </summary>
        /// <param name="transactionHash"></param>
        /// <returns></returns>
        Task<Transaction> TransactionByHash(string transactionHash);


		/// <summary>
		/// TransactionSender is a wrapper for eth_getTransactionByBlockHashAndIndex
		/// </summary>
		/// <param name="blockHash"></param>
		/// <param name="transactionIndex"></param>
		/// <returns></returns>
		Task<Transaction> TransactionSender(string blockHash, BigInteger transactionIndex);


		/// <summary>
		/// TransactionCount = eth_getBlockTransactionCountByHash
		/// </summary>
		/// <param name="blockHash"></param>
		/// <returns></returns>
		Task<BigInteger> TransactionCount(string blockHash);


		/// <summary>
		///  TransactionInBlock = eth_getTransactionByBlockHashAndIndex
		/// </summary>
		/// <param name="blockHash"></param>
		/// <param name="transactionIndex"></param>
		/// <returns></returns>
		Task<Transaction> TransactionInBlock(string blockHash, BigInteger transactionIndex);


		/// <summary>
		/// TransactionReceipt = eth_getTransactionReceipt
		/// </summary>
		/// <param name="transactionHash"></param>
		/// <returns></returns>
		Task<TransactionReceipt> TransactionReceipt(string transactionHash);


		/// <summary>
		/// SyncProgress = eth_syncing
		/// </summary>
		/// <returns></returns>
		Task<SyncStatus> SyncProgress();


		/// <summary>
		/// NetworkID = net_version
		/// </summary>
		/// <returns></returns>
		Task<string> NetworkId();
 
		/// <summary>
		/// BalanceAt = eth_getBalance
		/// </summary>
		/// <param name="address"></param>
		/// <param name="blockNumber"></param>
		/// <returns></returns>
		Task<BigInteger> BalanceAt(string address, string blockNumber);


		/// <summary>
		/// StorageAt = eth_getStorageAt
		/// </summary>
		/// <param name="address"></param>
		/// <param name="storagePosition"></param>
		/// <param name="blockNumber"></param>
		/// <returns></returns>
		Task<string> StorageAt(string address, BigInteger storagePosition, string blockNumber);


		/// <summary>
		/// CodeAt = eth_getCode
		/// </summary>
		/// <param name="address"></param>
		/// <param name="blockNumber"></param>
		/// <returns></returns>
		Task<string> CodeAt(string address, string blockNumber);


		/// <summary>
		/// NonceAt = eth_getTransactionCount
		/// </summary>
		/// <param name="address"></param>
		/// <param name="blockNumber"></param>
		/// <returns></returns>
		Task<BigInteger> NonceAt(string address, string blockNumber);

		/// <summary>
		/// FilterLogs = eth_getLogs
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		Task<List<Log>> FilterLogs(Filter filter);

		/// <summary>
		/// PendingBalanceAt = eth_getBalance ("pending")
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		Task<BigInteger> PendingBalanceAt(string address);


		/// <summary>
		/// PendingStorageAt = eth_getStorageAt ("pending")
		/// </summary>
		/// <param name="address"></param>
		/// <param name="storagePosition"></param>
		/// <returns></returns>
		Task<string> PendingStorageAt(string address, BigInteger storagePosition);


		/// <summary>
		/// PendingCodeAt = eth_getCode ("pending")
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		Task<string> PendingCodeAt(string address);

		/// <summary>
		/// PendingNonceAt = eth_getTransactionCount ("pending")
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		Task<BigInteger> PendingNonceAt(string address);

		/// <summary>
		/// PendingTransactionCount = eth_getBlockTransactionCountByNumber ("pending")
		/// </summary>
		/// <returns></returns>
		Task<BigInteger> PendingTransactionCount();

		/// <summary>
		/// CallContract = eth_call (blockNumber)
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		Task<string> CallContract(params object[] args);


		/// <summary>
		/// CallContractAtHash = eth_call (blockHash)
		/// </summary>
		/// <returns></returns>
		Task<string> CallContractAtHash();


		/// <summary>
		/// PendingCallContract = eth_call ("pending")
		/// </summary>
		/// <param name="transactionCall"></param>
		/// <returns></returns>
		Task<string> PendingCallContract(TransactionCall transactionCall);

		/// <summary>
		/// SuggestGasPrice = eth_gasPrice
		/// </summary>
		/// <returns></returns>
		Task<BigInteger> SuggestGasPrice();


		/// <summary>
		/// SuggestGasTipCap = eth_maxPriorityFeePerGas
		/// </summary>
		/// <returns></returns>
		Task<string> SuggestGasTipCap();

		/// <summary>
		/// FeeHistory = eth_feeHistory
		/// </summary>
		/// <param name="blockCount"></param>
		/// <param name="newestBlock"></param>
		/// <param name="REWARDPERCENTILES"></param>
		/// <returns></returns>
		Task<FeeHistoryResult> FeeHistory(string blockCount, string newestBlock, int? REWARDPERCENTILES);


		/// <summary>
		/// EstimateGas = eth_estimateGas
		/// </summary>
		/// <param name="transactionCall"></param>
		/// <returns></returns>
		Task<BigInteger> EstimateGas(TransactionCall transactionCall);


		/// <summary>
		/// SendTransaction = eth_sendRawTransaction
		/// return the transaction hash
		/// </summary>
		/// <param name="transaction"></param>
		/// <returns></returns>
		Task<string> SendTransaction(Transaction transaction);

		/// <summary>
		/// eth_sendRawTransaction on a signed transaction hash
		/// </summary>
		/// <param name="signedTransactionData"></param>
		/// <returns></returns>
		Task<string> SendRawTransaction(string signedTransactionData);
	}
}