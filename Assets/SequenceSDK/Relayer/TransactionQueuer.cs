using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sequence;
using Sequence.WaaS;
using UnityEngine;

namespace Sequence.Relayer
{
    public abstract class TransactionQueuer<TQueueableType, TReturnType> : MonoBehaviour
    {
        [SerializeField] private bool _autoSubmitTransactions = false;
        [SerializeField] private float _thresholdTimeBetweenTransactionsAddedBeforeSubmittedInSeconds = 60f; // If _autoSubmitTransactions, we will submit all the transactions in the queue if the threshold time lapses between adding subsequent items to the queue
        [SerializeField] private float _minimumTimeBetweenTransactionSubmissionsInSeconds = 30f; // The minimum time before a transaction can be submitted on-chain after the last one was submitted

        protected List<TQueueableType> _queue = new List<TQueueableType>();
        protected float _lastTransactionAddedTime = 0f;
        protected float _lastTransactionSubmittedTime = 0f;
        
        protected IWallet _wallet;
        protected Chain _chain;
        
        public virtual void Setup(IWallet wallet, Chain chain)
        {
            _wallet = wallet;
            _chain = chain;
        }

        private void Update()
        {
            if (!_autoSubmitTransactions) return;
            if (_lastTransactionAddedTime > 0f && Time.time - _lastTransactionAddedTime > _thresholdTimeBetweenTransactionsAddedBeforeSubmittedInSeconds)
            {
                SubmitTransactions();
            }
        }
        
        public virtual void Enqueue(TQueueableType transaction)
        {
            _queue.Add(transaction);
            _lastTransactionAddedTime = Time.time;
        }

        public async Task<TReturnType> SubmitTransactions(bool overrideWait = false, bool waitForReceipt = true)
        {
            if (_wallet == null)
            {
                throw new Exception("Wallet cannot be null. User had likely not signed in yet.");
            }
            
            if (!overrideWait && Time.time - _lastTransactionSubmittedTime < _minimumTimeBetweenTransactionSubmissionsInSeconds && _lastTransactionSubmittedTime > 0f)
            {
                return default;
            }

            TReturnType result = await DoSubmitTransactions(waitForReceipt);
            
            _queue.Clear();
            _lastTransactionAddedTime = 0f;
            _lastTransactionSubmittedTime = Time.time;

            return result;
        }

        protected abstract Task<TReturnType> DoSubmitTransactions(bool waitForReceipt = true);
        
        public override string ToString()
        {
            int transactions = _queue.Count;
            StringBuilder result = new StringBuilder($"{transactions} Queued Transactions: ");
            for (int i = 0; i < transactions - 1; i++)
            {
                result.Append(_queue[i].ToString());
                result.Append(" | ");
            }
            result.Append(_queue[transactions - 1].ToString());

            return result.ToString();
        }
    }
}