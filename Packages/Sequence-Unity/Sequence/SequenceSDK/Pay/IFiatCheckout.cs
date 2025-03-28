using System.Numerics;
using System.Threading.Tasks;
using Sequence.Pay.Transak;

namespace Sequence.Pay
{
    public interface IFiatCheckout
    {
        public bool OnRampEnabled();

        public Task<bool> NftCheckoutEnabled();

        public Task<string> GetOnRampLink();

        public Task<string> GetNftCheckoutLink();

        public void SetAmountRequested(BigInteger newAmount);
    }
}