using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Sequence;
using Sequence.Contracts;
using Sequence.WaaS;
using SequenceSDK.WaaS;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sequence.Relayer
{
    public class WaaSTransactionQueuer : TransactionQueuer<IQueueableTransaction, TransactionReturn>
    {
        public override void Enqueue(IQueueableTransaction transaction)
        {
            if (transaction is QueuedTokenTransaction queuedTransaction)
            {
                if (AttemptToUpdateTransaction(queuedTransaction))
                {
                    return;
                }
            }
            base.Enqueue(transaction);
        }
        
        private bool AttemptToUpdateTransaction(QueuedTokenTransaction transaction)
        {
            if (transaction.Type == QueuedTokenTransaction.TransactionType.BURN || transaction.Type == QueuedTokenTransaction.TransactionType.MINT || transaction.Type == QueuedTokenTransaction.TransactionType.TRANSFER)
            {
                int transactions = _queue.Count;
                for (int i = 0; i < transactions; i++)
                {
                    if (_queue[i] is QueuedTokenTransaction current)
                    {
                        if (current.Type == transaction.Type && current.ContractAddress == transaction.ContractAddress && current.TokenId == transaction.TokenId && current.ToAddress == transaction.ToAddress && current.FromAddress == transaction.FromAddress)
                        {
                            current.Amount += transaction.Amount;
                            _lastTransactionAddedTime = Time.time;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        protected override async Task<TransactionReturn> DoSubmitTransactions(bool waitForReceipt = true)
        {
            Transaction[] transactions = BuildTransactions();
            if (transactions.Length == 0)
            {
                return null;
            }

            TransactionReturn result = await _wallet.SendTransaction(_chain, transactions, waitForReceipt);

            return result;
        }

        public Transaction[] BuildTransactions()
        {
            int transactions = _queue.Count;
            Transaction[] builtTransactions = new Transaction[transactions];
            for (int i = 0; i < transactions; i++)
            {
                builtTransactions[i] = _queue[i].BuildTransaction();
            }

            return builtTransactions;
        }
    }
}