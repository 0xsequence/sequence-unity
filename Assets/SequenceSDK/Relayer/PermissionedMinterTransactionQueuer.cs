using System;
using System.Threading.Tasks;
using Sequence;
using Sequence.Relayer;
using Sequence.WaaS;
using UnityEngine;

namespace Sequence.Relayer
{
    public class PermissionedMinterTransactionQueuer : TransactionQueuer<PermissionedMintTransaction, PermissionedMinterQueueSubmissionResult>
    {
        private PermissionedMinter _minter;
        
        public void Setup(IWallet wallet, Chain chain, string mintEndpoint, string contractAddress)
        {
            base.Setup(wallet, chain);
            _minter = new PermissionedMinter(new MintingRequestProver(wallet, chain), mintEndpoint, contractAddress);
            _minter.OnMintTokenFailed += HandleMintTokenFailed;
        }

        private void HandleMintTokenFailed(string error)
        {
            Debug.LogError(error);
        }

        private void OnDestroy()
        {
            _minter.OnMintTokenFailed -= HandleMintTokenFailed;
        }

        public override void Enqueue(PermissionedMintTransaction transaction)
        {
            if (AttemptToUpdateTransaction(transaction))
            {
                return;
            }
            base.Enqueue(transaction);
        }
        
        private bool AttemptToUpdateTransaction(PermissionedMintTransaction transaction)
        {
            int transactions = _queue.Count;
            for (int i = 0; i < transactions; i++)
            {
                if (_queue[i].TokenId == transaction.TokenId)
                {
                    _queue[i].Amount += transaction.Amount;
                    _lastTransactionAddedTime = Time.time;
                    return true;
                }
            }

            return false;
        }

        protected override async Task<PermissionedMinterQueueSubmissionResult> DoSubmitTransactions(bool waitForReceipt = true)
        {
            int transactions = _queue.Count;
            bool success = true;
            string[] transactionHashes = new string[transactions];
            for (int i = 0; i < transactions; i++)
            {
                PermissionedMintTransaction transaction = _queue[i];
                string transactionHash = await _minter.MintToken(transaction.TokenId, transaction.Amount);
                
                if (string.IsNullOrEmpty(transactionHash) || transactionHash.Contains('{'))
                {
                    success = false;
                }
                transactionHashes[i] = transactionHash;
            }

            return new PermissionedMinterQueueSubmissionResult(success, transactionHashes);
        }
    }

    public class PermissionedMinterQueueSubmissionResult
    {
        public bool Success;
        public string[] TransactionHashes;
        
        public PermissionedMinterQueueSubmissionResult(bool success, string[] transactionHashes)
        {
            Success = success;
            TransactionHashes = transactionHashes;
        }
    }
}