using Sequence.Contracts;
using UnityEngine;

namespace Sequence.Demo
{
    public class MockCurrencyConverter : ICurrencyConverter
    {
        public Currency ConvertToCurrency(float amount, ERC20 token)
        {
            return ConvertToCurrency(amount, token.GetAddress());
        }

        public Currency ConvertToCurrency(float amount, string tokenAddress)
        {
            return new Currency("$", amount + Random.Range(-amount * .25f, amount * .25f));
        }
    }
}