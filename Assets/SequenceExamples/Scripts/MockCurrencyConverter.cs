using Sequence.Contracts;
using UnityEngine;

namespace Sequence.Demo
{
    public class MockCurrencyConverter : ICurrencyConverter
    {
        public Currency ConvertToCurrency(float amount, ERC20 token)
        {
            return new Currency("$", amount + Random.Range(-amount * .25f, amount * .25f));
        }

        public Currency ConvertToCurrency(float amount, string tokenAddress)
        {
            return ConvertToCurrency(amount, new ERC20(tokenAddress));
        }
    }
}