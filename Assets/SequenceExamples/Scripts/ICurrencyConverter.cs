using Sequence.Contracts;

namespace Sequence.Demo
{
    public interface ICurrencyConverter
    {
        public Currency ConvertToCurrency(float amount, ERC20 token);
        public Currency ConvertToCurrency(float amount, string tokenAddress);
    }

    public class Currency
    {
        public string Symbol { get; private set; }
        public float Amount { get; private set; }

        public Currency(string symbol, float amount)
        {
            this.Symbol = symbol;
            this.Amount = amount;
        }
    }
}