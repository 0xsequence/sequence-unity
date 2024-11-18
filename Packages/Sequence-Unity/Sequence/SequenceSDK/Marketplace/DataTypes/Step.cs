using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.EmbeddedWallet;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class Step
    {
        public StepType id;
        public string data;
        public string to;
        public string value;
        public Signature signature;
        public PostRequest post;

        [Preserve]
        public Step(StepType id, string data, string to, string value, Signature signature, PostRequest post)
        {
            this.id = id;
            this.data = data;
            this.to = to;
            this.value = value;
            this.signature = signature;
            this.post = post;
        }
    }

    public static class StepExtensions
    {
        public static Task<TransactionReturn> SubmitAsTransactions(this Step[] steps, IWallet wallet, Chain chain)
        {
            Transaction[] transactions = steps.AsTransactionArray();

            return wallet.SendTransaction(chain, transactions);
        }
        
        public static Transaction[] AsTransactionArray(this Step[] steps)
        {
            if (steps == null || steps.Length == 0)
            {
                throw new ArgumentException("Steps cannot be null or empty");
            }

            Transaction[] transactions = new Transaction[steps.Length];
            for (int i = 0; i < steps.Length; i++)
            {
                transactions[i] = new RawTransaction(steps[i].to, steps[i].value, steps[i].data);
            }

            return transactions;
        }
    }
}