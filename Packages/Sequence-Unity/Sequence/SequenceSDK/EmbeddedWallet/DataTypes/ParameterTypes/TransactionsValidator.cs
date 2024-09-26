using System;
using Sequence.Contracts;

namespace Sequence.EmbeddedWallet
{
    public static class TransactionsValidator
    {
        public static void Validate(Transaction[] transactions)
        {
            if (transactions == null)
            {
                throw new ArgumentNullException(nameof(transactions));
            }
            int length = transactions.Length;
            for (int i = 0; i < length; i++)
            {
                if (transactions[i] is DelayedEncode delayedEncode)
                {
                    if (!delayedEncode.data.abi.StartsWith('['))
                    {
                        if (!ABIRegex.MatchesFunctionABI(delayedEncode.data.abi))
                        {
                            throw new ArgumentException($"Given {nameof(DelayedEncode)} transaction with function abi {delayedEncode.data.abi} that does not match the required regex {ABIRegex.FunctionABIRegex}");
                        }
                    }

                    if (!ABIRegex.MatchesFunctionName(delayedEncode.data.func))
                    {
                        throw new ArgumentException(
                            $"Given {nameof(DelayedEncode)} transaction with function name {delayedEncode.data.func} that does not match the required regex {ABIRegex.FunctionNameRegex}");
                    }
                }
            }
        }
    }
}