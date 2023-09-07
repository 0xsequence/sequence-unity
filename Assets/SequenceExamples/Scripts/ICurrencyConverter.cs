using Sequence.Contracts;

namespace Sequence.Demo
{
    public interface ICurrencyConverter
    {
        public CurrencyValue ConvertToCurrency(float amount, ERC20 token);
    }

    public class CurrencyValue
    {
        public string Symbol { get; private set; }
        public float Amount { get; private set; }

        public CurrencyValue(string symbol, float amount)
        {
            this.Symbol = symbol;
            this.Amount = amount;
        }
    }
}