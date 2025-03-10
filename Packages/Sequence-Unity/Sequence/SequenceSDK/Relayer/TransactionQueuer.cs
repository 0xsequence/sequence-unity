using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sequence;
using Sequence.EmbeddedWallet;
using Sequence.Utils;

namespace Sequence.Relayer
{
#if UNITY_2017_1_OR_NEWER
    public abstract class TransactionQueuer<TQueueableType, TReturnType> : UnityEngine.MonoBehaviour
#else
    public abstract class TransactionQueuer<TQueueableType, TReturnType>
#endif
    {
#if UNITY_2017_1_OR_NEWER
        [UnityEngine.Serialization.FormerlySerializedAs("_autoSubmitTransactions")] public bool AutoSubmitTransactions = false;
        [UnityEngine.Serialization.FormerlySerializedAs("_thresholdTimeBetweenTransactionsAddedBeforeSubmittedInSeconds")] public float ThresholdTimeBetweenTransactionsAddedBeforeSubmittedInSeconds = 60f; // If _autoSubmitTransactions, we will submit all the transactions in the queue if the threshold time lapses between adding subsequent items to the queue
        [UnityEngine.Serialization.FormerlySerializedAs("_minimumTimeBetweenTransactionSubmissionsInSeconds")] public float MinimumTimeBetweenTransactionSubmissionsInSeconds = 30f; // The minimum time before a transaction can be submitted on-chain after the last one was submitted
#else
        public bool AutoSubmitTransactions = false;
        public float ThresholdTimeBetweenTransactionsAddedBeforeSubmittedInSeconds = 60f;
        public float MinimumTimeBetweenTransactionSubmissionsInSeconds = 30f;
#endif

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
            if (!AutoSubmitTransactions) return;
            if (_lastTransactionAddedTime > 0f && AppEnvironment.Time - _lastTransactionAddedTime > ThresholdTimeBetweenTransactionsAddedBeforeSubmittedInSeconds)
            {
                SubmitTransactions();
            }
        }
        
        /// <summary>
        /// Add a TQueueableType to the _queue and update _lastTransactionAddedTime
        /// </summary>
        /// <param name="transaction"></param>
        public virtual void Enqueue(TQueueableType transaction)
        {
            _queue.Add(transaction);
            _lastTransactionAddedTime = AppEnvironment.Time;
        }

        /// <summary>
        /// Submit transactions in the queue
        ///
        /// If overrideWait, do not wait for any MinimumTimeBetweenTransactionSubmissionsInSeconds, just submit the transactions when called
        ///
        /// Optionally specify whether you want to waitForReceipt in the event that the API times out while waiting for the TransactionReceipt
        /// In this case, the SDK will continually attempt to fetch the TransactionReceipt
        /// </summary>
        /// <param name="overrideWait"></param>
        /// <param name="waitForReceipt"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<TReturnType> SubmitTransactions(bool overrideWait = false, bool waitForReceipt = true)
        {
            if (_wallet == null)
            {
                throw new Exception("Wallet cannot be null. User had likely not signed in yet.");
            }
            
            if (!overrideWait && AppEnvironment.Time - _lastTransactionSubmittedTime < MinimumTimeBetweenTransactionSubmissionsInSeconds && _lastTransactionSubmittedTime > 0f)
            {
                return default;
            }

            TReturnType result = await DoSubmitTransactions(waitForReceipt);
            
            _queue.Clear();
            _lastTransactionAddedTime = 0f;
            _lastTransactionSubmittedTime = AppEnvironment.Time;

            return result;
        }

        protected abstract Task<TReturnType> DoSubmitTransactions(bool waitForReceipt = true);
        
        public override string ToString()
        {
            int transactions = _queue.Count;
            if (transactions == 0)
            {
                return "0 Queued Transactions";
            }
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