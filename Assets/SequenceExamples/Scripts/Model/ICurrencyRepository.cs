using System.Numerics;

namespace Sequence.Demo
{
    public interface ICurrencyRepository
    {
        public uint GetAmount();
        public void SetAmount(uint value);
        public string GetSymbol();
        public float GetPreviousCurrencyValue();
        public void SetPreviousCurrencyValue(float value);
        public Currency GetCurrency();
        public Address GetContractAddress();
    }
}