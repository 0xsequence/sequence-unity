using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence;
using Sequence.Relayer;
using Sequence.Utils;
using Sequence.EmbeddedWallet;
using UnityEngine;

namespace Sequence.Relayer
{
    public class PermissionedMinterTransactionQueuer : TransactionQueuer<(PermissionedMintTransaction, IMinter), PermissionedMinterQueueSubmissionResult>
    {
        private IMinter _minter;
        
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

        public void Enqueue(PermissionedMintTransaction transaction, IMinter minter = null)
        {
            if (AttemptToUpdateTransaction(transaction))
            {
                return;
            }

            if (minter == null)
            {
                minter = _minter;
            }
            base.Enqueue((transaction, minter));
        }
        
        private bool AttemptToUpdateTransaction(PermissionedMintTransaction transaction)
        {
            int transactions = _queue.Count;
            for (int i = 0; i < transactions; i++)
            {
                if (_queue[i].Item1.TokenId == transaction.TokenId)
                {
                    _queue[i].Item1.Amount += transaction.Amount;
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
            List<string> transactionHashes = new List<string>();
            for (int i = 0; i < transactions; i++)
            {
                PermissionedMintTransaction transaction = _queue[i].Item1;
                IMinter minter = _queue[i].Item2;
                string transactionHash = await minter.MintToken(transaction.TokenId, transaction.Amount);
                
                if (string.IsNullOrEmpty(transactionHash) || transactionHash.Contains('{'))
                {
                    success = false;
                    continue;
                }
                transactionHashes.Add(transactionHash);
            }

            return new PermissionedMinterQueueSubmissionResult(success, transactionHashes.ToArray());
        }

        public void InjectNewMinter(IMinter minter)
        {
            _minter = minter;
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