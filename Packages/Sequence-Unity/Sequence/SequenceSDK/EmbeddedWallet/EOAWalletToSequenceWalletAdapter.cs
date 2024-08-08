using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Sequence.Transactions;
using Sequence.Contracts;
using Sequence.Provider;
using Newtonsoft.Json.Linq;
using UnityEngine;
namespace Sequence.EmbeddedWallet
{
    public class EOAWalletToSequenceWalletAdapter : IWallet
    {
        Sequence.Wallet.IWallet _wallet;
        public EOAWalletToSequenceWalletAdapter(Sequence.Wallet.IWallet wallet)
        {
            _wallet = wallet;
        }
        public Address GetWalletAddress()
        {
            return _wallet.GetAddress();
        }

        public event Action<string> OnSignMessageComplete;
        public event Action<string> OnSignMessageFailed;

        public async Task<string> SignMessage(Chain network, string message, uint timeBeforeExpiry = 30)
        {
            byte[] byteChain = Encoding.UTF8.GetBytes(network.GetChainId());
            byte[] byteMessage = Encoding.UTF8.GetBytes(message);

            try
            {
                var signature = await _wallet.SignMessage(byteMessage, byteChain);
                OnSignMessageComplete?.Invoke(signature);
                return signature;

            }
            catch (Exception e)
            {
                OnSignMessageFailed?.Invoke("Error signing message: "+ e.Message);
                return e.Message;
            }
        }

        public async Task<IsValidMessageSignatureReturn> IsValidMessageSignature(Chain network, string message, string signature)
        {
            try
            {
                return new IsValidMessageSignatureReturn( await _wallet.IsValidSignature(signature, message, network));
            }
            catch (Exception e)
            {
               throw  new Exception("An error occurred while validating the message signature.", e);
            }
        }

        public event Action<SuccessfulTransactionReturn> OnSendTransactionComplete;
        public event Action<FailedTransactionReturn> OnSendTransactionFailed;

        public async Task<TransactionReturn> SendTransaction(Chain network, Transaction[] transactions, bool waitForReceipt = true, uint timeBeforeExpiry = 30)
        {
            var client = new SequenceEthClient(network);
            EthTransaction[] ethTransactions = new EthTransaction[transactions.Length];
            List<FailedTransactionReturn> failedTransactionReturns = new List<FailedTransactionReturn>();
            List<SuccessfulTransactionReturn> successfulTransactionReturns = new List<SuccessfulTransactionReturn>();

            for (int i = 0; i < transactions.Length; i++)
            {
                EthTransaction transaction;
                string toAddress;
                BigInteger value;
                string data = null;

                switch (transactions[i])
                {
                    case RawTransaction tx:
                        toAddress = tx.to;
                        value = BigInteger.Parse(tx.value);
                        data = tx.data;

                        transaction = await new GasLimitEstimator(client, _wallet.GetAddress())
                                                 .BuildTransaction(toAddress, data, value);
                        break;
                        
                    case SendERC20 tx:
                        toAddress = tx.to;
                        value = BigInteger.Parse(tx.value);

                        var erc20 = new ERC20(tx.tokenAddress);
                        transaction = await erc20.TransferFrom(_wallet.GetAddress(), toAddress, value)
                                                    .Create(client, new ContractCall(_wallet.GetAddress()));
                        break;

                    case SendERC721 tx:
                        toAddress = tx.to;

                        var erc721 = new ERC721(tx.tokenAddress);
                        transaction = await erc721.SafeTransferFrom(_wallet.GetAddress(), toAddress, BigInteger.Parse(tx.id), Encoding.UTF8.GetBytes(tx.data))
                                                    .Create(client, new ContractCall(_wallet.GetAddress()));
                        break;

                    case SendERC1155 tx:
                        toAddress = tx.to;

                        var erc1155 = new ERC1155(tx.tokenAddress);
                        BigInteger[] tokenIds = new BigInteger[tx.vals.Length];
                        BigInteger[] tokenValues = new BigInteger[tx.vals.Length];

                        for (int j = 0; j < tx.vals.Length; j++)
                        {
                            tokenIds[j] = BigInteger.Parse(tx.vals[j].id);
                            tokenValues[j] = BigInteger.Parse(tx.vals[j].amount);
                        }

                        transaction = await erc1155.SafeBatchTransferFrom(_wallet.GetAddress(), toAddress, tokenIds, tokenValues, Encoding.UTF8.GetBytes(tx.data))
                                                    .Create(client, new ContractCall(_wallet.GetAddress()));
                        break;

                    default:
                        return new FailedTransactionReturn("Error adapting to EthTransaction type. Unable to determine transaction type for:" + transactions[i].ToString(), null);
                }

                if (waitForReceipt)
                {
                    try
                    {
                        var receiptResponse = await _wallet.SendTransactionAndWaitForReceipt(client, transaction);

                        if (receiptResponse == null)
                        {
                            throw new Exception("Could not get a receipt");
                        }
                        else
                        {
                            var successfulReturn = new SuccessfulTransactionReturn(receiptResponse.transactionHash, null, null, null, JObject.FromObject(receiptResponse));
                            successfulTransactionReturns.Add(successfulReturn);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error while waiting for receipt: " + e.Message);
                        string receiptExString = e.Message;
                        var failedReturn = new FailedTransactionReturn(e.Message, null, null);
                        failedTransactionReturns.Add(failedReturn);
                    }
                }
                else
                {
                    var txHash = await _wallet.SendTransaction(client, transaction);
                    successfulTransactionReturns.Add(new SuccessfulTransactionReturn(txHash, null, null, null));
                }
            }

            if (transactions.Length < 2)
            {
                if (failedTransactionReturns.Count == 0)
                {
                    var successfulTransactionReturn = new SuccessfulTransactionReturn(
                        successfulTransactionReturns[0].txHash,
                        null,
                        null,
                        null,
                        successfulTransactionReturns[0].nativeReceipt
                    );
                    OnSendTransactionComplete?.Invoke(successfulTransactionReturn);
                    return successfulTransactionReturn;
                }
                else
                {
                    var failedTransactionReturn = new FailedTransactionReturn(
                        failedTransactionReturns[0].error,
                        null,
                        null
                    );
                    OnSendTransactionFailed?.Invoke(failedTransactionReturn);
                    return failedTransactionReturn;
                }
            }
            else
            {
                if (failedTransactionReturns.Count == 0)
                {
                    var successfulBatchTransactionReturn = new SuccessfulBatchTransactionReturn(
                        successfulTransactionReturns.ToArray()
                    );
                    OnSendTransactionComplete?.Invoke(successfulBatchTransactionReturn);
                    return successfulBatchTransactionReturn;
                }
                else
                {
                    var failedBatchTransactionReturn = new FailedBatchTransactionReturn(
                        successfulTransactionReturns.ToArray(),
                        failedTransactionReturns.ToArray()
                    );
                    OnSendTransactionFailed?.Invoke(failedBatchTransactionReturn);
                    return failedBatchTransactionReturn;
                }
            }
        }
        

        public event Action<SuccessfulContractDeploymentReturn> OnDeployContractComplete;
        public event Action<FailedContractDeploymentReturn> OnDeployContractFailed;

        public async Task<ContractDeploymentReturn> DeployContract(Chain network, string bytecode, string value)
        {
            var client = new SequenceEthClient(network);


            ContractDeploymentReturn contractDeploymentReturn;
            try
            {
                ContractDeploymentResult result = await ContractDeployer.Deploy(client, _wallet, bytecode);
                if (result.DeployedContractAddress != null)
                {
                    contractDeploymentReturn = new SuccessfulContractDeploymentReturn(new SuccessfulTransactionReturn(result.Receipt.transactionHash, null, null, null), result.DeployedContractAddress);
                }
                else
                {
                    throw new Exception(result.TransactionHash);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Contract deployment failed.");
                contractDeploymentReturn = new FailedContractDeploymentReturn(new FailedTransactionReturn("Contract deployment failed.", null), "Contract deployment failed."+ex.Message);
            }   
            return contractDeploymentReturn;
        }
                

        #region Unadaptable
        public event Action<string> OnDropSessionComplete;

        public Task<bool> DropSession(string dropSessionId)
        {
            throw new NotSupportedException();
        }

        public Task<bool> DropThisSession()
        {
            throw new NotSupportedException();
        }

        public event Action<Session[]> OnSessionsFound;

        public Task<Session[]> ListSessions()
        {
            throw new NotSupportedException();
        }

        public Task<SuccessfulTransactionReturn> WaitForTransactionReceipt(SuccessfulTransactionReturn successfulTransactionReturn)
        {
            throw new NotSupportedException();
        }

        public Task<FeeOptionsResponse> GetFeeOptions(Chain network, Transaction[] transactions, uint timeBeforeExpiry = 30)
        {
            throw new NotSupportedException();
        }

        public Task<TransactionReturn> SendTransactionWithFeeOptions(Chain network, Transaction[] transactions, FeeOption feeOption, string feeQuote, bool waitForReceipt = true, uint timeBeforeExpiry = 30)
        {
            throw new NotSupportedException();
        }

        public event Action<IntentResponseSessionAuthProof> OnSessionAuthProofGenerated;
        public event Action<string> OnFailedToGenerateSessionAuthProof;

        public Task<IntentResponseSessionAuthProof> GetSessionAuthProof(Chain network, string nonce = null)
        {
            throw new NotSupportedException();
        }
    }
    #endregion

}
