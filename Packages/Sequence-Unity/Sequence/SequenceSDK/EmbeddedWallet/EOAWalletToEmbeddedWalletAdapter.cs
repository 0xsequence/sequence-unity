using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Sequence.Transactions;
using Sequence.Contracts;
using Sequence.Provider;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
namespace Sequence.EmbeddedWallet
{
    public class EOAWalletToEmbeddedWalletAdapter : IWallet
    {
        Sequence.Wallet.IWallet Wallet;
        private SequenceEthClient testClient;
        public EOAWalletToEmbeddedWalletAdapter(Sequence.Wallet.IWallet wallet, SequenceEthClient TestClient)
        {
            testClient = TestClient;
            Wallet = wallet;
        }
        public Address GetWalletAddress()
        {
            return Wallet.GetAddress();
        }

        public event Action<string> OnSignMessageComplete;
        public event Action<string> OnSignMessageFailed;

        public async Task<string> SignMessage(Chain network, string message, uint timeBeforeExpiry = 30)
        {
            byte[] byteChain = Encoding.UTF8.GetBytes(network.GetChainId());
            byte[] byteMessage = Encoding.UTF8.GetBytes(message);

            try
            {
                var signature = await Wallet.SignMessage(byteMessage, byteChain);
                if (signature == "")
                {
                    throw new Exception("Message could not be signed.");
                }
                else
                {
                    OnSignMessageComplete?.Invoke(signature);
                    return signature;
                }
            }
            catch (Exception exception)
            {
                OnSignMessageFailed?.Invoke(exception.Message);
                return exception.Message;
            }
        }

        public async Task<IsValidMessageSignatureReturn> IsValidMessageSignature(Chain network, string message, string signature)
        {
            var isValid = await Wallet.IsValidSignature(signature, message, network);

            if (isValid)
            {
                return new IsValidMessageSignatureReturn { isValid = true };
            }
            else
            {
                return new IsValidMessageSignatureReturn { isValid = false };
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
                switch (transactions[i])
                {
                    case RawTransaction tx:
                        toAddress = tx.to;
                        value = BigInteger.Parse(tx.value);

                        transaction = await TransferEth.CreateTransaction(client, Wallet, toAddress, value);
                        break;

                    case SendERC20 tx:
                        toAddress = tx.to;
                        value = BigInteger.Parse(tx.value);

                        transaction = await TransferEth.CreateTransaction(client, Wallet, toAddress, value);
                        break;

                    case SendERC721 tx:
                        toAddress = tx.to;
                        ERC721 erc721 = new ERC721(tx.tokenAddress);
                        transaction = await erc721.SafeTransferFrom(Wallet.GetAddress(), toAddress, BigInteger.Parse(tx.id), Encoding.UTF8.GetBytes(tx.data)).Create(client, new ContractCall(new Address(Wallet.GetAddress())));

                        break;

                    case SendERC1155 tx:
                        toAddress = tx.to;

                        ERC1155 erc1155 = new ERC1155(tx.tokenAddress);

                        BigInteger[] tokenIds = new BigInteger[tx.vals.Length];
                        BigInteger[] tokenValues = new BigInteger[tx.vals.Length];

                        for (int j = 0; j < tx.vals.Length; j++)
                        {
                            tokenIds[j] = BigInteger.Parse(tx.vals[j].id);
                            tokenValues[j] = BigInteger.Parse(tx.vals[j].amount);

                        }
                        transaction = await erc1155.SafeBatchTransferFrom(Wallet.GetAddress(), toAddress, tokenIds, tokenValues, Encoding.UTF8.GetBytes(tx.data)).Create(client, new ContractCall(new Address(Wallet.GetAddress())));

                        break;
                    default:
                        return new FailedTransactionReturn("Error adapting to EthTransaction type. Unable to determine transaction type for:" + transactions[i].ToString(), null);
                }
                
                if (Wallet is Sequence.Wallet.EOAWallet wallet)
                {
                    if (waitForReceipt)
                    {
                        try
                        {
                            var responseReceipt = await Wallet.SendTransactionAndWaitForReceipt(client, transaction);

                            if (responseReceipt == null)
                            {
                                throw new Exception("No Receipt");
                            }
                            else
                            {
                                var successfullReturn = new SuccessfulTransactionReturn(responseReceipt.transactionHash, null, null, null, JObject.FromObject(responseReceipt));
                                successfulTransactionReturns.Add(successfullReturn);
                                OnSendTransactionComplete?.Invoke(successfullReturn);
                            }
                        }
                        catch (Exception receiptEx)
                        {
                            Debug.LogError("Error while waiting for the receipt: " + receiptEx.Message);
                            string receiptExString = receiptEx.Message;
                            var failedReturn = new FailedTransactionReturn(receiptEx.Message, null, null);
                            failedTransactionReturns.Add(failedReturn);
                            OnSendTransactionFailed?.Invoke(failedReturn);
                        }
                    }
                    else
                    {
                        var responseReceipt = await Wallet.SendTransaction(client, transaction);
                        successfulTransactionReturns.Add(new SuccessfulTransactionReturn(responseReceipt, null, null, null));
                    }
                }
            }

            if (failedTransactionReturns.Count == 0)
            {
                return successfulTransactionReturns.Count > 1
                    ? new SuccessfulBatchTransactionReturn(successfulTransactionReturns.ToArray())
                    : successfulTransactionReturns[0];
            }

            return failedTransactionReturns.Count > 1
                ? new FailedBatchTransactionReturn(successfulTransactionReturns.ToArray(), failedTransactionReturns.ToArray())
                : failedTransactionReturns[0];
        }
        
        public event Action<SuccessfulContractDeploymentReturn> OnDeployContractComplete;
        public event Action<FailedContractDeploymentReturn> OnDeployContractFailed;

        public async Task<ContractDeploymentReturn> DeployContract(Chain network, string bytecode, string value)
        {
            var client = new SequenceEthClient(network);


            ContractDeploymentReturn creturn;
            try
            {
                ContractDeploymentResult result = await ContractDeployer.Deploy(client, Wallet, bytecode);
                if (result.DeployedContractAddress != null)
                {
                    creturn = new SuccessfulContractDeploymentReturn(new SuccessfulTransactionReturn(result.Receipt.transactionHash, null, null, null), result.DeployedContractAddress);
                }
                else
                {
                    throw new Exception(result.TransactionHash);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Contract deployment failed.");
                creturn = new FailedContractDeploymentReturn(new FailedTransactionReturn("Contract deployment failed.", null), "Contract deployment failed."+ex.Message);
            }   
            return creturn;
        }
                

        #region Unadaptable
        public event Action<string> OnDropSessionComplete;

        public Task<bool> DropSession(string dropSessionId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DropThisSession()
        {
            throw new NotImplementedException();
        }

        public event Action<Session[]> OnSessionsFound;

        public Task<Session[]> ListSessions()
        {
            throw new NotImplementedException();
        }

        public Task<SuccessfulTransactionReturn> WaitForTransactionReceipt(SuccessfulTransactionReturn successfulTransactionReturn)
        {
            throw new NotImplementedException();
        }

        public Task<FeeOptionsResponse> GetFeeOptions(Chain network, Transaction[] transactions, uint timeBeforeExpiry = 30)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionReturn> SendTransactionWithFeeOptions(Chain network, Transaction[] transactions, FeeOption feeOption, string feeQuote, bool waitForReceipt = true, uint timeBeforeExpiry = 30)
        {
            throw new NotImplementedException();
        }

        public event Action<IntentResponseSessionAuthProof> OnSessionAuthProofGenerated;
        public event Action<string> OnFailedToGenerateSessionAuthProof;

        public Task<IntentResponseSessionAuthProof> GetSessionAuthProof(Chain network, string nonce = null)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

}
