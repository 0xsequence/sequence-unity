using Sequence.Contracts;

namespace Sequence.Demo
{
    public class MockCurrencyConverter : ICurrencyConverter
    {
        public CurrencyValue ConvertToCurrency(float amount, ERC20 token)
        {
            return new CurrencyValue("$", amount);
        }
    }
}