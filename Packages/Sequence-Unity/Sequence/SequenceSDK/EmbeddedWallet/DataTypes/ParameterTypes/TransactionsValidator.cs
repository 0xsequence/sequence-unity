using System;
using Sequence.ABI;
using Sequence.Contracts;
using UnityEngine;

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
                            string message = $"Given {nameof(DelayedEncode)} transaction with function abi {delayedEncode.data.abi} that does not match the required regex {ABIRegex.FunctionABIRegex} - for example: \"mint(uint256,uint256)\"";
                            Debug.LogWarning(message + "\nAttempting to recover and parse anyways");
                            delayedEncode.data.abi = EventParser.ParseEventDef(delayedEncode.data.abi).ToString();
                            if (!ABIRegex.MatchesFunctionABI(delayedEncode.data.abi))
                            {
                                throw new ArgumentException(message);
                            }
                        }
                    }

                    if (!ABIRegex.MatchesFunctionName(delayedEncode.data.func))
                    {
                        throw new ArgumentException(
                            $"Given {nameof(DelayedEncode)} transaction with function name {delayedEncode.data.func} that does not match the required regex {ABIRegex.FunctionNameRegex} - for example: \"mint\"");
                    }
                }
            }
        }
    }
}